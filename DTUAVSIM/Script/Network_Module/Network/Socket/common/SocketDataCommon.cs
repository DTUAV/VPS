using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace DTUAVCARS.DTNetWork.SocketNetwork
{
    [Serializable] // 指示可序列化
[StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
    public struct DataSize
    {
        public double size_0;
        public double size_1;
        public double size_2;
        public double size_3;
        public double size_4;
        public double size_5;
        public double size_6;
        public double size_7;
        public double size_8;
        public double size_9;
    };

    public class SocketDataCommon
    {
        public static DataSize GetDataSizeStrut(long size)
        {
            DataSize dataSizeStrut = new DataSize();
            dataSizeStrut.size_9 = size / 1000000000;
            long size_rest = size % 1000000000;
            dataSizeStrut.size_8 = size_rest / 100000000;
            size_rest = size_rest % 100000000;
            dataSizeStrut.size_7 = size_rest / 10000000;
            size_rest = size_rest % 10000000;
            dataSizeStrut.size_6 = size_rest / 1000000;
            size_rest = size_rest % 1000000;
            dataSizeStrut.size_5 = size_rest / 100000;
            size_rest = size_rest % 100000;
            dataSizeStrut.size_4 = size_rest / 10000;
            size_rest = size_rest % 10000;
            dataSizeStrut.size_3 = size_rest / 1000;
            size_rest = size_rest % 1000;
            dataSizeStrut.size_2 = size_rest / 100;
            size_rest = size_rest % 100;
            dataSizeStrut.size_1 = size_rest / 10;
            size_rest = size_rest % 10;
            dataSizeStrut.size_0 = size_rest;
            return dataSizeStrut;
        }

        public static long GetDataSize(DataSize dataSizeStrut)
        {
            long dataSize = (long) dataSizeStrut.size_9 * 1000000000
                            + (long) dataSizeStrut.size_8 * 100000000
                            + (long) dataSizeStrut.size_7 * 10000000
                            + (long) dataSizeStrut.size_6 * 1000000
                            + (long) dataSizeStrut.size_5 * 100000
                            + (long) dataSizeStrut.size_4 * 10000
                            + (long) dataSizeStrut.size_3 * 1000
                            + (long) dataSizeStrut.size_2 * 100
                            + (long) dataSizeStrut.size_1 * 10
                            + (long) dataSizeStrut.size_0 * 1;
            return dataSize;

        }
    }
}

