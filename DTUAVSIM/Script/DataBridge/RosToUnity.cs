using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Data_bridge
{
    public class RosToUnity
    {
        public Vector3 Ros2Unity(Vector3 vector3)
        {
            return new Vector3(-vector3.y, vector3.z, vector3.x);
        }



        public Quaternion Ros2Unity(Quaternion quaternion)
        {
            return new Quaternion(quaternion.y, -quaternion.z, -quaternion.x, quaternion.w);
        }

    }
}
