using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("移动的相机对象")]
    public Transform cameraTransform;
    [Header("相机的当前位置")]
    public float camera_position_x;
    public float camera_position_y;
    public float camera_position_z;
    [Header("控制键按下相机的步进")]
    public float step = 0.01f;
    public GUIStyle styleLable;
    void Start()
    {
        styleLable = new GUIStyle();
        styleLable.normal.textColor = Color.red;
        if (cameraTransform == null)
        {
            Debug.Log("脚本未指定要操作的相机对象");
        }
        else
        {
            //初始化相机的位置
            camera_position_x = cameraTransform.position.x;
            camera_position_y = cameraTransform.position.y;
            camera_position_z = cameraTransform.position.z;
        }
    }

    // Update is called once per frame
    void Update()
    {
        cameraTransform.position = new Vector3(camera_position_x, camera_position_y, camera_position_z);
    }

    void OnGUI()
    {
        GUILayout.Label("当前相机的位置： " + "(" + camera_position_x + "," + camera_position_y + "," + camera_position_z + ")", styleLable);
        GUILayout.Label("移动相机：左右箭头为左右移动相机，上下箭头为上下移动相机，F键为向前移动，B键为向后移动",styleLable);
        if(Input.GetKey(KeyCode.UpArrow))
        {
            camera_position_z += step;
            GUILayout.Label("向上的箭头已经按下",styleLable);
        }
        if(Input.GetKey(KeyCode.DownArrow))
        {
            camera_position_z -= step;
            GUILayout.Label("向上的箭头已经按下",styleLable);

        }
        if(Input.GetKey(KeyCode.LeftArrow))
        {
            camera_position_x -= step;
            GUILayout.Label("向左的箭头已经按下",styleLable);
        }

        if(Input.GetKey(KeyCode.RightArrow))
        {
            camera_position_x += step;
            GUILayout.Label("向右的箭头已经按下",styleLable);
        }

        if(Input.GetKey(KeyCode.F))
        {
            camera_position_y -= step;
            GUILayout.Label("F键已经按下", styleLable);
        }
        if(Input.GetKey(KeyCode.B))
        {
            camera_position_y += step;
            GUILayout.Label("B键已经按下",styleLable);
        }




    }


}
