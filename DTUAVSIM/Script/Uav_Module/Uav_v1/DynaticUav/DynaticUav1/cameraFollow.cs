using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour {

    public GameObject target;//相机跟踪的对象
    public float smoothing = 5f;
    Vector3 offset = Vector3.zero;

	// Use this for initialization
	void Start () {
        offset = transform.position - target.GetComponent<Rigidbody>().position;
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 targetCamPos = target.GetComponent<Rigidbody>().position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, 2);
    }
    void FixedUpdate()
    {
       
    }
}
