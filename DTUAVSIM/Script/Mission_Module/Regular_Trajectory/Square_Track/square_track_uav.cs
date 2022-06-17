﻿/*
 * Author: Yuanlin Yang
 * Date: 2022-1-3
 * Location: Guangdong University of Technology
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVCARS.DTMission;
using DTUAVCARS.DTPlanning;
using DTUAVCARS.TF;

public class square_track_uav : MonoBehaviour
{
    public velocity_command VelocityCommand;
    public Vector3 TrackCenter;   //The Center Position of Square Track
    public float Width;     //The Width of Square ---Y
    public float Lendth;   //The Lendth of Square ---X
    public float Heigh;   //The Heigh of Square ---Z
    public bool IsLoop;   //If loop the Square Track
    public float PositionError;//The Position Change Error
    public int PlaneId;//The Motion Plane:0:X-Y,1:X-Z.2:Y-Z,other:X-Y
    private square_track _squareTrack;
    private bool _isConfigure;
    // Start is called before the first frame update
    void Start()
    {
        _squareTrack = new square_track(TrackCenter,Width,Lendth,Heigh,IsLoop,PositionError,PlaneId);
        if (!_squareTrack.GetIsGetNewPose())
        {
            _isConfigure = false;
            Debug.Log("The Square Track Configure Error");
        }
        else
        {
            _isConfigure = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
       VelocityCommand.TargetPositionRos = _squareTrack.GetTargetPosition(TF.Unity2Ros(VelocityCommand.ObjectRb.position));
    }
}
