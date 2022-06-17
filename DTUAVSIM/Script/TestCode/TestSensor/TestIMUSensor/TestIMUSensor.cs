
using iot_msgs;
using lcm_iot_msgs;
using UnityEngine;
using LCM.LCM;
using sensor_msgs;

namespace DTUAVSIM.Test
{
    public class TestIMUSensor : MonoBehaviour, LCM.LCM.LCMSubscriber
    {
        public string IotMessageSubName;
        private LCM.LCM.LCM SubLcm;
        private bool is_recv_data = false;
        // private Vector3 recvData = new Vector3();
       
        // Start is called before the first frame update
        void Start()
        {
            SubLcm = new LCM.LCM.LCM();
            SubLcm.Subscribe(IotMessageSubName, this);
        }


        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
        {
            // Debug.Log("zzzzXXXXXX");
            if (channel == IotMessageSubName)
            {  /*
                imu imu_msg = new imu(ins);
                Debug.Log("imu_msg.linear_acceleration_x: " + imu_msg.linear_acceleration_x);
                Debug.Log("imu_msg.linear_acceleration_y: " + imu_msg.linear_acceleration_y);
                Debug.Log("imu_msg.linear_acceleration_z: " + imu_msg.linear_acceleration_z);
                Debug.Log("imu_msg.angular_velocity_x: " + imu_msg.angular_velocity_x);
                Debug.Log("imu_msg.angular_velocity_y: " + imu_msg.angular_velocity_y);
                Debug.Log("imu_msg.angular_velocity_z: " + imu_msg.angular_velocity_z);
                Debug.Log("imu_msg.orientation_x: " + imu_msg.orientation_x);
                Debug.Log("imu_msg.orientation_y: " + imu_msg.orientation_y);
                Debug.Log("imu_msg.orientation_z: " + imu_msg.orientation_z);
                Debug.Log("imu_msg.orientation_w: " + imu_msg.orientation_w);
                for (int i = 0; i < 9; i++)
                {
                    Debug.Log("imu_msg.linear_acceleration_covariance["+i+"]: " + imu_msg.linear_acceleration_covariance[i]);
                    Debug.Log("imu_msg.angular_velocity_covariance[" + i + "]: " + imu_msg.angular_velocity_covariance[i]);
                    Debug.Log("imu_msg.orientation_covariance[" + i + "]: " + imu_msg.orientation_covariance[i]);
                }
                */
            }


        }
       

    }
}
