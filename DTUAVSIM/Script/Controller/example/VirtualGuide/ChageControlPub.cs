using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
namespace RosSharp.RosBridgeClient
{
    public class ChageControlPub : UnityPublisher<MessageTypes.Std.Bool>
    {
        public float PublishedFrequency;

        private MessageTypes.Std.Bool message;
        private Thread _runningThread;
        private int _runningTimes;
        private bool _isStop;

        public bool _isGuideControl;
        protected override void Start()
        {
            base.Start();
            _isStop = false;
            _isGuideControl = false;
            message = new MessageTypes.Std.Bool();
            _runningTimes = (int)((1.0 / PublishedFrequency) * 1000);
            _runningThread = new Thread(PublishMsg);
            _runningThread.IsBackground = true;
            _runningThread.Start();
        }

        private void PublishMsg()
        {
            while (!_isStop)
            {
                message.data = _isGuideControl;
                Publish(message);
                System.Threading.Thread.Sleep(_runningTimes);
            }
        }
    }
}
