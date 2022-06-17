using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.LidarSensorV4
{
    public class testLidarMove : MonoBehaviour
    {
        float TranslateSpeed = 0.02f;
        float TranslateSpeedTime = 0.1f;
        public GameObject gameObject;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void FixedUpdate()
        {
            TranslateSpeedTime = 0.01f;
            if (gameObject.transform.position.y <= 0)
            {
                TranslateSpeed= 0.01f;
            }
            if(gameObject.transform.position.y>= 3)
            {
                TranslateSpeed = - 0.01f;
            }
            gameObject.transform.Translate(Vector3.up * TranslateSpeed);
        }
    }
}
