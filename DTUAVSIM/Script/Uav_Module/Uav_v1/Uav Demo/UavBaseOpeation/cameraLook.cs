using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraLook : MonoBehaviour
{
    [Header("无人机对象")]
    public GameObject lookAtUav;
    


    // Start is called before the first frame update
    void Start()
    {
        if(lookAtUav == null)
        {
            Debug.LogError("未指定无人机对象");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(lookAtUav.transform);
    }
}
