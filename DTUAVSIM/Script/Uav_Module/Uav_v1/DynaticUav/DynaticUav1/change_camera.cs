using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class change_camera : MonoBehaviour {

    GameObject main_camera = null;
    //GameObject uav_1_camera = null;
    //GameObject uav_2_camera = null;
    //GameObject uav_3_camera = null;
    //GameObject uav_4_camera = null;
    //GameObject gl_plot = null;
   // GameObject camera_senseor = null;//无人机相机传感器
    GameObject uav_2_1_camera = null;//用于无人机显示的相机：uav_2_1无人机
    GameObject uav_2_2_camera = null;//用于无人机显示的相机：uav_2_2无人机
    GameObject uav_1_1_camera = null;//用于无人机显示的相机：uav_1_1无人机
    GameObject uav_1_2_camera = null;//用于无人机显示的相机：uav_1_2无人机
                                     // Use this for initialization
    void Start () {
        main_camera = GameObject.Find("Main Camera");
        //uav_1_camera = GameObject.Find("Camera_uav_1");
        //uav_2_camera = GameObject.Find("Camera_uav_2");
        //uav_3_camera = GameObject.Find("Camera_uav_3");
        //uav_4_camera = GameObject.Find("Camera_uav_4");
        //gl_plot = GameObject.Find("GL_plot");
        uav_2_1_camera = GameObject.Find("uav2_1_camera");
        uav_2_2_camera = GameObject.Find("uav2_2_camera");
        uav_1_1_camera = GameObject.Find("uav1_1_camera");
        uav_1_2_camera = GameObject.Find("uav1_2_camera");
        main_camera.SetActive(true);
        uav_2_1_camera.SetActive(false);
        uav_2_2_camera.SetActive(false);
        uav_1_1_camera.SetActive(false);
        uav_1_2_camera.SetActive(false);
        //uav_1_camera.SetActive(false);
        //uav_2_camera.SetActive(false);
        //uav_3_camera.SetActive(false);
        //uav_4_camera.SetActive(false);
        //gl_plot.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Z))
        {
            main_camera.SetActive(true);
            uav_2_1_camera.SetActive(false);
            uav_2_2_camera.SetActive(false);
            uav_1_1_camera.SetActive(false);
            uav_1_2_camera.SetActive(false);
            //uav_1_camera.SetActive(false);
            //uav_2_camera.SetActive(false);
            //uav_3_camera.SetActive(false);
            //uav_4_camera.SetActive(false);
            //gl_plot.SetActive(false);
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            main_camera.SetActive(false);
            uav_2_1_camera.SetActive(false);
            uav_2_2_camera.SetActive(false);
            uav_1_1_camera.SetActive(true);
            uav_1_2_camera.SetActive(false);
            //uav_1_camera.SetActive(true);
            //uav_2_camera.SetActive(false);
            //uav_3_camera.SetActive(false);
            //uav_4_camera.SetActive(false);
            //gl_plot.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            //main_camera.SetActive(false);
            //uav_1_camera.SetActive(false);
            //uav_2_camera.SetActive(true);
            //uav_3_camera.SetActive(false);
            //uav_4_camera.SetActive(false);
            //gl_plot.SetActive(false);
            main_camera.SetActive(false);
            uav_2_1_camera.SetActive(false);
            uav_2_2_camera.SetActive(false);
            uav_1_1_camera.SetActive(false);
            uav_1_2_camera.SetActive(true);

        }

        if(Input.GetKeyDown(KeyCode.V))
        {
            //main_camera.SetActive(false);
            //uav_1_camera.SetActive(false);
            //uav_2_camera.SetActive(false);
            //uav_3_camera.SetActive(true);
            //uav_4_camera.SetActive(false);
            //gl_plot.SetActive(false);
            main_camera.SetActive(false);
            uav_2_1_camera.SetActive(true);
            uav_2_2_camera.SetActive(false);
            uav_1_1_camera.SetActive(false);
            uav_1_2_camera.SetActive(false);

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            //main_camera.SetActive(false);
            //uav_1_camera.SetActive(false);
            //uav_2_camera.SetActive(false);
            //uav_3_camera.SetActive(false);
            //uav_4_camera.SetActive(true);
            //gl_plot.SetActive(false);
            main_camera.SetActive(false);
            uav_2_1_camera.SetActive(false);
            uav_2_2_camera.SetActive(true);
            uav_1_1_camera.SetActive(false);
            uav_1_2_camera.SetActive(false);


        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            //main_camera.SetActive(false);
            //uav_1_camera.SetActive(false);
            //uav_2_camera.SetActive(false);
            //uav_3_camera.SetActive(false);
            //uav_4_camera.SetActive(false);
            //gl_plot.SetActive(true);
        }
    }
}
