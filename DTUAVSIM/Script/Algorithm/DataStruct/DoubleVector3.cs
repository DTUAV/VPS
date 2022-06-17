using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DTUAVCARS.Algorithm.DataStruct
{
    public struct DoubleVector3
    {
        public double x;
        public double y;
        public double z;
        public DoubleVector3(double _x,double _y,double _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }
        public DoubleVector3(double _x)
        {
            x = _x;
            y = 0;
            z = 0;
        }
        public DoubleVector3(double _x,double _y)
        {
            x = _x;
            y = _y;
            z = 0;
        }

        public double this[int index] 
        { 
            get 
            {
                if(index == 0)
                {
                    return x;
                }
                else if(index == 1)
                {
                    return y;
                }
                else if(index == 2)
                {
                    return z;
                }
                else
                {
                    return 0;
                }
             }
            set 
            {
                if(index == 0)
                {
                    x = value;
                }
                else if(index == 1)
                {
                    y = value;
                }
                else
                {
                    z = value;
                }
            } 
        }

        public DoubleVector3 right { get { return new DoubleVector3(1, 0, 0); } }
        public DoubleVector3 left { get { return new DoubleVector3(-1, 0, 0); } }
        public DoubleVector3 up { get { return new DoubleVector3(0, 1, 0); } }
        public DoubleVector3 down { get { return new DoubleVector3(0, -1, 0); } }
        public DoubleVector3 back { get { return new DoubleVector3(0, 0, -1); } }
        public DoubleVector3 forward { get { return new DoubleVector3(0, 0, 1); } }
        public DoubleVector3 one { get { return new DoubleVector3(1, 1, 1); } }
        public DoubleVector3 zero { get { return new DoubleVector3(0, 0, 0); } }
        public DoubleVector3 negativeInfinity { get { return new DoubleVector3(double.MinValue, double.MinValue, double.MinValue); } }
        public DoubleVector3 positiveInfinity { get { return new DoubleVector3(double.MaxValue, double.MaxValue, double.MaxValue); } }

        public DoubleVector3 Ros2Unity()
        {
            return new DoubleVector3 (-y, z, x);
        }

        public DoubleVector3 Unity2Ros()
        {
            return new DoubleVector3(z, -x, y);
        }
    }
}
