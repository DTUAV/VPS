using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DTUAVCARS.Algorithm.DataStruct;
namespace DTUAVCARS.Algorithm.Predictor
{
    public struct PredictStateStu
    {
        public List<float> Times;
        public List<float> States;
    }
    public class StatePredictor
    {
        private float _predictorRunHz ;//The Predictor Run Frequency
        private int _saveDataWindow;//The Save Data Window Size
        private int _predictWindow;
        private int _maxPredictWindow;//The Max Data Window Size of Predictor
        private int _minPredictWindow;//The Min Data Window Size of Predictor
        private float _predictTimeDt;//The increated time of Predictor
        private int _predictDataWindow;//The Current Data Window Size of Predictor
        private int _eventId;//The Event Id of Object State
        private int _predictOrder;//The Order of Predicted Function
        private float _queueCheckError;//The Error Value to Check the System State Queue to Use Different Predicted Ways
        private float _maxVelocity;//The Maximun Velocity of Motion
        private CircularBuffer<float> _ciclQueueT;
        private CircularBuffer<float> _ciclQueueX;
        private List<float> _runningDataT;
        private List<float> _runningDataX;
        private LeastSquare _leastSquare;
        private Thread _runningThread;
        private int _runningTime;
        private bool _isStop;
        
        private List<float> _predictStates;
        private List<float> _predictTimes;

        private PredictStateStu _predictStateStu;

       
        public bool SetPredictTimeDt(float timeDt)
        {
            _predictTimeDt = timeDt;
            return true;
        }
        private int GetPredictWindowSize()
        {
            int nowPredictWindowSize = _predictDataWindow;
            if(_eventId == 0)
            {
                nowPredictWindowSize = nowPredictWindowSize + 1;
            }
            else if(_eventId == 1)
            {
                nowPredictWindowSize = nowPredictWindowSize;
            }
            else
            {
                nowPredictWindowSize = _minPredictWindow;
            }

            if(nowPredictWindowSize>_maxPredictWindow)
            {
                nowPredictWindowSize = _maxPredictWindow;
            }
            else if(nowPredictWindowSize<_minPredictWindow)
            {
                nowPredictWindowSize = _minPredictWindow;
            }
            else
            {
                nowPredictWindowSize = nowPredictWindowSize;
            }
            return nowPredictWindowSize;
        }

        private int EventTrigger()
        {
            List<float> devDataQueue = new List<float>();
            int eventId = 0;
            for(int i=1;i<_runningDataX.Count;i++)
            {
                devDataQueue.Add(Mathf.Abs(_runningDataX[i] - _runningDataX[i - 1]));
            }
            float minValue = GetMinValue(_runningDataX);
            float maxValue = GetMaxValue(_runningDataX);
            if(maxValue>_queueCheckError&&minValue>_queueCheckError)
            {
                eventId = 0;
            }
            else if(maxValue<=_queueCheckError&&minValue<=_queueCheckError)
            {
                eventId = 1;
            }
            else
            {
                eventId = 2;
            }

            return eventId;
        }

        private float GetMaxValue(List<float> dataQueue)
        {
            float maxValue = -1000.0f;
            for(int i=0;i<dataQueue.Count;i++)
            {
                if(dataQueue[i]>maxValue)
                {
                    maxValue = dataQueue[i];
                }
            }
            return maxValue;
        }
        private float GetMinValue(List<float> dataQueue)
        {
            float minValue = 1000.0f;
            for(int i=0;i<dataQueue.Count;i++)
            {
                if(dataQueue[i]<minValue)
                {
                    minValue = dataQueue[i];
                }
            }
            return minValue;
        }

        private bool RunLeastSquaresMotion(float currentTime)
        {
            _leastSquare.SetT(_runningDataT);
            _leastSquare.SetX(_runningDataX);
            List<float> result = _leastSquare.GetResult();
            for (int i = 0; i < _predictWindow; i++)
            {
                _predictStateStu.Times[i] = currentTime + (i + 1) * _predictTimeDt;
                for(int j =result.Count-1;j>=0;j--)
                {
                    float predictTime = _runningDataX[_runningDataX.Count - 1] + (i + 1) * _predictTimeDt;
                    _predictStateStu.States[i] = _predictStateStu.States[i] + Mathf.Pow(predictTime, result.Count - 1 - j) * result[j];
                }
            }
            return true;
        }

        private bool RunMaxVelocityMotion(float currentTime)
        {
            float moveDir = _runningDataX[_runningDataX.Count - 1] - _runningDataX[_runningDataX.Count - 2];
            float moveVelocity = 0.0f;
            if(moveDir>0.0f)
            {
                moveVelocity = _maxVelocity;
            }
            else if(moveDir<0.0f)
            {
                moveVelocity = -_maxVelocity;
            }
            else
            {
                moveVelocity = 0.0f;
            }
            float currentState = _runningDataX[_runningDataX.Count - 1];
            for(int i=0;i<_predictWindow;i++)
            {
                _predictStateStu.Times[i] = (i + 1) * _predictTimeDt + currentTime;
                _predictStateStu.States[i] = (i + 1) * moveVelocity * _predictTimeDt + currentState;
            }

            return true;
        }
        private bool RunStaticMotion(float currentTime)
        {
            float currentState = _runningDataX[_runningDataX.Count - 1];
            for(int i=0;i<_predictWindow;i++)
            {
                _predictStateStu.Times[i] = (i + 1) * _predictTimeDt + currentTime;
                _predictStateStu.States[i] = currentState;
            }
            return true;
        }
        private void PredictorRunning()
        {
            while(!_isStop)
            {
                if(_ciclQueueT[_saveDataWindow-1] !=0.0f)
                {
                    float currentTime = _ciclQueueT[_saveDataWindow - 1];
                    float currentPosition = _ciclQueueX[_saveDataWindow - 1];
                    _runningDataT.Clear();
                    _runningDataX.Clear();
                    for(int i=_saveDataWindow-_predictDataWindow;i<_saveDataWindow;i++)
                    {
                        _runningDataT.Add(_ciclQueueT[i] - _ciclQueueT[_saveDataWindow - _predictDataWindow]);
                        _runningDataX.Add(_ciclQueueX[i]);
                    }
                    _eventId = EventTrigger();
                    _predictDataWindow = GetPredictWindowSize();
                    Debug.Log("eventID: " + _eventId);
                    switch(_eventId)
                    {
                        case 0:
                            {
                                RunLeastSquaresMotion(currentTime);
                            }
                            break;
                        case 1:
                            {
                                RunStaticMotion(currentTime);
                            }
                            break;
                        case 2:
                            {
                                RunMaxVelocityMotion(currentTime);
                            }
                            break;
                        default:
                            break;
                    }
                    _predictTimes = _predictStateStu.Times;
                    _predictStates = _predictStateStu.States;
                }

                System.Threading.Thread.Sleep(_runningTime);
            }
        }

        public List<float> GetPredictStates()
        {
            return _predictStates;
        }

        public List<float> GetPredictTimes()
        {
            return _predictTimes;
        }
        public bool StopPredictor()
        {
            _isStop = true;
            return true;
        }
        public bool UpdateDataT(float t)
        {
            _ciclQueueT.Push(t);
            return true;
        }
        public bool UpdateDataX(float x)
        {
            _ciclQueueX.Push(x);
            return true;
        }
        public StatePredictor(float predictorRunHz,int saveDataWindow,int predictWindow, int maxPredictWindow,int minPredictWindow,float predictTimeDt,int predictOrder,float queueCheckError,float maxVelocity,List<float> runningDataT,List<float> runningDataX)
        {
            _predictorRunHz = predictorRunHz;
            _saveDataWindow = saveDataWindow;
            _maxPredictWindow = maxPredictWindow;
            _minPredictWindow = minPredictWindow;
            _predictTimeDt = predictTimeDt;
            _predictOrder = predictOrder;
            _queueCheckError = queueCheckError;
            _maxVelocity = maxVelocity;
            _runningDataT = new List<float>();
            _runningDataT = runningDataT;
            _runningDataX = new List<float>();
            _runningDataX = runningDataX;


            _predictWindow = predictWindow;
            _predictDataWindow = maxPredictWindow;

            _ciclQueueT = new CircularBuffer<float>(_saveDataWindow);
            _ciclQueueX = new CircularBuffer<float>(_saveDataWindow);

            _leastSquare = new LeastSquare(_runningDataT, _runningDataX, _predictOrder);

            _isStop = false;
            _runningTime= (int)((1.0 / _predictorRunHz) * 1000);

            _predictTimes = new List<float>();
            _predictStates = new List<float>();

            _predictStateStu = new PredictStateStu();
            _predictStateStu.States = new List<float>();
            _predictStateStu.Times = new List<float>();

            for (int i = 0; i < _predictWindow; i++)
            {
                _predictTimes.Add(0.0f);
                _predictStates.Add(0.0f);

                _predictStateStu.States.Add(0.0f);
                _predictStateStu.Times.Add(0.0f);
            }

            _runningThread = new Thread(PredictorRunning);
            _runningThread.IsBackground = true;
            _runningThread.Start();

            
        }
    }
}
