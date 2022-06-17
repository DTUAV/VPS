using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.RFID
{
    public enum data_type
    {
        str,
        num,
        ref_position,
        self_position
    }
    public class RFID_Tag : MonoBehaviour
    {
        [Header("RFID的编号")] 
        public int id;
        [Header("RFID的标签对象")] 
        public GameObject obj;
        [Header("RFID的探测标志位")] 
        public bool isRange = false;

        [Header("RFID的探测范围")] 
        public double range;
        [Header("发送的信息类型及信息")] 
        [Header("字符")] 
        public bool str = false;

        public string data_str;

        [Header("数字")] 
        public bool num = false;

        public double data_num;

        [Header("位置")] 
        [Header("指定位置")] 
        public bool ref_position;

        public Vector3 data_ref_position;

        [Header("传感器自身的位置")] 
        public bool seft_position;
        
        [HideInInspector]
        public data_type dataType;
        [HideInInspector]
        public string data_send_str;

        [HideInInspector] 
        public double data_send_num;

        [HideInInspector] 
        public Vector3 data_send_ref_position;

        [HideInInspector] 
        public Vector3 data_send_self_position;
        
        // Start is called before the first frame update
        void Start()
        {
            if (obj == null)
            {
                Debug.LogError("未配置RFID标签的对象");
            }
            else
            {

                if (str)
                {
                    dataType = data_type.str;
                    data_send_str = data_str;
                }
                else if (num)
                {
                    dataType = data_type.num;
                    data_send_num = data_num;
                }
                else if (ref_position)
                {
                    dataType = data_type.ref_position;
                    data_send_ref_position = data_ref_position;
                }
                else if (seft_position)
                {
                    dataType = data_type.self_position;
                    data_send_self_position = obj.transform.position;
                }
            }
        }

        void RFID_Tag_Work()
        {
            if (isRange)
            {
                if (dataType == data_type.str)
                {
                    data_send_str = data_str;
                }
                else if(dataType == data_type.num)
                {
                    data_send_num = data_num;
                }
                else if (dataType == data_type.ref_position)
                {
                    data_send_ref_position = data_ref_position;
                }
                else if (dataType == data_type.self_position)
                {
                    data_send_self_position = obj.transform.position;
                }
            }
            else
            {
                data_send_str = string.Empty;
                data_send_num = 0;
                data_send_ref_position = Vector3.zero;
                data_send_self_position = Vector3.zero;
            }
        }
        // Update is called once per frame
        void Update()
        {
            RFID_Tag_Work();
        }
    }
}