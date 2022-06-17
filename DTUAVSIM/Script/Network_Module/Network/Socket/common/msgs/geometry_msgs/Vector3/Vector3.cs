
using System;
using System.Runtime.InteropServices;

namespace SimUnity.Network.msgs.geometry_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Vector3double
    {
        public double x;
        public double y;
        public double z;
    }
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Vector3float
    {
        public float x;
        public float y;
        public float z;
    }
}