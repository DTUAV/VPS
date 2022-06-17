using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.RFID
{


    public class RFID_Reader : MonoBehaviour
    {
        // Start is called before the first frame update
        List<RFID_Data> RFID_Datas = new List<RFID_Data>();
        public List<RFID_Tag> RFID_Tags;
        public GameObject reader_obj;
        private data_type dateType;
        void Start()
        {
            
        }

        void get_RFID_data()
        {
            foreach (RFID_Tag tag in RFID_Tags)
            {
                if (Vector3.Distance(reader_obj.transform.position, tag.obj.transform.position) <= (float)tag.range)
                {
                        tag.isRange = true;
                    
                        RFID_Data data = new RFID_Data();
                        data.Tag_ID = tag.id;
                        data.dataType = tag.dataType;
                        data.str = tag.data_send_str;
                        data.ref_position = tag.data_send_ref_position;
                        data.seft_position = tag.data_send_self_position;
                        data.num = tag.data_send_num;
                        RFID_Datas.Add(data);
                        Debug.Log(data.seft_position);
                      
                }
                else
                {
                    tag.isRange = false;
                }


            }

            if (RFID_Datas.Count >= 1000)
            {
                RFID_Datas.Clear();
            }
        }
        // Update is called once per frame
        void Update()
        {
            get_RFID_data();
        }
    }
}