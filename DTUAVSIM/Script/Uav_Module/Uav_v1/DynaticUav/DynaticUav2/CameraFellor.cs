using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFellor : MonoBehaviour
{
    //实现的是相机从上往下看物体
    [Header("相机移动的速度")]
    public float CameraMoveSpeed = 3f;//相机移动的速度
    [Header("相机旋转的速度")]
    public float CameraTurnSpeed = 10f;//相机旋转的速度
    [Header("相机跟踪的对象坐标，拖入对象")]
    public Transform FellorTarget;//相机跟踪的对象坐标
    [Header("相机跟踪的对象")]
    public GameObject FellorObject;//相机跟踪的对象
    private Vector3 offset;//相机与跟踪对象的初始偏移量
    private RaycastHit hit;//射线的碰撞信息
    private float distance;//相机与跟踪对象的距离
    private Vector3[] currentPoint;//相机的观察点

    //检测某个点能否看到跟踪对像
    bool CheckView(Vector3 position)
    {
        Vector3 dir = FellorTarget.position - position;
        if(Physics.Raycast(position,dir,out hit))
        {
            if(hit.collider.gameObject.tag == "uav1_1" )
            {
                return true;
            }
        }
            return false;
    }

    //相机旋转方法
    void SmoothRotate()
    {
        Vector3 dir = FellorTarget.position - transform.position;
        Quaternion qua = Quaternion.LookRotation(dir);//要旋转的角度
        transform.rotation = Quaternion.Lerp(transform.rotation, qua, Time.deltaTime*CameraTurnSpeed);//相机旋转
      //  transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, 0);//把相机的y轴和z轴锁死

    }









    void Awake()
    {
        currentPoint = new Vector3[5];
    }


    // Start is called before the first frame update
    void Start()
    {
        
        distance = Vector3.Distance(transform.position, FellorTarget.position);//初始相机和跟踪对象的距离
        offset = FellorTarget.position - transform.position;//初始相机和跟踪对象的位置偏差
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        Vector3 startPosition = FellorTarget.position - offset;//相机观察的第一个点
        Vector3 endPosition = FellorTarget.position + Vector3.up * distance;//相机观察的最后一个点

        currentPoint[0] = startPosition;
        currentPoint[1] = Vector3.Slerp(startPosition, endPosition, 0.25f);
        currentPoint[2] = Vector3.Slerp(startPosition, endPosition, 0.5f);
        currentPoint[3] = Vector3.Slerp(startPosition, endPosition, 0.75f);
        currentPoint[4] = endPosition;

        Vector3 viewPosition = currentPoint[0];
        for(int i =0;i<currentPoint.Length;i++)
        {
            if(CheckView(currentPoint[i]))
            {
                viewPosition = currentPoint[i];
                break;
            }
        }
        transform.position = Vector3.Lerp(transform.position, viewPosition, Time.deltaTime);
        SmoothRotate();
    }


}
