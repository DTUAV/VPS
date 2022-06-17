using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAVCARS.DTCamera
{
    public class CameraSwitch : MonoBehaviour
    {
        public List<Transform> Targets;
        public float distanceUp = 15f;
        public float distanceAway = 10f;
        public float smooth = 2f; //λ��ƽ���ƶ�ֵ

        public float camDepthSmooth = 5f;

        private int index;

        void Start()
        {
            index = 0;
        }
        // Update is called once per frame
        void Update()
        {
            // �������������Զ��
            if ((Input.mouseScrollDelta.y < 0 && Camera.main.fieldOfView >= 3) ||
                Input.mouseScrollDelta.y > 0 && Camera.main.fieldOfView <= 80)
            {
                Camera.main.fieldOfView += Input.mouseScrollDelta.y * camDepthSmooth * Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                if (index<Targets.Count-1)
                {
                    index++;
                }
                else
                {
                    index = 0;
                }
            }

        }

        void LateUpdate()
        {
            //�����λ��
            Vector3 disPos = Targets[index].position + Vector3.up * distanceUp - Targets[index].forward * distanceAway;
            transform.position = Vector3.Lerp(transform.position, disPos, Time.deltaTime * smooth);
            //����ĽǶ�
            transform.LookAt(Targets[index].position);
        }
    }
}
