/*
 * Author: Yuanlin Yang
 * Date: 2022-1-3
 * Location: Guangdong University of Technology
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace RosSharp.RosBridgeClient
{
    public class PredictorZSub : UnitySubscriber<MessageTypes.Std.Float64MultiArray>
    {
        public int predictedWindow;
        public double[] data;
        protected override void ReceiveMessage(Float64MultiArray message)
        {
            // throw new System.NotImplementedException();
            data = message.data;
        }
        protected override void Start()
        {
            base.Start();
            data = new double[predictedWindow];
            for (int i = 0; i < predictedWindow; i++)
            {
                data[i] = 0;
            }
        }
    }
}