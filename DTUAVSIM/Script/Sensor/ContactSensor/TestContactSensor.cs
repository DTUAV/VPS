using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimUnity.Sensor;

public class TestContactSensor : MonoBehaviour
{
    public ContactSensor contactSensor;
    public float stepMoveInit = 0.01f;
    public float stepMove = 0.01f;
    private Vector3 position = new Vector3(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = position;
        Debug.Log("getContractDir"+contactSensor.getContactSensorData().getContractDir_x());
        Debug.Log("getContractWorldPosition"+contactSensor.getContactSensorData().getContactWorldPoint_x());
        Debug.Log("getContractName"+contactSensor.getContactSensorData().getContactName());
    }

    void OnGUI()
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            position.x -= stepMove;
            stepMove += 0.0001f;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            stepMove = stepMoveInit;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            position.x += stepMove;
            stepMove += 0.0001f;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            stepMove = stepMoveInit;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            position.z -= stepMove;
            stepMove += 0.0001f;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            stepMove = stepMoveInit;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            position.z += stepMove;
            stepMove += 0.0001f;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            stepMove = stepMoveInit;
        }

        if (Input.GetKey(KeyCode.D))
        {
            position.y -= stepMove;
            stepMove += 0.0001f;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            stepMove = stepMoveInit;
        }
        if (Input.GetKey(KeyCode.U))
        {
            position.y += stepMove;
            stepMove += 0.0001f;
        }
        if (Input.GetKeyUp(KeyCode.U))
        {
            stepMove = stepMoveInit;
        }



    }




}

