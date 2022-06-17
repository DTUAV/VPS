using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class uwb_red : MonoBehaviour
{
    public float viewRadius = 5f;      // 代表视野最远的距离
    static public float viewAngleStep;     // 射线数量，越大就越密集，效果更好但硬件耗费越大
    public Collider uwb_collider;
    public List<float> diatance_list = new List<float>();
    public uwb_deal uwbDeal;
    static public RaycastHit hit = new RaycastHit();

    public float position_x;
    public float position_y;
    public float position_z;
    public bool isStop = false;
   

    //public Color laser_color;
    //  public Thread uwb_thread;
    void DrawFieldOfView()
    {
        /*  {
              // 获得最左边那条射线的向量，相对正前方，角度是-45
              Vector3 forward_left = Quaternion.Euler(0, -45, 0) * transform.forward * viewRadius;
              // 依次处理每一条射线
              for (int i = 0; i <= viewAngleStep; i++)
              {
                  // 每条射线都在forward_left的基础上偏转一点，最后一个正好偏转90度到视线最右侧
                  Vector3 v = Quaternion.Euler(0, (360.0f / viewAngleStep) * i, 0) * forward_left; ;

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
          */

        // while (!isStop)
        {
            float i = -1;
            float j = -1;
            float k = -1;
            // 依次处理每一条射线
            while (i <= 1)
            {
                while (j <= 1)
                {
                    while (k <= 1)
                    {
                        Vector3 v = new Vector3(i, j, k) * viewRadius;
                        // 创建射线
                        Ray ray = new Ray(new Vector3(position_x, position_y, position_z), v);

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
                        if(hit.collider == uwb_collider)
                        {
                            diatance_list.Add(hit.distance);
                        }
                        // 从玩家位置到pos画线段，只会在编辑器里看到
                        Debug.DrawLine(transform.position, pos, Color.red);
                        k += 0.08f;
                    }
                    k = -1;
                    j += 0.08f;
                }
                j = -1;
                k = -1;
                i += 0.08f;


            }
            float total = 0;
            for(int l= 0; l< diatance_list.Count; l++)
            {
                total += diatance_list[l];
            }
            uwbDeal.uwb_red_d = total / diatance_list.Count;


        }
    }






    // Use this for initialization
    void Start()
    {
        position_x = transform.position.x;
        position_y = transform.position.y;
        position_z = transform.position.z;
        //viewRadius = 0.5f;      // 代表视野最远的距离
                                // viewAngleStep = 1000f;     // 射线数量，越大就越密集，效果更好但硬件耗费越大
                                // uwb_thread = new Thread(DrawFieldOfView);
                                // uwb_thread.Start();

    }

    // Update is called once per frame
    void Update()
    {

        DrawFieldOfView();

    }

    void FixedUpdate()
    {
        //  transform.Rotate(0, 90, 0);
        //DrawFieldOfView();
    }
    void OnGUI()
    {
        if (GUILayout.Button("停止"))
        {
            isStop = true;
        }
    }

}
