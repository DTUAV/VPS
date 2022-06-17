using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.droneV3;
public class Behaviour : MonoBehaviour
{

    GUIStyle LabelStyle = new GUIStyle();
    

    // Start is called before the first frame update
    void Start()
    {
        LabelStyle.normal.textColor = Color.red;//显示字体为红色
    }


    //在界面显示无人机的控制模式和任务飞行模式
    void showComtrol(int ComModel,int MissionModel)
    {
        string ComModelStr = ComModel == 0 ? "手动模式" : "自动模式";
        string MissionModelStr = MissionModel == 0 ? "初始任务执行模式" : "从界面获取任务点执行";
        GUILayout.Label("无人机的控制模式:" + ComModelStr + "   " + "无人机任务点模式：" + MissionModelStr,LabelStyle);
    }



    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        showComtrol(ControlModel.ComModel, ControlModel.MissionModel);
    }



}
