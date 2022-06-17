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

public class MoveObject : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform ObjectTransform;
    public bool IsInitOriginPosition = false;
    public Transform VirtualUav;
    public float MoveInc;
    private float _moveStep;
    public float MoveStepFactor;
    private Vector3 _lastPosition;
    void Start()
    {
        _moveStep = 0.0f;
        if (IsInitOriginPosition)
        {
            ObjectTransform.position = VirtualUav.position;
        }
        _lastPosition = ObjectTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _moveStep += MoveInc* MoveStepFactor;
            _lastPosition.z += _moveStep;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        { 
            _moveStep+=MoveInc;
            _lastPosition.z += _moveStep;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            _moveStep = 0.0f;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _moveStep -= MoveInc * MoveStepFactor;
            _lastPosition.z += _moveStep;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            _moveStep -= MoveInc;
            _lastPosition.z += _moveStep;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            _moveStep = 0.0f;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _moveStep -= MoveInc * MoveStepFactor;
            _lastPosition.x += _moveStep;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _moveStep -= MoveInc;
            _lastPosition.x += _moveStep;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            _moveStep = 0.0f;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _moveStep += MoveInc * MoveStepFactor;
            _lastPosition.x += _moveStep;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _moveStep += MoveInc;
            _lastPosition.x += _moveStep;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            _moveStep = 0.0f;
        }

        ObjectTransform.position = _lastPosition;
    }
}
