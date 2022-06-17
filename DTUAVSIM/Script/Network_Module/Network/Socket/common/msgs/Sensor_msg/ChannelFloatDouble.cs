
using System;
using System.Runtime.InteropServices;

namespace SimUnity.Network.msgs.sensor_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
     public struct ChannelFloat
    {
        public string name;
        public float[] values;
    }

    public struct ChannelDouble
    {
        public string name;
        public double[] values;
    }


}

