/*
 * 2021 Yuanlin Yang, Guandong University of Technology, Guanzhou,China 
 */

using UnityEngine;
using System.Runtime.InteropServices;
public class TestSimpleProjectDll : MonoBehaviour
{
    [DllImport("Simple_Project")] //这里就是调用的dll名字
    public static extern int MyAddFunc(int x, int y);

    void Start()
    {
        int ret = MyAddFunc(200, 256666);
        Debug.LogFormat("--- ret:{0}", ret);
    }
}
