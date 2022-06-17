using System.Collections;
using System.Collections.Generic;
using DTUAVCARS.TF;
using UnityEngine;
using DTUAVCARS.Math.DataNoise;
public class UavMotion : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 RefLocalPoseRos;
    public Vector3 RefRotationRos;
    public float VelocityMove;
    public float RotateSpeed;
    public GameObject UavObj;
    private Vector3 InitPoseUnity;
    private Vector3 InitRotationUnity;

    void Start()
    {
        InitPoseUnity = new Vector3();
        InitPoseUnity = UavObj.transform.position;
        InitRotationUnity = UavObj.transform.eulerAngles;
    }

    [System.Obsolete]
    void FixedUpdate()
    {
        float stepMove = VelocityMove * Time.deltaTime;
        float stepRotate = RotateSpeed * Time.deltaTime;
        UavObj.transform.position = Vector3.MoveTowards(gameObject.transform.position,TF.Ros2Unity(RefLocalPoseRos)+InitPoseUnity, stepMove) + new Vector3((float)DataNoise.GaussNiose1()/1000.0f,(float)DataNoise.GaussNiose1()/1000.0f,(float)DataNoise.GaussNiose1()/1000.0f);
        Quaternion targetRotationUnity = Quaternion.Euler(TF.Ros2Unity(RefRotationRos)+InitRotationUnity + new Vector3((float)DataNoise.GaussNiose1() / 1000.0f, (float)DataNoise.GaussNiose1() / 1000.0f, (float)DataNoise.GaussNiose1() / 1000.0f));
        UavObj.transform.rotation = Quaternion.Slerp(UavObj.transform.rotation,targetRotationUnity,stepRotate);
    }

}
