using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAVSIM.Sensor.EnvironmentalPerception
{
    public class environment_perception 
    {
        private LayerMask _mask;                              //The Environment Layer of Perception Sensor
        private float _perceptionRange;                       //The Range of Environmental Perception Sensor Work
        private float _perceptionAngleStart;                  //The Start Angle of Environment Perception Sensor
        private float _perceptionAngleEnd;                    //The End Angle of Environment Perception Sensor
        private float _perceptionAngleInc;                    //The Increment Angle of Environment Perception Sensor

        private float _maxDistance;                           //The Maximum Distance Perception in Current Position
        private float _minDistance;                           //The Minimum Distance Perception in Current Position

        private int _minDistanceId;                           //The Id of Minimum Distance Perception in Current Position
        private int _maxDistanceId;                           //The Id of Maximum Distance Perception in Current Position

        private Vector3 _minDistanceDirection;                //The Perception Direction of Minimum Distance Perception in Current Position
        private Vector3 _maxDistanceDirection;                //The Perception Direction of Maximum Distance Perception in Current Position

        private List<Vector3> _perceptionDirections;          //The All Perception Direction of Perception Sensor
        private List<Vector3> _targetPerceptionPositions;     //The Target Perception Position of Perception Sensor
        private List<Vector3> _truePerceptionPositions;       //The True Perception Position of Perception Sensor
        private List<float> _allDistances;                    //The Hit Distance of Perception Sensor

        private Vector3 _currentPosition;                      //The Current Position of Environment Perception Sensor

        private float _safeDistance = 1.0f;                   //The Safe Distance of Perception Sensor
        private List<Vector3> _obstaceDirections;             //The Perception Direction of Safe Distance in Current Position
        private List<Vector3> _obstacePositions;              //The Perception Position of Safe Distance in Current Position
        private List<float> _obstaceDistances;                //The Perception Distance of Safe Distance in Current Position
        private bool _isSafe;

        private Vector3 _minDistancePosition;

        public Vector3 GetDangerMinDistancePosition()
        {
            return _minDistancePosition;
        }
        public float GetDangerMinDistance()
        {
            return _minDistance;
        }
        public bool GetIsSafe(float safeDistance)
        {
            _isSafe = true;
            _minDistance = 1000;
            _minDistanceId = 0;
            for (int i = 0; i < _allDistances.Count; i++)
            {
                if (_allDistances[i] < safeDistance&&_allDistances[i]<_minDistance)
                {
                    _isSafe = false;
                    //Debug.Log("danger: "+_allDistances[i]);
                    //break;
                    _minDistance = _allDistances[i];
                    _minDistanceId = i;
                    _minDistancePosition = _truePerceptionPositions[i];
                }
            }
            _minDistanceDirection = _perceptionDirections[_minDistanceId];
            return _isSafe;
        }
        public bool SetSafeDistance(float safeDistance)
        {
            _safeDistance = safeDistance;
            return true;
        }
        public bool UpdateObstaceInfo()
        {
            _obstaceDirections.Clear();
            _obstaceDistances.Clear();
            _obstacePositions.Clear();
            for(int i=0;i<_allDistances.Count;i++)
            {
                if(_allDistances[i]<=_safeDistance)
                {
                    _obstacePositions.Add(_truePerceptionPositions[i]);
                    _obstaceDirections.Add(_perceptionDirections[i]);
                    _obstaceDistances.Add(_allDistances[i]);
                    
                }
            }
            return true;
        }

        public bool UpdateObstaceInfo(float safeDistace)
        {
            _safeDistance = safeDistace;
            _obstaceDirections.Clear();
            _obstaceDistances.Clear();
            _obstacePositions.Clear();
            for (int i = 0; i < _allDistances.Count; i++)
            {
                if (_allDistances[i] <= _safeDistance)
                {
                    _obstacePositions.Add(_truePerceptionPositions[i]);
                    _obstaceDirections.Add(_perceptionDirections[i]);
                    _obstaceDistances.Add(_allDistances[i]);
                }
            }
            return true;
        }
        public List<Vector3> GetObstaceDirections()
        {
            return _obstaceDirections;
        }

        public List<Vector3> GetObstacePositions()
        {
            return _obstacePositions;
        }

        public List<float> GetObstaceDistances()
        {
            return _obstaceDistances;
        }
        
        public int GetMinDistanceId()
        {
            return _minDistanceId;
        }

        public int GetMaxDistanceId()
        {
            return _maxDistanceId;
        }

        public Vector3 GetMinDistanceDirection()
        {
            return _minDistanceDirection;
        }

        public Vector3 GetMaxDistanceDirection()
        {
            return _maxDistanceDirection;
        }

        public List<float> GetAllDistances()
        {
            return _allDistances;
        }

        public List<Vector3> GetTruePerceptionPositions()
        {
            return _truePerceptionPositions;
        }

        public List<Vector3> GetTargetPerceptionPositions()
        {
            return _targetPerceptionPositions;
        }
        public List<Vector3> GetPerceptionDirections()
        {
            return _perceptionDirections;
        }

        public LayerMask GetLayerMask()
        {
            return _mask;
        }

        public float GetPerceptionRange()
        {
            return _perceptionRange;
        }

        public float GetPerceptionAngleStart()
        {
            return _perceptionAngleStart;
        }

        public float GetPerceptionAngleEnd()
        {
            return _perceptionAngleEnd;
        }

        public float GetPerceptionAngleInc()
        {
            return _perceptionAngleInc;
        }

        public bool UpdateLayerMask(LayerMask mask)
        {
            _mask = mask;
            return true;
        }
        public bool UpdatePerceptionRange(float range)
        {
            _perceptionRange = range;
            return true;
        }

        public bool UpdatePerceptionAngleInc(float incAngle)
        {
            _perceptionAngleInc = incAngle;
            UpdatePerceptionDirections();
            return true;
        }
        public bool UpdatePerceptionAngleEnd(float endAngle)
        {
            _perceptionAngleEnd = endAngle;
            UpdatePerceptionDirections();
            return true;
        }
        public bool UpdatePerceptionAngleStart(float startAngle)
        {
            _perceptionAngleStart = startAngle;
            UpdatePerceptionDirections();
            return true;
        }

        private bool UpdatePerceptionDirections()
        {
            _perceptionDirections.Clear();
            float currentAngle = _perceptionAngleStart;
            Vector3 dec = Vector3.zero;
            while (currentAngle > _perceptionAngleEnd)
            {
                dec.x = Mathf.Sin(currentAngle * (Mathf.PI / 180));
                dec.z = Mathf.Cos(currentAngle * (Mathf.PI / 180));
                _perceptionDirections.Add(dec);
                currentAngle = currentAngle + _perceptionAngleInc;
            }
            return true;
        }

        public bool UpdateSensor(Vector3 currentPosition)
        {
            _truePerceptionPositions.Clear();
            _allDistances.Clear();
            SetCurrentPosition(currentPosition);
            Ray ray = new Ray(currentPosition, _perceptionDirections[0].normalized);
            RaycastHit hit;
            Vector3 targetRayPosition = Vector3.zero;

            for (int i=0;i< _perceptionDirections.Count;i++)
            {
                ray.direction = _perceptionDirections[i];
                if (Physics.Raycast(ray, out hit, _perceptionRange, _mask))
                {
                    Debug.DrawLine(currentPosition, hit.point, Color.red);
                    _truePerceptionPositions.Add(hit.point);
                    _allDistances.Add(hit.distance);
                }
                else
                {
                    targetRayPosition.x = currentPosition.x + _perceptionRange * ray.direction.x;
                    targetRayPosition.y = currentPosition.y + _perceptionRange * ray.direction.y;
                    targetRayPosition.z = currentPosition.z + _perceptionRange * ray.direction.z;
                    Debug.DrawLine(currentPosition, targetRayPosition, Color.blue);
                    _truePerceptionPositions.Add(targetRayPosition);
                    _allDistances.Add(_perceptionRange);
                }               
            }
            return true;
        }

        public float GetMaxDistance()
        {
            _maxDistance = -10000;
            _maxDistanceId = 0;
            for(int i = 0; i<_allDistances.Count;i++)
            {
                if(_allDistances[i]>_maxDistance)
                {
                    _maxDistance = _allDistances[i];
                    _maxDistanceId = i;
                }
            }
            _maxDistanceDirection = _perceptionDirections[_maxDistanceId];
            return _maxDistance;
        }

        public float GetMinDistance()
        {
            _minDistance = 10000;
            _minDistanceId = 0;
            for(int i = 0; i<_allDistances.Count;i++)
            {
                if(_allDistances[i]<_minDistance)
                {
                    _minDistance = _allDistances[i];
                    _minDistanceId = i;
                 //   Debug.Log("_minDistance: "+_minDistance);
                }
            }
            _minDistanceDirection = _perceptionDirections[_minDistanceId];
            return _minDistance;
        }

        public bool SetCurrentPosition(Vector3 currentPosition)
        {
            _targetPerceptionPositions.Clear();
            _currentPosition = currentPosition;
            Vector3 cur_position = Vector3.zero;
            float currentAngle = _perceptionAngleStart;
            while (currentAngle < _perceptionAngleEnd)
            {
                cur_position.x = currentPosition.x + Mathf.Sin((currentAngle * (Mathf.PI / 180)));
                cur_position.y = currentPosition.y;
                cur_position.z = currentPosition.z + Mathf.Cos((currentAngle * (Mathf.PI / 180)));
                _targetPerceptionPositions.Add(cur_position);
                currentAngle = currentAngle + _perceptionAngleInc;
            }
            return true;
        }

        public environment_perception(LayerMask mask, float perceptionRange, float perceptionAngleStart, float perceptionAngleEnd, float perceptionAngleInc)
        {
            _mask = mask;
            _perceptionAngleEnd = perceptionAngleEnd;
            _perceptionAngleStart = perceptionAngleStart;
            _perceptionAngleInc = perceptionAngleInc;
            _perceptionRange = perceptionRange;
            float currentAngle = perceptionAngleStart;
            Vector3 dec = Vector3.zero;
            _truePerceptionPositions = new List<Vector3>();
            _targetPerceptionPositions = new List<Vector3>();
            _allDistances = new List<float>();
            _currentPosition = new Vector3();
            _perceptionDirections = new List<Vector3>();
            _obstaceDirections = new List<Vector3>();
            _obstacePositions = new List<Vector3>();
            _obstaceDistances = new List<float>();
            while (currentAngle<perceptionAngleEnd)
            {
                dec.x = Mathf.Sin(currentAngle * (Mathf.PI / 180));
                dec.z = Mathf.Cos(currentAngle * (Mathf.PI / 180));
                _perceptionDirections.Add(dec);
                currentAngle = currentAngle + perceptionAngleInc;
            }
        }

        public environment_perception(LayerMask mask, float perceptionRange, float perceptionAngleStart, float perceptionAngleEnd, float perceptionAngleInc,float safeDistance)
        {
            _safeDistance = safeDistance;
            _mask = mask;
            _perceptionAngleEnd = perceptionAngleEnd;
            _perceptionAngleStart = perceptionAngleStart;
            _perceptionAngleInc = perceptionAngleInc;
            _perceptionRange = perceptionRange;
            float currentAngle = perceptionAngleStart;
            Vector3 dec = Vector3.zero;
            _truePerceptionPositions = new List<Vector3>();
            _targetPerceptionPositions = new List<Vector3>();
            _allDistances = new List<float>();
            _currentPosition = new Vector3();
            _perceptionDirections = new List<Vector3>();
            _obstaceDirections = new List<Vector3>();
            _obstacePositions = new List<Vector3>();
            _obstaceDistances = new List<float>();
            while (currentAngle < perceptionAngleEnd)
            {
                dec.x = Mathf.Sin(currentAngle * (Mathf.PI / 180));
                dec.z = Mathf.Cos(currentAngle * (Mathf.PI / 180));
                _perceptionDirections.Add(dec);
                currentAngle = currentAngle + perceptionAngleInc;
            }
        }


    }
}