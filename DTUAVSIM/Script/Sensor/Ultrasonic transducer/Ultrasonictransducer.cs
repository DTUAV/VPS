using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ultrasonictransducer : MonoBehaviour
{
    public float viewRadius = 5f;      // 代表视野最远的距离
    static public float viewAngleStep;     // 射线数量，越大就越密集，效果更好但硬件耗费越大
    static public RaycastHit hit = new RaycastHit();
    public float distant_hit;
   // public GUIStyle label_sty = new GUIStyle();
    public bool is_show = false;
    void DrawFieldOfView()
    {

        // 获得最左边那条射线的向量，相对正前方，角度是-45
        Vector3 forward_left = Quaternion.Euler(0, -45, 0) *  - transform.up * viewRadius;
        // 依次处理每一条射线
        for (int i = 0; i <= viewAngleStep; i++)
        {
            // 每条射线都在forward_left的基础上偏转一点，最后一个正好偏转90度到视线最右侧
            Vector3 v = Quaternion.Euler(0, (90.0f / viewAngleStep) * i, 0) * forward_left; ;
            Vector3 ray_position;
            ray_position.x = transform.position.x;
            ray_position.y = transform.position.y;
            ray_position.z = transform.position.z;

            // 创建射线
            Ray ray = new Ray(ray_position, v);

            // 射线只与两种层碰撞，注意名字和你添加的layer一致，其他层忽略
            int mask = LayerMask.GetMask("Obstacle");
            Physics.Raycast(ray, out hit, viewRadius, mask);

            // Player位置加v，就是射线终点pos
            Vector3 pos = transform.position + v;
            if (hit.transform != null)
            {
                // 如果碰撞到什么东西，射线终点就变为碰撞的点了
                pos = hit.point;
                distant_hit = hit.distance;
            }

            if (is_show)
                {
                    Debug.DrawLine(transform.position, pos, Color.yellow);
                    ;
                }
          

        }
        /*
        float total = 0;
        for (int l = 0; l < diatance_list.Count; l++)
        {
            total += diatance_list[l];
        }
        uwbDeal.uwb_yellow_d = total / diatance_list.Count;
        */
    }


    // Start is called before the first frame update
    void Start()
    {
        viewRadius = 5f;      // 代表视野最远的距离
        viewAngleStep = 100f;     // 射线数量，越大就越密集，效果更好但硬件耗费越大
       // label_sty.normal.textColor = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
    }
/*
    void OnGUI()
    {
        GUILayout.Label("障碍物的距离：" + distant_hit, label_sty);
    }
*/
}
