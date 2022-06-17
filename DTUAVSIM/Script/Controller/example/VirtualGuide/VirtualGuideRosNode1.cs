using DTUAVCARS.Controller;
using DTUAVSIM.Sensor.EnvironmentalPerception;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;


namespace RosSharp.RosBridgeClient
{
    public class VirtualGuideRosNode1 : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        public string FrameId = "Unity";

        private MessageTypes.Geometry.TwistStamped message;

        [Header("The Config of ROS Subscriber")]
        public PoseStampedSubscriber RUavPositionSub;
        public PredictorXSub RUavPositionXPredictorSub;
        public PredictorYSub RUavPositionYPredictorSub;
        public PredictorZSub RUavPositionZPredictorSub;
        public ChageControlPub RUavChageControlPub;

        [Header("The Config of Perception Sensor")]
        public LayerMask Mask;                              //The Environment Layer of Perception Sensor
        public float PerceptionRange;                       //The Range of Environmental Perception Sensor Work
        public float PerceptionAngleStart;                  //The Start Angle of Environment Perception Sensor
        public float PerceptionAngleEnd;                    //The End Angle of Environment Perception Sensor
        public float PerceptionAngleInc;                    //The Increment Angle of Environment Perception Sensor
        private environment_perception _enviromentPerception;

        [Header("The Config of Virtual Guider")]
        public int predictedSteps;
        public List<float> omiga;
        public float safeRange;
        public List<Vector3> PredictedPosition;
        public int PredictedWindow;//this equal to predictor predictedWindowX or predictedWindowY or predictedWindowZ
        public float left_theta = 45.0f;//theta_1
        public float right_theta = 45.0f;//theta_2
        public float adjustGuideRange = 0.8f;
        private VirtualGuide _virtualGuide;

        private Vector3 _virtualGuideOutput;

        private Vector3 _currentPosition;
        private Vector3 _predictPositionTemp;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _enviromentPerception = new environment_perception(Mask, PerceptionRange, PerceptionAngleStart, PerceptionAngleEnd, PerceptionAngleInc,safeRange);
            _virtualGuide = new VirtualGuide(predictedSteps, omiga, safeRange, _enviromentPerception,left_theta,right_theta,adjustGuideRange);

            _virtualGuideOutput = Vector3.zero;
            _currentPosition = Vector3.zero;
            _predictPositionTemp = Vector3.zero;
            PredictedPosition = new List<Vector3>();
            for (int i = 0; i < PredictedWindow; i++)
            {
                PredictedPosition.Add(Vector3.zero);
            }
            InitializeMessage();
        }
        private void InitializeMessage()
        {
            message = new MessageTypes.Geometry.TwistStamped
            {
                header = new MessageTypes.Std.Header()
                {
                    frame_id = FrameId
                }
            };
        }
        private void UpdateMessage()
        {
            message.header.Update();
            _currentPosition = RUavPositionSub.position;
            for (int i = 0; i < PredictedWindow; i++)
            {
                _predictPositionTemp.x = (float)RUavPositionXPredictorSub.data[i];
                _predictPositionTemp.y = (float)RUavPositionYPredictorSub.data[i];
                _predictPositionTemp.z = (float)RUavPositionZPredictorSub.data[i];
                PredictedPosition[i] = _predictPositionTemp.Ros2Unity();
            }
            _virtualGuide.UpdateController2(PredictedPosition, _currentPosition);
            RUavChageControlPub._isGuideControl = _virtualGuide.GetIsGuideControl();
            _virtualGuideOutput = _virtualGuide.GetControllerOutput();
            message.twist.linear.x = _virtualGuideOutput.Unity2Ros().x;
            message.twist.linear.y = _virtualGuideOutput.Unity2Ros().y;
            message.twist.linear.z = _virtualGuideOutput.Unity2Ros().z;
            Publish(message);
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }
    }
}
