using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using DTUAV.Algorithm_Module.Path_Planning.RRT;
using UnityEngine;
using DTUAV.Visualization_Module.Line;
using DTUAV.Tools;
using std_msgs;
using UnityEditor.Experimental.GraphView;
namespace DTUAV.Algorithm_Module.Path_Planning.Human
{
    public class HumanPlanning
    {
        private Transform _planningObject;//The object is used to moved in virtual scene by operator.
        private Line _line;//The class to plot the line in scene for planning object.
        private Line _pathLine;//The class to plot the line in scene for smoothing path.
        private float _plotLineHz;//The frequency of draw the line.
        private bool _stopThread;//The flag to stop drawing the line(end up the thread----the programmer end).
        private bool _runningPlotLine;//The flag to stop the running of drawing the line.
        private int _sleepTime;//The sleep time of drawing line thread.
        private float _height;//The height of path(the position z of running object).

        private Vector3 _startPosition;//The start position of the plot line.

        private float _smoothPathFactor = 0.1f;//The factor of path smoothing//this factor will determine the selected number.
        private List<Vector2> _smoothPath;//The path provided by human operator with smooth. 
        private List<Vector2> _path;//The path provided by human operator without smooth.
        private bool _isShowSmoothPath;//The flag to show the smooth path, provided by human operator.

        private EnvironmentalPerception2D _environmentalPerception2D;//The sensor to acquired the environment information.
        private float _timeCost;//The time cost of smooth path, provided by human operator.
        private float _safeCost;//The safe cost of smooth path, provided by human operator.
        private List<int> _dangerPositionIndexes;//The position indexes in smooth path.
        private float _safeDistance;//The safe distance of UAV.

        private RRT2D _rrt;//The algorithm of path planning.
        private float _fusionPathTimeCost;//The time cost of fusion path, provided by RRT and human operator.
        private float _fusionPathSafeCost;//The safe cost of fusion path, provided by RRT and human operator.
        private List<Vector2> _fusionPath;//The fusion path, provided by RRT and human operator.

        private bool _isFusionSuccess;

       public bool GetIsFusionSuccess()
        {
            return _isFusionSuccess;
        }
        public float GetFusionPathTimeCost()
        {
            return _fusionPathTimeCost;
        }

        public float GetFusionPathSafeCost()
        {
            return _fusionPathSafeCost;
        }

        public List<Vector2> GetFusionPath()
        {
            return _fusionPath;
        }

        [System.Obsolete]
        public List<Vector2> FusionPath(float factor)
        {
            _isFusionSuccess = true;
            List<Vector2> ret = new List<Vector2>();
            //remove danger point
            List<Vector2> data = _smoothPath;
            Vector3 tem = new Vector3();
            float dis = 0;
            //Debug.Log("The Path Size Before Fusion: "+data.Count);
            for (int i = data.Count - 1; i >= 0; i--)
            {
                tem.x = data[i].x;
                tem.z = data[i].y;
                tem.y = _height;
                _environmentalPerception2D.UpdateSensor(tem);
                dis = _environmentalPerception2D.GetMinDistance();
                if (dis <= _safeDistance)
                {
                    data.Remove(data[i]);
                }
            }
            //Debug.Log("The Path Size After Remove danger position: "+data.Count);
            int sampleNum = (int) (data.Count/(data.Count * factor));
            //Sample Data
            List<Vector2> SampleData = new List<Vector2>();
            for (int i = 0; i < data.Count; i += sampleNum)
            {
                SampleData.Add(data[i]);
            }

            if (SampleData[SampleData.Count - 1] != data[data.Count - 1])
            {
                SampleData.Add(data[data.Count - 1]);
            }
            //Debug.Log("The Path Size With the Sample: "+SampleData.Count);
            //fusion path by using RRT
            Vector2 startPosition = SampleData[0];
            for (int i = 1; i < SampleData.Count; i++)
            {
                if (_rrt.FindPath(startPosition, SampleData[i]))
                {
                    ret.AddRange(_rrt.GetPath());
                    startPosition = SampleData[i];
                }
                else
                {
                    Debug.Log("No Find...........");
                    if (i == SampleData.Count - 1)
                    {
                        _isFusionSuccess = false;
                    }
                }
            }
            /*======================show the rrt process based on human planning
            //_pathLine.ClearAllLine();
           // _pathLine.ClearAllLine();
           // Vector2 startPlotPosition = ret[0];
           // for (int i = 0; i < ret.Count; i++)
           // {
          //      _pathLine.SetPosition(new Vector3(startPlotPosition.x, _height, startPlotPosition.y),
           //         new Vector3(ret[i].x, _height, ret[i].y));
           //       _pathLine.DrawLine();
          //      startPlotPosition = ret[i];
         //   }
            */
            //Debug.Log("The Fusion Path Size: "+ret.Count);
            if (_isFusionSuccess)
            {
                _fusionPath.Clear();
                _fusionPath = _rrt.GetSmoothPath(ret, 0.1f);
                Debug.Log("before simply path size: "+_fusionPath.Count);

                _fusionPath = SimplyPath(_fusionPath, 1);
                Debug.Log("after simply path size: "+ _fusionPath.Count);
                _fusionPathSafeCost = CountPathSafeCost(_fusionPath);
                _fusionPathTimeCost = CountPathTimeCost(_fusionPath);
                // _rrt.ShowPath(ret);
                _rrt.ShowPath(_fusionPath);
            }
            return _fusionPath;
        }

        private List<Vector2> SimplyPath(List<Vector2> path,int inc)
        {
            List<Vector2> ret = new List<Vector2>();
            if (path.Count>0)
            {
                ret.Add(path[0]);
                Vector2 fistPoint = path[0];
                Vector2 lastPoint = path[0];
                for (int i = 0; i < path.Count; i+=inc)
                {
                    if (!CheckRangeIsSafe(fistPoint,path[i],10))
                    {
                        ret.Add(lastPoint);
                        fistPoint = lastPoint;
                    }
                    else
                    {
                        lastPoint = path[i];
                    }
                }
                ret.Add(path[path.Count-1]);
            }
            return ret;
        }

        private bool CheckRangeIsSafe(Vector2 start, Vector2 end,int num)
        {
            float inc_x = 0;
            float inc_y = 0;
            bool ret = true;
            Vector3 tem = new Vector3();
            if (Mathf.Abs(start.x-end.x)<10e-3)
            {
                inc_x = 0;
            }
            else
            {
                inc_x = (end.x - start.x) / num;
            }

            if (Mathf.Abs(start.y-end.y)<10e-3)
            {
                inc_y = 0;
            }
            else
            {
                inc_y = (end.y - start.y) / num;
            }

            for (int i = 0; i < num; i++)
            {
                tem.x = start.x + i * inc_x;
                tem.y = _height;
                tem.z = start.y + i * inc_y;
                _environmentalPerception2D.UpdateSensor(tem);
                if (!_environmentalPerception2D.GetIsSafe())
                {
                    ret = false;
                    break;
                }
            }

            return ret;

        }

        private float CountPathSafeCost(List<Vector2> path)
        {
            float ret = 0.0f;
            if (path.Count >= 1)
            {
                Vector3 tem = new Vector3();
                float dis = 0;
                for (int i = 0; i < path.Count; i++)
                {
                    tem.x = path[i].x;
                    tem.z = path[i].y;
                    tem.y = _height;
                    _environmentalPerception2D.UpdateSensor(tem);
                    dis = _environmentalPerception2D.GetMinDistance();
                    if (dis <= _safeDistance)
                    {
                       ret += _safeDistance - dis;
                    }
                }
            }
            else
            {
                Debug.Log("Path NULL!!!!!");
            }

            return ret;
        }

        private float CountPathTimeCost(List<Vector2> path)
        {
            float cost = 0.0f;
            if (path.Count>=1)
            {
                Vector3 tem = new Vector3();
                Vector3 lastPosition = new Vector3();
                lastPosition.x = path[0].x;
                lastPosition.y = _height;
                lastPosition.z = path[0].y;
                foreach (var data in path)
                {
                    tem.x = data.x;
                    tem.z = data.y;
                    tem.y = _height;
                    cost += Vector3.Distance(lastPosition, tem);
                    lastPosition = tem;
                }
            }
            else
            {
                Debug.Log("Path NULL!!!!!");
            }
            
            return cost;
        }


        private void CountPathCost()
        {
            _timeCost = 0;
            _safeCost = 0;
            _dangerPositionIndexes.Clear();
            if (_smoothPath.Count >= 1)
            {
                Vector3 tem = new Vector3();
                float dis = 0;
                Vector3 lastPosition = new Vector3();
                lastPosition.x = _smoothPath[0].x;
                lastPosition.y = _height;
                lastPosition.z = _smoothPath[0].y;
                for (int i = 0; i < _smoothPath.Count; i++)
                {
                    tem.x = _smoothPath[i].x;
                    tem.z = _smoothPath[i].y;
                    tem.y = _height;
                    _environmentalPerception2D.UpdateSensor(tem);
                    dis = _environmentalPerception2D.GetMinDistance();
                    if (dis <= _safeDistance)
                    {
                        _safeCost += _safeDistance - dis;
                        _dangerPositionIndexes.Add(i);
                    }

                    _timeCost += Vector3.Distance(lastPosition, tem);
                    lastPosition = tem;
                }
            }
        }

        public List<int> GetDangerPositionIndexes()
        {
            return _dangerPositionIndexes;
        }
        public float GetTimeCost()
        {
            return _timeCost;
        }

        public float GetSafeCost()
        {
            return _safeCost;
        }

        
        public bool ClearAllLine()
        {
            _pathLine.ClearAllLine();
            _line.ClearAllLine();
            _rrt.ClearShowPath();
            return true;
        }

        /*
         * Method Name: GetPath
         * Function: Get the path from Line
         * Input: NULL
         * Output: Path
         */
        public List<Vector2> GetPath()
        {
            _path.Clear();
            Vector2 tem = new Vector2();
            List<GameObject> objectTem = _line.GetAllLinePoint();
            for (int i = 0; i < objectTem.Count; i++)
            {
                tem.x = objectTem[i].transform.position.x;
                tem.y = objectTem[i].transform.position.z;
                _path.Add(tem);
            }
            return _path;
        }

        [System.Obsolete]
        public List<Vector2> GetSmoothPath()
        {
            _smoothPath.Clear();
            GetPath();
            int pathSize = _path.Count;
            //Debug.Log("pathSize: " + pathSize);
            if (pathSize >= 1)
            {
                if (pathSize == 1)
                {
                    _smoothPath.Add(_path[pathSize - 1]);
                }
                else if (pathSize == 2)
                {
                    _smoothPath.Add(_path[0]);
                    _smoothPath.Add(_path[pathSize - 1]);
                }
                else if (pathSize == 3)
                {
                    _smoothPath.Add(_path[0]);
                    _smoothPath.Add(_path[1]);
                    _smoothPath.Add(_path[pathSize - 1]);
                }
                else
                {
                    _smoothPath.Add(_path[0]);
                    _smoothPath.Add(_path[1]);
                    for (int i = 2; i < pathSize - 1; i++)
                    {
                        if ((Mathf.Abs(_path[i].x - _path[i - 1].x) <= 0.01f &&
                             Mathf.Abs(_path[i].y - _path[i - 1].y) >= 0.01f)||
                            (Mathf.Abs(_path[i].y - _path[i - 1].y) <= 0.01f)&&
                            (Mathf.Abs(_path[i].x - _path[i - 1].x) >= 0.01f))
                        {
                            _smoothPath.Add(_path[i]);
                        }
                        /*
                        if ((1 / (_path[i].x - _path[i - 1].x)) < 10e6 && ((1 / _path[i - 1].x - _path[i - 2].x)) < 10e6)
                        {
                             Debug.Log("value: "+ Mathf.Abs(((_path[i].y - _path[i - 1].y) / (_path[i].x - _path[i - 1].x)) - ((_path[i - 1].y - _path[i - 2].y) / (_path[i - 1].x - _path[i - 2].x))));
                            if (Mathf.Abs(((_path[i].y - _path[i - 1].y) / (_path[i].x - _path[i - 1].x)) - ((_path[i - 1].y - _path[i - 2].y) / (_path[i - 1].x - _path[i - 2].x))) >= _smoothPathFactor)
                            {
                                _smoothPath.Add(_path[i]);
                            }
                        }
                        else if((1 / (_path[i].x - _path[i - 1].x)) < 10e6 && ((1 / _path[i - 1].x - _path[i - 2].x)) > 10e6 || (1 / (_path[i].x - _path[i - 1].x)) > 10e6 && ((1 / _path[i - 1].x - _path[i - 2].x)) < 10e6)
                        {
                            _smoothPath.Add(_path[i]);
                        }
                        else if ((1 / (_path[i].y - _path[i - 1].y)) < 10e6 && ((1 / _path[i - 1].y - _path[i - 2].y)) > 10e6 || (1 / (_path[i].y - _path[i - 1].y)) > 10e6 && ((1 / _path[i - 1].y - _path[i - 2].y)) < 10e6)
                        {
                            _smoothPath.Add(_path[i]);
                        }
                        */
                    }
                    _smoothPath.Add(_path[pathSize - 1]);
                }

                List<Vector2> temPath = new List<Vector2>();
                temPath.Add(_smoothPath[0]);
                for (int i = 1; i < _smoothPath.Count-1; i+=1)
                {
                    temPath.Add(Vector2.Lerp(_smoothPath[i], _smoothPath[i + 1], 0.5f));

                }
                temPath.Add(_smoothPath[_smoothPath.Count - 1]);
                _smoothPath.Clear();
                _smoothPath = temPath;

                if (_isShowSmoothPath)
                {
                    _pathLine.ClearAllLine();
                    Vector2 startPosition = _smoothPath[0];
                    for (int i = 0; i < _smoothPath.Count; i++)
                    {
                        _pathLine.SetPosition(new Vector3(startPosition.x, _height, startPosition.y),
                            new Vector3(_smoothPath[i].x, _height, _smoothPath[i].y));
                      //  _pathLine.DrawLine();
                        startPosition = _smoothPath[i];
                    }
                }
            }

            CountPathCost();
           // Debug.Log("smoothPath: " + _smoothPath.Count);
            return _smoothPath;
        }

        /*
         * Method Name: SetPlotLineHz.
         * Function: Setting the plot line frequency according to the input parameter.
         * Input: The target frequency.
         * Output: The flag to indicate if set the frequency successfully.
         */
        public bool SetPlotLineHz(float hz)
        {
            _plotLineHz = hz;
            return true;
        }
        /*
         * Method Name: StopRunningPlotLine.
         * Function: Control the running process of plot line.
         * Input: True: close the plot line. False: Start the plot line.
         * Output: The flag to indicate if set the running process successfully.
         */
        public bool StopRunningPlotLine(bool data)
        {
            _runningPlotLine = data;
            return true;
        }

        /*
         * Method Name: StopPlotLine.
         * Function: Stop the thread of plot line.
         * Input: NULL.
         * Output: The flag to indicate if set the thread state successfully.
         */
        public bool StopPlotLine()
        {
            _stopThread = true;
            return true;
        }

        /*
         * Method Name: InitPlotLine
         * Function: Configure the plot line
         * Input: The material of line, the color of line, the width and height of line,the flag to show line
         * Output: The flag to indicate if configure the plot line instance successfully.
         */
        [System.Obsolete]
        public bool InitPlotPlanningObjectLine(Material mat, Color color, float widthX, float widthY,bool isPlot)
        {
            _line = new Line(mat, color, widthX, widthY, isPlot);
            Loom.RunAsync(
                () =>
                {
                    Thread thread = new Thread(PlotHumanPath);
                    thread.Start();
                }
            );
            return true;
        }

        public bool InitSensor(LayerMask layerMask, float minRange, float maxRange, Vector3 currentPosition,
            float horizontalAngleStart, float horizontalAngleEnd, float horizontalAngleInc, float safeRange,
            bool showLidar)
        {
            _environmentalPerception2D = new EnvironmentalPerception2D(layerMask, minRange, maxRange, currentPosition, horizontalAngleStart, horizontalAngleEnd, horizontalAngleInc, safeRange, showLidar);
            _safeDistance = safeRange;
            return true;
        }

        public bool InitRRT(float stepDetal, long maxFindStep, Vector2 startPosition, Vector2 targetPosition,
            Vector2 workMinRange, Vector2 workMaxRange, float safeRange, float findError, float rangeProbability,
            float height, LayerMask layerMask, float minRange, float maxRange, Vector3 currentPosition,
            float horizontalAngleStart, float horizontalAngleEnd, float horizontalAngleInc,
            bool showLidar, Material mat, Color color, float widthX, float widthY, bool isPlot)
        {
            _rrt = new RRT2D(stepDetal, maxFindStep, startPosition, targetPosition, workMinRange, workMaxRange, safeRange, findError, rangeProbability, height);
            _rrt.InitSensor(layerMask, minRange, maxRange, currentPosition, horizontalAngleStart, horizontalAngleEnd,
                horizontalAngleInc, safeRange, showLidar);
            _rrt.InitPlotLine(mat, color, widthX, widthY, isPlot);
            _rrt.SetIsShowPath(false);
            return true;
        }
        public bool InitPlotSmoothPathLine(Material mat, Color color, float widthX, float widthY, bool isPlot)
        {
            _pathLine = new Line(mat,color,widthX,widthY,isPlot);
            return true;
        }

        /*
         * Method Name: PlotHumanPath
         * Function: The thread function.
         * Input: NULL.
         * Output: NULL.
         */
        [System.Obsolete]
        void PlotHumanPath()
        {
            while (!_stopThread)
            {
                if (_runningPlotLine)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        _line.SetPosition(_startPosition, _planningObject.position);
                        _startPosition = _planningObject.position;
                        _line.DrawLine();
                    });
                }
                System.Threading.Thread.Sleep(_sleepTime);
            }
        }
        public HumanPlanning(Transform planningObject, float plotLineHz,float height)
        {
            _planningObject = planningObject;
            _plotLineHz = plotLineHz;
            _height = height;
            _stopThread = false;
            _runningPlotLine = false;
            _sleepTime = (int) ((1.0f / _plotLineHz) * 1000);
            _startPosition = _planningObject.position;
            _smoothPath = new List<Vector2>();
            _path = new List<Vector2>();
            _dangerPositionIndexes = new List<int>();
            _isShowSmoothPath = true;
            _fusionPath = new List<Vector2>();

        }

    }
}
