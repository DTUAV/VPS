
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LidarSensor : MonoBehaviour
{
    private float lastUpdate = 0;
   LinkedList<LidarSensorData> hits;
    private List<Laser> lasers;
    private float horizontalAngle = 0;//水平方向的角度

    public float numberOfLasers = 20.0f;//激光的线数
    public float rotationSpeedHz = 1.0f;//旋转速度
    public float rotationAnglePerStep = 45f;//每步旋转的角度
    public float rayDistance = 10f;//光线的距离
    public float upperFOV = 30f;//最高FOV
    public float lowerFOV = 30f;//最低FOV
    public float offset;//偏移
    public float upperNormal = 30f;
    public float lowerNormal = 30f;
    public float lapTime = 0;
    private float previousUpdate;
    private float lastLapTime;
    public GameObject LidarLaserObject;


    // Use this for initialization
    void Start()
    {
        {
           // LidarLaserObject = GameObject.Find("LidarLaser/main");
            lastLapTime = 0;
            lasers = new List<Laser>();
            hits = new LinkedList<LidarSensorData>();
            float upperTotalAngle = upperFOV / 2;
            float lowerTotalAngle = lowerFOV / 2;
            float upperAngle = upperFOV / (numberOfLasers / 2);
            float lowerAngle = lowerFOV / (numberOfLasers / 2);
            offset = (lowerTotalAngle + lowerNormal) / numberOfLasers;
            for (int i = 0; i < numberOfLasers; i++)
            {
                if (i < numberOfLasers / 2)
                {
                    lasers.Add(new Laser(LidarLaserObject,  i* offset, rayDistance, -offset, i));
                }
                else
                {
                    lasers.Add(new Laser(LidarLaserObject, -(i + 1 - numberOfLasers / 2) * offset, rayDistance, offset, i));                
                }
            }
        }
    }

        void Update()
        {
            // For debugging, shows visible ray in real time.

            foreach (Laser laser in lasers)
            {
                laser.DrawRay();
            }
            
        }

        void FixedUpdate()
        {
            // Check if number of steps is greater than possible calculations by unity.
            float numberOfStepsNeededInOneLap = 360 / Mathf.Abs(rotationAnglePerStep);
            float numberOfStepsPossible = 1 / Time.fixedDeltaTime / 5;
            float precalculateIterations = 1;
            // Check if we need to precalculate steps.
            if (numberOfStepsNeededInOneLap > numberOfStepsPossible)
            {
                precalculateIterations = (int)(numberOfStepsNeededInOneLap / numberOfStepsPossible);
                if (360 % precalculateIterations != 0)
                {
                    precalculateIterations += 360 % precalculateIterations;
                }
            }

            // Check if it is time to step. Example: 2hz = 2 rotations in a second.
            if (Time.fixedTime - lastUpdate > (1 / (numberOfStepsNeededInOneLap) / rotationSpeedHz) * precalculateIterations)
            {
                // Update current execution time.
                lastUpdate = Time.fixedTime;

                for (int i = 0; i < precalculateIterations; i++)
                {
                    // Perform rotation.
                    LidarLaserObject.transform.Rotate(0, 0, rotationAnglePerStep);
                    horizontalAngle += rotationAnglePerStep; // Keep track of our current rotation.
                    if (horizontalAngle >= 360)
                    {
                        horizontalAngle -= 360;
                       
                        lastLapTime = Time.fixedTime;

                    }


                    // Execute lasers.
                    foreach (Laser laser in lasers)
                    {
                        RaycastHit hit = laser.ShootRay();
                        float distance = hit.distance;
                        if (distance != 0) // Didn't hit anything, don't add to list.
                        {
                            float verticalAngle = laser.GetVerticalAngle();
                            hits.AddLast(new LidarSensorData(LidarLaserObject.transform.position, distance, verticalAngle, horizontalAngle, hit.point, laser.GetLaserId()));
                        }
                    }
                }


               

            }
        }
    
}
