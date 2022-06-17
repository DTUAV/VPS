using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Noise;
namespace SimUnity.Sensor.Imu
{
    public class ImuSensor : MonoBehaviour
    {
        // Start is called before the first frame update
        public Vector3 position;//位置
        public Vector3 velocity;//线速度
        public Vector3 acceleration;//加速度
        public Vector3 angularVelocity;//角速度
        public Quaternion orientation;//方向
        public Rigidbody rb;//刚体
        private Vector3 lastVelocity;
        private Vector3 lastPosition;
        private Vector3 lastAngle;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        void FixedUpdate()
        {
            if (rb != null)
            {
                position = rb.position + new Vector3((float)GaussNoisPlugin.GaussianNoiseData(0,0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02));
                velocity = transform.InverseTransformDirection(rb.velocity)+new Vector3((float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02));
                acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
                lastVelocity = velocity;
                acceleration -= transform.InverseTransformDirection(Physics.gravity);
                angularVelocity = rb.angularVelocity+new Vector3((float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02));
                orientation.x = rb.rotation.x + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02);
                orientation.y = rb.rotation.y + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02);
                orientation.z = rb.rotation.z + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02);
                orientation.w = rb.rotation.w + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02);
            }
            else
            {
                position = transform.position + new Vector3((float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02));
                velocity = transform.InverseTransformDirection(position - lastPosition);
                lastPosition = position;
                acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;
                lastVelocity = velocity;
                acceleration -= transform.InverseTransformDirection(Physics.gravity)+new Vector3((float)GaussNoisPlugin.GaussianNoiseData(0, 0.002), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.002), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.002)); ;
                angularVelocity = (transform.eulerAngles-lastAngle)/Time.fixedDeltaTime;
                lastAngle = transform.eulerAngles+new Vector3((float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02), (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02)); ;
                orientation.x = transform.rotation.x + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02);
                orientation.y = transform.rotation.y + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02);
                orientation.z = transform.rotation.z + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02);
                orientation.w = transform.rotation.w + (float)GaussNoisPlugin.GaussianNoiseData(0, 0.02);
            }
        }
    }

}