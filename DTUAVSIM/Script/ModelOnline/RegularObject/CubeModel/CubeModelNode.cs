/*
© Guandong Univesity of Technology , 2022
Author: Yuanlin Yang (yongwang0808@163.com)

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
using System.Collections;
using System.Collections.Generic;
using RosSharp;
using UnityEngine;

namespace DTUAV.ModelOnline.RegularObject.CubeModel
{
    public class CubeModelNode : MonoBehaviour
    {
        [Header("The Length of Cube Models.")]
        public float Length;

        [Header("The Width of Cube Models.")]
        public float Width;

        [Header("The Height of Cube Models.")]
        public float Height;

        [Header("The Layer of Cube Models.")]
        public string Layer;

        [Header("The Number of Cube Models.")] 
        public int ModelNum;

        [Header("The Position of Cube Models Under UWB Position System")]
        public List<Vector3> ModelPositionsWithRos;

        [Header("The Position of UAV Under UWB Position System")]
        public Vector3 UavPositionWithRos;

        [Header("IsUpdateModel")] 
        public bool IsUpdate;

        private CubeModel _cubeModelNode;

        // Start is called before the first frame update
        void Start()
        {
            _cubeModelNode = new CubeModel(Height,Length,Width,Layer,ModelNum);
            IsUpdate = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (IsUpdate)
            {
                Vector3 uavPositionWithUnity = UavPositionWithRos.Ros2Unity();
                uavPositionWithUnity.y = 0;
                List<Vector3> cubePositionsWithUnity = new List<Vector3>(ModelNum);
                for (int i = 0; i < ModelNum; i++)
                {
                    cubePositionsWithUnity.Add(ModelPositionsWithRos[i].Ros2Unity()-uavPositionWithUnity);
                }

                _cubeModelNode.UpdateModel(cubePositionsWithUnity);

                IsUpdate = false;
            }
        }
    }
}