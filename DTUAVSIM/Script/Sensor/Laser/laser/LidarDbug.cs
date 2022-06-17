using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarDbug : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        LidarLaser.Lidar.DebugDrawRay();
	}
}
