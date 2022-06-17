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
using DTUAV.Algorithm_Module.Path_Planning.Control;
using RosSharp.RosBridgeClient.MessageTypes.Std;

namespace RosSharp.RosBridgeClient
{
    public class StartSimulatorSubscriber : UnitySubscriber<MessageTypes.Std.Bool>
    {
        // Start is called before the first frame update
        public PlanningControl PlanningControlInstance; 
        protected override void Start()
        {
            base.Start();
        }
        protected override void ReceiveMessage(Bool message)
        {
            PlanningControlInstance.IsStart = message.data;
        }
    }
}
