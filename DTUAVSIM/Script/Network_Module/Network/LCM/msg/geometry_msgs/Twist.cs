/* LCM type definition class file
 * This file was automatically generated by lcm-gen
 * DO NOT MODIFY BY HAND!!!!
 */

using System;
using System.Collections.Generic;
using System.IO;
using LCM.LCM;
 
namespace geometry_msgs
{
    public sealed class Twist : LCM.LCM.LCMEncodable
    {
        public geometry_msgs.Vector3 linear;
        public geometry_msgs.Vector3 angular;
 
        public Twist()
        {
        }
 
        public static readonly ulong LCM_FINGERPRINT;
        public static readonly ulong LCM_FINGERPRINT_BASE = 0x3a4144772922add7L;
 
        static Twist()
        {
            LCM_FINGERPRINT = _hashRecursive(new List<String>());
        }
 
        public static ulong _hashRecursive(List<String> classes)
        {
            if (classes.Contains("geometry_msgs.Twist"))
                return 0L;
 
            classes.Add("geometry_msgs.Twist");
            ulong hash = LCM_FINGERPRINT_BASE
                 + geometry_msgs.Vector3._hashRecursive(classes)
                 + geometry_msgs.Vector3._hashRecursive(classes)
                ;
            classes.RemoveAt(classes.Count - 1);
            return (hash<<1) + ((hash>>63)&1);
        }
 
        public void Encode(LCMDataOutputStream outs)
        {
            outs.Write((long) LCM_FINGERPRINT);
            _encodeRecursive(outs);
        }
 
        public void _encodeRecursive(LCMDataOutputStream outs)
        {
            this.linear._encodeRecursive(outs); 
 
            this.angular._encodeRecursive(outs); 
 
        }
 
        public Twist(byte[] data) : this(new LCMDataInputStream(data))
        {
        }
 
        public Twist(LCMDataInputStream ins)
        {
            if ((ulong) ins.ReadInt64() != LCM_FINGERPRINT)
                throw new System.IO.IOException("LCM Decode error: bad fingerprint");
 
            _decodeRecursive(ins);
        }
 
        public static geometry_msgs.Twist _decodeRecursiveFactory(LCMDataInputStream ins)
        {
            geometry_msgs.Twist o = new geometry_msgs.Twist();
            o._decodeRecursive(ins);
            return o;
        }
 
        public void _decodeRecursive(LCMDataInputStream ins)
        {
            this.linear = geometry_msgs.Vector3._decodeRecursiveFactory(ins);
 
            this.angular = geometry_msgs.Vector3._decodeRecursiveFactory(ins);
 
        }
 
        public geometry_msgs.Twist Copy()
        {
            geometry_msgs.Twist outobj = new geometry_msgs.Twist();
            outobj.linear = this.linear.Copy();
 
            outobj.angular = this.angular.Copy();
 
            return outobj;
        }
    }
}

