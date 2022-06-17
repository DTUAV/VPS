using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.droneV2;
using SimUnity.droneV3;
public class BaseControl : MonoBehaviour
{
    [Header("Control")]
    public Controller Controller;
    public float ThrottleIncrease;

    [Header("Motors")]
    public Motor[] Motors;
    public float ThrottleValue;

    [Header("Internal")]
    public ComputerModule Computer;

    void FixedUpdate()
    {
        if(ControlModel.ComModel == 1)
        {
            ThrottleIncrease = AutoMission.throttleValue;
        }

       
            Computer.UpdateComputer(Controller.Pitch, Controller.Roll, Controller.Throttle * ThrottleIncrease);
        
        ThrottleValue = Computer.HeightCorrection;
        ComputeMotors();
        if (Computer != null)
        {
            Computer.UpdateGyro();
        }

        ComputeMotorSpeeds();

        //Debug.Log("dddddddddddddddd");
    }

   void ComputeMotors()
    {
        float yaw = 0.0f;
        Rigidbody rb = GetComponent<Rigidbody>();
        int i = 0;
        foreach (Motor motor in Motors)
        {
            motor.UpdateForceValues();
            yaw += motor.SideForce;
            i++;
            Transform t = motor.GetComponent<Transform>();
            rb.AddForceAtPosition(transform.up * motor.UpForce, t.position, ForceMode.Impulse);
            Debug.Log(motor.UpForce);
           
        }
        rb.AddTorque(Vector3.up * yaw, ForceMode.Force);

    }

  void ComputeMotorSpeeds()
    {
        foreach (Motor motor in Motors)
        {
            if (Computer.Gyro.Altitude < 0.1)
                motor.UpdatePropeller(0.0f);
            else
                motor.UpdatePropeller(5200.0f);
        }
    }











    //[Header("Control")]
    //public Controller Controller;
    //public float ThrottleIncrease;

    //[Header("Motors")]
    //public Motor[] Motors;
    //public float ThrottleValue;

    //[Header("Internal")]
    //public ComputerModule Computer;


    //void FixedUpdate()
    //{
    //    Computer.UpdateComputer(Controller.Pitch, Controller.Roll, Controller.Throttle * ThrottleIncrease);
    //    ThrottleValue = Computer.HeightCorrection;
    //    ComputeMotors();
    //    if (Computer != null)
    //        Computer.UpdateGyro();
    //    ComputeMotorSpeeds();
    //}

    //private void ComputeMotors()
    //{
    //    float yaw = 0.0f;
    //    Rigidbody rb = GetComponent<Rigidbody>();
    //    int i = 0;
    //    foreach (Motor motor in Motors)
    //    {
    //        motor.UpdateForceValues();
    //        yaw += motor.SideForce;
    //        i++;
    //        Transform t = motor.GetComponent<Transform>();
    //        rb.AddForceAtPosition(transform.up * motor.UpForce, t.position, ForceMode.Impulse);
    //    }
    //    rb.AddTorque(Vector3.up * yaw, ForceMode.Force);
    //}

    //private void ComputeMotorSpeeds()
    //{
    //    foreach(Motor motor in Motors)
    //    {
    //        if(Computer.Gyro.Altitude<0.1)
    //        {
    //            motor.UpdatePropeller(0.0f);
    //        }
    //        else
    //        {
    //            motor.UpdatePropeller(1200.0f);
    //        }
    //    }
    //}

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
