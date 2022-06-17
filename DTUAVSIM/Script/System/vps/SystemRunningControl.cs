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
using UnityEngine;
using DTUAV.Algorithm_Module.Path_Planning.Control;

namespace RosSharp.RosBridgeClient
{
    public class SystemRunningControl : UnityPublisher<MessageTypes.Std.Int32>
    {
        private MessageTypes.Std.Int32 message;
        public PoseStampedSubscriber V_UAV_1_PositionSub;//Path Planning
        public PoseStampedSubscriber V_UAV_2_PositionSub;//Virtual Simulator
        public PoseStampedSubscriber V_UAV_3_PositionSub;//Physical UAV
        public PlanningControl PlanningControlInstance;
        public ResetObjectPublisher ResetObjectPublisherInstance;

        public int StageId = 0;

        public float PositionError;//The system running process switch according this position error.
        public Vector3 TargetPosition;
        public Vector3 StartPosition;
        public bool IsSetTargetPosition = false;

        [Header("The show trakectory of the Physical UAV.")]
        public show_uav_trajectory ShowPhyiscalUavTrajectory;
        
        [Header("The show trakectory of the Simulator UAV.")]
        public show_uav_trajectory ShowSimulatorUavTrajectory;

        private bool _isFirstRun = true;

        private int _runningStage = 0;

        private int _stableRunning = 0;

        public bool EnterRunningState0()
        {
            message.data = 0;//start new mission
            _isFirstRun = false;
            PlanningControlInstance.IsStart = true;
            Debug.Log("Enter stage 0.");
            _runningStage = 0;
            _stableRunning = 0;
            return true;
        }

        void FixedUpdate()
        {
            if ((CheckDistance(V_UAV_1_PositionSub.position.x,V_UAV_2_PositionSub.position.x,V_UAV_3_PositionSub.position.x,PositionError)
            &&CheckDistance(V_UAV_1_PositionSub.position.y, V_UAV_2_PositionSub.position.y, V_UAV_3_PositionSub.position.y, PositionError)
            &&CheckDistance(V_UAV_1_PositionSub.position.z, V_UAV_2_PositionSub.position.z, V_UAV_3_PositionSub.position.z, PositionError)&&_runningStage==5)||_isFirstRun)
            {
                _stableRunning++;
                if (_stableRunning > 10)
                {
                    message.data = 0;//start new mission
                    _isFirstRun = false;
                    PlanningControlInstance.IsStart = true;
                    Debug.Log("Enter stage 0.");
                    _runningStage = 0;
                    _stableRunning = 0;
                }
                
            }
            else if (CheckDistance(V_UAV_1_PositionSub.position.x, TargetPosition.x, PositionError)
                     && CheckDistance(V_UAV_1_PositionSub.position.y, TargetPosition.y, PositionError)
                     && CheckDistance(V_UAV_1_PositionSub.position.z, TargetPosition.z, PositionError) && _runningStage == 0 && IsSetTargetPosition)
            {
                _runningStage = 1;
                PlanningControlInstance.ResetRunningPath();
                IsSetTargetPosition = false;
            }
            else if (CheckDistance(V_UAV_1_PositionSub.position.x, StartPosition.x, PositionError)
                     && CheckDistance(V_UAV_1_PositionSub.position.y, StartPosition.y, PositionError)
                     && CheckDistance(V_UAV_1_PositionSub.position.z, StartPosition.z, PositionError) && _runningStage == 1)
            {
                message.data = 1;
                ResetObjectPublisherInstance.PublishReset(StartPosition);
                PlanningControlInstance.IsStart = false;
                Debug.Log("Finish stage 0, now enter stage 1.");
                ShowPhyiscalUavTrajectory.ClearTrajectory();
                ShowSimulatorUavTrajectory.ClearTrajectory();
                _runningStage = 2;
            }
            else if (CheckDistance(V_UAV_2_PositionSub.position.x, TargetPosition.x, PositionError)
                     && CheckDistance(V_UAV_2_PositionSub.position.y, TargetPosition.y, PositionError)
                     && CheckDistance(V_UAV_2_PositionSub.position.z, TargetPosition.z, PositionError)&&_runningStage == 2)
            { 
                message.data = 0;
                PlanningControlInstance.ResetRunningPath();
                ResetObjectPublisherInstance.PublishReset(StartPosition);
                PlanningControlInstance.IsStart = false;
                _runningStage = 3;
            }
            else if (CheckDistance(V_UAV_1_PositionSub.position.x, StartPosition.x, PositionError)
                     && CheckDistance(V_UAV_1_PositionSub.position.y, StartPosition.y, PositionError)
                     && CheckDistance(V_UAV_1_PositionSub.position.z, StartPosition.z, PositionError) &&
                     _runningStage == 3)
            {
                message.data = 2;
                Debug.Log("Finish stage 1, now enter stage 2.");
                ShowPhyiscalUavTrajectory.ClearTrajectory();
                ShowSimulatorUavTrajectory.ClearTrajectory();
                _runningStage = 4;
            }
            else if (CheckDistance(V_UAV_3_PositionSub.position.x, TargetPosition.x, PositionError)
                     && CheckDistance(V_UAV_3_PositionSub.position.y, TargetPosition.y, PositionError)
                     && CheckDistance(V_UAV_3_PositionSub.position.z, TargetPosition.z, PositionError) &&
                     _runningStage == 4)
            {
                Debug.Log("Finish stage 2, now enter stage 0.");
                _runningStage = 5;
            }
            Publish(message);
            StageId = message.data;

        }

        private bool CheckDistance(float x1, float x2,float error)
        {
            if (Mathf.Abs(x1 - x2) <= error)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CheckDistance(float x1, float x2, float x3, float error)
        {
            if (Mathf.Abs(x1-x2)<=error&&Mathf.Abs(x1-x3)<=error&&Mathf.Abs(x2-x3)<=error)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override void Start()
        {
            base.Start();
            message = new MessageTypes.Std.Int32();
            TargetPosition = new Vector3();
            StartPosition = new Vector3();
        }
        public bool PublishMsg(int isGuideControl)
        {
            message.data = isGuideControl;
            Publish(message);
            return true;
        }
    }
}