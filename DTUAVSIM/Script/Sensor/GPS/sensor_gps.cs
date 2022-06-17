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

namespace DTUAVSIM.Sensor.Gps
{
    public enum GpsConfig
    {
        GuangdongUniversityOfTechnology,
        Custom,
    }
    public struct GpsParameterStruct
    {
        //The Flag Open or Close to Add Noise to Gps Sensor
        public bool GpsNoise;
        public double StdXY;
        public double StdZ;
        public double GpsCorellationTime;    // s
        public double GpsXYRandomWalk;       // (m/s) / sqrt(hz)
        public double GpsZRandomWalk;        // (m/s) / sqrt(hz)
        public double GpsXYNoiseDensity;    // (m) / sqrt(hz)
        public double GpsZNoiseDensity;     // (m) / sqrt(hz)
        public double GpsVxyNoiseDensity;   // (m/s) / sqrt(hz)
        public double GpsVzNoiseDensity;    // (m/s) / sqrt(hz)
        public double LatHome;//latitude--rad
        public double LonHome;//longitude--rad
        public double AltHome;//heigh-- meters
        public double GpsUpdateInterval;//5hz
        public double GpsDelay;//120ms
        public int GpsBufferSizeMax;
        //
    }

    public class sensor_gps : BasePub
    {
        [Header("IMU Sensor Type")]
        public GpsConfig GpsSensor;

        [Header("The Rigid Body of Uav")]
        public Rigidbody RigGps;

        public bool GpsNoise = true;
        public double StdXY = 0.5;
        public double StdZ = 0.5;
        public double GpsCorellationTime = 60.0;    // s
        public double GpsXYRandomWalk = 2.0;       // (m/s) / sqrt(hz)
        public double GpsZRandomWalk = 4.0;        // (m/s) / sqrt(hz)
        public double GpsXYNoiseDensity = 2e-4;    // (m) / sqrt(hz)
        public double GpsZNoiseDensity = 4e-4;     // (m) / sqrt(hz)
        public double GpsVxyNoiseDensity = 2e-1;   // (m/s) / sqrt(hz)
        public double GpsVzNoiseDensity = 4e-1;    // (m/s) / sqrt(hz)
        public double LatHome = 23.139209965161754;//latitude--rad
        public double LonHome = 113.30654978988578;//longitude--rad
        public double AltHome = 488.0;//heigh-- meters
        public double GpsUpdateInterval = 0.2;//5hz
        public double GpsDelay = 0.12;//120ms
        public int GpsBufferSizeMax = 1000;

        // Program Parameters 
        private int _sleepTimeS; //The sleep time for this thread
        private Thread _pubThread;//Define a thread to publish data
        private bool _isEnd;//The flag is used to close the thread

        private SITLGps _gpsMsg;//The Message for Gps
        private Groundtruth _groundtruthMsg;//The Message for GroundTruth(Not Noise)

        private double _lastGpsTime;
        private double _lastTime;

        private Queue<SITLGps> _gpsDelayBuffer;

        private Vector3Double _gpsBias;
        private Vector3Double _noiseGpsPos;
        private Vector3Double _nosieGpsVel;
        private Vector3Double _randomWalkGps;
        private Vector3Double _gravity;
        private Vector3Double _velocityPre;

        private GpsParameterStruct _gpsParameterStruct;
        private double _startSimlationTimeStamp = 0;
        private bool _isInitStartSimTimeStamp = false;
        private double _lastUpdateTimeStamp;

        //The Gps Information for Guandong University of Technology
        private bool _gpsNoise = true;
        private double _latHomeDefault = 23.139209965161754;//latitude
        private double _lonHomeDefault = 113.30654978988578;//longitude
        private double _altHomeDefault = 488.0;//heigh-- meters
        private double _earthRadius = 6353000.0;//meters
        private double _gpsUpdateIntervalDefault = 0.2;//5hz
        private double _gpsDelayDefault = 0.12;//120ms
        private int _gpsBufferSizeMaxDefault = 1000;
        //gps noise parameters
        private double _stdXYDefault = 0.5;//meters
        private double _stdZDefault = 0.5;//meters nnnnnnnnnnnn
        private double _gpsCorellationTimeDefault = 60.0;    // s
        private double _gpsXYRandomWalkDefault = 2.0;       // (m/s) / sqrt(hz)
        private double _gpsZRandomWalkDefault = 4.0;        // (m/s) / sqrt(hz)
        private double _gpsXYNoiseDensityDefault = 2e-4;    // (m) / sqrt(hz)
        private double _gpsZNoiseDensityDefault = 4e-4;     // (m) / sqrt(hz)
        private double _gpsVxyNoiseDensityDefault = 2e-1;   // (m/s) / sqrt(hz)
        private double _gpsVzNoiseDensityDefault = 4e-1;    // (m/s) / sqrt(hz)

        void SetStartSimulationTimeStamp(double timeStamp)
        {
            _startSimlationTimeStamp = timeStamp;
            _isInitStartSimTimeStamp = true;
            _lastUpdateTimeStamp = timeStamp;
        }

        void ParameterInit()
        {
            _lastGpsTime = SimTime.GetMachineRunTimeStampNs() * 1e-9 + _startSimlationTimeStamp * 1e-9;
            _lastTime = SimTime.GetMachineRunTimeStampNs() * 1e-9 + _startSimlationTimeStamp * 1e-9;
            _gpsDelayBuffer = new Queue<SITLGps>(_gpsParameterStruct.GpsBufferSizeMax);
            _gravity.x = 0;
            _gravity.y = 0;
            _gravity.z = -9.81;

        }

        void MessageInit()
        {

        }

        void SensorInit()
        {
            if (GpsSensor == GpsConfig.GuangdongUniversityOfTechnology)
            {
                _gpsParameterStruct.AltHome = _altHomeDefault;
                _gpsParameterStruct.GpsBufferSizeMax = _gpsBufferSizeMaxDefault;
                _gpsParameterStruct.GpsCorellationTime = _gpsCorellationTimeDefault;
                _gpsParameterStruct.GpsDelay = _gpsDelayDefault;
                _gpsParameterStruct.GpsNoise = _gpsNoise;
                _gpsParameterStruct.GpsUpdateInterval = _gpsUpdateIntervalDefault;
                _gpsParameterStruct.GpsVxyNoiseDensity = _gpsVxyNoiseDensityDefault;
                _gpsParameterStruct.GpsVzNoiseDensity = _gpsVzNoiseDensityDefault;
                _gpsParameterStruct.GpsXYNoiseDensity = _gpsXYNoiseDensityDefault;
                _gpsParameterStruct.GpsXYRandomWalk = _gpsXYRandomWalkDefault;
                _gpsParameterStruct.GpsZNoiseDensity = _gpsZNoiseDensityDefault;
                _gpsParameterStruct.GpsZRandomWalk = _gpsZRandomWalkDefault;
                _gpsParameterStruct.LatHome = _latHomeDefault * SimMath.M_PI / 180.0;
                _gpsParameterStruct.LonHome = _lonHomeDefault * SimMath.M_PI / 180.0;
                _gpsParameterStruct.StdXY = _stdXYDefault;
                _gpsParameterStruct.StdZ = _stdZDefault;
            }
            else if (GpsSensor == GpsConfig.Custom)
            {
                _gpsParameterStruct.GpsBufferSizeMax = GpsBufferSizeMax;
                _gpsParameterStruct.GpsXYNoiseDensity = GpsXYNoiseDensity;
                _gpsParameterStruct.GpsXYRandomWalk = GpsXYRandomWalk;
                _gpsParameterStruct.GpsZNoiseDensity = GpsZNoiseDensity;
                _gpsParameterStruct.GpsZRandomWalk = GpsZRandomWalk;
                _gpsParameterStruct.LatHome = LatHome * SimMath.M_PI / 180.0;
                _gpsParameterStruct.LonHome = LonHome * SimMath.M_PI / 180.0;
                _gpsParameterStruct.StdXY = StdXY;
                _gpsParameterStruct.StdZ = StdZ;
                _gpsParameterStruct.AltHome = AltHome;
                _gpsParameterStruct.GpsCorellationTime = GpsCorellationTime;
                _gpsParameterStruct.GpsDelay = GpsDelay;
                _gpsParameterStruct.GpsVxyNoiseDensity = GpsVxyNoiseDensity;
                _gpsParameterStruct.GpsVzNoiseDensity = GpsVzNoiseDensity;
                _gpsParameterStruct.GpsUpdateInterval = GpsUpdateInterval;
                _gpsParameterStruct.GpsNoise = GpsNoise;
            }
            else
            {
                _gpsParameterStruct.AltHome = _altHomeDefault;
                _gpsParameterStruct.GpsBufferSizeMax = _gpsBufferSizeMaxDefault;
                _gpsParameterStruct.GpsCorellationTime = _gpsCorellationTimeDefault;
                _gpsParameterStruct.GpsDelay = _gpsDelayDefault;
                _gpsParameterStruct.GpsNoise = _gpsNoise;
                _gpsParameterStruct.GpsUpdateInterval = _gpsUpdateIntervalDefault;
                _gpsParameterStruct.GpsVxyNoiseDensity = _gpsVxyNoiseDensityDefault;
                _gpsParameterStruct.GpsVzNoiseDensity = _gpsVzNoiseDensityDefault;
                _gpsParameterStruct.GpsXYNoiseDensity = _gpsXYNoiseDensityDefault;
                _gpsParameterStruct.GpsXYRandomWalk = _gpsXYRandomWalkDefault;
                _gpsParameterStruct.GpsZNoiseDensity = _gpsZNoiseDensityDefault;
                _gpsParameterStruct.GpsZRandomWalk = _gpsZRandomWalkDefault;
                _gpsParameterStruct.LatHome = _latHomeDefault * SimMath.M_PI / 180.0;
                _gpsParameterStruct.LonHome = _lonHomeDefault * SimMath.M_PI / 180.0;
                _gpsParameterStruct.StdXY = _stdXYDefault;
                _gpsParameterStruct.StdZ = _stdZDefault;
            }
        }

        Vector2Double Reproject(Vector2Double pos)
        {
            double x_rad = pos.x / _earthRadius;    // north
            double y_rad = pos.y / _earthRadius;    // east
            double c = SimMath.sqrtD(x_rad * x_rad + y_rad * y_rad);
            double sin_c = SimMath.sinD(c);
            double cos_c = SimMath.cosD(c);
            double lat_rad, lon_rad;

            if (c != 0.0)
            {
                lat_rad = SimMath.asinD(cos_c * SimMath.sinD(_gpsParameterStruct.LatHome) + (x_rad * sin_c * SimMath.asinD(_gpsParameterStruct.LatHome)) / c);
                lon_rad = (_gpsParameterStruct.LonHome + SimMath.atan2D(y_rad * sin_c, c * SimMath.cosD(_gpsParameterStruct.LatHome) * cos_c - x_rad * SimMath.sinD(_gpsParameterStruct.LatHome) * sin_c));
            }
            else
            {
                lat_rad = _gpsParameterStruct.LatHome;
                lon_rad = _gpsParameterStruct.LonHome;
            }

            Vector2Double ret;
            ret.x = lon_rad;
            ret.y = lat_rad;
            return ret;
        }

        void SensorUpdate()
        {
            double currentTime = SimTime.GetMachineRunTimeStampNs() * 1e-9 + _startSimlationTimeStamp * 1e-9;
            double currentTimeNs = SimTime.GetMachineRunTimeStampNs() + _startSimlationTimeStamp * 1e+9;
            Debug.Log("currentTime: " + currentTime);
            Debug.Log("lastTime: " + _lastUpdateTimeStamp);
            double dt = currentTime - _lastUpdateTimeStamp;
            Debug.Log("dt: " + dt);
            double t = currentTime;
            _lastUpdateTimeStamp = currentTime;
        }

      
        void Start()
        {
            SensorInit();
            ParameterInit();
            base.BaseStart();
            _sleepTimeS = (int)(1 / (base.MessagePubHz)) * 1000;//Count the sleep time for thread
            _pubThread = new Thread(PubData);
            _pubThread.IsBackground = true;//Only set true and the thread can exit follow the main thread
            _pubThread.Start();//Start run thread function
        }

        // Update is called once per frame
        void Update()
        {

        }

        void PubData()
        {
            while (!_isEnd)
            {
                DateTime centuryBegin = new DateTime(2001, 1, 1);
                DateTime currentDate = DateTime.Now;

                long elapsedTicks = currentDate.Ticks - centuryBegin.Ticks;
                TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
                //  poseMsg.timestamp = elapsedSpan.TotalMilliseconds;//返回ms。
                // poseMsg.position.x = RibPosition.x;
                //  poseMsg.position.y = RibPosition.y;
                //  poseMsg.position.z = RibPosition.z;
                //  poseMsg.orientation.x = RibQuaternion.x;
                // poseMsg.orientation.y = RibQuaternion.y;
                //  poseMsg.orientation.z = RibQuaternion.z;
                ///  poseMsg.orientation.w = RibQuaternion.w;
                //  base.BaseLcm.Publish(base.MesageName, poseMsg);
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
