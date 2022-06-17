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
using DTUAV.UAV_Module.Quadrotor;
namespace DTUAVCARS.DTPlanning
{
    public class VelocityCommand : MonoBehaviour
    {
        // Start is called before the first frame update
        public Rigidbody ObjectRb;
        public float K;
        public float MaxVelocityX;
        public float MaxVelocityY;
        public float MaxVelocityZ;
        public Vector3 TargetPositionRos;
        public MotionWithVelocityNode VecControl;
        public bool IsStop = false;
        private position_mission _positionMission;
        void Start()
        {
            _positionMission = new position_mission(K, MaxVelocityX, MaxVelocityY, MaxVelocityZ);
            _positionMission.SetCurrentPosition(TF.TF.Unity2Ros(ObjectRb.position));
            _positionMission.SetTargetPosition(TargetPositionRos);
        }

        // Update is called once per frame
        void Update()
        {
            if (IsStop)
            {
                _positionMission.SetCurrentPosition(TF.TF.Unity2Ros(ObjectRb.position));
                _positionMission.SetTargetPosition(TF.TF.Unity2Ros(ObjectRb.position));
                VecControl.targetVelocity = _positionMission.GetVelocityCommand();
            }
            else
            {
                _positionMission.SetCurrentPosition(TF.TF.Unity2Ros(ObjectRb.position));
                _positionMission.SetTargetPosition(TargetPositionRos);
                VecControl.targetVelocity = _positionMission.GetVelocityCommand();
            }
            
        }
    }
}