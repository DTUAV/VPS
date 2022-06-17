using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SimUnity.droneV3
{
    public class ControlModel : MonoBehaviour
    {
        /*
         这个脚本的作用是设置无人机的状态，通过按键实现无人控制模式的转换
        键盘a：自动控制模式
        键盘m：手动控制模式   
             */
        public static int ComModel = 0;//0：手动控制，1：自动控制

        //自动模式下的认为模式
        /*
        键盘z:获取初始化配置的任务点
        键盘x:获取GUI界面的任务点
             */

        public static int MissionModel = 0;//0：获取初始化配置的任务点，1：获取GUI界面的任务点



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.A))
            {
                ComModel = 1;
            }

            if (Input.GetKey(KeyCode.M))
            {
                ComModel = 0;
            }

            if (Input.GetKey(KeyCode.Z))
            {
                MissionModel = 0;
            }

            if (Input.GetKey(KeyCode.X))
            {
                MissionModel = 1;
            }



        }
    }
}
