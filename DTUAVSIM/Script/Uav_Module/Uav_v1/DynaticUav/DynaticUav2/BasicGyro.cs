using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicGyro 
{

    public float Pitch; // The current pitch for the given transform
    public float Roll; // The current roll for the given transform
    public float Yaw; // The current Yaw for the given transform
    public float Altitude; // The current altitude from the zero position
    public Vector3 VelocityVector; // Velocity vector
    public float VelocityScalar; // Velocity scalar value

    public void UpdateGyro(Transform transform)
    {
        Pitch = transform.eulerAngles.x;
        Pitch = (Pitch > 180) ? Pitch - 360 : Pitch;

        Roll = transform.eulerAngles.z;
        Roll = (Roll > 180) ? Roll - 360 : Roll;

        Yaw = transform.eulerAngles.y;
        Yaw = (Yaw > 180) ? Yaw - 360 : Yaw;

        Altitude = transform.position.y;

        VelocityVector = transform.GetComponent<Rigidbody>().velocity;
        VelocityScalar = VelocityVector.magnitude;
    }



    /* public float Pitch;//当前对象的俯仰角
     public float Roll;//当前对象的滚转角
     public float Yaw;//当前对象的偏航角

     public float Altitude;//当前对象的高度

     public Vector3 VelocityVector;//当前对象的速度矢量
     public float VelocityScalar;//当前对象的速度大小

     public void UpdateGyro(Transform ObjTransform)
     {
         Pitch = ObjTransform.eulerAngles.x;
         Pitch = (Pitch > 180) ? Pitch - 360 : Pitch;

         Roll = ObjTransform.eulerAngles.z;
         Roll = (Roll > 180) ? Roll - 360 : Roll;

         Yaw = ObjTransform.eulerAngles.y;
         Yaw = (Yaw > 180) ? Yaw - 360 : Yaw;

         Altitude = ObjTransform.position.y;
         VelocityVector = ObjTransform.GetComponent<Rigidbody>().velocity;
         VelocityScalar = VelocityVector.magnitude;
     }*/

}
