using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Uav_Ref_yaw : MonoBehaviour
{
    public UavControl uavControl;
    public float ref_yaw;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        uavControl.ref_yaw = ref_yaw;
        
    }
}
