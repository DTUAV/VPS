
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LidarMove : MonoBehaviour
{
    Rigidbody rb;
    float speed = 2f;
    float jumpforce;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        float jump = Input.GetAxis("Jump");

        Vector3 movent = new Vector3(-moveVertical, 0, moveHorizontal);
        transform.Translate(movent * speed * Time.deltaTime);
        rb.AddForce(0, jump * jumpforce, 0);
    }
}
