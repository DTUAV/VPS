using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class uav1_1_keep_data : MonoBehaviour {

    void CreateFile(string path, string name, string data)
    {
        StreamWriter sw;
        FileInfo t = new FileInfo(path + "//" + name);
        if(!t.Exists)
        {
            sw = t.CreateText();
        }
        else
        {
            sw = t.AppendText();
        }
        sw.WriteLine(data);
        sw.Close();
        sw.Dispose();
    }

	// Use this for initialization
	void Start () {
		
        

	}
	
	// Update is called once per frame
	void Update () {
		
        if(UAV1_1_power.keep_laser_data_flag)
        {
            int size =uav1_1_laser_data.hit_position.Count;
            //循环输入位置到文件
            for (int i = 0; i < size - 1; i++)
            {
                CreateFile(Application.dataPath, "laser_data", uav1_1_laser_data.hit_position[i].x.ToString()+"," + uav1_1_laser_data.hit_position[i].y.ToString() + ","+ uav1_1_laser_data.hit_position[i].z.ToString());//保存的格式为（x,y,z）
               
            }
            UAV1_1_power.keep_laser_data_flag = false;
            Debug.Log("成功保存激光的数据");
        }
        

    }
}
