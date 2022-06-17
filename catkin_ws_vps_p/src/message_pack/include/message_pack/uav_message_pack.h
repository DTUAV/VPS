#ifndef R_UAV_MESSAGE_PACK_H
#define R_UAV_MESSAGE_PACK_H
#include "ros/ros.h"
#include "iostream"
#include <cloud_common/socket_data_packet_stamp_msg.h>
#include <cloud_common/define_common.h>
#include "sensor_msgs/NavSatFix.h"
#include "geometry_msgs/PoseStamped.h"
#include "geometry_msgs/TwistStamped.h"
#include "mavros_msgs/State.h"
#include "x2struct/x2struct.hpp"
#include "dt_message_package/object_motion_model.h"
#include "dt_message_package/network_delay_time.h"
#include "sys/time.h"
#include "dt_message_package/update_model_parameter.h"
using namespace DTUAVCARS;
class uav_message_pack
{
public:
  uav_message_pack();
  void globalPosMsgSubCallback(const sensor_msgs::NavSatFix::ConstPtr& msg);
  void localPosMsgSubCallback(const geometry_msgs::PoseStamped::ConstPtr& msg);
  void localVelocitySubCallback(const geometry_msgs::TwistStamped::ConstPtr& msg);
  void uavStateSubCallback(const mavros_msgs::StateConstPtr& msg);
  void objectMotionModelSubCallback(const dt_message_package::update_model_parameterConstPtr& msg);
  void NetworkDelayTimeSubCallback(const dt_message_package::network_delay_timeConstPtr& msg);

private:
  ros::Publisher             _iotMessagePub;
  string                     _iotMessagePubTopic;
  int                        _sourceID;
  int                        _targetID;
  float                      _msgPubFrequency;
  string                     _globalPosMsgSubTopic;
  string                     _localPosMsgSubTopic;
  string                     _localVelMsgSubTopic;
  string                     _uavStateMsgSubTopic;

  string                     _objMotionModelMsgSubTopic;
  string                     _networkDelayTimeMsgSubTopic;



  ros::Subscriber            _gloPosMsgSub;
  ros::Subscriber            _localPosMsgSub;
  ros::Subscriber            _localVelMsgSub;
  ros::Subscriber            _uavStateMsgSub;

  ros::Subscriber            _objMotionModelMsgSub;
  ros::Subscriber            _networkDelayTimeMsgSub;

};

#endif // R_UAV_MESSAGE_PACK_H
