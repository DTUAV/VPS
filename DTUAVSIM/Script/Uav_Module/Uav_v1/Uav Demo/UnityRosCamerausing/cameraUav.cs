using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraUav : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject uav;
    private Vector3 offset;
    private Rigidbody rb;
    void Start()
    {   rb = uav.GetComponent<Rigidbody>();
        offset = transform.position - rb.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = rb.position + offset;
        transform.eulerAngles = new Vector3(0, rb.rotation.eulerAngles.y, 0);
    }
}
