using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Sensor.LidarSensorV4;

public class movelaser : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject laser;
    public GameObject uav;
    public Vector3 last_uav_position;
    public Vector3 current_uav_position;
    public Vector3 dir;
    public LidarSensorV4 lidarSensor;
    void Start()
    {
        dir = uav.transform.position - laser.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        laser.transform.position = uav.transform.position - dir;
     //   lidarSensor.updateOK = true;
        
    }

}
