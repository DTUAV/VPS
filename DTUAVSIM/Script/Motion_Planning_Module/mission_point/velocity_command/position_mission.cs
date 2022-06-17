/*
 * Author: Yuanlin Yang
 * Date: 2022-1-3
 * Location: Guangdong University of Technology
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAVCARS.DTPlanning
{
    /*
     * ------------------->How To Use This Code<-------------------------------------
     * (1)include the namespace: using DTUAVCARS.DTPlanning
     * (2)define a class instance : private position_mission _positionMission = new position_mission(float k,float maxVelocityX,float maxVelocityY,float maxVelocityZ)
     * (3)update && get data:1)set the targetPositionRos:_positionMission.SetTargetPosition(targetPositionRos)
     *                       2)set the currentPositionRos:_positionMission.SetCurrentPosition(currentPositionRos)
     *                       3)get the velocity command(with ros coordinate system):vector3 com = _positionMission.GetVelocityCommand()
     *
     */
    public class position_mission
    {
        
        private Vector3 _targetPositionRos;//The Target Position of Object With ROS Coordinate System(ENU)
        private Vector3 _currentPositionRos;//The Current Position of Object With ROS Coordinate System(ENU) 
        private float _k = -5.0f;//The Parameter of Velocity Command Generation Method
        private float _maxVelocityX = 5.0f;//The Maximum Velocity X of Motion Object
        private float _maxVelocityY = 5.0f;//The Maximum Velocity Y of Motion Object
        private float _maxVelocityZ = 5.0f;//The Maximum Velocity Z of Motion Object

       public position_mission(float k,float maxVelocityX,float maxVelocityY,float maxVelocityZ)
       {
           _k = k;
           _maxVelocityX = maxVelocityX;
           _maxVelocityY = maxVelocityY;
           _maxVelocityZ = maxVelocityZ;
           _targetPositionRos = new Vector3(0,0,0);
           _currentPositionRos = new Vector3(0,0,0);
       }

       public bool SetK(float k)
       {
           _k = k;
           return true;
       }

       public bool SetMaxVelocityX(float maxVelocityX)
       {
           _maxVelocityX = maxVelocityX;
           return true;
       }

       public bool SetMaxVelocityY(float maxVelocityY)
       {
           _maxVelocityY = maxVelocityY;
           return true;
       }

       public bool SetMaxVelocityZ(float maxVelocityZ)
       {
           _maxVelocityZ = maxVelocityZ;
           return true;
       }

       public bool SetTargetPosition(Vector3 targetPositionRos)
       {
           _targetPositionRos = targetPositionRos;
           return true;
       }

       public bool SetCurrentPosition(Vector3 currentPositionRos)
       {
           _currentPositionRos = currentPositionRos;
           return true;
       }

       public Vector3 GetVelocityCommand()
       {
           Vector3 ret = new Vector3(0,0,0);
           ret.x = CountVelocityCommand(_currentPositionRos.x, _targetPositionRos.x, _k, _maxVelocityX);
           ret.y = CountVelocityCommand(_currentPositionRos.y, _targetPositionRos.y, _k, _maxVelocityY);
           ret.z = CountVelocityCommand(_currentPositionRos.z, _targetPositionRos.z, _k, _maxVelocityZ);
           return ret;
       }
        private float CountVelocityCommand(float currentX,float targetX,float k,float maxVelocity)
      {
            float distance = currentX - targetX;
            return maxVelocity * (1 - Mathf.Exp(-k * distance)) / (1 + Mathf.Exp(-k * distance));
      }
    }
}
