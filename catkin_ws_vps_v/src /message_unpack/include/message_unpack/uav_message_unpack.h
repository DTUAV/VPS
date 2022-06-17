#ifndef R_UAV_MESSAGE_UNPACK_H
#define R_UAV_MESSAGE_UNPACK_H
#include <cloud_common/socket_data_packet_stamp_msg.h>
#include <cloud_common/define_common.h>
#include "std_msgs/Bool.h"
#include "geometry_msgs/PoseStamped.h"
#include "geometry_msgs/Twist.h"
#include "ros/ros.h"

using namespace DTUAVCARS;

class uav_message_unpack
{
public:
  uav_message_unpack();
  void IotMessageSubCallback(const cloud_common::socket_data_packet_stamp_msgConstPtr& msg);

private:
  ros::Subscriber _iotMessageSub;
  ros::Publisher _refPosMsgPub;
  ros::Publisher _refVelMsgPub;


  string _iotMessageSubTopic;

  string _refPoseMessageTopic;
  string _refVelocityMessageTopic;
};

#endif // R_UAV_MESSAGE_UNPACK_H
