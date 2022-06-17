using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main_Camera_fellow : MonoBehaviour {
   
    public GameObject uav_1;//相机跟踪的对象无人机1
    public GameObject uav_2;//相机跟踪的对象无人机2
    public GameObject uav_3;//相机跟踪的对象无人机3
    public GameObject uav_4;//相机跟踪的对象无人机4
    public GameObject uav_5;//相机跟踪的对象无人机5

    public float smoothing = 5f;
    Vector3 offset_uav1 = Vector3.zero;
    Vector3 offset_uav2 = Vector3.zero;
    Vector3 offset_uav3 = Vector3.zero;
    Vector3 offset_uav4 = Vector3.zero;
    Vector3 offset_uav5 = Vector3.zero;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        offset_uav1 = transform.position - uav_1.GetComponent<Rigidbody>().position;
        offset_uav2 = transform.position - uav_2.GetComponent<Rigidbody>().position;
        offset_uav3 = transform.position - uav_3.GetComponent<Rigidbody>().position;
        offset_uav4 = transform.position - uav_4.GetComponent<Rigidbody>().position;
        offset_uav5 = transform.position - uav_5.GetComponent<Rigidbody>().position;
    }
    void FixedUpdate()
    {
        if (change_control.uav_1_active_flag)
        {
            Vector3 targetCamPos = uav_1.GetComponent<Rigidbody>().position + offset_uav1;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, 2);
        }

        if (change_control.uav_2_active_flag)
        {
            Vector3 targetCamPos = uav_2.GetComponent<Rigidbody>().position + offset_uav2;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, 2);
        }

        if (change_control.uav_3_active_flag)
        {
            Vector3 targetCamPos = uav_3.GetComponent<Rigidbody>().position + offset_uav3;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, 2);
        }

        if (change_control.uav_4_active_flag)
        {
            Vector3 targetCamPos = uav_1.GetComponent<Rigidbody>().position + offset_uav4;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, 2);
        }

        if (change_control.uav_5_active_flag)
        {
            Vector3 targetCamPos = uav_5.GetComponent<Rigidbody>().position + offset_uav5;
            transform.position = Vector3.Lerp(transform.position, targetCamPos, 2);
        }

    }

}
