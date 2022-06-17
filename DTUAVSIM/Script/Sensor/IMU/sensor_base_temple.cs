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
using UnityEngine;
using System.Threading;
using DTUAVCARS.DTNetWork.Lcm.Publisher;
// ADIS16448 IMU
namespace DTUAVSIM.Sensor.base_temple
{
    public class sensor_base_temple : BasePub
    {
        // Program Parameters 
        private int _sleepTimeS; //The sleep time for this thread
        private Thread _pubThread;//Define a thread to publish data
        private bool _isEnd;//The flag is used to close the thread




        void Start()
        {
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
