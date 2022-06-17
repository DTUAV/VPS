using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infraredsensor : MonoBehaviour
{
    public float viewRadius = 5f;      // 代表视野最远的距离
    static public float viewAngleStep;     // 射线数量，越大就越密集，效果更好但硬件耗费越大
    static public RaycastHit hit = new RaycastHit();
    public float distant_hit;
    public GUIStyle label_sty = new GUIStyle();

    void DrawFieldOfView()
    {

        
        Vector3 forward_left =   -transform.up * viewRadius;
       
        
           
            Vector3 v =  forward_left; ;

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
               distant_hit = hit.distance;
            }


            // 从玩家位置到pos画线段，只会在编辑器里看到
            Debug.DrawLine(transform.position, pos, Color.red); 


       
    }
    // Start is called before the first frame update
    void Start()
    {
        viewRadius = 10f;      // 代表视野最远的距离
        viewAngleStep = 1f;     // 射线数量，越大就越密集，效果更好但硬件耗费越大
        label_sty.normal.textColor = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        DrawFieldOfView();
    }

    void OnGUI()
    {
        GUILayout.Label("障碍物的距离：" + distant_hit,label_sty);
    }

}
