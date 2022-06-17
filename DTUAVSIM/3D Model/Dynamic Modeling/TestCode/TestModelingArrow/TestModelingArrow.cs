using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace DTUAVCARS.Modeling.TreeDimensionObject
{
    public class TestModelingArrow : MonoBehaviour
    {
        public float ra = 0.3f;//The Radius of Cylinder
        public float ha = 3;//The Heigh of Cylinder
        public float rb = 1;//The Radius of Cone
        public float hb = 2;//The Heigh of Cone
        public int n = 12;//The Number of Regular Polygon

        // Start is called before the first frame update
        void Start()
        {
            Vector3[] vertices = new Vector3[3 * n + 1];
            int[] triangle = new int[18 * n - 12];
            for (int i = 0; i < n; i++)
            {
                // Բ���ĵ��涥��
                vertices[i] = new Vector3((float)(ra * Mathf.Cos(2 * Mathf.PI * i / n)), (float)(ra * Mathf.Sin(2 * Mathf.PI * i / n)), 0);
                // Բ���Ķ��涥��
                vertices[n + i] = new Vector3((float)(ra * Mathf.Cos(2 * Mathf.PI * i / n)), (float)(ra * Mathf.Sin(2 * Mathf.PI * i / n)), ha);
                // Բ׶�ĵ��涥��
                vertices[2 * n + i] = new Vector3((float)(rb * Mathf.Cos(2 * Mathf.PI * i / n)), (float)(rb * Mathf.Sin(2 * Mathf.PI * i / n)), ha);
            }
            // Բ׶�Ķ���
            vertices[3 * n] = new Vector3(0, 0, ha + hb);
            // ��������������Ϣ
            for (int i = 0; i < n - 2; i++)
            {
                triangle[3 * i] = 0;
                triangle[3 * i + 2] = i + 1;
                triangle[3 * i + 1] = i + 2;
                triangle[3 * (n - 2) + 6 * n + 3 * i] = 2 * n;
                triangle[3 * (n - 2) + 6 * n + 3 * i + 2] = 2 * n + i + 1;
                triangle[3 * (n - 2) + 6 * n + 3 * i + 1] = 2 * n + i + 2;
            }
            for (int i = 0; i < n - 1; i++)
            {
                triangle[3 * (n - 2) + 6 * i] = i;
                triangle[3 * (n - 2) + 6 * i + 2] = n + i;
                triangle[3 * (n - 2) + 6 * i + 1] = n + i + 1;
                triangle[3 * (n - 2) + 6 * i + 3] = n + i + 1;
                triangle[3 * (n - 2) + 6 * i + 5] = i + 1;
                triangle[3 * (n - 2) + 6 * i + 4] = i;
                triangle[3 * (n - 2) + 6 * n + 3 * (n - 2) + 3 * i] = 2 * n + i;
                triangle[3 * (n - 2) + 6 * n + 3 * (n - 2) + 3 * i + 2] = 3 * n;
                triangle[3 * (n - 2) + 6 * n + 3 * (n - 2) + 3 * i + 1] = 2 * n + i + 1;
            }
            triangle[3 * (n - 2) + 6 * (n - 1)] = n - 1;
            triangle[3 * (n - 2) + 6 * (n - 1) + 2] = n + n - 1;
            triangle[3 * (n - 2) + 6 * (n - 1) + 1] = n;
            triangle[3 * (n - 2) + 6 * (n - 1) + 3] = n;
            triangle[3 * (n - 2) + 6 * (n - 1) + 5] = 0;
            triangle[3 * (n - 2) + 6 * (n - 1) + 4] = n - 1;
            triangle[3 * (n - 2) + 6 * n + 3 * (n - 2) + 3 * (n - 1)] = 2 * n + n - 1;
            triangle[3 * (n - 2) + 6 * n + 3 * (n - 2) + 3 * (n - 1) + 2] = 3 * n;
            triangle[3 * (n - 2) + 6 * n + 3 * (n - 2) + 3 * (n - 1) + 1] = 2 * n;
            // ��ȡ���������
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            // ���ԭ������
            mesh.Clear();
            // ���������µĶ���
            mesh.vertices = vertices;
            // ���������µ�������Ϣ 
            mesh.triangles = triangle;
            // �����ؼ��㷨��
            mesh.RecalculateNormals();
            GameObject[] gameobj = new GameObject[1];
            gameobj[0] = GameObject.Find("de");
            FBXExporter.ExportFBX("", "Arrow",gameobj, true);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
