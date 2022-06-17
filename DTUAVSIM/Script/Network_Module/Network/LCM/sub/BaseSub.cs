using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAVCARS.DTNetWork.Lcm.Subscriber
{
    public class BaseSub : MonoBehaviour
    {
        // Start is called before the first frame update
        public string MessageName;
        public LCM.LCM.LCM BaseLcm;

       public void BaseStart()
        {
            BaseLcm = new LCM.LCM.LCM();
        }
    }
}
