using System;
using System.Runtime.InteropServices;
namespace SimUnity.Network.msgs.geometry_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Polygondouble
    {
       public Pointdouble[] points;
    }
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Polygonfloat
    {
        public Pointfloat[] points;
    }
}