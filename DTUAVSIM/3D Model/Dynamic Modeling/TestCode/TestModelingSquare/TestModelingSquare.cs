using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DTUAVCARS.Modeling.TreeDimensionObject
{
    public class TestModelingSquare : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Vector3[] vertices = new Vector3[4];
            vertices[0] = new Vector3(0, 0, 0);
            vertices[1] = new Vector3(0, 0, 1);
            vertices[2] = new Vector3(0, 1, 0);
            vertices[3] = new Vector3(0, 1, 1);
            int[] triangle = new int[6] { 0, 2, 1, 1, 2, 3 };
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangle;
            mesh.RecalculateNormals();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
