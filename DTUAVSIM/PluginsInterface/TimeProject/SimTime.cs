/*
 * 2021 Yuanlin Yang, Guandong University of Technology, Guanzhou,China 
 */

using System.Runtime.InteropServices;

namespace DTUAVSIM.Time
{
    public class SimTime
    {
        [DllImport("Time_Project")]
        public extern static long GetSystemTimeStampNs();//Get the System Time with Nanosecond Format

        [DllImport("Time_Project")]
        public extern static long GetSystemTimeStampUs();//Get the System Time with Microseconds Format

        [DllImport("Time_Project")]
        public extern static long GetSystemTimeStampMs();//Get the System Time with Millisecond Format

        [DllImport("Time_Project")]
        public extern static long GetSystemTimeStampS();//Get the System Time with Seconds Format

        [DllImport("Time_Project")]
        public extern static long GetMachineRunTimeStampNs();//Get the Application Start to Now Time with Nanosecond Format

        [DllImport("Time_Project")]
        public extern static long GetMachineRunTimeStampUs();//Get the Application Start to Now Time with Microseconds Format

        [DllImport("Time_Project")]
        public extern static long GetMachineRunTimeStampMs();//Get the Application Start to Now Time with Millisecond Format

        [DllImport("Time_Project")]
        public extern static long GetMachineRunTimeStampS();//Get the Application Start to Now Time with Seconds Format
    }
}
