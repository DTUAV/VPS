#include "../include/physical_message_pack/uav_message_pack.h"
uav_message_pack::uav_message_pack()
{
  ros::NodeHandle n("~");
  n.getParam("iot_pub_topic",_iot_pub_topic);
  n.getParam("SourceID",_sourceID);
  n.getParam("TargetID",_targetID);
  n.getParam("virtual_object_position_sub_topic",_virtual_object_position_sub_topic);
  n.getParam("virtual_guide_sub_topic",_virtual_guide_sub_topic);
  n.getParam("virtual_guide_control_sub_topic",_virtual_guide_control_sub_topic);
  n.getParam("model_learner_control_sub_topic",_model_learner_control_sub_topic);
  _iot_pub = n.advertise<cloud_common::socket_data_packet_stamp_msg>(_iot_pub_topic,1);
  _virtual_object_position_sub = n.subscribe(_virtual_object_position_sub_topic,1,&uav_message_pack::virtual_object_position_sub_cb,this);
  _virtual_guide_sub = n.subscribe(_virtual_guide_sub_topic,1,&uav_message_pack::virtual_guide_sub_cb,this);
  _virtual_guide_control_sub = n.subscribe(_virtual_guide_control_sub_topic,1,&uav_message_pack::virtual_guide_control_sub_cb,this);
  _model_learner_control_sub = n.subscribe(_model_learner_control_sub_topic,1,&uav_message_pack::model_learner_control_sub_cb,this);
}

void uav_message_pack::model_learner_control_sub_cb(const std_msgs::BoolConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = ModelLearnControlMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = ros::Time::now().toNSec();//ns
  ModelLearnControlMessage learnControlMsg;
  learnControlMsg.learn = msg.get()->data;
  cloudMsgPub.MessageData = x2struct::X::tojson(learnControlMsg);
  _iot_pub.publish(cloudMsgPub);
}

void uav_message_pack::virtual_guide_control_sub_cb(const std_msgs::Int32ConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = VirtualGuideControlMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = ros::Time::now().toNSec();//ns
  VirtualGuideControlMessage guideControlMsg;
  guideControlMsg.guideType = msg.get()->data;
  cloudMsgPub.MessageData = x2struct::X::tojson(guideControlMsg);
  _iot_pub.publish(cloudMsgPub);
}

void uav_message_pack::virtual_guide_sub_cb(const geometry_msgs::TwistStampedConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = VirtualGuideMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = ros::Time::now().toNSec();//ns
  VirtualGuideMessage guideMsg;
  guideMsg.linearVelocityX = msg.get()->twist.linear.x;
  guideMsg.linearVelocityY = msg.get()->twist.linear.y;
  guideMsg.linearVelocityZ = msg.get()->twist.linear.z;
  cloudMsgPub.MessageData = x2struct::X::tojson(guideMsg);
  _iot_pub.publish(cloudMsgPub);

}

void uav_message_pack::virtual_object_position_sub_cb(const geometry_msgs::PoseStampedConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = UavLocalPositionMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = ros::Time::now().toNSec();//ns
  UavLocalPositionMessage positionMsg;
  positionMsg.position_x = msg.get()->pose.position.x;
  positionMsg.position_y = msg.get()->pose.position.y;
  positionMsg.position_z = msg.get()->pose.position.z;

  positionMsg.rotation_x = msg.get()->pose.orientation.x;
  positionMsg.rotation_y = msg.get()->pose.orientation.y;
  positionMsg.rotation_z = msg.get()->pose.orientation.z;
  positionMsg.rotation_w = msg.get()->pose.orientation.w;
  cloudMsgPub.MessageData = x2struct::X::tojson(positionMsg);
  _iot_pub.publish(cloudMsgPub);
}

