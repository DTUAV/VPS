
[System.Serializable]
public class PDControl 
{
    public float Pfactor;//积分系数
    public float Dfactor;//微分系数

    public float lastError;//上一次的误差

    public PDControl(float pFactor, float dFactor)
    {
        this.Dfactor = dFactor;
        this.Pfactor = pFactor;
    }


    public float UpdatePD(float refValue, float feeBackValue, float timeFrame)
    {
        float presentError = refValue - feeBackValue;
        float deriv = (presentError - lastError) / timeFrame;
        lastError = presentError;
        float finalPID = presentError * Pfactor  + deriv * Dfactor;
        //设置PID的精度
       /* if ((finalPID > -0.1) && (finalPID < 0.1))
        {
            finalPID = 0;
        }*/

        return finalPID;


    }
   
}
