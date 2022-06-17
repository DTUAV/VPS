
using UnityEngine;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using SimUnity.Sensor.LidarSensorV4;
using System.Threading;
using RosSharp.RosBridgeClient;
namespace RosSharp.RosBridgeClient
{
    public class LidarSensorV4PointCloud : UnityPublisher<MessageTypes.Sensor.PointCloud>
    {
        private Thread c_getData;
        public string FrameId = "Unity";
        public LidarSensorV4 lidarSensor;
        private MessageTypes.Sensor.PointCloud message;
        private float scanPeriod;
        private float previousScanTime = 0;
        private bool endFlag = false;

        protected override void Start()
        {
            base.Start();
            InitializeMessage();
            c_getData = new Thread(UpdateMsg);
            c_getData.IsBackground = true;
            c_getData.Start();
        }

        private void UpdateMsg()
        {
            while (!endFlag)
            {
               // if (Time.realtimeSinceStartup >= previousScanTime + scanPeriod)
                {
                    UpdateMessage();
                  //  previousScanTime = Time.realtimeSinceStartup;
                }
            }
        }

        private void InitializeMessage()
        {

            message = new MessageTypes.Sensor.PointCloud();

            message.header = new MessageTypes.Std.Header { frame_id = FrameId };
            message.points = new MessageTypes.Geometry.Point32[lidarSensor.MeasurementsPerRotation*lidarSensor.LaserCount];
            message.channels = new MessageTypes.Sensor.ChannelFloat32[1];
            message.channels[0] = new MessageTypes.Sensor.ChannelFloat32();
            for (int i = 0; i < lidarSensor.MeasurementsPerRotation * lidarSensor.LaserCount; i++)
            {
                message.points[i] = new Point32();
            }
            message.channels[0].values = new float[lidarSensor.MeasurementsPerRotation * lidarSensor.LaserCount];
        }

        private void UpdateMessage()
        {
           
            int i = 0;
            if (lidarSensor.sendOK)
            {
               

                    foreach (UnityEngine.Vector3 d in lidarSensor.hitDataPosition.GetRange(0, lidarSensor.MeasurementsPerRotation * lidarSensor.LaserCount))
                    {
                        message.points[i].x = d.z;
                        message.points[i].y = -d.x;
                        message.points[i].z = d.y;
                        i++;
                    }
                    i = 0;
                    foreach (float den in lidarSensor.hitDensity.GetRange(0, lidarSensor.MeasurementsPerRotation * lidarSensor.LaserCount))
                    {
                        message.channels[0].name = "rgb";
                        message.channels[0].values[i] = den;
                        i++;
                    }
                    message.header.Update();
                    Publish(message);

                
                lidarSensor.sendOK = false;
                lidarSensor.updateOK = true;
            }
           
        }

        void OnDestroy()
        {
            endFlag = true;
            if (c_getData.IsAlive)
            {
                c_getData.Abort();
            }

        }
    }
}
