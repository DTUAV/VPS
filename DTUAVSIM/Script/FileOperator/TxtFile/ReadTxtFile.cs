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
using System.Runtime.InteropServices.WindowsRuntime;
using MavLink;
using UnityEngine;

namespace DTUAV.FileOperator.Txt
{
    public class ReadTxtFile
    {
        private string _fileName;
        private List<string[]> _fileData;
        private bool _isRead;

        public ReadTxtFile(string fileName)
        {
            _fileName = fileName;
            _isRead = false;
        }

        public List<Vector3> Get3DPosition()
        {
            List<string[]> txtData = GetTxtData();
            List<Vector3> ret = new List<Vector3>();
            if (_isRead)
            {
                Vector3 tem = new Vector3();
                foreach (var str in txtData)
                {
                    tem.x = float.Parse(str[0]);
                    tem.y = float.Parse(str[1]);
                    tem.z = float.Parse(str[2]);
                    ret.Add(tem);
                }

                Debug.Log("Get 3D Position Successfully!!!");
            }
            else
            {
                Debug.Log("Get 3D Position Fail !!!");
            }

            return ret;
        }

        public List<string[]> GetTxtData()
        {
            if (_fileName != "")
            {
                TextAsset txtData = Resources.Load(_fileName) as TextAsset;
                if (txtData != null)
                {
                    Debug.Log("Read Txt File Successfully!!!");
                    _fileData = LineToStr(TexToLine(txtData));
                    _isRead = true;
                }
                else
                {
                    Debug.Log("Read Txt File Fail !!!");
                }
            }
            else
            {
                Debug.Log("The Txt File Name is Invalid !!!");
            }

            return _fileData;
        }

        private string[] TexToLine(TextAsset waypointTxt)
        {
            if (waypointTxt == null)
            {
                return null;
            }
            else
            {
                string[] str = waypointTxt.text.Split('\n');
                return str;
            }
        }

        private List<string[]> LineToStr(string[] str)
        {
            if (str == null)
            {
                return null;
            }
            else
            {
                List<string[]> StrList = new List<string[]>();
                foreach (string strs in str)
                {
                    StrList.Add(strs.Split(','));
                }

                return StrList;
            }
        }
    }
}
