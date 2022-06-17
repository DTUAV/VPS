/*
 * 2021-8-17 In Guangdong University of Technology By Yang Yuanlin
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAVCARS.UAV
{
    public class VelocityControl : MonoBehaviour
    {
        public Rigidbody UavRib;

        public bool IsStart;

        public float SimulationTime;

        public Vector3 RefVelocityRos;

        private Coroutine _velocityCot;
        // Start is called before the first frame update
        void Start()
        {
            IsStart = true;
          //  _velocityCot = StartCoroutine(AddVelocity());
        }

        void FixedUpdate()
        {
            UavRib.velocity = TF.TF.Local2Global(TF.TF.Ros2Unity(RefVelocityRos), UavRib.rotation.eulerAngles.y * Mathf.Deg2Rad);
        }

            [System.Obsolete]
        private IEnumerator AddVelocity()
        {
            float runTime = 0;
            while (IsStart)
            {
                if (runTime>=SimulationTime)
                {
                    UavRib.velocity = TF.TF.Local2Global(TF.TF.Ros2Unity(RefVelocityRos), UavRib.rotation.eulerAngles.y*Mathf.Deg2Rad);
                    runTime = 0;
                }
                else
                {
                  //  UavRib.velocity = Vector3.zero;
                    runTime += Time.deltaTime;
                }
                yield return null;
            }

        }
    }
}
