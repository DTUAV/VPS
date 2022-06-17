using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DTUAVCARS.Controller
{
    public class TrackingGuide
    {
        private float _directionThreshold;//The threshold for tracking direction, only the cost of direction is more than it, the tracking guide of direction can work.
        private float _positionThreshold;//The threshold for tracking distance, only the cost of position distance is more than it, the tracking guide of position can work. 
        private float _directionGain;//The gain of direction adjusting in the guide output.
        private float _positionGain;//The gain of position adjusting in the guide output.

        private Vector3 _directionCost;
        private Vector3 _positionCost;

        private Vector3 _trackingCost;

        private Vector3 _virtualLastDir;
        private Vector3 _physicalLastDir;

        private bool _isFirst;
        private bool _isPhysicalOK;
        private bool _isVirtualOK;

        public Vector3 GetPositionCost()
        {
            return _positionCost;
        }
        public Vector3 GetDirectionCost()
        {
            return _directionCost;
        }
        public Vector3 GetTrackingCost()
        {
            return _trackingCost;
        }
        public bool SetPositionGain(float positionGain)
        {
            _positionGain = positionGain;
            return true;
        }
        public bool SetDirectionGain(float directionGain)
        {
            _directionGain = directionGain;
            return true;
        }

        public bool SetPositionThreshold(float positionThreshold)
        {
            _positionThreshold = positionThreshold;
            return true;
        }
        public bool SetDirectionThreshold(float directionThreshold)
        {
            _directionThreshold = directionThreshold;
            return true;
        }



        public float GetDirectionThreshold()
        {
            return _directionThreshold;

        }

        public float GetPositionThreshold()
        {
            return _positionThreshold;
        }

        public float GetDirectionGain()
        {
            return _directionGain;
        }

        public float GetPositionGain()
        {
            return _positionGain;
        }

        public Vector3 UpdateTrackingCost(Vector3 currentPositionObj1, Vector3 predictPositionObj1,
            Vector3 currentPositionObj2, Vector3 currentDirectionObj2,bool direction)
        {
            if (direction)
            {
                if (_isFirst)
                {
                    Vector3 dir_r_tem = (predictPositionObj1 - currentPositionObj1);
                    if (dir_r_tem.x < 10e-6 && dir_r_tem.y < 10e-6 && dir_r_tem.z < 10e-6)
                    {

                    }
                    else
                    {
                        _physicalLastDir = dir_r_tem / dir_r_tem.magnitude;
                        _isPhysicalOK = true;
                    }

                    if (currentDirectionObj2.x < 10e-6 && currentDirectionObj2.y < 10e-6 && currentDirectionObj2.z < 10e-6)
                    {

                    }
                    else
                    {
                        _virtualLastDir = currentDirectionObj2 / currentDirectionObj2.magnitude;
                        _isVirtualOK = true;
                    }

                    if (_isPhysicalOK && _isVirtualOK)
                    {
                        _isFirst = false;
                    }
                }
                else
                {
                    Vector3 dir_v = new Vector3();
                    Vector3 dir_r = new Vector3();

                    Vector3 dir_r_tem = (predictPositionObj1 - currentPositionObj1);
                    if (dir_r_tem.x < 10e-6 && dir_r_tem.y < 10e-6 && dir_r_tem.z < 10e-6)
                    {
                        dir_r = _physicalLastDir;
                    }
                    else
                    {
                        dir_r = dir_r_tem / dir_r_tem.magnitude;
                        _physicalLastDir = dir_r;
                    }

                    if (currentDirectionObj2.x < 10e-6 && currentDirectionObj2.y < 10e-6 && currentDirectionObj2.z < 10e-6)
                    {
                        dir_v = _virtualLastDir;
                    }
                    else
                    {
                        dir_v = currentDirectionObj2 / currentDirectionObj2.magnitude;
                        _virtualLastDir = dir_v;
                    }



                    _directionCost = dir_v - dir_r;
                    _positionCost = currentPositionObj2 - currentPositionObj1;
                    float kdg, kpg;

                    if (VectorDot(dir_r, dir_v) > _directionThreshold)
                    {
                        kdg = 0;
                    }
                    else
                    {
                        kdg = 1;
                    }

                    if (_positionCost.magnitude < _positionThreshold)
                    {
                        kpg = 0;
                    }
                    else
                    {
                        kpg = 1;
                    }

                    _trackingCost = _directionGain * kdg * _directionCost + _positionGain * kpg * _positionCost;
                }
                
            }
            
            return _trackingCost;
        }
        public Vector3 UpdateTrackingCost(Vector3 currentPositionObj1, Vector3 predictPositionObj1,
            Vector3 currentPositionObj2, Vector3 lastPositionObj2)
        {
            if (_isFirst)
            {
                Vector3 dir_r_tem = (predictPositionObj1 - currentPositionObj1);
                if (dir_r_tem.x < 10e-6 && dir_r_tem.y < 10e-6 && dir_r_tem.z < 10e-6)
                {

                }
                else
                {
                    _physicalLastDir = dir_r_tem / dir_r_tem.magnitude;
                    _isPhysicalOK = true;
                }

                if ((currentPositionObj2 - lastPositionObj2).x < 10e-6 && (currentPositionObj2 - lastPositionObj2).y < 10e-6 && (currentPositionObj2 - lastPositionObj2).z < 10e-6)
                {

                }
                else
                {
                    _virtualLastDir = (currentPositionObj2 - lastPositionObj2) / (currentPositionObj2 - lastPositionObj2).magnitude;
                    _isVirtualOK = true;
                }

                if (_isPhysicalOK && _isVirtualOK)
                {
                    _isFirst = false;
                }
            }
            else
            {

                Vector3 dir_r = (predictPositionObj1 - currentPositionObj1) /
                                Vector3.Distance(predictPositionObj1, currentPositionObj1);
                Vector3 dir_v = (currentPositionObj2 - lastPositionObj2) /
                                Vector3.Distance(currentPositionObj2, lastPositionObj2);
                _directionCost = dir_v - dir_r;
                _positionCost = currentPositionObj2 - currentPositionObj1;
                float kdg, kpg;

                if (VectorDot(dir_r, dir_v) > _directionThreshold)
                {
                    kdg = 0;
                }
                else
                {
                    kdg = 1;
                }

                if (_positionCost.magnitude < _positionThreshold)
                {
                    kpg = 0;
                }
                else
                {
                    kpg = 1;
                }

                _trackingCost = _directionGain * kdg * _directionCost + _positionGain * kpg * _positionCost;
            }

            return _trackingCost;
        }

        private float VectorDot(Vector3 data1, Vector3 data2)
        {
            return (data1.x * data2.x + data1.y * data2.y + data1.z * data2.z);
        }


        public TrackingGuide(float directionThreshold,float positionThreshold,float directionGain,float positionGain)
        {
            _directionThreshold = directionThreshold;
            _positionThreshold = positionThreshold;
            _directionGain = directionGain;
            _positionGain = positionGain;
            _directionCost = new Vector3();
            _positionCost = new Vector3();
            _trackingCost = new Vector3();
            _physicalLastDir = new Vector3();
            _virtualLastDir = new Vector3();
            _isFirst = true;
            _isPhysicalOK = false;
            _isVirtualOK = false;
        }

    }
}
