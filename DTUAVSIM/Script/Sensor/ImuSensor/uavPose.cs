using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimUnity.Sensor.Imu
{
    public class uavPose : MonoBehaviour
    {
        // Start is called before the first frame update
        public Rigidbody rb;
        public Quaternion quaternion;
        public Vector3 position;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        void FixedUpdate()
        {
            quaternion = rb.rotation;
            position = rb.position;
        }
    }
}
