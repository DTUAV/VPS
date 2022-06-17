/*
 * 2021 Yuanlin Yang, Guandong University of Technology, Guanzhou,China 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVSIM.Time;
public class TestTimeProjectPlguin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("MachineRunTimeStampMs: " + SimTime.GetMachineRunTimeStampMs());
        Debug.Log("MachineRunTimeStampNs: "+ SimTime.GetMachineRunTimeStampNs());
        Debug.Log("MachineRunTimeStampS: "+ SimTime.GetMachineRunTimeStampS());
        Debug.Log("MachineRunTimeStampUs: "+ SimTime.GetMachineRunTimeStampUs());
        Debug.Log("SystemTimeStampMs: " + SimTime.GetSystemTimeStampMs());
        Debug.Log("SystemTimeStampNs: "+ SimTime.GetSystemTimeStampNs());
        Debug.Log("SystemTimeStampUs: " + SimTime.GetSystemTimeStampUs());
        Debug.Log("SystemTimeStampS: "+ SimTime.GetSystemTimeStampS());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
