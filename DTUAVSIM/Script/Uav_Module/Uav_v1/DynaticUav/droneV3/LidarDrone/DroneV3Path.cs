using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimUnity.droneV3
{
    public class DroneV3Path
    {
        public Vector4[] waypoints;//所有路点
        public int index = -1;//当前路点索引
        public Vector4 waypoint;//当前路点
        public Vector3 waypoint_position;//位置
        public bool isLoop = false;//是否循环
        public float deviation = 0.1f;//到底的误差
        public bool isFinish = false;//是否完成任务
        public bool change = false;
        public bool IsChangePoint = false;
        public void InitPath(Vector4[] waypos, bool isloop, float devi)
        {
            this.waypoints = waypos;
            this.isLoop = isloop;
            this.deviation = devi;
            this.waypoint = waypoints[0];
            this.waypoint_position = new Vector3(waypoint.x, waypoint.y, waypoint.z);
        }
        //是否到达目的地
        public bool isReach(Transform transform)
        {
            Vector3 position = transform.position;
            waypoint_position = new Vector3(waypoint.x, waypoint.y, waypoint.z);
            float distance = Vector3.Distance(waypoint_position, position);
            return distance < deviation;
        }

        public bool isReach(Vector3 Rbposition, float erro)
        {
            Vector3 position = Rbposition;

            if (Mathf.Abs(waypoint.x - position.x) < erro && Mathf.Abs(waypoint.y - position.y) < erro && Mathf.Abs(waypoint.z - position.z) < erro)
            {
                change = true;
                return true;

            }
            else
                return false;
        }

        public bool isReach(GameObject obj, float erro)
        {
            Vector3 position = obj.GetComponent<Rigidbody>().position;

            if (Mathf.Abs(waypoint.x - position.x) < erro && Mathf.Abs(waypoint.y - position.y) < erro && Mathf.Abs(waypoint.z - position.z) < erro)
            {
                change = true;
                IsChangePoint = true;
                return true;

            }
            else
                return false;
        }


        //下个路点
        public void NextWayPoint()
        {
            if (index < 0)
            {
                index++;
                return;

            }
            else
            {
                if (index < waypoints.Length - 1)
                {
                    index++;
                }
                else
                {
                    if (isLoop)
                    {
                        index = 0;

                    }
                    else
                    {
                        isFinish = true;
                    }
                }
                waypoint = waypoints[index];
            }
        }

        //自动切换路径
        public void autoChangePath(Transform transform)
        {
            if (isReach(transform))
            {
                NextWayPoint();
            }
        }

        public void autoChangePath(GameObject obj, float erro)
        {
            if (isReach(obj, erro) && change == true)
            {
                NextWayPoint();
                // Debug.Log("aaa");
                change = false;
            }
        }

        public void autoChangePath(Vector3 Rb_position, float erro)
        {
            if (isReach(Rb_position, erro) && change == true)
            {
                NextWayPoint();
                // Debug.Log("aaa");
                change = false;
            }
        }

    }
}