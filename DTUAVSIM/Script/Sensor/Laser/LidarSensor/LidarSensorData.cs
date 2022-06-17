using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarSensorData  {

    private Vector3 position;
    private float distance;
    private float verticalAngle;
    private float horizontalAngle;
    private Vector3 hitPoint;
    private int LaserId;

    public LidarSensorData( Vector3 position,float distance,float verticalAngle,float horizontalAngle,Vector3 hitPoint,int LaserId)
    {
        this.position = position;
        this.distance = distance;
        this.verticalAngle = verticalAngle;
        this.horizontalAngle = horizontalAngle;
        this.hitPoint = hitPoint;
        this.LaserId = LaserId;
    }
    //hits.AddLast(new SphericalCoordinate(distance, verticalAngle, horizontalAngle, hit.point, laser.GetLaserId()));

    public Vector3 GetPosition()
    {
        return position;
    }

    public float GetDistance()
    {
        return distance;
    }

    public float GetVerticalAngle()
    {
        return verticalAngle;
    }

    public float GetHorizontalAngle()
    {
        return horizontalAngle;
    }

    public Vector3 GetHitPoint()
    {
        return hitPoint;
    }

    public int GetLaserId()
    {
        return LaserId;
    }



}
