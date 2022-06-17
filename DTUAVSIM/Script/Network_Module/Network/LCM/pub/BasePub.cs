using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCM;

namespace DTUAVCARS.DTNetWork.Lcm.Publisher
{
    public class BasePub : MonoBehaviour
    {
        // Start is called before the first frame update
        public string MesageName;
        public float MessagePubHz;
        public LCM.LCM.LCM BaseLcm;

        public void BaseStart()
        {
            BaseLcm = LCM.LCM.LCM.Singleton;
        }



    }
}
