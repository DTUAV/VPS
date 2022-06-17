using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_change_yaw : MonoBehaviour
{
    public Camera sensor_camera;
    public float rotation_y = 0;
    // Start is called before the first frame update
    void Start()
    {
        if(sensor_camera ==null)
        {
            Debug.Log("camera_change_yaw未指定相机传感器对象");
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    void FixedUpdate()
    {
        Quaternion targetAngels;
        float rotateSpeed = 2f;
        targetAngels = Quaternion.Euler(0, rotation_y, 0);
          sensor_camera.transform.rotation = Quaternion.Slerp(transform.rotation, targetAngels, rotateSpeed * Time.deltaTime);
          if (Quaternion.Angle(targetAngels, transform.rotation) < 1)
          {
              sensor_camera.transform.rotation = targetAngels;
           }
    }
}
