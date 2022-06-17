using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimUnity.droneV3
{
    public class droneV3 : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject propellorFR;
        public GameObject propellorFL;
        public GameObject propellorBR;
        public GameObject propellorBL;
        public GameObject drone;
        public Vector3 target_position;
        public Vector3 current_position;
        public Vector3 target_angle;
        public Vector3 current_angle;
        void Start()
        {
            if(propellorFR!=null&&propellorFL!=null&&propellorBR!=null&&propellorBL!=null)
            {
                Debug.Log("对象初始化成功");
            }
            else
            {
                Debug.LogError("未初始化所有对象");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
        void FixedUpdate()
        {

        }
    }
}