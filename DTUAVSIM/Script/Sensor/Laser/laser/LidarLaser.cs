using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarLaser : MonoBehaviour {

    private float LastUpdate = 0;//上次的更新
    private float HorizontalAngle = 0;//水平方向的角度
    public float RotationSpeedHz = 1.0f;//激光雷达旋转的频率
    public float RotationAnglePerStep = 45;//激光雷达每次旋转的角度
    public float RayDistance = 10f;//激光的最大距离
    public float FOV = 30.0f;//视场的大小
    public float Offset = 0.1f;//偏移
    public float Normal = 30f;//标准的大小
    public float LapTime = 0;//当前仿真的时间
    public float LastLapTime = 0;//上一次仿真的时间
    public GameObject LidarLaserObject;
   static public laser_base Lidar;

	// Use this for initialization
	void Start () {

        //LidarLaserObject = GameObject.Find("LidarLaser1/main");
 //===============================================================================
        float TotalAngle = FOV / 2;
        float Angle = FOV / (1/ 2);  
        Offset = (Offset / 100) / 2; // Convert offset to centimeters.
        Lidar = new laser_base(LidarLaserObject, TotalAngle + Normal, RayDistance, Offset,1);
//===============================================================================================
    }
	// Update is called once per frame
	void Update () {
       // Lidar.DebugDrawRay();
	}

    void FixedUpdate()
    {
//============================================================================================================================
        // Check if number of steps is greater than possible calculations by unity.
        float numberOfStepsNeededInOneLap = 360 / Mathf.Abs(RotationAnglePerStep);
        float numberOfStepsPossible = 1 / Time.fixedDeltaTime / 5;
        float precalculateIterations = 1;
        // Check if we need to precalculate steps.
        if (numberOfStepsNeededInOneLap > numberOfStepsPossible)
        {
            precalculateIterations = (int)(numberOfStepsNeededInOneLap / numberOfStepsPossible);
            Debug.Log("precalculateIterations" + precalculateIterations);
            if (360 % precalculateIterations != 0)
            {
                precalculateIterations += 360 % precalculateIterations;
            }
        }
        // Check if it is time to step. Example: 2hz = 2 rotations in a second.
        if (Time.fixedTime - LastUpdate > (1 / (numberOfStepsNeededInOneLap) / RotationSpeedHz) * precalculateIterations)
        {
            // Update current execution time.
            LastUpdate = Time.fixedTime;
           
            for (int i = 0; i < precalculateIterations; i++)
            {
                // Perform rotation.
                LidarLaserObject.transform.Rotate(0, 0, RotationAnglePerStep);
                HorizontalAngle += RotationAnglePerStep; // Keep track of our current rotation.
                if (HorizontalAngle >= 360)
                {
                    HorizontalAngle -= 360;
                    //GameObject.Find("RotSpeedText").GetComponent<Text>().text =  "" + (1/(Time.fixedTime - lastLapTime));
                    LastLapTime = Time.fixedTime;

                }
               
                // Execute lasers.
                    RaycastHit hit = Lidar.ShootRay();
                    float distance = hit.distance;
                    if (distance != 0) // Didn't hit anything, don't add to list.
                    {
                    float verticalAngle = Lidar.GetVerticalAngle();
                    // hits.AddLast(new SphericalCoordinate(distance, verticalAngle, horizontalAngle, hit.point, laser.GetLaserId()));
                    Debug.Log("verticalAngle" + verticalAngle);
                    Debug.Log("distance" + distance);
                    Debug.Log("horizontalAngle" + HorizontalAngle);

                } 
               
            }
            }
//=================================================================================================================================




    }


}
