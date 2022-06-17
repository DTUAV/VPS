using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUav : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("无人机对象")]
    public GameObject uav;

    [Header("旋翼对象")]
    public GameObject propellorFR;
    public GameObject propellorFL;
    public GameObject propellorBR;
    public GameObject propellorBL;
    
    [Header("无人机当前的位置")]
    public Vector3 uavPosition;

    [Header("无人机当前的姿态角")]
    public Vector3 uavAngle;
    [Header("移动的步进")]
    public float stepMove = 0.001f;

    [Header("旋转的步进")]
    public float stepRotate = 0.01f;

    [Header("螺旋桨步进")]
    public float stepPropellor = 0.1f;

    [Header("无人机原点的位置")]
    public Vector3 uavPositionInit;

    [Header("无人机初始角度")]
    public Vector3 uavAngleInit;

    GUIStyle styleLable;
    GUIStyle styleLable1;
    float stepMoveInit;
    float stepRotateInit;
    float stepPropellorInit;
    void Start()
    {
        if(uav == null && propellorBL == null && propellorBR == null && propellorFL == null && propellorFR == null )
        {
            Debug.LogError("操作的对象未指定");
        }
        else
        {
            uavPositionInit = uav.transform.position;
            uavAngleInit = uav.transform.eulerAngles;
            styleLable = new GUIStyle();
            styleLable.normal.textColor = Color.black;
            styleLable1 = new GUIStyle();
            styleLable1.normal.textColor = Color.red;
            uavPosition = uavPositionInit;
            uavAngle = uavAngleInit;
            stepMoveInit = stepMove;
            stepRotateInit = stepRotate;
            stepPropellorInit = stepPropellor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // uav.transform.Translate(uavPosition);
        uav.transform.position = uavPosition;
        // uav.transform.Rotate(uavAngle.x, uavAngle.y, uavAngle.z, Space.World);
        uav.transform.eulerAngles = uavAngle;
    }

    void OnGUI()
    {
        GUILayout.Label("无人机当前位置： " + uavPosition,styleLable1);
        GUILayout.Label("无人机当前姿态角：" + uavAngle, styleLable1);
        GUILayout.Label("无人机操作说明：向8/2箭头键：无人机向上/下飞行 向4/6箭头键：无人机向左/右移动 1键向前移动 3键向后移动",styleLable);
        GUILayout.Label("无人机螺旋桨操作说明：Q键为前左FL螺旋桨顺时针旋转 W键为前左FL螺旋桨逆时针旋转",styleLable);
        GUILayout.Label("无人机螺旋桨操作说明：E键为前右FR螺旋桨顺时针旋转 R键为前右FR螺旋桨逆时针旋转",styleLable);
        GUILayout.Label("无人机螺旋桨操作说明：A键为后左BL螺旋桨顺时针旋转 S键为后左BL螺旋桨逆时针旋转",styleLable);
        GUILayout.Label("无人机螺旋桨操作说明：D键为后右BR螺旋桨顺时针旋转 F键为后右BR螺旋桨逆时针旋转",styleLable);
        GUILayout.Label("无人机姿态角操作说明：俯仰角pitch：T键加 Y键减  滚转角Roll：G键加 H键向减 偏航角：B键加 N键减",styleLable);
        GUILayout.Label("功能性按键：F1:无人机回到初始位置  F2:无人机姿态角回到初始位置 F3：无人机复位",styleLable);
        if(Input.GetKey(KeyCode.Alpha8))
        {
            uavPosition.y += stepMove;
            stepMove += 0.0001f;
            
            GUILayout.Label("数字8已经按下",styleLable1);
        }
        if(Input.GetKeyUp(KeyCode.Alpha8))
        {
            stepMove = stepMoveInit;
            GUILayout.Label("数字8已经弹起", styleLable1);
        }



        if(Input.GetKey(KeyCode.Alpha2))
        {
            uavPosition.y -= stepMove;
            stepMove += 0.0001f;
            GUILayout.Label("数字已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            stepMove = stepMoveInit;
            GUILayout.Label("数字2已经弹起", styleLable1);
        }





        if (Input.GetKey(KeyCode.Alpha6))
        {
            uavPosition.x += stepMove;
            stepMove += 0.0001f;
            GUILayout.Label("数字6已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha6))
        {
            stepMove = stepMoveInit;
            GUILayout.Label("数字6已经弹起", styleLable1);
        }

        if (Input.GetKey(KeyCode.Alpha4))
        {
            uavPosition.x -= stepMove;
            stepMove += 0.0001f;
            GUILayout.Label("数字4已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            stepMove = stepMoveInit;
            GUILayout.Label("数字4已经弹起", styleLable1);
        }



        if (Input.GetKey(KeyCode.Alpha1))
        {
            uavPosition.x += stepMove;
            stepMove += 0.0001f;
            GUILayout.Label("数字1已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            stepMove = stepMoveInit;
            GUILayout.Label("数字1已经弹起", styleLable1);
        }



        if (Input.GetKey(KeyCode.Alpha3))
        {
            uavPosition.x -= stepMove;
            stepMove += 0.0001f;
            GUILayout.Label("数字3已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            stepMove = stepMoveInit;
            GUILayout.Label("数字3已经弹起", styleLable1);
        }



        if (Input.GetKey(KeyCode.T))
        {
            uavAngle.x += stepRotate;
            stepRotate += 0.01f;
            GUILayout.Label("键T已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            stepRotate = stepRotateInit;
            GUILayout.Label("数字T已经弹起", styleLable1);
        }





        if (Input.GetKey(KeyCode.Y))
        {
            uavAngle.x -= stepRotate;
            stepRotate += 0.01f;
            GUILayout.Label("键Y已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.Y))
        {
            stepRotate = stepRotateInit;
            GUILayout.Label("数字Y已经弹起", styleLable1);
        }


        if (Input.GetKey(KeyCode.G))
        {
            uavAngle.z -= stepRotate;
            stepRotate += 0.01f;
            GUILayout.Label("键G已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            stepRotate = stepRotateInit;
            GUILayout.Label("数字G已经弹起", styleLable1);
        }



        if (Input.GetKey(KeyCode.H))
        {
            uavAngle.z += stepRotate;
            stepRotate += 0.01f;
            GUILayout.Label("键H已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.H))
        {
            stepRotate = stepRotateInit;
            GUILayout.Label("数字H已经弹起", styleLable1);
        }




        if (Input.GetKey(KeyCode.B))
        {
            uavAngle.y += stepRotate;
            stepRotate += 0.01f;
            GUILayout.Label("键B已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            stepRotate = stepRotateInit;
            GUILayout.Label("数字B已经弹起", styleLable1);
        }




        if (Input.GetKey(KeyCode.N))
        {
            uavAngle.y -= stepRotate;
            stepRotate += 0.01f;
            GUILayout.Label("键N已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            stepRotate = stepRotateInit;
          
            GUILayout.Label("数字N已经弹起", styleLable1);
        }




        if (Input.GetKey(KeyCode.Q))
        {
            propellorFL.transform.Rotate(0,stepPropellor,0,Space.Self);
            stepPropellor+= 0.01f;
            GUILayout.Label("键Q已经按下", styleLable1);
        }
        if(Input.GetKeyUp(KeyCode.Q))
        {
            stepPropellor = stepPropellorInit;
            GUILayout.Label("键Q已经弹起", styleLable1);
        }
        if (Input.GetKey(KeyCode.W))
        {
            propellorFL.transform.Rotate(0, -stepPropellor, 0, Space.Self);
            stepPropellor += 0.01f;
            GUILayout.Label("键W已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            stepPropellor = stepPropellorInit;
            GUILayout.Label("键W已经弹起", styleLable1);
        }

        if (Input.GetKey(KeyCode.A))
        {
            propellorBL.transform.Rotate(0, stepPropellor, 0, Space.Self);
            stepPropellor += 0.01f;
            GUILayout.Label("键A已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            stepPropellor = stepPropellorInit;
            GUILayout.Label("键A已经弹起", styleLable1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            propellorBL.transform.Rotate(0, -stepPropellor, 0, Space.Self);
            stepPropellor += 0.01f;
            GUILayout.Label("键S已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            stepPropellor = stepPropellorInit;
            GUILayout.Label("键S已经弹起", styleLable1);
        }

        if (Input.GetKey(KeyCode.E))
        {
            propellorFR.transform.Rotate(0, stepPropellor, 0, Space.Self);
            stepPropellor += 0.01f;
            GUILayout.Label("键E已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            stepPropellor = stepPropellorInit;
            GUILayout.Label("键E已经弹起", styleLable1);
        }
        if (Input.GetKey(KeyCode.R))
        {
            propellorFR.transform.Rotate(0, -stepPropellor, 0, Space.Self);
            stepPropellor += 0.01f;
            GUILayout.Label("键R已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            stepPropellor = stepPropellorInit;
            GUILayout.Label("键R已经弹起", styleLable1);
        }

        if (Input.GetKey(KeyCode.D))
        {
            propellorBR.transform.Rotate(0, stepPropellor, 0, Space.Self);
            stepPropellor += 0.01f;
            GUILayout.Label("键D已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            stepPropellor = stepPropellorInit;
            GUILayout.Label("键D已经弹起", styleLable1);
        }
        if (Input.GetKey(KeyCode.F))
        {
            propellorBR.transform.Rotate(0, -stepPropellor, 0, Space.Self);
            stepPropellor += 0.01f;
            GUILayout.Label("键F已经按下", styleLable1);
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            stepPropellor = stepPropellorInit;
            GUILayout.Label("键F已经弹起", styleLable1);
        }

        if(Input.GetKey(KeyCode.F1))
        {
            uavPosition = uavPositionInit;
            GUILayout.Label("键F1已经按下", styleLable1);
        }

        if (Input.GetKey(KeyCode.F2))
        {
            uavAngle = uavAngleInit;
            GUILayout.Label("键F2已经按下", styleLable1);
        }

        if (Input.GetKey(KeyCode.F3))
        {
            uavPosition = uavPositionInit;
            uavAngle = uavAngleInit;
            GUILayout.Label("键F3已经按下", styleLable1);
        }





    }
}
