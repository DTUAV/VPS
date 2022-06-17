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

namespace DTUAV.Algorithm_Module.Path_Planning.RRT
{
    public class RRTNode : MonoBehaviour
    {
        [Header("RRT")]
        public float stepDetal;//the step to move
        public long maxFindStep;//the max step to find the path
        public float safeRange;//the safe range
        public float findError;
        public float rangeProbability;
        public Vector2 startPosition;
        public Vector2 targetPosition;
        public Vector2 workMinRange;
        public Vector2 workMaxRange;
        public float height;

        [Header("Sensor")]
        public LayerMask layerMask;
        public float minRange;
        public float maxRange;
        public Vector3 currentPosition;
        public float horizontalAngleStart;
        public float horizontalAngleEnd;
        public float horizontalAngleInc;
        public bool showLidar;

        [Header("Draw Line")] 
        public Material mat;
        public Color color;
        public float widthX;
        public float widthY;
        public bool isPlot;

        private RRT2D _rrt2D;

        public bool isGetPath;
        public bool IsOtherControl = false;
        public Rigidbody UavRigidbody;
        public Transform TargetObjectTransform;

        public List<Vector2> smoothPath;
        private bool _isFind;

        public float GetPathSafeCost()
        {
            return _rrt2D.GetPathSafeCost();
        }
        public float GetPathTimeCost()
        {
            return _rrt2D.GetPathTimeCost();
        }
        [System.Obsolete]
        public bool StartFindPath()
        {
            _isFind = true;
            startPosition = new Vector2(UavRigidbody.position.x, UavRigidbody.position.z);
            _isFind = _rrt2D.FindPath(startPosition, new Vector2(TargetObjectTransform.position.x, TargetObjectTransform.position.z));
            if(_isFind)
            {
                smoothPath = _rrt2D.GetSmoothPath();
            }
            else
            {
                _isFind = false;
            }

            return _isFind;
        }
        public List<Vector2> GetSmoothPath()
        {
            return smoothPath;
        }

        public bool ClearAllLine()
        {
            _rrt2D.ClearShowPath();
            return true;
        }
        // Start is called before the first frame update
        [System.Obsolete]
        void Start()
        {
            startPosition = new Vector2(UavRigidbody.position.x, UavRigidbody.position.z);
            isGetPath = false;
            _rrt2D = new RRT2D(stepDetal, maxFindStep,startPosition, targetPosition, workMinRange, workMaxRange, safeRange, findError, rangeProbability, height);
            _rrt2D.InitSensor(layerMask, minRange, maxRange, currentPosition, horizontalAngleStart, horizontalAngleEnd,
                horizontalAngleInc, safeRange, showLidar);
            _rrt2D.InitPlotLine(mat, color, widthX, widthY, isPlot);
           // _rrt2D.FindPath(startPosition, targetPosition);
           // Debug.Log(_rrt2D.GetIsFind());
           // Debug.Log(_rrt2D.GetPath());
        }

        // Update is called once per frame
        [System.Obsolete]
        void Update()
        {
            if (isGetPath&&!IsOtherControl)
            {
                startPosition = new Vector2(UavRigidbody.position.x, UavRigidbody.position.z);
                _rrt2D.FindPath(startPosition, new Vector2(TargetObjectTransform.position.x, TargetObjectTransform.position.z));
                smoothPath = _rrt2D.GetSmoothPath();
                isGetPath = false;
            }
           
        }
    }
}
