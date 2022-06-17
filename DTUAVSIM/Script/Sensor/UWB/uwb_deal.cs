using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uwb_deal : MonoBehaviour
{
    [Header("一号基站")]
    public GameObject uwb_red_obj;
    [Header("二号基站")]
    public GameObject uwb_blue_obj;
    [Header("三号基站")]
    public GameObject uwb_black_obj;
    [Header("四号基站")]
    public GameObject uwb_yellow_obj;

    [Header("无人机位置")]
    public Vector3 uav_position = new Vector3(0, 2, 0);

    private Vector2 uwb_red_position;//一号基站的位置
    private Vector2 uwb_blue_position;//二号基站的位置
    private Vector2 uwb_black_position;//三号基站的位置
    private Vector2 uwb_yellow_position;//四号基站的位置

    public float uwb_red_d;//无人机距离一号基站的距离
    public float uwb_blue_d;//无人机距离二号基站的距离
    public float uwb_black_d;//无人机距离三号基站的距离
    public float uwb_yellow_d;//无人机距离四号基站的距离

    public float x1;
    public float x2;
    public float x3;
    public float x4;
    void uwb_init()
    {
        uwb_red_position = new Vector2(-2.5f, -2.5f);
        uwb_blue_position = new Vector2(2.5f, -2.5f);
        uwb_black_position = new Vector2(2.5f, 2.5f);
        uwb_yellow_position = new Vector2(-2.5f, 2.5f);
    }

    void getUavPosition()
    {
        x1 = -0.5f * Mathf.Pow(((uwb_red_d + uwb_blue_d - 5) * (uwb_red_d + uwb_blue_d + 5) * (uwb_red_d - uwb_blue_d + 5) * (-uwb_red_d + uwb_blue_d + 5)) / 25, 0.5f) - 5 / 2;
        x2 = 0.5f * Mathf.Pow(((uwb_red_d + uwb_blue_d - 5) * (uwb_red_d + uwb_blue_d + 5) * (uwb_red_d - uwb_blue_d + 5) * (-uwb_red_d + uwb_blue_d + 5)) / 25, 0.5f) - 5 / 2;
        x3 = 5 / 2 - 0.5f * Mathf.Pow(((uwb_black_d + uwb_yellow_d - 5) * (uwb_black_d + uwb_yellow_d + 5) * (uwb_black_d - uwb_yellow_d + 5) * (-uwb_black_d + uwb_yellow_d + 5)) / 25, 0.5f);
        x4 = 5 / 2 + 0.5f * Mathf.Pow(((uwb_black_d + uwb_yellow_d - 5) * (uwb_black_d + uwb_yellow_d + 5) * (uwb_black_d - uwb_yellow_d + 5) * (-uwb_black_d + uwb_yellow_d + 5)) / 25, 0.5f);
        Debug.Log(Mathf.Pow(((uwb_red_d + uwb_blue_d - 5) * (uwb_red_d + uwb_blue_d + 5) * (uwb_red_d - uwb_blue_d + 5) * (-uwb_red_d + uwb_blue_d + 5)) / 25, 0.5f));
        float z = Mathf.Pow(uwb_red_d, 2) / 10 - Mathf.Pow(uwb_blue_d, 2) / 10;

        if (Mathf.Abs(x1 -x3)<=1)
        {
            uav_position.x = (x1 + x3)/2;
        }
        else if (Mathf.Abs(x1 - x4) <= 1)
        {
            uav_position.x = (x1 + x4)/2;
        }
        else if(Mathf.Abs(x2 - x3) <= 1)
        {
            uav_position.x = (x2 + x3)/2;
        }
        else if(Mathf.Abs(x2 - x4) <= 1)
        {
            uav_position.x = (x2 + x4)/2;
        }

        uav_position.z = z;


    }



    // Start is called before the first frame update
    void Start()
    {
        uwb_init();
    }

    // Update is called once per frame
    void Update()
    {
        getUavPosition();
    }
}
