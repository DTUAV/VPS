using System;
using System.Collections;
using System.Collections.Generic;
using DTUAVSIM.Sensor.EnvironmentalPerception;
using UnityEngine;

namespace DTUAVCARS.Controller
{
    public class GlobalVirtualGuide
    {
        private Vector3 _physicalObjPosition;//The current position of physical object.
        private Vector3 _controllerOutput;//The mpc controller output.
        private Vector3 _virtualGuideOutput;//The virtual guider output.
        private Vector3 _predictPhysicalObjPosition;//The predicted position of physical object by using controller output or virtual guider output.
        private float _runningTime;//The running time of physical object, for one control signal input. 
        private environment_perception _environmentPerception;                               //The Environment Perception Sensor
        private float _safeDistance;//The safe distance for physical object.
        private float _omiga;//The revised factor for virtual guider output.
        private int _maxRevisedTime;
        private bool _isRevised;
        private float _omigaNow = 0.8f;

        public bool GetIsRevised()
        {
            return _isRevised;
        }
        public Vector3 UpdateGuideOutput(Vector3 physicalObjPosition,Vector3 physicalObjDirection ,Vector3 controllerOutput)
        {
            _physicalObjPosition = physicalObjPosition;
            _controllerOutput = controllerOutput;
            float costX;
            float costY;
            float costZ;
            _predictPhysicalObjPosition = _physicalObjPosition + _controllerOutput * _runningTime;
            _environmentPerception.UpdateSensor(_predictPhysicalObjPosition);
           // _environmentPerception.UpdateObstaceInfo();
            if (_environmentPerception.GetIsSafe(_safeDistance))
            {
               // Debug.Log("Safe-->output controller output!!!");
                _isRevised = true;
                return _controllerOutput;
            }
            else
            {
                _virtualGuideOutput = _controllerOutput;
                float runningTime = 0;
                bool isSafe = false;
                while (!isSafe&&runningTime<_maxRevisedTime)
                {
                    Vector3 cost = physicalObjPosition - _environmentPerception.GetDangerMinDistancePosition();


                   // Debug.Log("minDistance: "+_environmentPerception.GetDangerMinDistance());

                  //  costX = _omiga * (_environmentPerception.GetDangerMinDistance() - _safeDistance) * _environmentPerception.GetMinDistanceDirection().normalized.x;

                   // costX = _omiga * VectorDot(physicalObjDirection.normalized, _environmentPerception.GetMinDistanceDirection().normalized) * (_environmentPerception.GetDangerMinDistance() - _safeDistance) * _environmentPerception.GetMinDistanceDirection().normalized.x;

                   // costY = _omiga  * (_environmentPerception.GetDangerMinDistance() - _safeDistance) * _environmentPerception.GetMinDistanceDirection().normalized.y;

                   // costY = _omiga * VectorDot(physicalObjDirection.normalized, _environmentPerception.GetMinDistanceDirection().normalized) * (_environmentPerception.GetDangerMinDistance() - _safeDistance) * _environmentPerception.GetMinDistanceDirection().normalized.y;


                   // costZ = _omiga * (_environmentPerception.GetDangerMinDistance() - _safeDistance) * _environmentPerception.GetMinDistanceDirection().normalized.z;

                    //costZ = _omiga * VectorDot(physicalObjDirection.normalized, _environmentPerception.GetMinDistanceDirection().normalized) * (_environmentPerception.GetDangerMinDistance() - _safeDistance) * _environmentPerception.GetMinDistanceDirection().normalized.z;
                  //  Debug.Log("CostX: " + costX + " CostY: " + costY + " CostZ: " + costZ);
                  //  if (Mathf.Abs(physicalObjDirection.x) > 10e-6)
                  //  {
                  //      _virtualGuideOutput.x += physicalObjDirection.x / Mathf.Abs(physicalObjDirection.x) * CostFunction(costX);
                  //  }
                  //  if (Mathf.Abs(physicalObjDirection.y) > 10e-6)
                  //  {
                  //      _virtualGuideOutput.y += physicalObjDirection.y / Mathf.Abs(physicalObjDirection.y) * CostFunction(costX);
                  //  }
                   // if (Mathf.Abs(physicalObjDirection.z) > 10e-6)
                   // {
                   //     _virtualGuideOutput.z += physicalObjDirection.z / Mathf.Abs(physicalObjDirection.z) * CostFunction(costX);
                   // }

                   _virtualGuideOutput += cost*_omiga;
                    _physicalObjPosition = physicalObjPosition;
                    _predictPhysicalObjPosition = _physicalObjPosition + _virtualGuideOutput * _runningTime;

                    _environmentPerception.UpdateSensor(_predictPhysicalObjPosition);
                   // _environmentPerception.UpdateObstaceInfo();
                    runningTime++;
                    isSafe = _environmentPerception.GetIsSafe(_safeDistance);
                }
               // Debug.Log("Controller Input: "+ _controllerOutput);
               // Debug.Log("VirtualGuideOutput: "+ _virtualGuideOutput);
                if (isSafe)
                {
                    _isRevised = true;

                }
                else
                {
                    _isRevised = false;
                    _virtualGuideOutput = _omigaNow*(physicalObjPosition - _environmentPerception.GetDangerMinDistancePosition()) * (_safeDistance - Vector3.Distance(physicalObjPosition, _environmentPerception.GetDangerMinDistancePosition())) / Vector3.Distance(physicalObjPosition, _environmentPerception.GetDangerMinDistancePosition());//new control rule to promise the more danger, the output more huge.
                    Debug.Log("only safe guide");
                }


                return _virtualGuideOutput;
            }
        }
        private float VectorDot(Vector3 a, Vector3 b)
        {
            return (a.x * b.x + a.y * b.y + a.z * b.z);
        }
        private float CostFunction(float cost)
        {
            if (cost <= 0.0)
            {
                return cost;
            }
            else
            {
                return 0.0f;
            }
        }

        public GlobalVirtualGuide(float runningTime,environment_perception environmentPerception,float safeDistance,float omiga,int maxRevisedTime)
        {
           
            _runningTime = runningTime;
            _environmentPerception = environmentPerception;
            _safeDistance = safeDistance;
            _omiga = omiga;
            _maxRevisedTime = maxRevisedTime;
            _physicalObjPosition = new Vector3();
            _controllerOutput = new Vector3();
            _virtualGuideOutput = new Vector3();
            _predictPhysicalObjPosition = new Vector3();
            _isRevised = false;

        }
    }
}
