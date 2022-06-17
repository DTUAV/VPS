using DTUAVCARS.Controller;
using DTUAVSIM.Sensor.EnvironmentalPerception;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RosSharp.RosBridgeClient;
using UnityEditor.Experimental.GraphView;


namespace RosSharp.RosBridgeClient
{
    public class VirtualGuideRosNode2 : UnityPublisher<MessageTypes.Geometry.TwistStamped>
    {
        public string FrameId = "Unity";

        private MessageTypes.Geometry.TwistStamped message;

        [Header("The Config of ROS Subscriber")]
        public PoseStampedSubscriber RUavPositionSub;
        public PredictorXSub RUavPositionXPredictorSub;
        public PredictorYSub RUavPositionYPredictorSub;
        public PredictorZSub RUavPositionZPredictorSub;
        public GuideTypePub RUavGuideTypePub;
        public PoseStampedSubscriber VUavPositionSub;

        [Header("The Config of Perception Sensor")]
        public LayerMask Mask;                              //The Environment Layer of Perception Sensor
        public float PerceptionRange;                       //The Range of Environmental Perception Sensor Work
        public float PerceptionAngleStart;                  //The Start Angle of Environment Perception Sensor
        public float PerceptionAngleEnd;                    //The End Angle of Environment Perception Sensor
        public float PerceptionAngleInc;                    //The Increment Angle of Environment Perception Sensor
        private environment_perception _environmentPerception;

        [Header("The Config of Virtual Guider")]
        public int PredictedSteps;
        public List<float> Omiga;
        public float SafeRange;
        public List<Vector3> PredictedPosition;
        public int PredictedWindow;//this equal to predictor predictedWindowX or predictedWindowY or predictedWindowZ
        public float LeftTheta = 45.0f;//theta_1
        public float RightTheta = 45.0f;//theta_2
        public float AdjustGuideRange = 0.8f;
        private VirtualGuide _virtualGuide;

        [Header("The Config of Tracking Guider")]
        public float DirectionThreshold;//The threshold for tracking direction, only the cost of direction is more than it, the tracking guide of direction can work.
        public float PositionThreshold;//The threshold for tracking distance, only the cost of position distance is more than it, the tracking guide of position can work. 
        public float DirectionGain;   //The gain of direction adjusting in the guide output.
        public float PositionGain;   //The gain of position adjusting in the guide output.
        private TrackingGuide _trackingGuide;

        [Header("The Config of Fusion Guider")]
        public float GuideThreshold;
        public float SafeGain;
        public float TrackingGain;

        private Vector3 _virtualGuideOutput;
        private Vector3 _currentPosition;
        private Vector3 _predictPositionTemp;

        private Vector3 _safeGuideOutput;
        private Vector3 _trackingGuideOutput;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            _environmentPerception = new environment_perception(Mask, PerceptionRange, PerceptionAngleStart, PerceptionAngleEnd, PerceptionAngleInc, SafeRange);
            _virtualGuide = new VirtualGuide(PredictedSteps, Omiga, SafeRange, _environmentPerception, LeftTheta, RightTheta, AdjustGuideRange);
            _trackingGuide = new TrackingGuide(DirectionThreshold,PositionThreshold,DirectionGain,PositionGain);
            _virtualGuideOutput = Vector3.zero;
            _currentPosition = Vector3.zero;
            _predictPositionTemp = Vector3.zero;
            _safeGuideOutput = Vector3.zero;
            _trackingGuideOutput = Vector3.zero;

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
            Debug.Log("CurrentPosition: "+_currentPosition);
            for (int i = 0; i < PredictedWindow; i++)
            {
                _predictPositionTemp.x = (float)RUavPositionXPredictorSub.data[i];
                _predictPositionTemp.y = (float)RUavPositionYPredictorSub.data[i];
                _predictPositionTemp.z = (float)RUavPositionZPredictorSub.data[i];
                PredictedPosition[i] = _predictPositionTemp.Ros2Unity();
            }

            _predictPositionTemp.x = (float) RUavPositionXPredictorSub.data[0];
            _predictPositionTemp.y = (float)RUavPositionYPredictorSub.data[0];
            _predictPositionTemp.z = (float)RUavPositionZPredictorSub.data[0];

            _virtualGuide.UpdateController2(PredictedPosition, _currentPosition);
            _safeGuideOutput = _virtualGuide.GetControllerOutput();
            _trackingGuideOutput = _trackingGuide.UpdateTrackingCost(_currentPosition, _predictPositionTemp,VUavPositionSub.position, VUavPositionSub.Direction, true);
            _trackingGuideOutput.x = Mathf.Clamp(_trackingGuideOutput.x, -1.0f, 1.0f);
            _trackingGuideOutput.y = Mathf.Clamp(_trackingGuideOutput.y, -1.0f, 1.0f);
            _trackingGuideOutput.z = Mathf.Clamp(_trackingGuideOutput.z, -1.0f, 1.0f);
            Debug.Log("Tracking Cost: "+_trackingGuideOutput);
            Debug.Log("SafeCuideOutput: "+_safeGuideOutput);
            if (_safeGuideOutput.x <= 10e-6 && _safeGuideOutput.y <= 10e-6 && _safeGuideOutput.z <= 10e-6)
            {
                _predictPositionTemp = _trackingGuideOutput;
            }
            else
            {
                _predictPositionTemp = _safeGuideOutput;
               // _predictPositionTemp = SafeGain * _safeGuideOutput + TrackingGain * _trackingGuideOutput;
            }
            
            Debug.Log("predictPositionTem: "+_predictPositionTemp);
            RUavGuideTypePub.PublishMsg(2);//all guide
            message.twist.linear.x = _predictPositionTemp.Unity2Ros().x;
            message.twist.linear.y = _predictPositionTemp.Unity2Ros().y;
            message.twist.linear.z = _predictPositionTemp.Unity2Ros().z;
            Publish(message);
            /*
            if (_predictPositionTemp.x < GuideThreshold || _predictPositionTemp.y < GuideThreshold ||
                _predictPositionTemp.z < GuideThreshold)
            {
                RUavGuideTypePub.PublishMsg(2);//all guide
                message.twist.linear.x = _predictPositionTemp.Unity2Ros().x;
                message.twist.linear.y = _predictPositionTemp.Unity2Ros().y;
                message.twist.linear.z = _predictPositionTemp.Unity2Ros().z;
                Publish(message);
            }
            else if(_predictPositionTemp.x>=GuideThreshold&&_predictPositionTemp.x<0|| _predictPositionTemp.y >= GuideThreshold && _predictPositionTemp.y < 0 || _predictPositionTemp.z >= GuideThreshold && _predictPositionTemp.z < 0)
            {
                RUavGuideTypePub.PublishMsg(1);//half guide
                message.twist.linear.x = _predictPositionTemp.Unity2Ros().x;
                message.twist.linear.y = _predictPositionTemp.Unity2Ros().y;
                message.twist.linear.z = _predictPositionTemp.Unity2Ros().z;
                Publish(message);
            }
            else
            {
                RUavGuideTypePub.PublishMsg(0);//no guide
            }
            */
        }

        private void FixedUpdate()
        {
            UpdateMessage();
        }
    }
}
