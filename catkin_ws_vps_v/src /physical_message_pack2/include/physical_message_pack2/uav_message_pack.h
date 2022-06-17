#ifndef UAV_MESSAGE_PACK_H
#define UAV_MESSAGE_PACK_H
#include "ros/ros.h"
#include "iostream"
#include <cloud_common/socket_data_packet_stamp_msg.h>
#include <cloud_common/define_common.h>
#include "geometry_msgs/PoseStamped.h"
#include "geometry_msgs/TwistStamped.h"
#include "x2struct/x2struct.hpp"
#include "sys/time.h"
#include "std_msgs/Int32.h"
#include "std_msgs/Bool.h"
using namespace DTUAVCARS;
class uav_message_pack
{
public:
  uav_message_pack();
void virtual_object_position_sub_cb(const geometry_msgs::PoseStampedConstPtr& msg);
void virtual_guide_sub_cb(const geometry_msgs::TwistStampedConstPtr& msg);
void virtual_guide_control_sub_cb(const std_msgs::Int32ConstPtr& msg);
void model_learner_control_sub_cb(const std_msgs::BoolConstPtr& msg);
void system_running_control_sub_cb(const std_msgs::Int32ConstPtr& msg);

private:

  std::string _iot_pub_topic;
  std::string _virtual_object_position_sub_topic;
  std::string _virtual_guide_sub_topic;
  std::string _virtual_guide_control_sub_topic;
  std::string _model_learner_control_sub_topic;
  std::string _system_running_control_sub_topic;

  ros::Subscriber _virtual_object_position_sub;
  ros::Subscriber _virtual_guide_sub;
  ros::Subscriber _virtual_guide_control_sub;
  ros::Subscriber _model_learner_control_sub;
  ros::Subscriber _system_running_control_sub;

  ros::Publisher _iot_pub;

  int _sourceID;
  int _targetID;

  int _system_running_id;
};

#endif // R_UAV_MESSAGE_PACK_H
