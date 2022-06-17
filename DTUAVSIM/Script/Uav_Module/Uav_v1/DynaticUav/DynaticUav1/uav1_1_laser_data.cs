using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uav1_1_laser_data : MonoBehaviour {

  static public  List<Vector3> hit_position;//新建数据链表保存激光检测到的位置

    // Use this for initialization
    void Start () {
      hit_position =   new List<Vector3>();//初始化链表
    }
	
	// Update is called once per frame
	void Update () {
        int size = hit_position.Count;//计算链表的长度

        for (int i = 0; i < size - 1; i++)
        {
            if(uav1_1_laser.hit.point == hit_position[i])
            {
                hit_position.RemoveAt(i);//去掉重复的位置
            }
        }
        hit_position.Add(uav1_1_laser.hit.point);//加入检测到的位置
    }
    void FixedUpdate()
    {
        Debug.Log("hit_position.Count" + hit_position.Count);
    }
}
