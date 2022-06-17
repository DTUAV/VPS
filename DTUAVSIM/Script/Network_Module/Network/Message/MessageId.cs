/*
 * Update Information:
 *                    First: 2021-8-4 In Guangdong University of Technology By Yang Yuanlin
 *
 */


namespace DTUAVCARS.DTNetwok.Message
{
    public class MessageId
    {
        public static int CurrentPositionMessageID = 1;
        public static int TargetPositionMessageID = 2;

        public static int CompressedImageMessageID = 28;

        public static int UavCurrentPoseMessageID = 101;
        public static int UavRefPoseMessageID = 102;
        public static int UavCurrentVelocityMessageID = 103;
        public static int UavRefVelocityMessageID = 104;
        public static int UavStartMessageID = 105;
        public static int UavGlobalPositionMessageID = 106;
        public static int UavLocalPositionMessageID = 107;
        public static int UavLocalVelocityMessageID = 108;

        public static int VirtualGuideMessageID = 109;


        public static int HitMessageID = 1000;
    }
}
