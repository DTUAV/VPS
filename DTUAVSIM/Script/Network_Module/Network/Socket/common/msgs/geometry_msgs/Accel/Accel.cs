using System;
using System.Runtime.InteropServices;
namespace SimUnity.Network.msgs.geometry_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Acceldouble 
    {
     public   Vector3double linear;
     public   Vector3double angular;
    }
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Accelfloat
    {
      public  Vector3float linear;
      public  Vector3float angular;
    }

}
