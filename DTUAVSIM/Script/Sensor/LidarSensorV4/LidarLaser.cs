using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.LidarSensorV4
{
    public class LidarLaser 
    {
        private int laserId;//激光光线的编号
        public Ray ray;//光线
        private RaycastHit hit;//光线碰撞的返回
        private bool isHit;//是否与物体碰撞的标志
        public float rayDistance;//光线的距离
        private float verticalAngle;//垂直方向的角度
        private GameObject parentObject;//父对象
        public Color laser_color;

        public LidarLaser(GameObject parent, float verticalAngle, float distance, int laserId)
        {
            this.laserId = laserId;
            parentObject = parent;
            
            this.verticalAngle = verticalAngle;
            rayDistance = distance;
            ray = new Ray();
            UpdateRay();
        }

        // Should be called from Update(), for best performance.
        // This is only visual, for debugging.
        public void DrawRay()
        {
            if (isHit)
            {
                Debug.DrawLine(ray.origin, hit.point, laser_color);
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.GetPoint(rayDistance), laser_color);
            }
        }

        public void DebugDrawRay()
        {
            float distance = rayDistance;
            if (isHit)
            {
                distance = hit.distance;
            }
            Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
        }

        // Should be called from FixedUpdate(), for best performance.
        public RaycastHit ShootRay()
        {
            // Perform raycast
            UpdateRay();
            int mask = LayerMask.GetMask("Obstacle");
            isHit = Physics.Raycast(ray, out hit, rayDistance, mask);
            DrawRay();

            if (isHit)
            {
                return hit;
            }
            return new RaycastHit();
        }

        // Update existing ray. Don't create 'new' ray object, that is heavy.
        private void UpdateRay()
        {
            Quaternion q = Quaternion.AngleAxis(verticalAngle, Vector3.forward);
            // Vector3 direction = parentObject.transform.TransformDirection(q * Vector3.right);
            Vector3 direction = parentObject.transform.TransformDirection(q * Vector3.right);
            ray.origin = parentObject.transform.position; //(parentObject.transform.forward * offset);
            ray.direction = direction;
        }

        public Ray GetRay()
        {
            return ray;
        }

        public float GetVerticalAngle()
        {
            return verticalAngle;
        }

        public int GetLaserId()
        {
            return laserId;
        }
    }
}
