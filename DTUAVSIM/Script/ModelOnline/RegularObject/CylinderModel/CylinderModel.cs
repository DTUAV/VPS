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
using UnityEditor.Experimental.GraphView;
using UnityEngine.PlayerLoop;

namespace DTUAV.ModelOnline.RegularObject.CylinderModel
{
    public class CylinderModel
    {
        private float _height;
        private float _radius;
        private string _layer;
        private ReadTxtFile _readTxtFileNode;
        private List<Vector3> _cylinderModelPositions;
        private List<GameObject> _cylinderGameObjects;

        public bool UpdateHeight(float height)
        {
            _height = height;
            Update();
            return true;
        }

        public bool UpdateRadius(float radius)
        {
            _radius = radius;
            Update();
            return true;
        }

        public bool UpdateLayer(string layer)
        {
            _layer = layer;
            Update();
            return true;
        }
        public CylinderModel(float height,float radius, string layer,string txtFileName)
        {
            _height = height;
            _radius = radius;
            _layer = layer;
            _readTxtFileNode = new ReadTxtFile(txtFileName);
            _cylinderModelPositions = _readTxtFileNode.Get3DPosition();
            Vector3 tem = new Vector3(_radius,_height/2, _radius);
            _cylinderGameObjects = new List<GameObject>();
            int i = 0;
            foreach (var pos in _cylinderModelPositions)
            {
                GameObject cy1 = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                cy1.transform.localScale = tem;
                cy1.transform.position = pos;
                cy1.name = "vps_obstacle_" + i;
                cy1.layer = LayerMask.NameToLayer(_layer);
                _cylinderGameObjects.Add(cy1);
                i++;
            }
        }

        public void Update()
        {
            Vector3 tem = new Vector3(_radius, _height / 2, _radius);
            if (_cylinderGameObjects.Count == _cylinderModelPositions.Count)
            {
                for (int i = 0; i < _cylinderGameObjects.Count; i++)
                {
                    _cylinderGameObjects[i].transform.position = _cylinderModelPositions[i];
                    _cylinderGameObjects[i].transform.localScale = tem;
                    _cylinderGameObjects[i].layer = LayerMask.NameToLayer(_layer);
                }
                Debug.Log("Update all objects.");
            }
            else if (_cylinderGameObjects.Count>_cylinderModelPositions.Count)
            {
                for (int i = 0; i < _cylinderModelPositions.Count; i++)
                {
                    _cylinderGameObjects[i].transform.position = _cylinderModelPositions[i];
                    _cylinderGameObjects[i].transform.localScale = tem;
                    _cylinderGameObjects[i].layer = LayerMask.NameToLayer(_layer);
                }

                Debug.Log("Update the number of object is " + _cylinderModelPositions.Count + ".");
            }
            else
            {
                for(int i = 0; i < _cylinderGameObjects.Count; i++)
                {
                    _cylinderGameObjects[i].transform.position = _cylinderModelPositions[i];
                    _cylinderGameObjects[i].transform.localScale = tem;
                    _cylinderGameObjects[i].layer = LayerMask.NameToLayer(_layer);
                }
                Debug.Log("Update the number of object is " + _cylinderGameObjects.Count + ".");
            }

           
        }
        


    }
}
