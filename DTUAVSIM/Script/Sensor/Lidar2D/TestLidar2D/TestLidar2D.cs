using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLidar2D : MonoBehaviour
{
   // public Vector3 targetPosition;

    public LayerMask Mask;
    public float CheckRange = 10.0f;
    public Vector3 coutPosition;
    public float theta = 360;
    private List<Vector3> laser_targetposition;
    private List<Vector3> laser_decs;
    public float laser_inc = 5.0f;
    public int laser_count;
    private int laser_id = 0;
    // Start is called before the first frame update
    void Start()
    {
        laser_count = ((int)(theta / laser_inc));
        laser_targetposition = new List<Vector3>(laser_count);
        laser_decs = new List<Vector3>(laser_count);
        Vector3 dec = Vector3.zero;
        for(int i=0;i<theta;i=i+(int)laser_inc)
        {
            laser_targetposition.Add( new  Vector3(transform.position.x + Mathf.Sin(i * laser_inc * (Mathf.PI / 180)), transform.position.y, transform.position.z+ Mathf.Cos(i * laser_inc * (Mathf.PI / 180))));
            dec.x = Mathf.Sin(i * laser_inc * (Mathf.PI / 180));
            dec.z = Mathf.Cos(i * laser_inc * (Mathf.PI / 180));
            laser_decs.Add( dec);
        }
    }



    // Update is called once per frame
    void Update()
    {
        int i = laser_id;//
        if (Input.GetKeyDown(KeyCode.Space))//
        {//
            laser_id++;//
            if (laser_id == laser_targetposition.Count)//
            {//
                laser_id = 0;//
            }//
        }//
            //  for(int i=0;i<laser_targetposition.Count;i++)
            {
                Vector3 dir = (laser_targetposition[i] - transform.position).normalized;
                //  Ray ray = new Ray(transform.position, dir);
                Ray ray = new Ray(transform.position, laser_decs[i].normalized);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, CheckRange, Mask))
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);
                }
                else
                {
                    Vector3 targetRayPosition = new Vector3(transform.position.x + CheckRange * ray.direction.x, transform.position.y + CheckRange * ray.direction.y, transform.position.z + CheckRange * ray.direction.z);
                    coutPosition = targetRayPosition;
                    Debug.DrawLine(transform.position, targetRayPosition, Color.blue);
                }
            }
      
        
    }
}
