using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public class Motor : MonoBehaviour
    {
        public float UpForce = 0.0f; // Total force to be applied by this motor.  This may be transfered to the parent RigidBody
        public float SideForce = 0.0f; // Torque or side force applied by this motor.  This may be transfered to the parent RigidBody and get computed with others motors
        public float Power = 2; // A power multiplier.  An easy way to create more potent motors
        public float ExceedForce = 0.0f; // Negative force value when Upforce gets below 0

        public float YawFactor = 0.0f; // A factor to be applied to the side force.  Higher values get a faster Yaw movement
        public bool InvertDirection; // Whether the direction of the motor is counter or counterclockwise
        public float PitchFactor = 0.0f; // A factor to be applied to the pitch correction
        public float RollFactor = 0.0f; // A factor to be applied to the roll correction

        public float Mass = 0.0f;

        public BaseControl mainController; // Parent main controller.  Where usualy may be found the RigidBody
        public GameObject Propeller; // The propeller object.  Annimation will be done here.
        private float SpeedPropeller = 0;

        // Method called by BasicControl class to calculate force value of this specific motor.  The force application itself will be done at BasicControl class
        public void UpdateForceValues()
        {
            float UpForceThrottle = 0;

            UpForceThrottle = Mathf.Clamp(mainController.ThrottleValue, 0, 1) * Power;
            float UpForceTotal = UpForceThrottle;


            UpForceTotal -= mainController.Computer.PitchCorrection * PitchFactor;
            UpForceTotal -= mainController.Computer.RollCorrection * RollFactor;

            UpForce = UpForceTotal;

            SideForce = PreNormalize(mainController.Controller.Yaw, YawFactor);

            SpeedPropeller = Mathf.Lerp(SpeedPropeller, UpForce * 15500.0f, Time.deltaTime);
            UpdatePropeller(SpeedPropeller);
        }

        public void UpdatePropeller(float speed)
        {
            Propeller.transform.Rotate(0.0f, SpeedPropeller * 2 * Time.deltaTime, 0.0f);
        }

        // Method to apply the factor and clamp the torque to its limit
        float PreNormalize(float input, float factor)
        {
            float finalValue = input;

            if (InvertDirection)
                finalValue = Mathf.Clamp(finalValue, -1, 0);
            else
                finalValue = Mathf.Clamp(finalValue, 0, 1);

            return finalValue * (YawFactor);
        }



        //public float UpForce = 0.0f;//电机旋转产生向上的力
        //public float SideForce = 0.0f;//电机旋转的力矩
        //public float Power = 2;//能力
        //public float ExceedFore = 0.0f;//当电机向上的力小于零的差值
        //public float YawFactor = 0.0f;//偏航角参数
        //public float PitchFactor = 0.0f;//俯仰角参数
        //public float RollFactor = 0.0f;//滚转角参数
        //public bool InvertDirection;//电机旋转的方向
        //public float Mass = 0.0f;//电机的重量
        //public BaseControl mainController;
        //public GameObject propeller;
        //private float SpeedPropeller = 0;

        //public void  UpdateForceValues()
        //{
        //    float UpForceThrottle = Mathf.Clamp(mainController.ThrottleValue, 0, 1);
        //    float UpForceTotal = UpForceThrottle;
        //    UpForceTotal -= mainController.Computer.PitchCorrection * PitchFactor;
        //    UpForceTotal -= mainController.Computer.RollCorrection * RollFactor;
        //    UpForce = UpForceTotal;
        //    SideForce = PreNormalize(mainController.Controller.Yaw, YawFactor);
        //    SpeedPropeller = Mathf.Lerp(SpeedPropeller, UpForce * 2500.0f,Time.deltaTime);
        //    UpdatePropeller(SpeedPropeller);
        //}

        //public void UpdatePropeller(float speed)
        //{
        //    propeller.transform.Rotate(0.0f, SpeedPropeller * 2 * Time.deltaTime, 0.0f);
        //}





        //private float PreNormalize(float input,float factor)
        //{
        //    float finalValue = input;
        //    if(InvertDirection)
        //    {
        //        finalValue = Mathf.Clamp(finalValue, -1, 0);
        //    }
        //    else
        //    {
        //        finalValue = Mathf.Clamp(finalValue, 0, 1);
        //    }
        //    return finalValue * (YawFactor);
        //}









        //// Start is called before the first frame update
        //void Start()
        //{

        //}

        //// Update is called once per frame
        // void Update()
        // {
        //    Propeller.transform.Rotate(0.0f, 1000 * 2 * Time.deltaTime, 0.0f);
        //  }
    }
