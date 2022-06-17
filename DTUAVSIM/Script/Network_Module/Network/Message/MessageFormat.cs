/*
 * Update Information:
 *                    First: 2021-8-4 In Guangdong University of Technology By Yang Yuanlin
 *
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace DTUAVCARS.DTNetwok.Message
{
    [Serializable]
    public class MessageFormat
    {

    }
    
    public class BaseMessage
    {
     //    T data = new T();
    }
    /// <typeparam name="T"></typeparam>
    public class IotMessageList
    {
        public int TargetID;
        public int SourceID;
        public int MessageID;
        public ArrayList MessageData;
        public double TimeStamp;
    }




    [Serializable]
    public class IotMessage 
    {
        public int TargetID;
        public int SourceID;
        public int MessageID;
        public string MessageData;
        public double TimeStamp;
    }


    [Serializable]
    public class CurrentPoseMessage
    {
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public float RotationW;
    }

    [Serializable]
    public class TargetPoseMessage
    {
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float RotationX;
        public float RotationY;
        public float RotationZ;
        public float RotationW;
    }

    [Serializable]
    public class UavCurrentPoseMessage
    {
        public double position_x;
        public double position_y;
        public double position_z;
        public double rotation_x;
        public double rotation_y;
        public double rotation_z;
        public double rotation_w;
    }

    [Serializable]
    public class UavRefPoseMessage
    {
        public double position_x;
        public double position_y;
        public double position_z;
        public double rotation_x;
        public double rotation_y;
        public double rotation_z;
        public double rotation_w;
    }

    [Serializable]
    public class UavCurrentVelocityMessage
    {
        public double linear_velocity_x;
        public double linear_velocity_y;
        public double linear_velocity_z;
        public double anger_velocity_x;
        public double anger_velocity_y;
        public double anger_velocity_z;
    }

    [Serializable]
    public class UavRefVelocityMessage
    {
        public double linear_velocity_x;
        public double linear_velocity_y;
        public double linear_velocity_z;
        public double anger_velocity_x;
        public double anger_velocity_y;
        public double anger_velocity_z;
    }

    [Serializable]
    public class UavStartMessage
    {
        public bool start;
    }

    [Serializable]
    public class UavGlobalPositionMessage
    {
        public int status;
        public int service;
        public double latitude;
        public double longitude;
        public double altitude;
        public double[] position_covariance;
        public int position_covariance_type;
    }

    [Serializable]
    public class UavLocalPositionMessage
    {
        public double position_x;
        public double position_y;
        public double position_z;
        public double rotation_x;
        public double rotation_y;
        public double rotation_z;
        public double rotation_w;
    } 

    [Serializable]
    public class UavLocalVelocityMessage
    {
        public double linear_velocity_x;
        public double linear_velocity_y;
        public double linear_velocity_z;
        public double anger_velocity_x;
        public double anger_velocity_y;
        public double anger_velocity_z;
    }

    [Serializable]
    public class HitMessage
    {
        public bool isHit;
    }

    [Serializable]
    public class CompressedImageMessage
    {
        public string format;
        public byte[] data;
    }
    
    [Serializable]
    public class VirtualGuideMessage
    {
        public float guideVelocityX;
        public float guideVelocityY;
        public float guideVelocityZ;
    }
}
