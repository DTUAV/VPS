using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Data_bridge
{

    public class UnityToRos
    {
        public static Vector3 Unity2Ros(Vector3 vector3)
        {
            // return new Vector3(vector3.z, vector3.x, vector3.y);
            //return new Vector3(vector3.z, -vector3.x, vector3.y);
            return new Vector3(vector3.x, vector3.z, vector3.y);
        }



        public static Quaternion Unity2Ros( Quaternion quaternion)
        {
           // return new Quaternion(quaternion.z, quaternion.x, quaternion.y, quaternion.w);
            //return new Quaternion(-quaternion.z, quaternion.x, -quaternion.y, quaternion.w);
            return new Quaternion(quaternion.x, quaternion.z, quaternion.y, quaternion.w);
        }

    }
}

