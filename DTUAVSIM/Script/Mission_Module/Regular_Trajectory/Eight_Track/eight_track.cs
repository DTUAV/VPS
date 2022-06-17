/*
 * Author: Yuanlin Yang
 * Date: 2022-1-3
 * Location: Guangdong University of Technology
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DTUAVCARS.DTMission
{
    /*------------>How To Use This Code<-----------------------------
     * (1)include the namespace: DTUAVCARS.DTMission
     * (2)define a class instance : private eight_track _eightTrack = new eight_track(Vector3 center,float width,float lendth,float heigh,bool isLoop,float positionError,int planeId)
     * (3)check if configure finnish:bool isFinnish = _eightTrack.GetIsGetNewPose()
     * (4)update && get data:1)Vector3 targetPosition = _eightTrack.GetTargetPosition(Vector3 currentPosition)
     */
    public class eight_track
    {
        private Vector3 _center;   //The Center Position of Eight Track
        private float _width;     //The Width of Eight ---Y
        private float _lendth;   //The Lendth of Eight ---X
        private float _heigh;   //The Heigh of Eight ---Z
        private bool _isLoop;   //If loop the Eight Track
        private float _positionError;//The Position Change Error
        private int _planeId;//The Motion Plane:0:X-Y,1:X-Z.2:Y-Z,other:X-Y
        private List<Vector3> _targetPositions;//The All Target Positions
        private int _nowPositionId;//The Target Position Id 
        private bool _isGetNewPose;//The flag is show all operate if finish

        public bool UpdatePlaneId(int planeId)
        {
            switch (_planeId)
            {
                case 0://X-Y
                {
                    float t = 0.0f;
                    for (int i = 0; i < 40; i++)
                    {
                        
                        _targetPositions[i] = _center + new Vector3(_lendth * Mathf.Sin(Mathf.PI * t ) ,_width*Mathf.Cos(20*Mathf.PI * t),0);
                        t = t + 0.05f;
                    }
                    return true;
                    }

                case 1: //X-Z
                    {
                        if (_center.z -  _heigh > 0.5f)
                        {
                            float t = 0.0f;
                            for (int i = 0; i < 40; i++)
                            {
                                _targetPositions[i] = _center + new Vector3(_lendth * Mathf.Sin(Mathf.PI * t), 0, _heigh * Mathf.Cos(20 * Mathf.PI * t));
                                t = t + 0.05f;
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 2://Y-Z
                    {
                        if (_center.z - _heigh > 0.5f)
                        {
                            float t = 0.0f;
                            for (int i = 0; i < 40; i++)
                            {
                                _targetPositions[i] = _center + new Vector3(0, _width * Mathf.Sin(Mathf.PI * t), _heigh * Mathf.Cos(20 * Mathf.PI * t ));
                                t = t + 0.05f;
                            }
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default://X-Y     
                    {
                        float t = 0.0f;
                        for (int i = 0; i < 40; i++)
                        {
                            _targetPositions[i] = _center + new Vector3(_lendth * Mathf.Sin(Mathf.PI * t), _width * Mathf.Cos(20 * Mathf.PI * t), 0);
                            t = t + 0.05f;
                        }
                        return true;
                    }
            }


        }
        public bool UpdateCenter(Vector3 center)
        {
            _center = center;
            _isGetNewPose = UpdatePlaneId(_planeId);
            return _isGetNewPose;
        }

        public bool UpdateIsLoop(bool isLoop)
        {
            _isLoop = isLoop;
            return true;
        }

        public bool UpdatePositionError(float positionError)
        {
            _positionError = positionError;
            return true;
        }
        public bool UpdateLendth(float lendth)
        {
            _lendth = lendth;
            return UpdatePlaneId(_planeId);
        }

        public bool UpdateWidth(float width)
        {
            _width = width;
            return UpdatePlaneId(_planeId);
        }

        public bool UpdateHeigh(float heigh)
        {
            _heigh = heigh;
            return UpdatePlaneId(_planeId);
        }

        public bool GetIsGetNewPose()
        {
            return _isGetNewPose;
        }

        public eight_track(Vector3 center, float width, float lendth, float heigh, bool isLoop, float positionError, int planeId)
        {
            _center = center;
            _width = width;
            _lendth = lendth;
            _heigh = heigh;
            _isLoop = isLoop;
            _positionError = positionError;
            _planeId = planeId;
            _nowPositionId = 0;
            _targetPositions = new List<Vector3>(40);
            for (int i = 0; i < 40; i++)
            {
                _targetPositions.Add(Vector3.zero);
            }
            _isGetNewPose = UpdatePlaneId(_planeId);

        }

        public Vector3 GetTargetPosition(Vector3 currentPosition)
        {
            if (Mathf.Abs(_targetPositions[_nowPositionId].x - currentPosition.x) <= _positionError && Mathf.Abs(_targetPositions[_nowPositionId].y - currentPosition.y) <= _positionError && Mathf.Abs(_targetPositions[_nowPositionId].z - currentPosition.z) <= _positionError)
            {
                _nowPositionId = _nowPositionId + 1;
                if (_nowPositionId > 39)
                {
                    if (_isLoop)
                    {
                        _nowPositionId = 0;
                    }
                    else
                    {
                        _nowPositionId = 39;
                    }
                }

                return _targetPositions[_nowPositionId];
            }
            else
            {
                return _targetPositions[_nowPositionId];
            }
        }
    }
}