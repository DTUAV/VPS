
using System.Runtime.InteropServices;
namespace SimUnity.Noise
{
    public class GaussNoisPlugin
    {
        [DllImport("GaussNoiseDll")]
        public extern static double GaussianNoiseData(double mean, double dev);

        
    }
    
}
