using System;
using System.Runtime.InteropServices;
using SimUnity.Network.msgs.geometry_msgs;
namespace SimUnity.Network.msgs.sensor_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct PointCloudDouble
    {
        public Pointdouble[] points;
        public ChannelDouble[] channels;

    }
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct PointCloudFloat
    {
        public Pointfloat[] points;
        public ChannelFloat[] channels;
    }


}
