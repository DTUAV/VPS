using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_velocity : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody CarTransform;
    public float CarVelocity;
    public float CarYaw;
    public Transform WheelForwardLeft;
    public Transform WheelForwardRight;
    public Transform WheelBackLeft;
    public Transform WheelBackRight;
    public PalyerMovement PlayMovementNode;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       CarTransform.velocity = new Vector3(0,0, CarVelocity);
       
       WheelForwardLeft.Rotate(PlayMovementNode.agent.speed, 0,0);
       WheelForwardRight.Rotate(PlayMovementNode.agent.speed, 0,0);
       WheelBackLeft.Rotate(PlayMovementNode.agent.speed, 0,0);
       WheelBackRight.Rotate(PlayMovementNode.agent.speed, 0,0);
    }
}
