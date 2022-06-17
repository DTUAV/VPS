/*
 * Author: Yuanlin Yang
 * Date: 2022-1-3
 * Location: Guangdong University of Technology
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVCARS.UAV;
using Unity.Collections;
using DTUAVCARS.TF;
namespace DTUAVCARS.DTPlanning
{
    public class velocity_command : MonoBehaviour
    {
        // Start is called before the first frame update
        public Rigidbody ObjectRb;
        public float K;
        public float MaxVelocityX;
        public float MaxVelocityY;
        public float MaxVelocityZ;
        public Vector3 TargetPositionRos;
        public VelocityControl VecControl;
        private position_mission _positionMission;
        void Start()
        {
            _positionMission = new position_mission(K,MaxVelocityX,MaxVelocityY,MaxVelocityZ);
            _positionMission.SetCurrentPosition(TF.TF.Unity2Ros(ObjectRb.position));
            _positionMission.SetTargetPosition(TargetPositionRos);
        }

        // Update is called once per frame
        void Update()
        {
            _positionMission.SetCurrentPosition(TF.TF.Unity2Ros(ObjectRb.position));
            _positionMission.SetTargetPosition(TargetPositionRos);
            VecControl.RefVelocityRos = _positionMission.GetVelocityCommand();
        }
    }
}
