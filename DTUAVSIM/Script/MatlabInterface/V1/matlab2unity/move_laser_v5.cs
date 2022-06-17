using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_laser_v5 : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject laser;
    public GameObject uav;
    public Vector3 dir;
    void Start()
    {
        dir = uav.transform.position - laser.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        laser.transform.position = uav.transform.position - dir;
        

    }
}
