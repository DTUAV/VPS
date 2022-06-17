using System;
using System.Runtime.InteropServices;
namespace SimUnity.Network.msgs.geometry_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Quaterniondouble
    {
      public  double x;
      public  double y;
      public  double z;
      public  double w;
    }
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Quaternionfloat
    {
       public float x;
       public float y;
       public float z;
       public float w;
    }


}


