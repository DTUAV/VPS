using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVCARS.Algorithm.DataStruct;
public class TestCircularBuffer : MonoBehaviour
{
    // Start is called before the first frame update
    public CircularBuffer<float> circularBuffer;
    void Start()
    {
        circularBuffer = new CircularBuffer<float>(3);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            circularBuffer.Push(transform.position.x);
            Debug.Log("data: " + circularBuffer[0] + "," + circularBuffer[1] + "," + circularBuffer[2]);
        }
    }
}
