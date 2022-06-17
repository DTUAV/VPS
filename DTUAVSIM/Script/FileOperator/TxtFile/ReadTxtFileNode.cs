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

namespace DTUAV.FileOperator.Txt
{
    public class ReadTxtFileNode : MonoBehaviour
    {
        public string FileName;

        private ReadTxtFile _readTxtFileNode;
        // Start is called before the first frame update
        void Start()
        {
            _readTxtFileNode = new ReadTxtFile(FileName);
            List<Vector3> positionData = _readTxtFileNode.Get3DPosition();
            Debug.Log("data size: "+positionData.Count);
        }

    }
}
