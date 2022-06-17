using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Sensor;
using SimUnity.Noise;

public class testAltimeterSensor : MonoBehaviour
{
    public AltimeterSensor altimeterSensor;
    public GameObject controlObject;
    public bool auto_run = true;
    public float stepMoveInit = 0.01f;
    public float stepMove = 0.01f;
    private Vector3 position = new Vector3(0, 0, 0);

    private float maxY = 50f;
    private float mixY = 1f;
    private int direction = 1;
    private float moveSpeed = 3;
    private int moveRightSpeedX = 3;

    // Start is called before the first frame update
    void Start()
    {
        if (controlObject != null)
        {
            if (controlObject.GetComponent<Rigidbody>() != null)
            {
                altimeterSensor.AltimeterSensorInit(controlObject.GetComponent<Rigidbody>(), true, NoiseType.Gaussian);
               
            }
            else
            {
                altimeterSensor.AltimeterSensorInit(controlObject.GetComponent<Transform>(), true, NoiseType.Gaussian);
               
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (auto_run == false)
        {
            controlObject.transform.position = position;
        }
        else
        {
            //controlObject.GetComponent<Rigidbody>().velocity = new Vector3(moveRightSpeedX,0,moveRightSpeedX);
            Vector3 tmpVec = controlObject.transform.position;
            tmpVec.y += direction * moveSpeed * Time.fixedDeltaTime;
            controlObject.transform.position = tmpVec;
            moveSpeed = moveSpeed + 0.1f;

            if (controlObject.transform.position.y > maxY)
            {
                direction = -1;
                moveSpeed = 1;
            }
            else if (controlObject.transform.position.y < mixY)
            {
                direction = 1;
                moveSpeed = 1;
            }

        }
        Debug.Log("Vertical_position"+altimeterSensor.getVertical_position());
        Debug.Log("Vertical_velocity" + altimeterSensor.getVertical_velocity());

    }
    void FixedUpdate()
    {
       
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
