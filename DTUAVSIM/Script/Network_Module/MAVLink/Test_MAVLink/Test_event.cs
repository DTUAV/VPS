using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_event : MonoBehaviour
{ 
  
    public delegate void TestDelegate(int msg);

    public event TestDelegate TestEvent;
    // Start is called before the first frame update
    void Start()
    {
        TestEvent += test;
        if (TestEvent != null)
        {

            TestEvent(6);
        }
        else
        {
            Debug.Log("TestEvent null" );
        }
    }

    void test(int msg)
    {
        Debug.Log("test"+" "+msg);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
