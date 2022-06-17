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
     * (2)define a class instance : private square_track _squareTrack = new square_track(Vector3 center,float width,float lendth,float heigh,bool isLoop,float positionError,int planeId)
     * (3)check if configure finnish:bool isFinnish = _squareTrack.GetIsGetNewPose()
     * (4)update && get data:1)Vector3 targetPosition = _squareTrack.GetTargetPosition(Vector3 currentPosition)
     */
    public class square_track
    {
        private Vector3 _center;   //The Center Position of Square Track
        private float _width;     //The Width of Square ---Y
        private float _lendth;   //The Lendth of Square ---X
        private float _heigh;   //The Heigh of Square ---Z
        private bool _isLoop;   //If loop the Square Track
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
                        _targetPositions[0] = _center + new Vector3(-0.5f * _lendth, -0.5f * _width, 0);
                        _targetPositions[1] = _center + new Vector3(-0.5f * _lendth, 0.5f * _width, 0);
                        _targetPositions[2] = _center + new Vector3(0.5f * _lendth, 0.5f * _width, 0);
                        _targetPositions[3] = _center + new Vector3(0.5f * _lendth, -0.5f * _width, 0);
                        return true;
                    }
                   
                case 1: //X-Z
                    {
                        if (_center.z - 0.5f * _heigh > 0.5f)
                        {
                            _targetPositions[0] = _center + new Vector3(-0.5f * _lendth, 0, -0.5f * _heigh);
                            _targetPositions[1] = _center + new Vector3(-0.5f * _lendth, 0, 0.5f * _heigh);
                            _targetPositions[2] = _center + new Vector3(0.5f * _lendth, 0, 0.5f * _heigh);
                            _targetPositions[3] = _center + new Vector3(0.5f * _lendth, 0, -0.5f * _heigh);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                case 2://Y-Z
                    {
                        if (_center.z - 0.5f * _heigh > 0.5f)
                        {
                            _targetPositions[0] = _center + new Vector3(0, -0.5f * _width, -0.5f * _heigh);
                            _targetPositions[1] = _center + new Vector3(0, -0.5f * _width, 0.5f * _heigh);
                            _targetPositions[2] = _center + new Vector3(0, 0.5f * _width, 0.5f * _heigh);
                            _targetPositions[3] = _center + new Vector3(0, 0.5f * _width, -0.5f * _heigh);
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                default://X-Y     
                    {
                        _targetPositions[0] = _center + new Vector3(-0.5f * _lendth, -0.5f * _width, 0);
                        _targetPositions[1] = _center + new Vector3(-0.5f * _lendth, 0.5f * _width, 0);
                        _targetPositions[2] = _center + new Vector3(0.5f * _lendth, 0.5f * _width, 0);
                        _targetPositions[3] = _center + new Vector3(0.5f * _lendth, -0.5f * _width, 0);
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

        public square_track(Vector3 center,float width,float lendth,float heigh,bool isLoop,float positionError,int planeId)
        {
            _center = center;
            _width = width;
            _lendth = lendth;
            _heigh = heigh;
            _isLoop = isLoop;
            _positionError = positionError;
            _planeId = planeId;
            _nowPositionId = 0;
            _targetPositions = new List<Vector3>(4);
            for (int i = 0; i < 4; i++)
            {
                _targetPositions.Add(Vector3.zero);
            }
            _isGetNewPose =  UpdatePlaneId(_planeId);

        }

        public Vector3 GetTargetPosition(Vector3 currentPosition)
        {
            if (Mathf.Abs(_targetPositions[_nowPositionId].x - currentPosition.x)<=_positionError&& Mathf.Abs(_targetPositions[_nowPositionId].y - currentPosition.y) <= _positionError && Mathf.Abs(_targetPositions[_nowPositionId].z - currentPosition.z) <= _positionError)
            {
                _nowPositionId = _nowPositionId + 1;
                if (_nowPositionId>3)
                {
                    if (_isLoop)
                    {
                        _nowPositionId = 0;
                    }
                    else
                    {
                        _nowPositionId = 3;
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