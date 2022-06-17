using System;
using System.Runtime.InteropServices;
namespace SimUnity.Network.msgs.geometry_msgs
{
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
   public struct AccelWithCovariancedouble
    {
       public Acceldouble accel;
       public double[] covariance ;
    }
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct AcclWithCovariancefloat
    {
        public Accelfloat accel;
        public float[] covariance;
    }

}

