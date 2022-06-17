using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAVSIM.Sensor.Lidar2D
{
    public class Lidar2D 
    {
        public Lidar2D(float angleMin,float angleMax,float angleIncrement,float timeIncrement,float scanTime, float rangeMin,float rangeMax)
        {
            _angleMin = angleMin;
            _angleMax = angleMax;
            _angleIncrement = angleIncrement;
            _timeIncrement = timeIncrement;
            _scanTime = scanTime;
            _rangeMin = rangeMin;
            _rangeMax = rangeMax;

            _pointNumber = (uint) ((_angleMax - _angleMin) / _angleIncrement);


        }

        public void UpdateLidar2D(Vector3 lidar2DPosition,float yaw)
        {
            for (int i = 0; i < _pointNumber; i++)
            {
                float angle = _angleIncrement*(180.0f/Mathf.PI) * i + yaw;
                if (angle > 360)
                {
                    angle = angle - 360;
                }

                angle = angle * (Mathf.PI / 180);
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;

            }
        }

        private uint _pointNumber;                      //the number of all laser point
        private float _angleMin;                        //start angle of the scan [rad]
        private float _angleMax;                        //end angle of the scan [rad]
        private float _angleIncrement;                  //angular distance between measurements [rad]
        private float _timeIncrement;                   //time between measurements [seconds] - if your scanner is moving, this will be used in interpolating position of 3d points
        private float _scanTime;                        //time between scans [seconds]
        private float _rangeMin;                        //minimum range value [m]
        private float _rangeMax;                        //maximum range value [m]
        private float[] _ranges;                        //range data [m] (Note: values < range_min or > range_max should be discarded)
        private float[] _intensities;                   //intensity data [device-specific units].  If your device does not provide intensities, please leave the array empty.
        private List<Ray> _rays;

    }

}
