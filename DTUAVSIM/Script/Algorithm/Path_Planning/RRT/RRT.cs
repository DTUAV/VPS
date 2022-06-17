using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using Random = System.Random;
using DTUAV.Visualization_Module.Line;

namespace DTUAV.Algorithm_Module.Path_Planning.RRT
{
  
    struct TreeNode
    {
        public int parentId;
        public int ownId;
        public Vector2 nodePosition;
    }

    public class RRT2D
    {
        private float _stepDetal; //the step to move
        private float _stepDetalFactor = 0.9f;//the factor to reduce the move step
        private long _maxReduceDetalTimes = 10;//the maximum times to reduce the detal of move step
        private long _maxFindStep; //the max step to find the path
        private float _smoothPathFactor = 0.1f;//the factor of path smoothing//this factor will determine the selected number 
        private float _safeRange; //the safe range
        private float _findError;
        private float _rangeProbability;
        private Vector2 _startPosition;
        private Vector2 _targetPosition;
        private Vector2 _workMinRange;
        private Vector2 _workMaxRange;
        private float _height;
        private Random _random;
        private List<Vector2> _path;
        private List<Vector2> _smoothPath;
        private List<TreeNode> _treeNodes;
        private bool _isFind;
        private EnvironmentalPerception2D _environmentalPerception2D;
        private Line _line;
        private bool _isShowSmoothPath = true;
        private bool _isShowPath = true;

        private List<Vector2> SimplyPath(List<Vector2> path, int inc)
        {
            List<Vector2> ret = new List<Vector2>();
            if (path.Count > 0)
            {
                ret.Add(path[0]);
                Vector2 fistPoint = path[0];
                Vector2 lastPoint = path[0];
                for (int i = 0; i < path.Count; i += inc)
                {
                    
                    if (!CheckRangeIsSafe(fistPoint, path[i], (int)Vector2.Distance(fistPoint,path[i])))//check points number: 20
                    {
                        ret.Add(lastPoint);
                        fistPoint = lastPoint;
                    }
                    else
                    {
                        lastPoint = path[i];
                    }
                }
                ret.Add(path[path.Count - 1]);
            }
            return ret;
        }

        private bool CheckRangeIsSafe(Vector2 start, Vector2 end, int num)
        {
            float inc_x = 0;
            float inc_y = 0;
            bool ret = true;
            Vector3 tem = new Vector3();
            if (num==0)
            {
                num = 1;
            }
            if (Mathf.Abs(start.x - end.x) < 10e-3)
            {
                inc_x = 0;
            }
            else
            {
                inc_x = (end.x - start.x) / num;
            }

            if (Mathf.Abs(start.y - end.y) < 10e-3)
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
        public float GetPathTimeCost()
        {
            return CountPathTimeCost(_smoothPath);
        }
        public float GetPathSafeCost()
        {
            return CountPathSafeCost(_smoothPath);
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
                    if (dis <= _safeRange)
                    {
                        ret += _safeRange - dis;
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
            if (path.Count >= 1)
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


        public bool ClearShowPath()
        {
            _line.ClearAllLine();
            return true;
        }

        public bool ShowPath(List<Vector2> path)
        {
            if (path.Count>1)
            {
                _line.ClearAllLine();
                Vector2 startPosition = path[0];
                for (int i = 0; i < path.Count; i++)
                {
                    _line.SetPosition(new Vector3(startPosition.x, _height, startPosition.y),
                        new Vector3(path[i].x, _height, path[i].y));
                    _line.DrawLine();
                    startPosition = path[i];
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SetIsShowPath(bool data)
        {
            _isShowPath = data;
            return true;
        }

        public bool SetIsShowSmoothPath(bool data)
        {
            _isShowSmoothPath = data;
            return true;
        }
        public bool SetSmoothPathFactor(float factor)
        {
            _smoothPathFactor = factor;
            return true;
        }

        public bool SetStepDetalFactor(float factor)
        {
            _stepDetalFactor = factor;
            return true;
        }

        public bool SetMaxReduceDetalTimes(long times)
        {
            _maxReduceDetalTimes = times;
            return true;
        }

        public bool SetWorkMaxRange(Vector2 workMaxRange)
        {
            _workMaxRange = workMaxRange;
            return true;
        }

        public bool SetWorkMinRange(Vector2 workMinRange)
        {
            _workMinRange = workMinRange;
            return true;
        }

        public bool SetTargetPosition(Vector2 targetPosition)
        {
            _targetPosition = targetPosition;
            return true;
        }

        public bool SetStartPosition(Vector2 startPosition)
        {
            _startPosition = startPosition;
            return true;
        }

        public bool SetSafeRange(float safeRange)
        {
            _safeRange = safeRange;
            return true;
        }

        public bool SetMaxFindStep(long maxFindStep)
        {
            _maxFindStep = maxFindStep;
            return true;
        }

        public bool SetStepDetal(float stepDetal)
        {
            _stepDetal = stepDetal;
            return true;
        }

        public float GetSmoothPathFactor()
        {
            return _smoothPathFactor;
        }

        public List<Vector2> GetSmoothPath(List<Vector2> path,float smoothPathFactor)
        {
            List<Vector2> smoothPath = new List<Vector2>();
            int pathSize = path.Count;
            //Debug.Log("pathSize: " + pathSize);
            if (pathSize >= 1)
            {
                if (pathSize == 1)
                {
                    smoothPath.Add(path[pathSize - 1]);
                }
                else if (pathSize == 2)
                {
                    smoothPath.Add(path[0]);
                    smoothPath.Add(path[pathSize - 1]);
                }
                else if (pathSize == 3)
                {
                    smoothPath.Add(path[0]);
                    smoothPath.Add(path[1]);
                    smoothPath.Add(path[pathSize - 1]);
                }
                else
                {
                    smoothPath.Add(path[0]);
                    smoothPath.Add(path[1]);
                    for (int i = 2; i < pathSize - 1; i++)
                    {
                        if ((1 / (path[i].x - path[i - 1].x)) < 10e6 && ((1 / path[i - 1].x - path[i - 2].x)) < 10e6)
                        {
                            // Debug.Log("value: "+ Mathf.Abs(((_path[i].y - _path[i - 1].y) / (_path[i].x - _path[i - 1].x)) - ((_path[i - 1].y - _path[i - 2].y) / (_path[i - 1].x - _path[i - 2].x))));
                            if (Mathf.Abs(((path[i].y - path[i - 1].y) / (path[i].x - path[i - 1].x)) - ((path[i - 1].y - path[i - 2].y) / (path[i - 1].x - path[i - 2].x))) >= smoothPathFactor)
                            {
                                smoothPath.Add(path[i]);
                            }
                        }
                    }
                    smoothPath.Add(path[pathSize - 1]);
                }
            }
           // Debug.Log("smoothPath: " + _smoothPath.Count);

            return smoothPath;
        }

        public List<Vector2> GetSmoothPath()
        {
            _smoothPath.Clear();
            int pathSize = _path.Count;
          //  Debug.Log("pathSize: "+pathSize);
            if (pathSize>=1)
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
                        if ((1/(_path[i].x - _path[i - 1].x)) < 10e6 && ((1/_path[i - 1].x - _path[i - 2].x)) < 10e6)
                        {
                           // Debug.Log("value: "+ Mathf.Abs(((_path[i].y - _path[i - 1].y) / (_path[i].x - _path[i - 1].x)) - ((_path[i - 1].y - _path[i - 2].y) / (_path[i - 1].x - _path[i - 2].x))));
                            if (Mathf.Abs(((_path[i].y - _path[i - 1].y) / (_path[i].x - _path[i - 1].x)) - ((_path[i - 1].y - _path[i - 2].y) / (_path[i - 1].x - _path[i - 2].x))) >= _smoothPathFactor)
                            {
                                _smoothPath.Add(_path[i]);
                            }
                        }
                    }
                    _smoothPath.Add(_path[pathSize - 1]);
                }
                //show before simply smoothPath
               /* 
                if (_isShowSmoothPath)
                {
                    _line.ClearAllLine();
                    Vector2 startPosition = _smoothPath[0];
                    for (int i = 0; i < _smoothPath.Count; i++)
                    {
                        _line.SetPosition(new Vector3(startPosition.x, _height, startPosition.y),
                            new Vector3(_smoothPath[i].x, _height, _smoothPath[i].y));
                        _line.DrawLine();
                        startPosition = _smoothPath[i];
                    }
                }
                */
            }
           Debug.Log("RRT before simply smoothPath: "+_smoothPath.Count);
           List<Vector2> _simplyPath = SimplyPath(_path, 1);
           Debug.Log("RRT after simply path size: " + _simplyPath.Count);
           //show after simply smoothPath
           
           if (_isShowSmoothPath)
           {
               _line.ClearAllLine();
               Vector2 startPosition = _simplyPath[0];
               for (int i = 0; i < _simplyPath.Count; i++)
               {
                   _line.SetPosition(new Vector3(startPosition.x, _height, startPosition.y),
                       new Vector3(_simplyPath[i].x, _height, _simplyPath[i].y));
                   _line.DrawLine();
                   startPosition = _simplyPath[i];
               }
           }
           
           // return _smoothPath;
           return _simplyPath;
        }

        public float GetStepDetalFactor()
        { 
            return _stepDetalFactor;
        }

        public long GetMaxReduceDetalTimes()
        {
            return _maxReduceDetalTimes;
        }

        public bool GetIsFind()
        {
            return _isFind;
        }

        public List<Vector2> GetPath()
        {
            return _path;
        }

        public Vector2 GetMaxRange()
        {
            return _workMinRange;
        }

        public Vector2 GetMinRange()
        {
            return _workMaxRange;
        }

        public Vector2 GetTargetPosition()
        {
            return _targetPosition;
        }

        public Vector2 GetStartPosition()
        {
            return _startPosition;
        }

        public float GetMaxFindStep()
        {
            return _maxFindStep;
        }

        public float GetStepDetal()
        {
            return _stepDetal;
        }

        private float randData(float minRange, float maxRange)
        {
            return (float)(_random.NextDouble() * (maxRange - minRange) + minRange);
        }

        [System.Obsolete]
        public bool FindPath()
        {
            long currentFindTime = 1;
            _line.ClearAllLine();
            _path.Clear();
            TreeNode startNode = new TreeNode();
            startNode.nodePosition = _startPosition;
            startNode.parentId = 0;
            startNode.ownId = 0;
            Vector2 nextTargetPosition = new Vector2();
            Vector2 tempDir = new Vector2();
            TreeNode tempNode = new TreeNode();
            TreeNode newNode = new TreeNode();
            int j = 1;
            _isFind = false;
            float stepDetal = _stepDetal;
            _environmentalPerception2D.UpdateSensor(new Vector3(_startPosition.x, _height, _startPosition.y));
            if (_environmentalPerception2D.GetIsSafe())
            {
                while (!_isFind && currentFindTime <= _maxReduceDetalTimes)
                {
                    _treeNodes.Clear();
                    _treeNodes.Add(startNode);
                    for (int i = 0; i < _maxFindStep; i++)
                    {
                        if (_random.NextDouble() >= _rangeProbability)
                        {
                            nextTargetPosition = _targetPosition;
                        }
                        else
                        {
                            nextTargetPosition = new Vector2(randData(_workMinRange.x, _workMaxRange.x),
                                randData(_workMinRange.y, _workMaxRange.y));
                        }

                        //Debug.Log(nextTargetPosition);
                        tempNode = FindNearNode(nextTargetPosition);
                        tempDir.x = (nextTargetPosition.x - tempNode.nodePosition.x) * stepDetal /
                                    Vector2.Distance(nextTargetPosition, tempNode.nodePosition);
                        tempDir.y = (nextTargetPosition.y - tempNode.nodePosition.y) * stepDetal /
                                    Vector2.Distance(nextTargetPosition, tempNode.nodePosition);
                        newNode.nodePosition = tempNode.nodePosition + tempDir;
                        _environmentalPerception2D.UpdateSensor(new Vector3(newNode.nodePosition.x, _height,
                            newNode.nodePosition.y));
                        newNode.ownId = j;
                        newNode.parentId = tempNode.ownId;
                        if (_environmentalPerception2D.GetIsSafe())
                        {
                            _treeNodes.Add(newNode);
                            j = j + 1;
                            // _line.SetPosition(new Vector3(tempNode.nodePosition.x, _height, tempNode.nodePosition.y), new Vector3(newNode.nodePosition.x, _height, newNode.nodePosition.y));
                            // _line.DrawLine();
                        }

                        if (Vector2.Distance(newNode.nodePosition, _targetPosition) <= _findError)
                        {
                            _isFind = true;
                            break;
                        }
                    }

                    currentFindTime++;
                    stepDetal *= _stepDetalFactor;
                }

                if (_isFind)
                {
                  //  Debug.Log("stepDetal: "+stepDetal);
                  //  Debug.Log("currentFindTime: "+currentFindTime);
                    int parentId = _treeNodes[_treeNodes.Count - 1].parentId;
                    List<Vector2> _tempPath = new List<Vector2>();
                    _tempPath.Add(_treeNodes[_treeNodes.Count - 1].nodePosition);
                    TreeNode tempTreeNode = new TreeNode();
                    while (parentId != 0)
                    {
                        tempTreeNode = GetParentNode(parentId);
                        parentId = tempTreeNode.parentId;
                        _tempPath.Add(tempTreeNode.nodePosition);
                    }

                    _tempPath.Add(_startPosition);

                    for (int i = _tempPath.Count - 1; i >= 0; i--)
                    {
                        _path.Add(_tempPath[i]);
                    }

                    if (_isShowPath)
                    {
                        _line.ClearAllLine();
                        Vector2 startPosition = _path[0];
                        for (int i = 0; i < _path.Count; i++)
                        {
                            _line.SetPosition(new Vector3(startPosition.x, _height, startPosition.y),
                                new Vector3(_path[i].x, _height, _path[i].y));
                            _line.DrawLine();
                            startPosition = _path[i];
                        }
                    }

                    return true;
                }
            }

            return false;

        }
        
    private TreeNode GetParentNode(int ParentID)
        {
            TreeNode retNode = new TreeNode();
            for (int i = 0; i < _treeNodes.Count; i++)
            {
                if (_treeNodes[i].ownId == ParentID)
                {
                    retNode = _treeNodes[i]; 
                }
            }

            return retNode;
        }

        private TreeNode FindNearNode(Vector2 targetPosition)
        {
            TreeNode retNode = new TreeNode();
            float distance = 10000;
            for (int i = 0; i < _treeNodes.Count; i++)
            {
                if (Vector2.Distance(targetPosition,_treeNodes[i].nodePosition)<= distance)
                {
                    retNode = _treeNodes[i];
                    distance = Vector2.Distance(targetPosition, _treeNodes[i].nodePosition);
                }
            }
           // Debug.Log(distance);
            return retNode;
        }

        [System.Obsolete]
        /*
         *  Function Information
         *  Input: The target position: "targetPosition" && The start position: "startPosition"
         *  Output: The path find flag is used to indicated if find the path.(bool)
         */
        public bool FindPath(Vector2 startPosition, Vector2 targetPosition)
        {
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            return FindPath();
        }

        public bool InitPlotLine(Material mat, Color color, float widthX, float widthY,
            bool isPlot)
        {
            _line = new Line(mat,color,widthX,widthY, isPlot);
            return true;
        }

        public bool InitSensor(LayerMask layerMask, float minRange, float maxRange, Vector3 currentPosition,
            float horizontalAngleStart, float horizontalAngleEnd, float horizontalAngleInc, float safeRange,
            bool showLidar)
        {
            _environmentalPerception2D = new EnvironmentalPerception2D(layerMask,minRange,maxRange,currentPosition,horizontalAngleStart,horizontalAngleEnd,horizontalAngleInc,safeRange,showLidar);
            return true;
        }

        public RRT2D(float stepDetal,long maxFindStep,Vector2 startPosition,Vector2 targetPosition,Vector2 workMinRange, Vector2 workMaxRange, float safeRange, float findError, float rangeProbability, float height)
        {
            _stepDetal = stepDetal;
            _maxFindStep = maxFindStep;
            _workMinRange = workMinRange;
            _workMaxRange = workMaxRange;
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            _safeRange = safeRange;
            _findError = findError;
            _rangeProbability = rangeProbability;
            _height = height;
            _random = new Random();
            _path = new List<Vector2>();
            _smoothPath = new List<Vector2>();
            _treeNodes = new List<TreeNode>();
        }

        public RRT2D(float stepDetal, long maxFindStep, Vector2 startPosition, Vector2 targetPosition, Vector2 workMinRange, Vector2 workMaxRange, float safeRange, float findError, float rangeProbability, float height, float stepDetalFactor)
        {
            _stepDetal = stepDetal;
            _maxFindStep = maxFindStep;
            _workMinRange = workMinRange;
            _workMaxRange = workMaxRange;
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            _safeRange = safeRange;
            _findError = findError;
            _rangeProbability = rangeProbability;
            _height = height;
            _stepDetalFactor = stepDetalFactor;
            _random = new Random();
            _path = new List<Vector2>();
            _smoothPath = new List<Vector2>();
            _treeNodes = new List<TreeNode>();
        }
        public RRT2D(float stepDetal, long maxFindStep, Vector2 startPosition, Vector2 targetPosition, Vector2 workMinRange, Vector2 workMaxRange, float safeRange, float findError, float rangeProbability, float height, float stepDetalFactor,long maxReduceDetalTimes)
        {
            _stepDetal = stepDetal;
            _maxFindStep = maxFindStep;
            _workMinRange = workMinRange;
            _workMaxRange = workMaxRange;
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            _safeRange = safeRange;
            _findError = findError;
            _rangeProbability = rangeProbability;
            _height = height;
            _stepDetalFactor = stepDetalFactor;
            _maxReduceDetalTimes = maxReduceDetalTimes;
            _random = new Random();
            _path = new List<Vector2>();
            _smoothPath = new List<Vector2>();
            _treeNodes = new List<TreeNode>();
        }

        public RRT2D(float stepDetal, long maxFindStep, Vector2 startPosition, Vector2 targetPosition, Vector2 workMinRange, Vector2 workMaxRange, float safeRange, float findError, float rangeProbability, float height,long maxReduceDetalTimes)
        {
            _stepDetal = stepDetal;
            _maxFindStep = maxFindStep;
            _workMinRange = workMinRange;
            _workMaxRange = workMaxRange;
            _startPosition = startPosition;
            _targetPosition = targetPosition;
            _safeRange = safeRange;
            _findError = findError;
            _rangeProbability = rangeProbability;
            _height = height;
            _maxReduceDetalTimes = maxReduceDetalTimes;
            _random = new Random();
            _path = new List<Vector2>();
            _smoothPath = new List<Vector2>();
            _treeNodes = new List<TreeNode>();
        }
    }

}
