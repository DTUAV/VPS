
using UnityEngine;
using SimUnity.Sensor.LidarSensorV3;
namespace RosSharp.RosBridgeClient
{
    public class LidarV3Pub : UnityPublisher<MessageTypes.Sensor.LaserScan>
    {
       
        public LidarSensorV3 lidarSensorV3;
        public string FrameId = "Unity";
        private MessageTypes.Sensor.LaserScan message;
        private float scanPeriod;
        private float previousScanTime = 0;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
        }

        private void FixedUpdate()
        {
           // if (Time.realtimeSinceStartup >= previousScanTime + scanPeriod)
            {
                UpdateMessage();
                previousScanTime = Time.realtimeSinceStartup;
            }
        }

        private void InitializeMessage()
        {
            //   scanPeriod = (float)laserScanReader.samples / (float)laserScanReader.update_rate;

            message = new MessageTypes.Sensor.LaserScan
            {
                header = new MessageTypes.Std.Header { frame_id = FrameId },
                angle_min = 0,
                angle_max = 0,
                angle_increment = lidarSensorV3.HorizontalAngleDealt,
                time_increment = lidarSensorV3.dealtTime,
                range_min = lidarSensorV3.MinDistance,
                range_max = lidarSensorV3.MaxDistance,
                ranges = lidarSensorV3.distanct1,
                intensities = lidarSensorV3.density1
            };
        }

        private void UpdateMessage()
        {
            message.header.Update();
            message.time_increment = lidarSensorV3.dealtTime;
            message.ranges = lidarSensorV3.distanct1;
            message.intensities = lidarSensorV3.density1;
            Publish(message);
        }
    }
}
