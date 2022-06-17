/*
 * yyl
 * 2020/12/12
 */
using System.Runtime.InteropServices;

namespace DigitalTwin
{
    public class GaussNoise
    {
        [DllImport("GaussNoiseDll")]
        public extern static double GaussianNoiseData(double mean, double dev);
    }

}