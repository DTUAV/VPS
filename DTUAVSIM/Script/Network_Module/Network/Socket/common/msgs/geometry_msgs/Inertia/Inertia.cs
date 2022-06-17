using System;
using System.Runtime.InteropServices;
namespace SimUnity.Network.msgs.geometry_msgs {
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Inertiadouble
    {
       public double m;//Mass(kg)
       public Vector3double com;//Center of mass(m)
       public double ixx;
       public double ixy;
       public double ixz;
       public double iyy;
       public double iyz;
       public double izz;
    }
    [Serializable] // 指示可序列化
    [StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct Inertiafloat
    {
       public float m;//Mass(kg)
       public Vector3float com;//Center of mass(m)
       public float ixx;
       public float ixy;
       public float ixz;
       public float iyy;
       public float iyz;
       public float izz;
    }

}