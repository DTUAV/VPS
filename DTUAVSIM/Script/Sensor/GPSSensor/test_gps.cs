using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Sensor.Gps;
public class test_gps : MonoBehaviour
{
    // Start is called before the first frame update
    public  GameObject obj;
     gps_sensor gps;
    void Start()
    {
        gps = new gps_sensor(obj);
        
    }

    // Update is called once per frame
    void Update()
    {
        gps.update_gps();
    }
}
