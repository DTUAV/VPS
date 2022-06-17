using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVSIM.Sensor.EnvironmentalPerception;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Generic;
namespace DTUAVCARS.Controller
{
    public class VirtualGuide
    {
        private int _predictedSteps;                                                         //The Number of Predicted Steps                                          
        private List<Vector3> _predictedPositions;                                           //The All Positions of Predictor
        private Vector3 _currentPosition;                                                    //The Position of Current Object
        private Vector3 _currentDirection;                                                   //The Motion Direction of Object
        private float _safeRange;                                                            //The Safe Range of Object Motion
        private Vector3 _controllerOutput;                                                   //The Value of Controller Output
        private List<float> _omiga;                                                          //The Weight of Diffence Step of Predicted Position
        private float _omigaNow = 1.0f;
        private environment_perception _environmentPerception;                               //The Environment Perception Sensor
        private float _costX;                                                                //The Cost of X Direction
        private float _costY;                                                                //The Cost of Y Direction
        private float _costZ;                                                                //The Cost of Z Direction
        private float _motionDirectionX;                                                     //The Current Motion Direction X of Object
        private float _motionDirectionY;                                                     //The Current Motion Direction Y of Object
        private float _motionDirectionZ;                                                     //The Current Motion Direction Z of Object

        private float _leftTheta = 45.0f;//theta_1                                           //The Value Perception Range of Left 
        private float _rightTheta = 45.0f;//theta_2                                          //The Value Perception Range of Right

        private float _adjustGuideRange = 0.8f;                                              //The Paramter to Adjust the Auide Range
        private Vector3 _leftThetaDirection;                                                 //Temp Parameter

        private Vector3 _rightThetaDirection;                                                //Temp Parameter
        private bool _isGuideControl;                                                        //If Start Guide Control:True or False


        public bool SetOmigaNow(float omigaNow)
        {
            _omigaNow = omigaNow;
            return true;
        }
        public bool SetAdjustGuideRange(float adjustGuideRange)
        {
            _adjustGuideRange = adjustGuideRange;
            return true;
        }
        public bool GetIsGuideControl()
        {
            return _isGuideControl;
        }
        public Vector3 GetLeftThetaDirection()
        {
            return _leftThetaDirection;
        }

        public Vector3 GetRightThetaDirection()
        {
            return _rightThetaDirection;
        }
        public bool UpdateSafeRange(float safeRange)
        {
            _safeRange = safeRange;
            return true;
        }
        public float GetCostX()
        {
            return _costX;
        }

        public float GetCostY()
        {
            return _costY;
        }

        public float GetCostZ()
        {
            return _costZ;
        }

        public bool UpdateOmiga(List<float> omiga)
        {
            _omiga = omiga;
            return true;
        }

        public Vector3 GetControllerOutput()
        {
            return _controllerOutput;
        }

        public void UpdateController3(List<Vector3> predictedPositions, Vector3 currentPosition)
        {
            _costX = 0;
            _costY = 0;
            _costZ = 0;
            _currentPosition = currentPosition;
            _predictedPositions = predictedPositions;   
            _environmentPerception.UpdateSensor(currentPosition);
            _controllerOutput = Vector3.zero;
            if (_environmentPerception.GetIsSafe(_safeRange))
            {
                for (int i = 0; i < _predictedPositions.Count; i++)
                {
                    _environmentPerception.UpdateSensor(_predictedPositions[i]);
                    if (!_environmentPerception.GetIsSafe(_safeRange))
                    {
                        // _controllerOutput += _omiga[i] * (_predictedPositions[i] - _environmentPerception.GetDangerMinDistancePosition());
                        _controllerOutput += _omiga[i] * (_predictedPositions[i] - _environmentPerception.GetDangerMinDistancePosition()) * (_safeRange - Vector3.Distance(_currentPosition, _environmentPerception.GetDangerMinDistancePosition())) / Vector3.Distance(_currentPosition, _environmentPerception.GetDangerMinDistancePosition());//new control rule to promise the more danger, the output more huge.;
                    }
                }
            }
            else
            {
               // Vector3 cost = currentPosition - _environmentPerception.GetDangerMinDistancePosition();
                Vector3 cost = (_currentPosition - _environmentPerception.GetDangerMinDistancePosition())*(_safeRange-Vector3.Distance(_currentPosition, _environmentPerception.GetDangerMinDistancePosition()))/ Vector3.Distance(_currentPosition, _environmentPerception.GetDangerMinDistancePosition());//new control rule to promise the more danger, the output more huge.
                _controllerOutput = _omigaNow * cost;
        
            }
            
        }

        public void UpdateController2(List<Vector3> predictedPositions, Vector3 currentPosition)
        {
            _costX = 0;
            _costY = 0;
            _costZ = 0;
            _currentPosition = currentPosition;
            _predictedPositions = predictedPositions;
            Debug.Log("predictedPositionSize: " + _predictedPositions.Count);
            if (Mathf.Abs(_predictedPositions[0].x - _currentPosition.x) <= 0.0001)
            {
                _motionDirectionX = 0;
            }
            else
            {
                _motionDirectionX = (_predictedPositions[0].x - _currentPosition.x) / Mathf.Abs(_predictedPositions[0].x - _currentPosition.x);
            }
            if (Mathf.Abs(_predictedPositions[0].y - _currentPosition.y) <= 0.0001)
            {
                _motionDirectionY = 0;
            }
            else
            {
                _motionDirectionY = (_predictedPositions[0].y - _currentPosition.y) / Mathf.Abs(_predictedPositions[0].y - _currentPosition.y);
            }
            if (Mathf.Abs(_predictedPositions[0].z - _currentPosition.z) <= 0.0001)
            {
                _motionDirectionZ = 0;
            }
            else
            {
                _motionDirectionZ = (_predictedPositions[0].z - _currentPosition.z) / Mathf.Abs(_predictedPositions[0].z - _currentPosition.z);
            }

            Debug.Log("CurrentPosition: " + _currentPosition);
            Debug.Log("PredictedPosition: " + _predictedPositions[0]);
            Debug.Log("MotionDirectionX: " + _motionDirectionX + " MotionDirectionY: " + _motionDirectionY + " MotionDirectionZ: " + _motionDirectionZ);
            float minDistance = 0;

            for (int i = 0; i < _predictedPositions.Count; i++)
            {

                _environmentPerception.UpdateSensor(_predictedPositions[i]);
                _environmentPerception.UpdateObstaceInfo();
                _currentDirection = _predictedPositions[i] - _currentPosition;
                minDistance = _environmentPerception.GetMinDistance();
                Debug.Log("minDistance :"+minDistance);
                if (i == 0)//Only Use the First Predicted Position To Detemined If Start Virtual Guide Control
                {
                    if (minDistance <= _adjustGuideRange * _safeRange)//---or (if(minDistance<=_adjustGuideRange*_safeRange&& VectorDot(_currentDirection.normalized, _environmentPerception.GetMinDistanceDirection().normalized)>=0))
                    {
                        _isGuideControl = true;
                    }
                    else
                    {
                        _isGuideControl = false;
                    }
                    Debug.Log("isGuide: "+_isGuideControl);
                }
                if (_isGuideControl)
                {
                    List<float> obstaceDistances = _environmentPerception.GetObstaceDistances();
                    List<Vector3> obstaceDirections = _environmentPerception.GetObstaceDirections();
                    float costTempX = 0;
                    float costTempY = 0;
                    float costTempZ = 0;

                    float x0 = _currentDirection.x;
                    float z0 = _currentDirection.z;

                    //max_area_left
                    float x1 = (Mathf.Cos(_leftTheta * Mathf.PI / 180.0f) - (z0 * (z0 * Mathf.Cos(_leftTheta * Mathf.PI / 180.0f) + x0 * Mathf.Pow(x0 * x0 - Mathf.Cos(_leftTheta * Mathf.PI / 180.0f) * Mathf.Cos(_leftTheta * Mathf.PI / 180.0f) + z0 * z0, 0.5f)))) / x0;
                    float z1 = z0 * Mathf.Cos(_leftTheta * Mathf.PI / 180.0f) + x0 * Mathf.Pow(x0 * x0 - Mathf.Cos(_leftTheta * Mathf.PI / 180.0f) * Mathf.Cos(_leftTheta * Mathf.PI / 180.0f) + z0 * z0, 0.5f);
                    // min_area_righ
                    float x2 = (Mathf.Cos(_rightTheta * Mathf.PI / 180.0f) - (z0 * (z0 * Mathf.Cos(_rightTheta * Mathf.PI / 180.0f) - x0 * Mathf.Pow(x0 * x0 - Mathf.Cos(_rightTheta * Mathf.PI / 180.0f) * Mathf.Cos(_rightTheta * Mathf.PI / 180.0f) + z0 * z0, 0.5f)))) / x0;
                    float z2 = z0 * Mathf.Cos(_rightTheta * Mathf.PI / 180.0f) - x0 * Mathf.Pow(x0 * x0 - Mathf.Cos(_rightTheta * Mathf.PI / 180.0f) * Mathf.Cos(_rightTheta * Mathf.PI / 180.0f) + z0 * z0, 0.5f);

                    int xFlag = x1 - x2 >= 0 ? 1 : 0;
                    int zFlag = z1 - z2 >= 0 ? 1 : 0;
                    bool isUpdateCost = false;
                    for (int j =0;j<obstaceDistances.Count;j++)
                    {
                        if (xFlag == 1 && zFlag == 1)
                        {
                            if (obstaceDirections[j].x >= x2 && obstaceDirections[j].x <= x1 && obstaceDirections[j].z >= z2 && obstaceDirections[j].z <= z1)
                            {
                                isUpdateCost = true;
                            }
                            else
                            {
                                isUpdateCost = false;
                            }
                        }
                        else if (xFlag == 1 && zFlag == 0)
                        {
                            if (obstaceDirections[j].x >= x2 && obstaceDirections[j].x <= x1 && obstaceDirections[j].z >= z1 && obstaceDirections[j].z <= z2)
                            {
                                isUpdateCost = true;
                            }
                            else
                            {
                                isUpdateCost = false;
                            }
                        }

                        else if (xFlag == 0&&zFlag==1)
                        {
                            if (obstaceDirections[j].x >= x1 && obstaceDirections[j].x <= x2 && obstaceDirections[j].z >= z2 && obstaceDirections[j].z <= z1)
                            {
                                isUpdateCost = true;
                            }
                            else
                            {
                                isUpdateCost = false;
                            }
                        }
                        else
                        {
                            if (obstaceDirections[j].x >= x1 && obstaceDirections[j].x <= x2 && obstaceDirections[j].z >= z1 && obstaceDirections[j].z <= z2)
                            {
                                isUpdateCost = true;
                            }
                            else
                            {
                                isUpdateCost = false;
                            }
                        }

                        //if(isUpdateCost)
                        {
                            costTempX += VectorDot(_currentDirection.normalized, obstaceDirections[j].normalized) * (obstaceDistances[j] - _safeRange) * obstaceDirections[j].normalized.x;


                            costTempY += VectorDot(_currentDirection.normalized, obstaceDirections[j].normalized) * (obstaceDistances[j] - _safeRange) * obstaceDirections[j].normalized.y;

                            costTempZ += VectorDot(_currentDirection.normalized, obstaceDirections[j].normalized) * (obstaceDistances[j] - _safeRange) * obstaceDirections[j].normalized.z;
                        }   
                    }
                    _costX += _omiga[i] * costTempX;
                    _costY += _omiga[i] * costTempY;
                    _costZ += _omiga[i] * costTempZ;
                }
                else
                {
                    _costX = 0;
                    _costY = 0;
                    _costZ = 0;
                    break;
                }
                _currentPosition = _predictedPositions[i];
            }
                Debug.Log("CostX: " + _costX + " CostY: " + _costY + " CostZ: " + _costZ);
                _controllerOutput.x = _motionDirectionX * CostFunction(_costX);
                _controllerOutput.y = _motionDirectionY * CostFunction(_costY);
                _controllerOutput.z = _motionDirectionZ * CostFunction(_costZ);


            /*
             * lack verify code to physical model motion
             */
             
        }
        public void UpdateController(List<Vector3> predictedPositions,Vector3 currentPosition)
        {
            _costX = 0;
            _costY = 0;
            _costZ = 0;
            _currentPosition = currentPosition;
            _predictedPositions = predictedPositions;
            Debug.Log("predictedPositionSize: " + _predictedPositions.Count);
            if(Mathf.Abs(_predictedPositions[0].x - _currentPosition.x)<=0.0001)
            {
                _motionDirectionX = 0;
            }
            else
            {
                _motionDirectionX = (_predictedPositions[0].x - _currentPosition.x) / Mathf.Abs(_predictedPositions[0].x - _currentPosition.x);
            }
            if(Mathf.Abs(_predictedPositions[0].y - _currentPosition.y)<=0.0001)
            {
                _motionDirectionY = 0;
            }
            else
            {
                _motionDirectionY = (_predictedPositions[0].y - _currentPosition.y) / Mathf.Abs(_predictedPositions[0].y - _currentPosition.y);
            }
            if (Mathf.Abs(_predictedPositions[0].z - _currentPosition.z) <= 0.0001)
            {
                _motionDirectionZ = 0;
            }
            else
            {
                _motionDirectionZ = (_predictedPositions[0].z - _currentPosition.z) / Mathf.Abs(_predictedPositions[0].z - _currentPosition.z);
            }
            
            Debug.Log("CurrentPosition: " + _currentPosition);
            Debug.Log("PredictedPosition: " + _predictedPositions[0]);
            Debug.Log("MotionDirectionX: " + _motionDirectionX + " MotionDirectionY: " + _motionDirectionY + " MotionDirectionZ: " + _motionDirectionZ);
            for (int i=0;i<_predictedPositions.Count;i++)
            {
                _environmentPerception.UpdateSensor(_predictedPositions[i]);
                _currentDirection = _predictedPositions[i] - _currentPosition;
                _costX += _omiga[i] * VectorDot(_currentDirection.normalized, _environmentPerception.GetMinDistanceDirection().normalized) *(_environmentPerception.GetMinDistance()-_safeRange) * _environmentPerception.GetMinDistanceDirection().normalized.x;

                _costY += _omiga[i] * VectorDot(_currentDirection.normalized, _environmentPerception.GetMinDistanceDirection().normalized) * (_environmentPerception.GetMinDistance() - _safeRange) * _environmentPerception.GetMinDistanceDirection().normalized.y;

                _costZ += _omiga[i] * VectorDot(_currentDirection.normalized, _environmentPerception.GetMinDistanceDirection().normalized) * (_environmentPerception.GetMinDistance() - _safeRange) * _environmentPerception.GetMinDistanceDirection().normalized.z;
                _currentPosition = _predictedPositions[i];
            }
            Debug.Log("CostX: " + _costX + " CostY: " + _costY + " CostZ: " + _costZ);
            _controllerOutput.x = _motionDirectionX * CostFunction(_costX);
            _controllerOutput.y = _motionDirectionY * CostFunction(_costY);
            _controllerOutput.z = _motionDirectionZ * CostFunction(_costZ);
        }

        private float VectorDot(Vector3 a,Vector3 b)
        {
            return (a.x * b.x + a.y * b.y + a.z * b.z);
        }
        private float CostFunction(float cost)
        {
            if(cost<=0.0)
            {
                return cost;
            }
            else
            {
                return 0.0f;
            }
        }



        public VirtualGuide(int predictedSteps,List<float> omiga,float safeRange,environment_perception environmentPerception)
        {
            _predictedSteps = predictedSteps;
            _omiga = omiga;
            _safeRange = safeRange;
            _environmentPerception = environmentPerception;
            for(int i = 0; i<omiga.Count-_predictedSteps;i++)
            {
                _omiga.Add(0.0f);
            }
            _controllerOutput = Vector3.zero;
            _leftThetaDirection = Vector3.zero;
            _rightThetaDirection = Vector3.zero;
            _isGuideControl = false;
        }

        public VirtualGuide(int predictedSteps, List<float> omiga, float safeRange, environment_perception environmentPerception,float left_theta,float right_theta)
        {
            _leftTheta = left_theta;
            _rightTheta = right_theta;
            _predictedSteps = predictedSteps;
            _omiga = omiga;
            _safeRange = safeRange;
            _environmentPerception = environmentPerception;
            for (int i = 0; i < omiga.Count - _predictedSteps; i++)
            {
                _omiga.Add(0.0f);
            }
            _controllerOutput = Vector3.zero;
            _leftThetaDirection = Vector3.zero;
            _rightThetaDirection = Vector3.zero;
            _isGuideControl = false;
        }
        public VirtualGuide(int predictedSteps, List<float> omiga, float safeRange, environment_perception environmentPerception, float left_theta, float right_theta,float adjustGuideRange)
        {
            _leftTheta = left_theta;
            _rightTheta = right_theta;
            _predictedSteps = predictedSteps;
            _omiga = omiga;
            _safeRange = safeRange;
            _adjustGuideRange = adjustGuideRange;
            _environmentPerception = environmentPerception;
            for (int i = 0; i < omiga.Count - _predictedSteps; i++)
            {
                _omiga.Add(0.0f);
            }
            _controllerOutput = Vector3.zero;
            _leftThetaDirection = Vector3.zero;
            _rightThetaDirection = Vector3.zero;
            _isGuideControl = false;
        }

        public VirtualGuide(int predictedSteps, List<float> omiga, float safeRange, environment_perception environmentPerception, float left_theta, float right_theta, float adjustGuideRange,float omigaNow)
        {
            _leftTheta = left_theta;
            _rightTheta = right_theta;
            _predictedSteps = predictedSteps;
            _omiga = omiga;
            _omigaNow = omigaNow;
            _safeRange = safeRange;
            _adjustGuideRange = adjustGuideRange;
            _environmentPerception = environmentPerception;
            for (int i = 0; i < omiga.Count - _predictedSteps; i++)
            {
                _omiga.Add(0.0f);
            }
            _controllerOutput = Vector3.zero;
            _leftThetaDirection = Vector3.zero;
            _rightThetaDirection = Vector3.zero;
            _isGuideControl = false;
        }
    }
}
