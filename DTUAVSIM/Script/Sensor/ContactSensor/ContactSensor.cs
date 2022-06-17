using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimUnity.Noise;
using SimUnity.Sensor;

namespace SimUnity.Sensor
{
    public class ContactSensor : MonoBehaviour
    {
        private List<ContactSensorData> contactSensorDataAllList = new List<ContactSensorData>();
        private ContactSensorData contactSensorDataCul = new ContactSensorData();
        private double Mean;
        private double Dev;
        void Start()
        {
            Mean = 0;
            Dev = 0.01;
            if (GetComponents<Rigidbody>() == null)
            {
                Debug.LogError("The ContactSensor need Rigidbody");
            }
            else
            {
                Debug.Log("Load ContactSensor Sucessfully");
            }
           
        }


        void Update()
        {

        }
        // 开始接触
        void OnTriggerEnter(Collider collider)
        {
            double colliderPosition_x = collider.gameObject.transform.position.x;
            double colliderPosition_y = collider.gameObject.transform.position.y;
            double colliderPosition_z = collider.gameObject.transform.position.z;
            string colliderObjectName = collider.gameObject.name;
            contactSensorDataCul.setContactName(colliderObjectName);
            colliderPosition_x = colliderPosition_x + (float)GaussNoisPlugin.GaussianNoiseData(Mean, Dev);
            colliderPosition_y = colliderPosition_y + (float)GaussNoisPlugin.GaussianNoiseData(Mean, Dev);
            colliderPosition_z = colliderPosition_z + (float)GaussNoisPlugin.GaussianNoiseData(Mean, Dev);
            contactSensorDataCul.setContactWorldPoint_x(colliderPosition_x);
            contactSensorDataCul.setContactWorldPoint_y(colliderPosition_y);
            contactSensorDataCul.setContactWorldPoint_z(colliderPosition_z);

            contactSensorDataCul.setContractDir_x((transform.position.x- colliderPosition_x)/Vector3.Distance(transform.position,new Vector3((float)colliderPosition_x, (float)colliderPosition_y, (float)colliderPosition_z)));
            contactSensorDataCul.setContractDir_y((transform.position.y - colliderPosition_y) / Vector3.Distance(transform.position, new Vector3((float)colliderPosition_x, (float)colliderPosition_y, (float)colliderPosition_z)));
            contactSensorDataCul.setContractDir_z((transform.position.z - colliderPosition_z) / Vector3.Distance(transform.position, new Vector3((float)colliderPosition_x, (float)colliderPosition_y, (float)colliderPosition_z)));

        }

        // 接触结束
        void OnTriggerExit(Collider collider)
        {
            contactSensorDataCul.setContactName("null");
            contactSensorDataCul.setContactWorldPoint_x(0);
            contactSensorDataCul.setContactWorldPoint_y(0);
            contactSensorDataCul.setContactWorldPoint_z(0);
            contactSensorDataCul.setContractDir_x(0);
            contactSensorDataCul.setContractDir_y(0);
            contactSensorDataCul.setContractDir_z(0);
        }

        // 接触持续中
        void OnTriggerStay(Collider collider)
        {
            double colliderPosition_x = collider.gameObject.transform.position.x;
            double colliderPosition_y = collider.gameObject.transform.position.y;
            double colliderPosition_z = collider.gameObject.transform.position.z;
            string colliderObjectName = collider.gameObject.name;
            contactSensorDataCul.setContactName(colliderObjectName);
            colliderPosition_x = colliderPosition_x + (float)GaussNoisPlugin.GaussianNoiseData(Mean, Dev);
            colliderPosition_y = colliderPosition_y + (float)GaussNoisPlugin.GaussianNoiseData(Mean, Dev);
            colliderPosition_z = colliderPosition_z + (float)GaussNoisPlugin.GaussianNoiseData(Mean, Dev);
            contactSensorDataCul.setContactWorldPoint_x(colliderPosition_x);
            contactSensorDataCul.setContactWorldPoint_y(colliderPosition_y);
            contactSensorDataCul.setContactWorldPoint_z(colliderPosition_z);

            contactSensorDataCul.setContractDir_x((transform.position.x - colliderPosition_x) / Vector3.Distance(transform.position, new Vector3((float)colliderPosition_x, (float)colliderPosition_y, (float)colliderPosition_z)));
            contactSensorDataCul.setContractDir_y((transform.position.y - colliderPosition_y) / Vector3.Distance(transform.position, new Vector3((float)colliderPosition_x, (float)colliderPosition_y, (float)colliderPosition_z)));
            contactSensorDataCul.setContractDir_z((transform.position.z - colliderPosition_z) / Vector3.Distance(transform.position, new Vector3((float)colliderPosition_x, (float)colliderPosition_y, (float)colliderPosition_z)));
        }

        public ContactSensorData getContactSensorData()
        {

            return contactSensorDataCul;

        }

    }
}