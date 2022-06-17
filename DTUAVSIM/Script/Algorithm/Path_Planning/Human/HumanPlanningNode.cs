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
using UnityEngine;

namespace DTUAV.Algorithm_Module.Path_Planning.Human
{
    public class HumanPlanningNode : MonoBehaviour
    {
        [Header("Draw Line")]
        public Material ObjectMoveMat;
        public Color ObjectMoveColor;
        public Material PathMat;
        public Color PathColor;
        public float WidthX;
        public float WidthY;
        public bool IsPlot;

        [Header("Human Planning")]
        public Transform PlanningObject;//The object is used to moved in virtual scene by operator.
        public float PlotLineHz;       //The frequency of draw the line.
        public float Height;

        [Header("Sensor")]
        public LayerMask LayerMask;
        public float MinRange;
        public float MaxRange;
        public float SafeRange;//the safe range
        public Vector3 CurrentPosition;
        public float HorizontalAngleStart;
        public float HorizontalAngleEnd;
        public float HorizontalAngleInc;
        public bool ShowLidar;


        [Header("RRT")]
        public float StepDetal;//the step to move
        public long MaxFindStep;//the max step to find the path
        public float FindError;
        public float RangeProbability;
        public Vector2 StartPosition;
        public Vector2 TargetPosition;
        public Vector2 WorkMinRange;
        public Vector2 WorkMaxRange;
        public Material RRTMat;
        public Color RRTColor;


        private HumanPlanning _humanPlanning;

        private bool _isRunningPlotLine;
        public bool IsOtherControl = false;
        public List<Vector2> _smoothPath;
        public List<Vector2> _fustionPath;

        public bool GetIsFusionSuccess()
        {
            return _humanPlanning.GetIsFusionSuccess();
        }
        public bool ClearAllPathLine()
        {
            _humanPlanning.ClearAllLine();
            return true;
        }
        public bool StopPlotObjectPath(bool data)
        {
            _humanPlanning.StopRunningPlotLine(data);
            return true;
        }
        public float GetFusionPathTimeCost()
        {
            return _humanPlanning.GetFusionPathTimeCost();
        }

        public float GetFusionPathSafeCost()
        {
            return _humanPlanning.GetFusionPathSafeCost();
        }
        public float GetHumanPathTimeCost()
        {
            return _humanPlanning.GetTimeCost();
        }

        public float GetHumanPathSafeCost()
        {
            return _humanPlanning.GetSafeCost();
        }

        [System.Obsolete]
        public List<Vector2> GetFusionPath()
        {
            _smoothPath = _humanPlanning.GetSmoothPath();
            Debug.Log("timeCost: " + _humanPlanning.GetTimeCost());
            Debug.Log("Safe Cost: " + _humanPlanning.GetSafeCost());
            Debug.Log("danger position number: " + _humanPlanning.GetDangerPositionIndexes().Count);
            _fustionPath = _humanPlanning.FusionPath(0.5f);
            Debug.Log("soomthPath: " + _smoothPath.Count);
            Debug.Log("fustionPath: " + _fustionPath.Count);
            Debug.Log("fusion path time cost: " + _humanPlanning.GetFusionPathTimeCost());
            Debug.Log("fusion path safe cost: " + _humanPlanning.GetFusionPathSafeCost());
            return _fustionPath;
        }
        public List<Vector2> GetHumanPath()
        {
            _smoothPath = _humanPlanning.GetSmoothPath();
            Debug.Log("timeCost: " + _humanPlanning.GetTimeCost());
            Debug.Log("Safe Cost: " + _humanPlanning.GetSafeCost());
            Debug.Log("danger position number: " + _humanPlanning.GetDangerPositionIndexes().Count);
            Debug.Log("soomthPath: " + _smoothPath.Count);
            return _smoothPath;
        }

        // Start is called before the first frame update
        [System.Obsolete]
        void Start()
        {
            _isRunningPlotLine = false;
            _humanPlanning = new HumanPlanning(PlanningObject,PlotLineHz, Height);
            _humanPlanning.InitPlotPlanningObjectLine(ObjectMoveMat, ObjectMoveColor, WidthX, WidthY, IsPlot);
            _humanPlanning.InitPlotSmoothPathLine(PathMat, PathColor, WidthX, WidthY, IsPlot);
            _humanPlanning.InitSensor(LayerMask, MinRange, MaxRange, CurrentPosition, HorizontalAngleStart, HorizontalAngleEnd,
                HorizontalAngleInc, SafeRange, ShowLidar);
            _humanPlanning.InitRRT(StepDetal, MaxFindStep, StartPosition, TargetPosition, WorkMinRange, WorkMaxRange,SafeRange, FindError, RangeProbability, Height, LayerMask, MinRange, MaxRange, CurrentPosition,HorizontalAngleStart, HorizontalAngleEnd,HorizontalAngleInc, ShowLidar, RRTMat, RRTColor, WidthX, WidthY, IsPlot);
            _smoothPath = new List<Vector2>();
        }

        // Update is called once per frame

        void Update()
        {
            if (!IsOtherControl)
            {

                if (Input.GetMouseButtonDown(0))
                {
                    if (_isRunningPlotLine)
                    {
                        _isRunningPlotLine = false;
                        _smoothPath = _humanPlanning.GetSmoothPath();
                        Debug.Log("timeCost: " + _humanPlanning.GetTimeCost());
                        Debug.Log("Safe Cost: " + _humanPlanning.GetSafeCost());
                        Debug.Log("danger position number: " + _humanPlanning.GetDangerPositionIndexes().Count);
                        _fustionPath = _humanPlanning.FusionPath(0.5f);
                        Debug.Log("soomthPath: " + _smoothPath.Count);
                        Debug.Log("fustionPath: " + _fustionPath.Count);
                        Debug.Log("fusion path time cost: " + _humanPlanning.GetFusionPathTimeCost());
                        Debug.Log("fusion path safe cost: " + _humanPlanning.GetFusionPathSafeCost());
                    }
                    else
                    {
                        _isRunningPlotLine = true;
                    }

                    _humanPlanning.StopRunningPlotLine(_isRunningPlotLine);


                    Debug.Log("Running Plot Object Line---" + _isRunningPlotLine);
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (!_isRunningPlotLine)
                    {
                        _humanPlanning.ClearAllLine();
                    }
                }

            }
        }
        void OnDestroy()
        {
            _humanPlanning.StopPlotLine();
        }

    }



}