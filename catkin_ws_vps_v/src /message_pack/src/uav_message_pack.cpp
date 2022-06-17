#include "../include/message_pack/uav_message_pack.h"
int send_time =0;
uav_message_pack::uav_message_pack()
{
  ros::NodeHandle n("~");
  n.getParam("IotMessagePubTopic",_iotMessagePubTopic);
  n.getParam("SourceID",_sourceID);
  n.getParam("TargetID",_targetID);
  n.getParam("GlobalPositionMessageSubTopic",_globalPosMsgSubTopic);
  n.getParam("LocalPositionMessageSubTopic",_localPosMsgSubTopic);
  n.getParam("LocalVelocityMessageSubTopic",_localVelMsgSubTopic);
  n.getParam("UavStateMessageSubTopic",_uavStateMsgSubTopic);
  n.getParam("MessagePubFrequency",_msgPubFrequency);
  n.getParam("ObjectMotionModelMessageSubTopic",_objMotionModelMsgSubTopic);
  n.getParam("NetworkDelayTimeMessageSubTopic",_networkDelayTimeMsgSubTopic);
  _iotMessagePub = n.advertise<cloud_common::socket_data_packet_stamp_msg>(_iotMessagePubTopic,1);
  _gloPosMsgSub = n.subscribe(_globalPosMsgSubTopic,1,&uav_message_pack::globalPosMsgSubCallback,this);
  _localPosMsgSub = n.subscribe(_localPosMsgSubTopic,1,&uav_message_pack::localPosMsgSubCallback,this);
  _localVelMsgSub = n.subscribe(_localVelMsgSubTopic,1,&uav_message_pack::localVelocitySubCallback,this);
  _uavStateMsgSub = n.subscribe(_uavStateMsgSubTopic,1,&uav_message_pack::uavStateSubCallback,this);
  _objMotionModelMsgSub = n.subscribe(_objMotionModelMsgSubTopic,1,&uav_message_pack::objectMotionModelSubCallback,this);
  _networkDelayTimeMsgSub = n.subscribe(_networkDelayTimeMsgSubTopic,1,&uav_message_pack::NetworkDelayTimeSubCallback,this);
  send_time = 0;
}

void uav_message_pack::uavStateSubCallback(const mavros_msgs::StateConstPtr &msg)
{
  //if(send_time==0)
  {
    cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
    cloudMsgPub.MessageID = UavStateMsgID;
    cloudMsgPub.SourceID = _sourceID;
    cloudMsgPub.TargetID = _targetID;
    cloudMsgPub.TimeStamp = msg.get()->header.stamp.toNSec();//_nsec是纳秒
    UavStateMsg uavStateMsg;
    uavStateMsg.armed = msg.get()->armed;
    uavStateMsg.connected = msg.get()->connected;
    uavStateMsg.guided = msg.get()->guided;
    uavStateMsg.manual_input = msg.get()->manual_input;
    uavStateMsg.mode = msg.get()->mode;
    uavStateMsg.system_status = msg.get()->system_status;
    cloudMsgPub.MessageData = x2struct::X::tojson(uavStateMsg);
    //_iotMessagePub.publish(cloudMsgPub);
    send_time =1;
  }
}

void uav_message_pack::globalPosMsgSubCallback(const sensor_msgs::NavSatFix::ConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = UavGlobalPositionMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = msg.get()->header.stamp.toNSec();//_nsec是纳秒
  UavGlobalPositionMessage gloPosMsg;
  gloPosMsg.altitude = msg.get()->altitude;
  gloPosMsg.latitude = msg.get()->latitude;
  gloPosMsg.longitude = msg.get()->longitude;
  gloPosMsg.position_covariance_type = msg.get()->position_covariance_type;
  gloPosMsg.position_covariance.resize(9);
  gloPosMsg.service = msg.get()->status.service;
  gloPosMsg.status = msg.get()->status.status;
  for(int i =0;i<9;i++)
  {
    gloPosMsg.position_covariance.at(i) = msg.get()->position_covariance.at(i);
  }
  cloudMsgPub.MessageData = x2struct::X::tojson(gloPosMsg);
  // _iotMessagePub.publish(cloudMsgPub);
}

void uav_message_pack::localPosMsgSubCallback(const geometry_msgs::PoseStamped::ConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = UavLocalPositionMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = msg.get()->header.stamp.toNSec();//_nsec是纳秒
  UavLocalPositionMessage localPosMsg;
  localPosMsg.position_x = msg.get()->pose.position.x;
  localPosMsg.position_y = msg.get()->pose.position.y;
  localPosMsg.position_z = msg.get()->pose.position.z;
  localPosMsg.rotation_x = msg.get()->pose.orientation.x;
  localPosMsg.rotation_y = msg.get()->pose.orientation.y;
  localPosMsg.rotation_z = msg.get()->pose.orientation.z;
  localPosMsg.rotation_w = msg.get()->pose.orientation.w;
  cloudMsgPub.MessageData = x2struct::X::tojson(localPosMsg);
  _iotMessagePub.publish(cloudMsgPub);
}

void uav_message_pack::localVelocitySubCallback(const geometry_msgs::TwistStamped::ConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = UavLocalVelocityMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = msg.get()->header.stamp.toNSec();//_nsec是纳秒
  UavLocalVelocityMessage localVelMsg;
  localVelMsg.linear_velocity_x = msg.get()->twist.linear.x;
  localVelMsg.linear_velocity_y = msg.get()->twist.linear.y;
  localVelMsg.linear_velocity_z = msg.get()->twist.linear.z;
  localVelMsg.anger_velocity_x = msg.get()->twist.angular.x;
  localVelMsg.anger_velocity_y = msg.get()->twist.angular.y;
  localVelMsg.anger_velocity_z = msg.get()->twist.angular.z;
  cloudMsgPub.MessageData = x2struct::X::tojson(localVelMsg);
  //_iotMessagePub.publish(cloudMsgPub);
}

void uav_message_pack::objectMotionModelSubCallback(const dt_message_package::update_model_parameterConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  struct timeval tv;
  gettimeofday(&tv, NULL);
  long rawtime_ms = tv.tv_sec * 1000 + tv.tv_usec / 1000;
  cloudMsgPub.MessageID = ObjectMotionModelMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = rawtime_ms;
  ObjectMotionModelMessage motionModelMsg;
  motionModelMsg.A.resize(9);
  for(int i =0;i<9;i++)
  {
    if(i==0||i==4||i==8)
    {
      motionModelMsg.A.at(i) = 1;
    }
    else
    {
      motionModelMsg.A.at(i) = 0;
    }
  }
  motionModelMsg.B.resize(9);
  motionModelMsg.B.at(0) = msg.get()->b1;
  motionModelMsg.B.at(1) = msg.get()->b2;
  motionModelMsg.B.at(2) = msg.get()->b3;
  motionModelMsg.B.at(3) = msg.get()->b4;
  motionModelMsg.B.at(4) = msg.get()->b5;
  motionModelMsg.B.at(5) = msg.get()->b6;
  motionModelMsg.B.at(6) = msg.get()->b7;
  motionModelMsg.B.at(7) = msg.get()->b8;
  motionModelMsg.B.at(8) = msg.get()->b9;

  motionModelMsg.W.resize(3);
  motionModelMsg.W.at(0) = msg.get()->wx;
  motionModelMsg.W.at(1) = msg.get()->wy;
  motionModelMsg.W.at(2) = msg.get()->wz;
  cloudMsgPub.MessageData = x2struct::X::tojson(motionModelMsg);
  _iotMessagePub.publish(cloudMsgPub);
}

void uav_message_pack::NetworkDelayTimeSubCallback(const dt_message_package::network_delay_timeConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  struct timeval tv;
  gettimeofday(&tv, NULL);
  long rawtime_ms = tv.tv_sec * 1000 + tv.tv_usec / 1000;
  cloudMsgPub.MessageID = ObjectMotionModelMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = rawtime_ms;
  NetworkDelayTimeMessage networkDelayMsg;
  networkDelayMsg.delay_time = msg.get()->delay_time;
  cloudMsgPub.MessageData = x2struct::X::tojson(networkDelayMsg);
  _iotMessagePub.publish(cloudMsgPub);
}
