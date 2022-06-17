#ifndef VIRTUAL_GUIDE_WRAPPER_H
#define VIRTUAL_GUIDE_WRAPPER_H
#include "ros/ros.h"
#include "iostream"
#include <cloud_common/socket_data_packet_stamp_msg.h>
#include <cloud_common/define_common.h>
#include "geometry_msgs/TwistStamped.h"
#include "std_msgs/Int32.h"
#include "std_msgs/Bool.h"
#include "x2struct/x2struct.hpp"
using namespace DTUAVCARS;

class virtual_guide_wrapper
{
public:
  virtual_guide_wrapper();
  void virtual_guide_control_msg_cb(const std_msgs::Int32ConstPtr& msg);
  void virtual_guide_msg_cb(const geometry_msgs::TwistStamped::ConstPtr& msg);
private:
  ros::Publisher             _iotMessagePub;
  string                     _iotMessagePubTopic;
  int                        _sourceID;
  int                        _targetID;

  string                     _virtualGuideSubTopic;
  ros::Subscriber            _virtualGuideSub;

  string                     _virtualGuideControlSubTopic;
  ros::Subscriber            _virtualGuideControlSub;
};

#endif // VIRTUAL_GUIDE_WRAPPER_H
