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
using UnityEngine;
using DTUAV.FileOperator.Txt;

namespace DTUAV.ModelOnline.RegularObject.CubeModel
{
    public class CubeModel
    {
        private float _height;
        private float _length;
        private float _width;
        private string _layer;
        private ReadTxtFile _readTxtFileNode;
        private List<Vector3> _cubeModelPositions;
        private List<GameObject> _cubeGameObjects;
        private int _modelNum;

        public bool UpdateModel(List<Vector3> cubeModelPositions)
        {
            _cubeModelPositions = cubeModelPositions;
            Vector3 tem = new Vector3(_length, _height, _width);
            if (_cubeGameObjects.Count == _cubeModelPositions.Count)
            {
                for (int i = 0; i < _cubeGameObjects.Count; i++)
                {
                    _cubeGameObjects[i].transform.position = _cubeModelPositions[i];
                    _cubeGameObjects[i].transform.localScale = tem;
                    _cubeGameObjects[i].layer = LayerMask.NameToLayer(_layer);
                }
                Debug.Log("Update all objects.");
            }
            else if (_cubeGameObjects.Count > _cubeModelPositions.Count)
            {
                for (int i = 0; i < _cubeModelPositions.Count; i++)
                {
                    _cubeGameObjects[i].transform.position = _cubeModelPositions[i];
                    _cubeGameObjects[i].transform.localScale = tem;
                    _cubeGameObjects[i].layer = LayerMask.NameToLayer(_layer);
                }

                Debug.Log("Update the number of object is " + _cubeModelPositions.Count + ".");
            }
            else
            {
                for (int i = 0; i < _cubeGameObjects.Count; i++)
                {
                    _cubeGameObjects[i].transform.position = _cubeModelPositions[i];
                    _cubeGameObjects[i].transform.localScale = tem;
                    _cubeGameObjects[i].layer = LayerMask.NameToLayer(_layer);
                }
                Debug.Log("Update the number of object is " + _cubeGameObjects.Count + ".");
            }

            return true;
        }

        public CubeModel(float height,float length, float width,string layer,int modelNum)
        {
            _modelNum = modelNum;
            _height = height;
            _length = length;
            _width = width;
            _layer = layer;
            _cubeGameObjects = new List<GameObject>(modelNum);
            _cubeModelPositions = new List<Vector3>(modelNum);
            Vector3 tem = new Vector3(_length, _height, _width);
            Vector3 pos = Vector3.zero;
            for (int i = 0; i < modelNum; ++i)
            {
                GameObject cy1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cy1.transform.localScale = tem;
                cy1.transform.position = pos;
                cy1.name = "vps_obstacle_" + i;
                cy1.layer = LayerMask.NameToLayer(_layer);
                _cubeGameObjects.Add(cy1);
                _cubeModelPositions.Add(pos);
            }
        }

        public CubeModel(float height, float length, float width, string layer, string txtFileName)
        {
            _height = height;
            _length = length;
            _width = width;
            _layer = layer;
            _readTxtFileNode = new ReadTxtFile(txtFileName);
            _cubeModelPositions = _readTxtFileNode.Get3DPosition();
            Vector3 tem = new Vector3(_length, _height, _width);
            _cubeGameObjects = new List<GameObject>();
            int i = 0;
            foreach (var pos in _cubeModelPositions)
            {
                GameObject cy1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cy1.transform.localScale = tem;
                cy1.transform.position = pos;
                cy1.name = "vps_obstacle_" + i;
                cy1.layer = LayerMask.NameToLayer(_layer);
                _cubeGameObjects.Add(cy1);
                i++;
            }
        }
    }
}
