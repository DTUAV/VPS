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

namespace DTUAV.ModelOnline.RegularObject.CylinderModel
{
    public class CylinderModelNode : MonoBehaviour
    {
        [Header("The height of Cylinder Models.")]
        public float Height;

        [Header("The Radius of Cylinder Models.")]
        public float Radius;

        [Header("The Layer of Cylinder Models.")]
        public string Layer;

        [Header("The Name of Cylinder Model Position File.")]
        public string FileName;

        private CylinderModel _cylinderModelNode;
        // Start is called before the first frame update
        void Start()
        {
            _cylinderModelNode = new CylinderModel(Height,Radius,Layer,FileName);
        }

    }
}