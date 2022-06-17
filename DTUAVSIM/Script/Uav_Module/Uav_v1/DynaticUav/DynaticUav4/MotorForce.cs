using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorForce : MonoBehaviour
{
    public float MaxThrust = .5f;
    public float Thrust = 0f;

    private Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
   public void UpdateForce(float percent)
    {
        Thrust = Mathf.Clamp01(percent) * MaxThrust;
        rigid.AddForce(transform.up * (Thrust * Physics.gravity.magnitude));
        rigid.AddForce(-transform.forward * (Thrust * Physics.gravity.magnitude));
    }

    


}
