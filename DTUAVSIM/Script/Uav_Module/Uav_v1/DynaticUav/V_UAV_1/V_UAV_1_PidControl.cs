using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V_UAV_1
{
    public class V_UAV_1_PidControl
    {
        public float pFactor;//the pid of p//比例系数
        public float iFactor;//the pid of i//积分系数
        public float dFactor;//the pid of d//微分系数

        public float integral;//积分的累积量
        public float lastError;//上一次的误差

        //类初始化方法
        public V_UAV_1_PidControl(float pFactor, float iFactor, float dFactor)
        {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }


        //PID 更新
        public float Update(float setPoint, float actual, float timeFrame)
        {

            // setPoint: 设置的目标值
            // actual：当前实际值
            // timeFrame：当前每一帧的时间


            float presentError = setPoint - actual;
            integral += presentError * timeFrame;
            float deriv = (presentError - lastError) / timeFrame;
            lastError = presentError;
            float finalPID = presentError * pFactor + integral * iFactor + deriv * dFactor;
            //设置PID的精度
            if ((finalPID > -0.1) && (finalPID < 0.1))
            {
                finalPID = 0;
            }

            return finalPID;


        }
    }
}
