using System;
using System.Runtime.InteropServices;
namespace SimUnity.Network.msgs.geometry_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Pose2Ddouble
    {
        public double x;
        public double y;
        public double theta;
    }
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Pose2Dfloat
    {
        public float x;
        public float y;
        public float theta;
    }
}