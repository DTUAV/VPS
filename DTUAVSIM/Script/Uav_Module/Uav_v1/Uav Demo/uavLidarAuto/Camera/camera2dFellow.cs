using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera2dFellow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject target;//相机跟踪的对象
    public float smoothing = 5f;
    Vector3 offset = Vector3.zero;
    void Start()
    {
        offset = transform.position - target.GetComponent<Rigidbody>().position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        Vector3 targetCamPos = target.GetComponent<Rigidbody>().position + offset;
        //  transform.position = Vector2.Lerp(transform.position, targetCamPos, 2);
        transform.position = new Vector3(targetCamPos.x, transform.position.y, targetCamPos.z);
    }
}
