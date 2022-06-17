namespace SimUnity.droneV2
{

    [System.Serializable]
    public class PID
    {
        public float pFactor, iFactor, dFactor;

        float integral;
        float lastError;


        public PID(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }


        public float Update(float setpoint, float actual, float timeFrame)
        {
            float present = setpoint - actual;
            integral += present * timeFrame;
            float deriv = (present - lastError) / timeFrame;
            lastError = present;
            float finalPID = present * pFactor + integral * iFactor + deriv * dFactor;
            if ((finalPID > -0.1) && (finalPID < 0.1))
                finalPID = 0;
            return finalPID;
        }




        /* public float pFactor;//the pid of p//比例系数
         public float iFactor;//the pid of i//积分系数
         public float dFactor;//the pid of d//微分系数

         float integral;//积分的累积量
         float lastError;//上一次的误差

         //类初始化方法
         public PID (float pFactor,float iFactor,float dFactor)
         {
             this.pFactor = pFactor;
             this.iFactor = iFactor;
             this.dFactor = dFactor;
         }


         //PID 更新
         public float Update(float setPoint, float actual,float timeFrame)
         {
             *//*
              setPoint:设置的目标值
              actual：当前实际值
              timeFrame：当前每一帧的时间
              *//*
             float presentError = setPoint - actual;
             integral += presentError * timeFrame;
             float deriv = (presentError - lastError) / timeFrame;
             lastError = presentError;
             float finalPID = presentError * pFactor + integral * iFactor + deriv * dFactor;
             //设置PID的精度
             if((finalPID > -0.1)&&(finalPID < 0.1))
             {
                 finalPID = 0;
             }

             return finalPID;


         }
     */



    }

}