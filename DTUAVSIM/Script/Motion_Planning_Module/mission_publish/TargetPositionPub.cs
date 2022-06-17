using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAV.Algorithm_Module.Path_Planning.RRT;
namespace DTUAVCARS.DTPlanning
{
    public class TargetPositionPub : MonoBehaviour
    {
        public RRTNode PathNode;//the rrt node instance for find path
        public VelocityCommand PubVelocityCommand;//the publisher for transferring the target position to velocity command
        public bool StartTestPath;
        public bool NextTargetPosition;
        public Rigidbody VirtualUAV1;
        public float AutoSwitchError;
        private List<Vector2> _originPath;

        private int _pathIndex;

        private Vector2 _currentPosition;

        private Vector2 _currentTargetPosition;
        // Start is called before the first frame update
        void Start()
        {
            StartTestPath = false;
            NextTargetPosition = false;
            _pathIndex = 0;
            _originPath = new List<Vector2>();
            _currentPosition = new Vector2(VirtualUAV1.position.x,VirtualUAV1.position.z);
            _currentTargetPosition = new Vector2();
        }

        // Update is called once per frame
        void Update()
        {
            if (StartTestPath)
            {
                _originPath = PathNode.GetSmoothPath();
                StartTestPath = false;
                _pathIndex = 0;
                _currentTargetPosition.x = VirtualUAV1.position.x;
                _currentTargetPosition.y = VirtualUAV1.position.z;
            }

            if (_pathIndex<_originPath.Count)
            {
                _currentPosition.x = VirtualUAV1.position.x;
                _currentPosition.y = VirtualUAV1.position.z;
                if (Vector2.Distance(_currentPosition,_currentTargetPosition)<=AutoSwitchError&& NextTargetPosition)
                {
                    Vector3 targetPositionUnity = new Vector3(_originPath[_pathIndex].x, 0, _originPath[_pathIndex].y);
                    Vector3 targetPositionRos = TF.TF.Unity2Ros(targetPositionUnity);
                    PubVelocityCommand.TargetPositionRos.x = targetPositionRos.x;
                    PubVelocityCommand.TargetPositionRos.y = targetPositionRos.y;
                    _currentTargetPosition = _originPath[_pathIndex];
                    _pathIndex++;
                    // NextTargetPosition = false;
                }

            }

            

        }
    }
}
