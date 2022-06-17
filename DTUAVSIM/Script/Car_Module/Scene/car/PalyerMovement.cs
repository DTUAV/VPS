using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PalyerMovement : MonoBehaviour
{

    public NavMeshAgent agent;

    void Start()
    {
        //获取角色上的NavMeshAgent组件
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
       Transform tf =  agent.gameObject.transform;
       tf.position = new Vector3(tf.position.x,1.5f,tf.position.z);
        //鼠标左键
        if (Input.GetMouseButtonDown(0))
        {
            //射线检测
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool isCollider = Physics.Raycast(ray, out hit);
            if (isCollider)
            {
                //hit.point射线触碰的Position
                //SetDestination设置下一步的位置
                Debug.Log(hit.point);
                Vector3 point = new Vector3(hit.point.x,1.5f,hit.point.z);
                agent.SetDestination(point);
            }
        }
    }

}
