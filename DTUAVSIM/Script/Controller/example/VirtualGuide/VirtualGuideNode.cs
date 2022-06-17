using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVSIM.Sensor.EnvironmentalPerception;
using DTUAVCARS.Controller;
using lcm_iot_msgs;
using System.Threading;
using predictor_msgs;
using LCM.LCM;
using DTUAVCARS.DTNetwok.Message;
using System;
using DTUAVCARS.TF;
public class VirtualGuideNode : MonoBehaviour, LCM.LCM.LCMSubscriber
{

    //The Config of LCM Network
    [Header("The Config of LCM Network")]
    public int SourceID;
    public int TargetID;
    public string IotMessagePubName;
    public string PredictorMessageSubName;
    public float NodeRunningHz;
    private LCM.LCM.LCM PubLcm;
    private LCM.LCM.LCM SubLcm;
    private LcmIotMessage _lcmIotMessage;
    private Thread _runningThread;
    private int _runningTime;
    private bool _isStop;
    private PredictorMsgs _lcmPredictorSublishMsgs;

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
    private VirtualGuide _virtualGuide;
    private VirtualGuideMessage _virtualGuideMsg;
    private Vector3 _virtualGuideOutput;

    private Vector3 _currentPosition;
    private Vector3 _predictPositionTemp;
    // Start is called before the first frame update
    void Start()
    {
        _enviromentPerception = new environment_perception(Mask, PerceptionRange, PerceptionAngleStart, PerceptionAngleEnd, PerceptionAngleInc);
        _virtualGuide = new VirtualGuide(predictedSteps, omiga, safeRange, _enviromentPerception);
        _virtualGuideMsg = new VirtualGuideMessage();

        _virtualGuideOutput = Vector3.zero;
        _currentPosition = Vector3.zero;
        _predictPositionTemp = Vector3.zero;
        PredictedPosition = new List<Vector3>();
        for(int i=0;i<PredictedWindow;i++)
        {
            PredictedPosition.Add(Vector3.zero);
        }


        SubLcm = new LCM.LCM.LCM();
        SubLcm.Subscribe(PredictorMessageSubName, this);
        PubLcm = LCM.LCM.LCM.Singleton;
        _lcmIotMessage = new LcmIotMessage();

        _runningTime = (int)((1.0 / NodeRunningHz) * 1000);

        _isStop = false;

        _runningThread = new Thread(RunVirtualGuideNode);
        _runningThread.IsBackground = true;
        _runningThread.Start();

        
    }

    public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCMDataInputStream ins)
    {
        if (channel == PredictorMessageSubName)
        {
            _lcmPredictorSublishMsgs = new PredictorMsgs(ins);
            _currentPosition.x = _lcmPredictorSublishMsgs.currentPositionX;
            _currentPosition.y = _lcmPredictorSublishMsgs.currentPositionY;
            _currentPosition.z = _lcmPredictorSublishMsgs.currentPositionZ;
            for(int i=0;i<PredictedWindow;i++)
            {
                _predictPositionTemp.x = _lcmPredictorSublishMsgs.predictedStatesX[i];
                _predictPositionTemp.y = _lcmPredictorSublishMsgs.predictedStatesY[i];
                _predictPositionTemp.z = _lcmPredictorSublishMsgs.predictedStatesZ[i];
                PredictedPosition[i] = _predictPositionTemp;
            }
            _virtualGuide.UpdateController(PredictedPosition, _currentPosition);

        }

       // throw new System.NotImplementedException();
    }

    public void RunVirtualGuideNode()
    {
        while (!_isStop)
        {
            _virtualGuideOutput = _virtualGuide.GetControllerOutput();
            Vector3 guideRos = TF.Unity2Ros(_virtualGuideOutput);
            _virtualGuideMsg.guideVelocityX = guideRos.x;
            _virtualGuideMsg.guideVelocityY = guideRos.y;
            _virtualGuideMsg.guideVelocityZ = guideRos.z;
            _lcmIotMessage.SourceID = SourceID;
            _lcmIotMessage.TargetID = TargetID;
            DateTime centuryBegin = new DateTime(1970, 1, 1);
            DateTime currentDate = DateTime.Now;
            long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            _lcmIotMessage.TimeStamp = elapsedSpan.TotalMilliseconds;//·µ»Øms¡£
            _lcmIotMessage.MessageID = MessageId.VirtualGuideMessageID;
            _lcmIotMessage.MessageData = JsonUtility.ToJson(_virtualGuideMsg);
            PubLcm.Publish(IotMessagePubName, _lcmIotMessage);
            System.Threading.Thread.Sleep(_runningTime);
        }
    }

    void OnDestroy()
    {
        _isStop = true;
    }
}
