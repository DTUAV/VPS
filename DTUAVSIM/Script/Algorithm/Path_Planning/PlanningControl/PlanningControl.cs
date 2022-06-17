/*
 * Author: Yuanlin Yang
 * Date: 2022-1-3
 * Location: Guangdong University of Technology
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections.Generic;
using DTUAV.Algorithm_Module.Path_Planning.Human;
using DTUAV.Algorithm_Module.Path_Planning.RRT;
using DTUAVCARS.DTPlanning;
using RosSharp.RosBridgeClient;
using UnityEngine;

namespace DTUAV.Algorithm_Module.Path_Planning.Control
{
    public class PlanningControl : MonoBehaviour
    {
        public HumanPlanningNode HumanPlanningNodeInstance;//The class instance of HumanPlanningNode.

        public RRTNode RRTNodeInstance;//The class instance of RRTNode.

        public VelocityCommand PubVelocityCommand;//the publisher for transferring the target position to velocity command.
        public bool HuamanInMode = false;
        public bool IsSelectBest = false;
        public bool StartTestPath;
        public bool NextTargetPosition = true;           //The next target position signal, changed by virtual guide node.
        public bool IsStart = true;
        public bool StopRunning = false;                  //The StopRunning signal, when the physical UAV fail control, it will be set true by virtual guide.
        public Rigidbody VirtualUAV1;             //The Rigidbody of virtual UAV 1.
        public float AutoSwitchError;             //The error of automatic change target position.
        public float TimeCost;
        public float SafeCost;
        private List<Vector2> _originPath;
        private List<Vector2> _runningPath;
        private int _pathIndex;
        private Vector2 _currentPosition;
        private Vector2 _currentTargetPosition;
        private bool _isRunningPlotLine;

        public bool IsFinish;
        public bool IsSystemRun = false;
        public SystemRunningControl SystemRunningControlInstance;

        public bool ResetRunningPath()
        {
            _pathIndex = 0;
            Vector3 targetPositionRos = DTUAVCARS.TF.TF.Unity2Ros(new Vector3(_runningPath[_pathIndex].x,VirtualUAV1.position.y, _runningPath[_pathIndex].y));
            PubVelocityCommand.TargetPositionRos.x = targetPositionRos.x;
            PubVelocityCommand.TargetPositionRos.y = targetPositionRos.y;
            _currentTargetPosition.x = VirtualUAV1.position.x;
            _currentTargetPosition.y = VirtualUAV1.position.z;
            return true;
        }
        public bool BackMotion()
        {
            if (_pathIndex>2)
            {
                _pathIndex -= 2;
                Vector3 targetPositionUnity = new Vector3(_runningPath[_pathIndex].x, 0, _runningPath[_pathIndex].y);
                Vector3 targetPositionRos = DTUAVCARS.TF.TF.Unity2Ros(targetPositionUnity);
                PubVelocityCommand.TargetPositionRos.x = targetPositionRos.x;
                PubVelocityCommand.TargetPositionRos.y = targetPositionRos.y;
                _currentTargetPosition = _runningPath[_pathIndex];
                _pathIndex++;
                StopRunning = false;
                return true;
            }
            else
            {
                Debug.Log("pathIndex less than 2!!!!!!!!");
                return false;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            _isRunningPlotLine = false;
            StartTestPath = false;
            NextTargetPosition = false;
            IsFinish = true;
            _pathIndex = 0;
            _originPath = new List<Vector2>();
            _runningPath = new List<Vector2>();
            _currentPosition = new Vector2(VirtualUAV1.position.x, VirtualUAV1.position.z);
            _currentTargetPosition = new Vector2();
        }

        // Update is called once per frame
        [System.Obsolete]
        void Update()
        {
            PubVelocityCommand.IsStop = StopRunning;
            if (Input.GetKeyDown(KeyCode.R))
            {
                _runningPath = _originPath;
                Debug.Log("origin_path_size: "+_originPath.Count);
                _pathIndex = 0;
                _currentTargetPosition.x = VirtualUAV1.position.x;
                _currentTargetPosition.y = VirtualUAV1.position.z;
                if (IsSystemRun)
                {
                    SystemRunningControlInstance.TargetPosition = new Vector3(_runningPath[_runningPath.Count - 1].x, VirtualUAV1.position.y, _runningPath[_runningPath.Count - 1].y);
                    SystemRunningControlInstance.StartPosition = new Vector3(_runningPath[0].x, VirtualUAV1.position.y, _runningPath[0].y);
                    SystemRunningControlInstance.IsSetTargetPosition = true;
                }
               
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                NextTargetPosition = (NextTargetPosition == true) ? false : true;
            }

            if (IsStart)
            {
                if (_pathIndex < _runningPath.Count)
                {
                    _currentPosition.x = VirtualUAV1.position.x;
                    _currentPosition.y = VirtualUAV1.position.z;
                    if (Vector2.Distance(_currentPosition, _currentTargetPosition) <= AutoSwitchError &&
                        NextTargetPosition)
                    {
                        Vector3 targetPositionUnity =
                            new Vector3(_runningPath[_pathIndex].x, 0, _runningPath[_pathIndex].y);
                        Vector3 targetPositionRos = DTUAVCARS.TF.TF.Unity2Ros(targetPositionUnity);
                        PubVelocityCommand.TargetPositionRos.x = targetPositionRos.x;
                        PubVelocityCommand.TargetPositionRos.y = targetPositionRos.y;
                        _currentTargetPosition = _runningPath[_pathIndex];
                        _pathIndex++;
                        // NextTargetPosition = false;
                    }

                    IsFinish = false;
                }
                else
                {
                    IsFinish = true;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_isRunningPlotLine&&IsFinish)
                {
                    IsFinish = false;
                    _isRunningPlotLine = false;
                    
                    if (!HuamanInMode)
                    {
                        if (RRTNodeInstance.StartFindPath())
                        {
                            _originPath = RRTNodeInstance.GetSmoothPath();
                            TimeCost = RRTNodeInstance.GetPathTimeCost();
                            SafeCost = RRTNodeInstance.GetPathSafeCost();
                        }
                    }
                    else
                    {
                        if (IsSelectBest)
                        {
                            float timeCost = 1000000.0f;
                            bool isRRTFind = RRTNodeInstance.StartFindPath();
                            if (isRRTFind)
                            {
                                timeCost = RRTNodeInstance.GetPathTimeCost();
                                Debug.Log("rrtTimeCost: "+timeCost);
                                _originPath = HumanPlanningNodeInstance.GetFusionPath();
                                if (HumanPlanningNodeInstance.GetIsFusionSuccess())
                                {
                                    if (timeCost > HumanPlanningNodeInstance.GetFusionPathTimeCost())
                                    {
                                        TimeCost = HumanPlanningNodeInstance.GetFusionPathTimeCost();
                                        SafeCost = HumanPlanningNodeInstance.GetFusionPathSafeCost();
                                    }
                                    else
                                    {
                                        TimeCost = timeCost;
                                        SafeCost = RRTNodeInstance.GetPathSafeCost();
                                        _originPath = RRTNodeInstance.GetSmoothPath();
                                    }
                                }
                                else
                                {
                                    Debug.Log("Human Planning Fail, Select RRT Path!!!");
                                    _originPath = RRTNodeInstance.GetSmoothPath();
                                    TimeCost = RRTNodeInstance.GetPathTimeCost();
                                    SafeCost = RRTNodeInstance.GetPathSafeCost();
                                }
                            }
                            else
                            {
                                if (HumanPlanningNodeInstance.GetIsFusionSuccess())
                                {

                                    _originPath = HumanPlanningNodeInstance.GetFusionPath();
                                    TimeCost = HumanPlanningNodeInstance.GetFusionPathTimeCost();
                                    SafeCost = HumanPlanningNodeInstance.GetFusionPathSafeCost();
                                }
                                else
                                {
                                    Debug.Log("RRT and Human Planning Fail, Start Planning Again!!!!");
                                }
                            }
                        }
                        else
                        {
                            if (HumanPlanningNodeInstance.GetIsFusionSuccess())
                            {
                                _originPath = HumanPlanningNodeInstance.GetFusionPath();
                                Debug.Log("size_orin: " + _originPath.Count);
                                TimeCost = HumanPlanningNodeInstance.GetFusionPathTimeCost();
                                SafeCost = HumanPlanningNodeInstance.GetFusionPathSafeCost();
                            }
                            else
                            {
                                Debug.Log("Human Planning Fail, Start Planning Again!!!!");
                            }
                        }
                    }

                   
                }
                else
                {
                    _isRunningPlotLine = true;
                }

                HumanPlanningNodeInstance.StopPlotObjectPath(_isRunningPlotLine);


                Debug.Log("Running Plot Object Line---"+_isRunningPlotLine);
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (!_isRunningPlotLine)
                {
                    HumanPlanningNodeInstance.ClearAllPathLine();
                    RRTNodeInstance.ClearAllLine();
                }
            }
            
        }
    }
}
