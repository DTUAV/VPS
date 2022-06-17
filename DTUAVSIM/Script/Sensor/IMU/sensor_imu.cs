/*------>This DTUAVSIM project is based on gazebo simulator<--------- 
 * Copyright 2015 Fadri Furrer, ASL, ETH Zurich, Switzerland
 * Copyright 2015 Michael Burri, ASL, ETH Zurich, Switzerland
 * Copyright 2015 Mina Kamel, ASL, ETH Zurich, Switzerland
 * Copyright 2015 Janosch Nikolic, ASL, ETH Zurich, Switzerland
 * Copyright 2015 Markus Achtelik, ASL, ETH Zurich, Switzerland
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * 2021 Yuanlin Yang, Guandong University of Technology, Guanzhou,China 
 */

using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using System.Threading;
using DTUAVCARS.DTNetWork.Lcm.Publisher;
using DTUAVSIM.Math;
using DTUAVSIM.Time;
using DTUAVSIM.TF;
using sensor_msgs;

// ADIS16448 IMU
namespace DTUAVSIM.Sensor.Imu
{
    public enum ImuConfig
    {
        ADIS16448,
        Custom,
    }
    public struct ImuParameterStruct
    {
     
        // Gyroscope noise density (two-sided spectrum) [rad/s/sqrt(Hz)]
        public double GyroscopeNoiseDensity;
        // Gyroscope bias random walk [rad/s/s/sqrt(Hz)]
        public double GyroscopeRandomWalk;
        // Gyroscope bias correlation time constant [s]
        public double GyroscopeBiasCorrelationTime;
        // Gyroscope turn on bias standard deviation [rad/s]
        public double GyroscopeTurnOnBiasSigma;
        // Accelerometer noise density (two-sided spectrum) [m/s^2/sqrt(Hz)]
        public double AccelerometerNoiseDensity;
        // Accelerometer bias random walk. [m/s^2/s/sqrt(Hz)]
        public double AccelerometerRandomWalk;
        // Accelerometer bias correlation time constant [s]
        public double AccelerometerBiasCorrelationTime;
        // Accelerometer turn on bias standard deviation [m/s^2]
        public double AccelerometerTurnOnBiasSigma;
        // Norm of the gravitational acceleration [m/s^2]
        public double GravityMagnitude;

    }

    public class sensor_imu : BasePub
    {
        [Header("IMU Sensor Type")]
        public ImuConfig ImuSensor;

        [Header("The Rigid Body of Uav")] 
        public Rigidbody RigImu;

        // Gyroscope noise density (two-sided spectrum) [rad/s/sqrt(Hz)]
        public double gyroscope_noise_density;
        // Gyroscope bias random walk [rad/s/s/sqrt(Hz)]
        public double gyroscope_random_walk;
        // Gyroscope bias correlation time constant [s]
        public double gyroscope_bias_correlation_time;
        // Gyroscope turn on bias standard deviation [rad/s]
        public double gyroscope_turn_on_bias_sigma;
        // Accelerometer noise density (two-sided spectrum) [m/s^2/sqrt(Hz)]
        public double accelerometer_noise_density;
        // Accelerometer bias random walk. [m/s^2/s/sqrt(Hz)]
        public double accelerometer_random_walk;
        // Accelerometer bias correlation time constant [s]
        public double accelerometer_bias_correlation_time;
        // Accelerometer turn on bias standard deviation [m/s^2]
        public double accelerometer_turn_on_bias_sigma;
        // Norm of the gravitational acceleration [m/s^2]
        public double gravity_magnitude;

        // Program Parameters 
        private int _sleepTimeS; //The sleep time for this thread
        private Thread _pubThread;//Define a thread to publish data
        private bool _isEnd;//The flag is used to close the thread

        private ImuParameterStruct _imuParameterStruct;//The init parameter of imu 
        private imu _imuSensorMsg;
        private Vector3Double _gyroscopeTurnOnBia;
        private Vector3Double _accelerometerTurnOnBias;
        private Vector3Double _gyroscopBias;
        private Vector3Double _accelerometerBias;
        private Vector3Double _lastlinearVelocityUnity;
        private Vector3Double _gravityUnity;
        private Quaternion _currentRotationValue;
        private Vector3 _currentLinearVelocityValue;
        private Vector3 _currentAngularVelocityValue;
        private double _startSimlationTimeStamp = 0;
        private bool _isInitStartSimTimeStamp = false;
        private double _lastUpdateTimeStamp;

//The parameters for ADIS16448 IMU
       private double kDefaultAdisGyroscopeNoiseDensity =
            2.0 * 35.0 / 3600.0 / 180.0 * SimMath.M_PI;
       private double kDefaultAdisGyroscopeRandomWalk =
            2.0 * 4.0 / 3600.0 / 180.0 * SimMath.M_PI;
       private double kDefaultAdisGyroscopeBiasCorrelationTime =
            1.0e+3; 
       private double kDefaultAdisGyroscopeTurnOnBiasSigma =
            0.5 / 180.0 * SimMath.M_PI;
       private double kDefaultAdisAccelerometerNoiseDensity =
            2.0 * 2.0e-3;
       private double kDefaultAdisAccelerometerRandomWalk =
            2.0 * 3.0e-3;
       private double kDefaultAdisAccelerometerBiasCorrelationTime =
            300.0;
       private double kDefaultAdisAccelerometerTurnOnBiasSigma =
            20.0e-3 * 9.8; 
       // Earth's gravity in Zurich (lat=+47.3667degN, lon=+8.5500degE, h=+500m, WGS84)
       private double kDefaultGravityMagnitude = 9.8068;

       void UpdateWorld()
       {
           _currentAngularVelocityValue = RigImu.angularVelocity;
           _currentLinearVelocityValue = RigImu.velocity;
           _currentRotationValue = RigImu.rotation;
        }
       void UpdateSensor()
       {
           double currentTime = SimTime.GetMachineRunTimeStampNs() * 1e-9 + _startSimlationTimeStamp * 1e-9;
           double currentTimeNs = SimTime.GetMachineRunTimeStampNs() + _startSimlationTimeStamp *1e+9;
           Debug.Log("currentTime: "+currentTime);
           Debug.Log("lastTime: "+_lastUpdateTimeStamp);
           double dt = currentTime - _lastUpdateTimeStamp;
           Debug.Log("dt: " + dt);
           double t = currentTime;
           _lastUpdateTimeStamp = currentTime;
           Quaternion currentRotationUnity = _currentRotationValue;
           Quaternion currentRotationNED = SimTf.UnityToNED(currentRotationUnity);
           _imuSensorMsg.orientation_x = currentRotationNED.x;
           _imuSensorMsg.orientation_y = currentRotationNED.y;
           _imuSensorMsg.orientation_z = currentRotationNED.z;
           _imuSensorMsg.orientation_w = currentRotationNED.w;
           Vector3 currentLinearVelocityUnity = _currentLinearVelocityValue;
           Vector3Double currentLinearAccelerationUnity;
               currentLinearAccelerationUnity.x = (currentLinearVelocityUnity.x  - _lastlinearVelocityUnity.x) / dt;
               currentLinearAccelerationUnity.y = (currentLinearVelocityUnity.y  - _lastlinearVelocityUnity.y) / dt;
               currentLinearAccelerationUnity.z = (currentLinearVelocityUnity.z  - _lastlinearVelocityUnity.z) / dt;
           _lastlinearVelocityUnity.x = currentLinearVelocityUnity.x;
           _lastlinearVelocityUnity.y = currentLinearVelocityUnity.y;
           _lastlinearVelocityUnity.z = currentLinearVelocityUnity.z;

            Vector3Double currentLinearAccelerationNED = SimTf.UnityToNED(currentLinearAccelerationUnity);
            Vector3Double currentGravityNED = SimTf.UnityToNED(_gravityUnity);

            currentLinearAccelerationNED.x = currentLinearAccelerationNED.x - currentGravityNED.x;
            currentLinearAccelerationNED.y = currentLinearAccelerationNED.y - currentGravityNED.y;
            currentLinearAccelerationNED.z = currentLinearAccelerationNED.z - currentGravityNED.z;

            Vector3 currentAngularVelocityUnityRaw = _currentAngularVelocityValue;
            Vector3Double currentAngularVelocityUnity;
            currentAngularVelocityUnity.x = currentAngularVelocityUnityRaw.x;
            currentAngularVelocityUnity.y = currentAngularVelocityUnityRaw.y;
            currentAngularVelocityUnity.z = currentAngularVelocityUnityRaw.z;
            Vector3Double currentAngularVelocityNED = SimTf.UnityToNED(currentAngularVelocityUnity);

            Debug.Log("currentLinearAcceleration.x: "+ currentLinearAccelerationNED.x);
            Debug.Log("currentLinearAcceleration.y: " + currentLinearAccelerationNED.y);
            Debug.Log("currentLinearAcceleration.z: " + currentLinearAccelerationNED.z);
            Debug.Log("currentAngularVelocity.x: " + currentAngularVelocityNED.x);
            Debug.Log("currentAngularVelocity.y: " + currentAngularVelocityNED.y);
            Debug.Log("currentAngularVelocity.z: " + currentAngularVelocityNED.z);


            AddNoise(ref currentLinearAccelerationNED, ref currentAngularVelocityNED, dt);

            Debug.Log("addnoisecurrentLinearAcceleration.x: " + currentLinearAccelerationNED.x);
            Debug.Log("addnoisecurrentLinearAcceleration.y: " + currentLinearAccelerationNED.y);
            Debug.Log("addnoisecurrentLinearAcceleration.z: " + currentLinearAccelerationNED.z);

            Debug.Log("currentAngularVelocity.x: " + currentAngularVelocityNED.x);
            Debug.Log("currentAngularVelocity.y: " + currentAngularVelocityNED.y);
            Debug.Log("currentAngularVelocity.z: " + currentAngularVelocityNED.z); 
            _imuSensorMsg.linear_acceleration_x = currentLinearAccelerationNED.x;
            _imuSensorMsg.linear_acceleration_y = currentLinearAccelerationNED.y;
            _imuSensorMsg.linear_acceleration_z = currentLinearAccelerationNED.z;
            _imuSensorMsg.angular_velocity_x = currentAngularVelocityNED.x;
            _imuSensorMsg.angular_velocity_y = currentAngularVelocityNED.y;
            _imuSensorMsg.angular_velocity_z = currentAngularVelocityNED.z;
            _imuSensorMsg.timestamp_us = currentTimeNs;

       }
       void SetStartSimulationTimeStamp(double timeStamp)
       {
           _startSimlationTimeStamp = timeStamp;
           _isInitStartSimTimeStamp = true;
           _lastUpdateTimeStamp = timeStamp;
       }
       void AddNoise(ref Vector3Double linear_acceleration, ref Vector3Double angular_velocity, double dt)
       {
           if (dt > 0.0)
           {
               double tau_g = _imuParameterStruct.GyroscopeBiasCorrelationTime;
               double sigma_g_d = 1 / SimMath.sqrtD(dt) * _imuParameterStruct.GyroscopeNoiseDensity;
               double sigma_b_g = _imuParameterStruct.GyroscopeRandomWalk;
               double sigma_b_g_d = SimMath.sqrtD(-sigma_b_g * sigma_b_g * tau_g / 2.0 * (SimMath.expD(-2.0 * dt / tau_g) - 1.0));
               double phi_g_d = SimMath.expD(-1.0 / tau_g * dt);


               
               _gyroscopBias.x = phi_g_d * _gyroscopBias.x + sigma_b_g_d * SimMath.NormalDistribution();
               angular_velocity.x = angular_velocity.x + _gyroscopBias.x +
                                    sigma_g_d * SimMath.NormalDistribution() + _gyroscopeTurnOnBia.x;
               _gyroscopBias.y = phi_g_d * _gyroscopBias.y + sigma_b_g_d * SimMath.NormalDistribution();
               angular_velocity.y = angular_velocity.y + _gyroscopBias.y +
                                    sigma_g_d * SimMath.NormalDistribution() + _gyroscopeTurnOnBia.y;
               _gyroscopBias.z = phi_g_d * _gyroscopBias.z + sigma_b_g_d * SimMath.NormalDistribution();
               angular_velocity.z = angular_velocity.z + _gyroscopBias.z +
                                    sigma_g_d * SimMath.NormalDistribution() + _gyroscopeTurnOnBia.z;
                /*
                _gyroscopBias.x = phi_g_d * _gyroscopBias.x + sigma_b_g_d * SimMath.NormalDistribution(0.0, 1.0);
               angular_velocity.x = angular_velocity.x + _gyroscopBias.x +
                                    sigma_g_d * SimMath.NormalDistribution(0.0, 1.0) + _gyroscopeTurnOnBia.x;
               _gyroscopBias.y = phi_g_d * _gyroscopBias.y + sigma_b_g_d * SimMath.NormalDistribution(0.0, 1.0);
               angular_velocity.y = angular_velocity.y + _gyroscopBias.y +
                                    sigma_g_d * SimMath.NormalDistribution(0.0, 1.0) + _gyroscopeTurnOnBia.y;
               _gyroscopBias.z = phi_g_d * _gyroscopBias.z + sigma_b_g_d * SimMath.NormalDistribution(0.0, 1.0);
               angular_velocity.z = angular_velocity.z + _gyroscopBias.z +
                                    sigma_g_d * SimMath.NormalDistribution(0.0, 1.0) + _gyroscopeTurnOnBia.z;
                */

               double tau_a = _imuParameterStruct.AccelerometerBiasCorrelationTime;
               double sigma_a_d = 1 / SimMath.sqrtD(dt) * _imuParameterStruct.AccelerometerNoiseDensity;
               double sigma_b_a = _imuParameterStruct.AccelerometerRandomWalk;
               double sigma_b_a_d = SimMath.sqrtD(-sigma_b_a * sigma_b_a * tau_a / 2.0 * (SimMath.expD(-2.0 * dt / tau_a) - 1.0));
               double phi_a_d = SimMath.expD(-1.0 / tau_a * dt);
               
               _accelerometerBias.x = phi_a_d * _accelerometerBias.x + sigma_b_a_d * SimMath.NormalDistribution();
              linear_acceleration.x = linear_acceleration.x + _accelerometerBias.x +
                                   sigma_a_d * SimMath.NormalDistribution() + _accelerometerTurnOnBias.x;
              _accelerometerBias.y = phi_a_d * _accelerometerBias.y + sigma_b_a_d * SimMath.NormalDistribution();
              linear_acceleration.y = linear_acceleration.y + _accelerometerBias.y +
                                      sigma_a_d * SimMath.NormalDistribution() + _accelerometerTurnOnBias.y;
              _accelerometerBias.z = phi_a_d * _accelerometerBias.z + sigma_b_a_d * SimMath.NormalDistribution();
              linear_acceleration.z = linear_acceleration.z + _accelerometerBias.z +
                                      sigma_a_d * SimMath.NormalDistribution() + _accelerometerTurnOnBias.z;
               
                /*
               _accelerometerBias.x = phi_a_d * _accelerometerBias.x + sigma_b_a_d * SimMath.NormalDistribution(0.0, 1.0);
               linear_acceleration.x = linear_acceleration.x + _accelerometerBias.x +
                                       sigma_a_d * SimMath.NormalDistribution(0.0, 1.0) + _accelerometerTurnOnBias.x;
               _accelerometerBias.y = phi_a_d * _accelerometerBias.y + sigma_b_a_d * SimMath.NormalDistribution(0.0, 1.0);
               linear_acceleration.y = linear_acceleration.y + _accelerometerBias.y +
                                       sigma_a_d * SimMath.NormalDistribution(0.0, 1.0) + _accelerometerTurnOnBias.y;
               _accelerometerBias.z = phi_a_d * _accelerometerBias.z + sigma_b_a_d * SimMath.NormalDistribution(0.0, 1.0);
               linear_acceleration.z = linear_acceleration.z + _accelerometerBias.z +
                                       sigma_a_d * SimMath.NormalDistribution(0.0, 1.0) + _accelerometerTurnOnBias.z;
                */
            }
           else
           {
               Debug.Log("dt<0.0-->"+"error");
           }
       }

       void MessageInit()
       {
           _imuSensorMsg = new imu();
           for (int i = 0; i < 9; i++)
           {
               switch (i)
               {
                   case 0:
                       _imuSensorMsg.angular_velocity_covariance[i] = _imuParameterStruct.GyroscopeNoiseDensity * _imuParameterStruct.GyroscopeNoiseDensity;
                       _imuSensorMsg.orientation_covariance[i] = -1.0;
                       _imuSensorMsg.linear_acceleration_covariance[i] = _imuParameterStruct.AccelerometerNoiseDensity * _imuParameterStruct.AccelerometerNoiseDensity;
                       break;
                   case 1:
                   case 2:
                   case 3:
                       _imuSensorMsg.angular_velocity_covariance[i] =0.0;
                       _imuSensorMsg.orientation_covariance[i] = -1.0;
                       _imuSensorMsg.linear_acceleration_covariance[i] = 0.0;
                       break;
                   case 4:
                       _imuSensorMsg.angular_velocity_covariance[i] = _imuParameterStruct.GyroscopeNoiseDensity * _imuParameterStruct.GyroscopeNoiseDensity;
                       _imuSensorMsg.orientation_covariance[i] = -1.0;
                       _imuSensorMsg.linear_acceleration_covariance[i] = _imuParameterStruct.AccelerometerNoiseDensity * _imuParameterStruct.AccelerometerNoiseDensity;
                       break;
                   case 5:
                   case 6:
                   case 7:
                       _imuSensorMsg.angular_velocity_covariance[i] = 0.0;
                       _imuSensorMsg.orientation_covariance[i] = -1.0;
                       _imuSensorMsg.linear_acceleration_covariance[i] = 0.0;
                        break;
                   case 8:
                       _imuSensorMsg.angular_velocity_covariance[i] = _imuParameterStruct.GyroscopeNoiseDensity * _imuParameterStruct.GyroscopeNoiseDensity;
                       _imuSensorMsg.orientation_covariance[i] = -1.0;
                       _imuSensorMsg.linear_acceleration_covariance[i] = _imuParameterStruct.AccelerometerNoiseDensity * _imuParameterStruct.AccelerometerNoiseDensity;
                        break;
               }
           }

           double sigma_bon_g = _imuParameterStruct.GyroscopeTurnOnBiasSigma;
           double sigma_bon_a = _imuParameterStruct.AccelerometerTurnOnBiasSigma;
           _gyroscopeTurnOnBia.x = sigma_bon_g * SimMath.NormalDistribution(0.0, 1.0);
           _gyroscopeTurnOnBia.y = sigma_bon_g * SimMath.NormalDistribution(0.0, 1.0);
           _gyroscopeTurnOnBia.z = sigma_bon_g * SimMath.NormalDistribution(0.0, 1.0);
           _accelerometerTurnOnBias.x = sigma_bon_a * SimMath.NormalDistribution(0.0, 1.0);
           _accelerometerTurnOnBias.y = sigma_bon_a * SimMath.NormalDistribution(0.0, 1.0);
           _accelerometerTurnOnBias.z = sigma_bon_a * SimMath.NormalDistribution(0.0, 1.0);
           _gyroscopBias.x = 0;
           _gyroscopBias.y = 0;
           _gyroscopBias.z = 0;
           _accelerometerBias.x = 0;
           _accelerometerBias.y = 0;
           _accelerometerBias.z = 0;

       }
       void SensorInit()
       {
           if (ImuSensor == ImuConfig.Custom)
           {
               _imuParameterStruct.AccelerometerBiasCorrelationTime = accelerometer_bias_correlation_time;
               _imuParameterStruct.AccelerometerNoiseDensity = accelerometer_noise_density;
               _imuParameterStruct.AccelerometerRandomWalk = accelerometer_random_walk;
               _imuParameterStruct.AccelerometerTurnOnBiasSigma = accelerometer_turn_on_bias_sigma;
               _imuParameterStruct.GravityMagnitude = gravity_magnitude;
               _imuParameterStruct.GyroscopeBiasCorrelationTime = gyroscope_bias_correlation_time;
               _imuParameterStruct.GyroscopeNoiseDensity = gyroscope_noise_density;
               _imuParameterStruct.GyroscopeRandomWalk = gyroscope_random_walk;
               _imuParameterStruct.GyroscopeTurnOnBiasSigma = gyroscope_turn_on_bias_sigma;
           }
           else if(ImuSensor==ImuConfig.ADIS16448)
           {
               _imuParameterStruct.AccelerometerTurnOnBiasSigma = kDefaultAdisAccelerometerTurnOnBiasSigma;
               _imuParameterStruct.AccelerometerBiasCorrelationTime = kDefaultAdisAccelerometerBiasCorrelationTime;
               _imuParameterStruct.AccelerometerNoiseDensity = kDefaultAdisAccelerometerNoiseDensity;
               _imuParameterStruct.AccelerometerRandomWalk = kDefaultAdisAccelerometerRandomWalk;
               _imuParameterStruct.GravityMagnitude = kDefaultGravityMagnitude;
               _imuParameterStruct.GyroscopeBiasCorrelationTime = kDefaultAdisGyroscopeBiasCorrelationTime;
               _imuParameterStruct.GyroscopeNoiseDensity = kDefaultAdisGyroscopeNoiseDensity;
               _imuParameterStruct.GyroscopeRandomWalk = kDefaultAdisGyroscopeRandomWalk;
               _imuParameterStruct.GyroscopeTurnOnBiasSigma = kDefaultAdisGyroscopeTurnOnBiasSigma;
           }
             //else if(ImuSensor==ImuConfig.XXX)
     //            {
    //
  //          }
           else
           {
               _imuParameterStruct.AccelerometerTurnOnBiasSigma = kDefaultAdisAccelerometerTurnOnBiasSigma;
               _imuParameterStruct.AccelerometerBiasCorrelationTime = kDefaultAdisAccelerometerBiasCorrelationTime;
               _imuParameterStruct.AccelerometerNoiseDensity = kDefaultAdisAccelerometerNoiseDensity;
               _imuParameterStruct.AccelerometerRandomWalk = kDefaultAdisAccelerometerRandomWalk;
               _imuParameterStruct.GravityMagnitude = kDefaultGravityMagnitude;
               _imuParameterStruct.GyroscopeBiasCorrelationTime = kDefaultAdisGyroscopeBiasCorrelationTime;
               _imuParameterStruct.GyroscopeNoiseDensity = kDefaultAdisGyroscopeNoiseDensity;
               _imuParameterStruct.GyroscopeRandomWalk = kDefaultAdisGyroscopeRandomWalk;
               _imuParameterStruct.GyroscopeTurnOnBiasSigma = kDefaultAdisGyroscopeTurnOnBiasSigma;
            }

       }
        void Start()
        {
            base.BaseStart();
            SensorInit();
            MessageInit();
            _lastUpdateTimeStamp = _lastUpdateTimeStamp * 1e-9 + SimTime.GetMachineRunTimeStampNs()*1e-9;
            _sleepTimeS = (int)(1 / (base.MessagePubHz)) * 1000;//Count the sleep time for thread
            _pubThread = new Thread(PubData);
            _pubThread.IsBackground = true;//Only set true and the thread can exit follow the main thread
            _pubThread.Start();//Start run thread function
            if (RigImu == null)
            {
                Debug.LogError("The Rigid Body of Object Not Init,Check Inspector Window");
            }
            else
            {
                Debug.Log("Start Imu Sensor");
                _currentAngularVelocityValue = RigImu.angularVelocity;
                _currentLinearVelocityValue = RigImu.velocity;
                _currentRotationValue = RigImu.rotation;
                _lastlinearVelocityUnity.x = RigImu.velocity.x;
                _lastlinearVelocityUnity.y = RigImu.velocity.y;
                _lastlinearVelocityUnity.z = RigImu.velocity.z;
                _gravityUnity.x = 0;
                _gravityUnity.z = 0;
                _gravityUnity.y = -9.8;
            }

        }

        // Update is called once per frame
        void Update()
        {
            UpdateWorld();
        }

        void PubData()
        {
            while (!_isEnd)
            {
                UpdateSensor();
                base.BaseLcm.Publish(base.MesageName, _imuSensorMsg);
                System.Threading.Thread.Sleep(_sleepTimeS);
            }
        }
        void OnDestroy()
        {
            _isEnd = true;
            if (_pubThread != null)
            {
                if (_pubThread.IsAlive)
                {
                    _pubThread.Abort();
                }
            }
        }
    }
}
