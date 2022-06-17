using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Force : MonoBehaviour
{
    public float upforce = 0;
    public Transform motor;
    public GameObject UavBatch;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            Rigidbody rb = UavBatch.GetComponent<Rigidbody>();
            // rb.AddForceAtPosition(UavBatch.transform.up * upforce, motor.position, ForceMode.Impulse);
            // rb.AddRelativeForce(UavBatch.transform.up * upforce, ForceMode.Impulse);
            rb.AddForce(motor.transform.up * upforce, ForceMode.Impulse);
            Debug.Log(UavBatch.transform.up);
        }
    }

    void OnGUI()
    {

        
    }
}
