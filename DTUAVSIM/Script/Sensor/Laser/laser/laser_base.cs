using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser_base
{
    private int LaserID;//激光的编号
    private Ray ray;//光线
    private RaycastHit hit;//激光碰撞后的返回
    private bool isHit;//激光是否与物体碰撞
    private float MaxDistanceRay;//激光射线的最大距离
    private float VerticalAngule;//激光光线的垂直方向的角度
    private GameObject RayObject;//发射激光的对象
    private float offset;//激光光线的偏移
  

    public laser_base(GameObject RayObject,float VerticalAngule,float MaxDistanceRay,float offset,int LaseID)
    {
        //类的初始化
        this.LaserID = LaseID;
        this.RayObject = RayObject;
        this.offset = offset;
        this.VerticalAngule = VerticalAngule;
        this.MaxDistanceRay = MaxDistanceRay;
        ray = new Ray();
   
        UpdateRay();
    }

    private void UpdateRay()
    {
        Quaternion q = Quaternion.AngleAxis(VerticalAngule, Vector3.up);//forward
        Vector3 direction = RayObject.transform.TransformDirection(q * Vector3.right);//up
        ray.origin = RayObject.transform.position + (RayObject.transform.forward * offset);//光线的起点
        ray.direction = direction;//光线的方向
    }
    public void DrawRay()
    {
        if (isHit)
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
        else
        {
            Debug.DrawLine(ray.origin, ray.GetPoint(MaxDistanceRay), Color.red);
        }
    }

    public void DebugDrawRay()
    {
        float distance = MaxDistanceRay;
        if (isHit)
        {
            distance = hit.distance;
        }
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
    }
    public RaycastHit ShootRay()
    {
        
                Quaternion q = Quaternion.AngleAxis(VerticalAngule, Vector3.forward);//Z轴
                Vector3 direction = RayObject.transform.TransformDirection(q * Vector3.up);//X轴
                ray.origin = RayObject.transform.position + new Vector3(0,offset,0);//光线的起点
                ray.direction = direction;//光线的方向

                isHit = Physics.Raycast(ray, out hit, MaxDistanceRay,8);
                DrawRay();
            
        
        
        //Quaternion q = Quaternion.AngleAxis(VerticalAngule, Vector3.forward);//Z轴
        //Vector3 direction = RayObject.transform.TransformDirection(q * Vector3.up);//X轴
        //ray.origin = RayObject.transform.position + (RayObject.transform.forward * offset);//光线的起点
        //ray.direction = direction;//光线的方向

        //isHit = Physics.Raycast(ray, out hit, MaxDistanceRay);
        //DrawRay();

        if (isHit)
        {
            return hit;
       }
        return new RaycastHit();
    }

    public Ray GetRay()
    {
        return ray;
    }

    public float GetVerticalAngle()
    {
        return VerticalAngule;
    }

    public int GetLaserId()
    {
        return LaserID;
    }

   
}
