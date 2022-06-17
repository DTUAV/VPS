#include "../include/virtual_guide_predictor/virtual_guide_wrapper.h"

virtual_guide_wrapper::virtual_guide_wrapper()
{
  ros::NodeHandle n("~");
  n.getParam("IotMessagePubTopic",_iotMessagePubTopic);
  n.getParam("SourceID",_sourceID);
  n.getParam("TargetID",_targetID);
  n.getParam("VirtualGuideSubTopic",_virtualGuideSubTopic);
  n.getParam("VirtualGuideControlSubTopic",_virtualGuideControlSubTopic);
  _iotMessagePub = n.advertise<cloud_common::socket_data_packet_stamp_msg>(_iotMessagePubTopic,1);
  _virtualGuideSub = n.subscribe(_virtualGuideSubTopic,1,&virtual_guide_wrapper::virtual_guide_msg_cb,this);
  _virtualGuideControlSub = n.subscribe(_virtualGuideControlSubTopic,1,&virtual_guide_wrapper::virtual_guide_control_msg_cb,this);
}

void virtual_guide_wrapper::virtual_guide_msg_cb(const geometry_msgs::TwistStamped::ConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = VirtualGuideMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = ros::Time::now().toNSec();//_nsec是纳秒
  VirtualGuideMessage virtualGuideMsg;
  virtualGuideMsg.linearVelocityX = msg.get()->twist.linear.x;
  virtualGuideMsg.linearVelocityY = msg.get()->twist.linear.y;
  virtualGuideMsg.linearVelocityZ = msg.get()->twist.linear.z;
  cloudMsgPub.MessageData = x2struct::X::tojson(virtualGuideMsg);
  _iotMessagePub.publish(cloudMsgPub);
}

void virtual_guide_wrapper::virtual_guide_control_msg_cb(const std_msgs::Int32ConstPtr &msg)
{
  cloud_common::socket_data_packet_stamp_msg cloudMsgPub;
  cloudMsgPub.MessageID = VirtualGuideControlMessageID;
  cloudMsgPub.SourceID = _sourceID;
  cloudMsgPub.TargetID = _targetID;
  cloudMsgPub.TimeStamp = ros::Time::now().toNSec();//_nsec是纳秒
  VirtualGuideControlMessage virtualGuideControlMsg;
  virtualGuideControlMsg.guideType = msg.get()->data;
  cloudMsgPub.MessageData = x2struct::X::tojson(virtualGuideControlMsg);
  _iotMessagePub.publish(cloudMsgPub);

}
