using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser_1 : MonoBehaviour
{
     public float viewRadius = 10;      // 代表视野最远的距离
     public float viewAngleStep = 1000;     // 射线数量，越大就越密集，效果更好但硬件耗费越大
    public float angle_inc;//角度增量
     public RaycastHit hit = new RaycastHit();


    void DrawFieldOfView()
    {
        // 获得最左边那条射线的向量，相对正前方，角度是-45
        Vector3 forward_left = Quaternion.Euler(0, 0, 0) * transform.forward * viewRadius;
        // 依次处理每一条射线
        angle_inc = 360.0f / viewAngleStep;
        for (int i = 0; i <= viewAngleStep; i++)
        {
            // 每条射线都在forward_left的基础上偏转一点，最后一个正好偏转90度到视线最右侧
            Vector3 v = Quaternion.Euler(0, angle_inc * i, 0) * forward_left; ;
            transform.Rotate(0, angle_inc * i, 0);
            // 创建射线
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y, transform.position.z), v);

            // 射线只与两种层碰撞，注意名字和你添加的layer一致，其他层忽略
            int mask = LayerMask.GetMask("Obstacle");
            Physics.Raycast(ray, out hit, viewRadius, mask);

            // Player位置加v，就是射线终点pos
            Vector3 pos = transform.position + v;
            if (hit.transform != null)
            {
                // 如果碰撞到什么东西，射线终点就变为碰撞的点了
                pos = hit.point;
            }
            // 从玩家位置到pos画线段，只会在编辑器里看到
            Debug.DrawLine(transform.position, pos, Color.red); ;


        }
    }

    // Use this for initialization
    void Start()
    {

       // viewRadius = 10f;      // 代表视野最远的距离
       // viewAngleStep = 1000f;     // 射线数量，越大就越密集，效果更好但硬件耗费越大

    }

    // Update is called once per frame
    void Update()
    {

      

    }

    void FixedUpdate()
    {
      
        DrawFieldOfView();
    }
}
