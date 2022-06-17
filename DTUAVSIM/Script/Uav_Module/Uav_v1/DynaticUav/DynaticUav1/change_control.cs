using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_control : MonoBehaviour {

    GameObject uav_1 = null;
    //GameObject uav_4 = null;
    static public bool uav_1_active_flag = false;
    static public bool uav_2_active_flag = false;
    static public bool uav_3_active_flag = false;
    static public bool uav_4_active_flag = false;
    static public bool uav_5_active_flag = false;


	// Use this for initialization
	void Start () {
        uav_1 = GameObject.Find("uav2_1");
       // uav_4 = GameObject.Find("uav_4");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        GUILayout.Space(550);

        if(GUILayout.Button("无人机1号控制"))
        {
           // uav_1.SetActive(true);
           // uav_4.SetActive(false);

            uav_1_active_flag = true;
            uav_2_active_flag = false;
            uav_3_active_flag = false;
            uav_4_active_flag = false;
            uav_5_active_flag = false;

        }

        if (GUILayout.Button("无人机2号控制"))
        {
            //  uav_1.SetActive(false);
            //  uav_4.SetActive(true);

            uav_1_active_flag = false;
            uav_2_active_flag = true;
            uav_3_active_flag = false;
            uav_4_active_flag = false;
            uav_5_active_flag = false;

        }

        if (GUILayout.Button("无人机3号控制"))
        {
            //  uav_1.SetActive(false);
            //  uav_4.SetActive(true);

            uav_1_active_flag = false;
            uav_2_active_flag = false;
            uav_3_active_flag = true;
            uav_4_active_flag = false;
            uav_5_active_flag = false;

        }

        if (GUILayout.Button("无人机4号控制"))
        {
          //  uav_1.SetActive(false);
          //  uav_4.SetActive(true);

            uav_1_active_flag = false;
            uav_2_active_flag = false;
            uav_3_active_flag = false;
            uav_4_active_flag = true;
            uav_5_active_flag = false;

        }

        

        if (GUILayout.Button("无人机5号控制"))
        {
            //  uav_1.SetActive(false);
            //  uav_4.SetActive(true);

            uav_1_active_flag = false;
            uav_2_active_flag = false;
            uav_3_active_flag = false;
            uav_4_active_flag = false;
            uav_5_active_flag = true;

        }

        if(GUILayout.Button("退出仿真平台"))
        {
            Application.Quit();
            System.Environment.Exit(0);
        }

    }
}
