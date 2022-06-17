using System;
using System.Runtime.InteropServices;
namespace SimUnity.Network.msgs.geometry_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Posedouble
    {
        public Pointdouble position;
        public Quaterniondouble orientation;
    }

    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Posefloat
    {
        public Pointfloat position;
        public Quaternionfloat orientation;
    }
}