using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepStead : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject obj;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // transform.eulerAngles = Vector3.zero;
        
    }
    void FixedUpdate()
    {
        float yaw = obj.transform.localEulerAngles.y;
        obj.transform.localEulerAngles = new Vector3(0, yaw, 0);
    }
}
