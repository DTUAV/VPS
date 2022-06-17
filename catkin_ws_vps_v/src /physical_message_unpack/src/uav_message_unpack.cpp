#include "../include/physical_message_unpack/uav_message_unpack.h"

double uav_message_unpack::getMeanWithRemoveMaxMin(boost::circular_buffer<double> cicl_delay_time)
{
  double maxData = -20000;
  double minData = 20000;
  int maxDataId = 0;
  int minDataId = 0;
  double sum = 0;
  for(int i=0;i<cicl_delay_time.size();i++)
  {
    // std::cout<<i<<" : "<<cicl_delay_time.at(i)<<std::endl;
    if(cicl_delay_time.at(i)>maxData)
    {
      maxDataId = i;
      maxData = cicl_delay_time.at(i);
    }
    if(cicl_delay_time.at(i)<minData)
    {
      minDataId = i;
      minData = cicl_delay_time.at(i);
    }
  }

  for(int i=0;i<cicl_delay_time.size();i++)
  {
    if(i!=maxDataId&&i!=minDataId)
    {
      sum +=cicl_delay_time.at(i);
    }
  }
  return sum/(cicl_delay_time.size()-2);

}

uav_message_unpack::uav_message_unpack()
{
  ros::NodeHandle n("~");
  n.getParam("iot_msg_sub_topic",_iot_msg_sub_topic);
  n.getParam("local_position_msg_pub_topic",_local_position_msg_pub_topic);
  _velocity_msg_pub_topic = "/physical_uav/velocity";
  n.getParam("velocity_msg_pub_topic",_velocity_msg_pub_topic);
  n.getParam("v2p_delay_time_msg_pub_topic",_v2p_delay_time_msg_pub_topic);
  n.getParam("p2v_delay_time_msg_pub_topic",_p2v_delay_time_msg_pub_topic);
  n.getParam("update_model_msg_pub_topic",_update_model_msg_pub_topic);
  n.getParam("dt_object_position_recv_hz",_dt_object_position_recv_hz);
  n.getParam("object_move_position_pub_topic",_object_move_position_pub_topic);
  _iot_msg_sub = n.subscribe(_iot_msg_sub_topic,1,&uav_message_unpack::IotMessageSubCallback,this);

  _local_position_msg_pub = n.advertise<geometry_msgs::PoseStamped>(_local_position_msg_pub_topic,1);
  _velocity_msg_pub = n.advertise<geometry_msgs::TwistStamped>(_velocity_msg_pub_topic,1);
  _v2p_delay_time_msg_pub = n.advertise<std_msgs::Float32>(_v2p_delay_time_msg_pub_topic,1);
  _p2v_delay_time_msg_pub = n.advertise<std_msgs::Float32>(_p2v_delay_time_msg_pub_topic,1);
  _update_model_msg_pub = n.advertise<dt_message_package::update_model_parameter>(_update_model_msg_pub_topic,1);
  _object_move_position_pub = n.advertise<dt_message_package::object_move_position2>(_object_move_position_pub_topic,1);

  _data_times = (1.0/_dt_object_position_recv_hz)*1000;//ms
  _cicl_delay_time.resize(10);
  _is_first = true;
}
void uav_message_unpack::IotMessageSubCallback(const cloud_common::socket_data_packet_stamp_msgConstPtr& msg)
{
  bool is_push = false;
  if(_is_first)
  {
    if(msg.get()->MessageID == UavLocalPositionMessageID)
    {
      DTUAVCARS::UavLocalPositionMessage position_msg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,position_msg,false);
      if(is_load)
      {

        struct timeval tv;
        gettimeofday(&tv, NULL);
        long rawtime_ms = tv.tv_sec * 1000 + tv.tv_usec / 1000;
        _last_times = rawtime_ms;

        _is_first = false;                             //The First Data To Configure the Last Receive Time
        ROS_INFO("Receive Message From Network Sucessfully");
        ROS_INFO("Start Time is %f ns",_last_times);
      }
    }
  }
  else
  {

    switch (msg.get()->MessageID) {
    case UavLocalPositionMessageID:
    {
      UavLocalPositionMessage position_msg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,position_msg,false);
      if(is_load)
      {
        struct timeval tv;
        gettimeofday(&tv, NULL);
        long rawtime_ms = tv.tv_sec * 1000 + tv.tv_usec / 1000;
        double delay_times_ms = (rawtime_ms - _last_times);//cout the delay time roughly

        if(delay_times_ms - _data_times > 0.0)
        {
          _p2v_delay_time = delay_times_ms - _data_times;
          is_push = true;
        }
        else
        {
          _p2v_delay_time = 0.0;
        }

        if(_push_time<10)
        {
          if(is_push)
          {
          _cicl_delay_time.push_back(_p2v_delay_time);
          _push_time++;
          }
        }
        else
        {
          if(is_push)
          {
          _cicl_delay_time.push_back(_p2v_delay_time);
           _delay_time = getMeanWithRemoveMaxMin(_cicl_delay_time);
          _last_times = rawtime_ms;
          std_msgs::Float32 delayTimeMsg;
          delayTimeMsg.data = _delay_time;
          _p2v_delay_time_msg_pub.publish(delayTimeMsg);
          }
          // _push_time = 0;
        }

        geometry_msgs::PoseStamped PoseMsg;
        PoseMsg.header.frame_id = "r_uav";
        PoseMsg.header.stamp = ros::Time::now();
        PoseMsg.pose.orientation.x = position_msg.rotation_x;
        PoseMsg.pose.orientation.y = position_msg.rotation_y;
        PoseMsg.pose.orientation.z = position_msg.rotation_z;
        PoseMsg.pose.orientation.w = position_msg.rotation_w;
        PoseMsg.pose.position.x    = position_msg.position_x;
        PoseMsg.pose.position.y    = position_msg.position_y;
        PoseMsg.pose.position.z    = position_msg.position_z;
        _local_position_msg_pub.publish(PoseMsg);
        dt_message_package::object_move_position2 move_position_msg;
        gettimeofday(&tv, NULL);
        long rawtime_mss = tv.tv_sec * 1000 + tv.tv_usec / 1000;
        move_position_msg.time_stamp_ms = rawtime_mss;
        move_position_msg.position_x = position_msg.position_x;
        move_position_msg.position_y = position_msg.position_y;
        move_position_msg.position_z = position_msg.position_z;
        _object_move_position_pub.publish(move_position_msg);
      }
    }
      break;

    case ObjectMotionModelMessageID:
    {
      ObjectMotionModelMessage motionModelMsg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,motionModelMsg,false);
      if(is_load)
      {
        dt_message_package::update_model_parameter modelParamMsg;
        bool isUpdateModel = false;
        if(motionModelMsg.B.size()==9)
        {
          modelParamMsg.b1 = motionModelMsg.B.at(0);
          modelParamMsg.b2 = motionModelMsg.B.at(1);
          modelParamMsg.b3 = motionModelMsg.B.at(2);
          modelParamMsg.b4 = motionModelMsg.B.at(3);
          modelParamMsg.b5 = motionModelMsg.B.at(4);
          modelParamMsg.b6 = motionModelMsg.B.at(5);
          modelParamMsg.b7 = motionModelMsg.B.at(6);
          modelParamMsg.b8 = motionModelMsg.B.at(7);
          modelParamMsg.b9 = motionModelMsg.B.at(8);
          isUpdateModel = true;
        }
        if(isUpdateModel)
        {
          isUpdateModel = false;
          if(motionModelMsg.W.size()==3)
          {
            modelParamMsg.wx = motionModelMsg.W.at(0);
            modelParamMsg.wy = motionModelMsg.W.at(1);
            modelParamMsg.wz = motionModelMsg.W.at(2);
            isUpdateModel = true;
          }
        }
        if(isUpdateModel)
        {
          _update_model_msg_pub.publish(modelParamMsg);
        }
      }
    }
      break;

    case NetworkDelayTimeMessageID:
    {
      NetworkDelayTimeMessage delayTimeMsg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,delayTimeMsg,false);
      if(is_load)
      {
        std_msgs::Float32 v2pDelayMsg;
        v2pDelayMsg.data = delayTimeMsg.delay_time;
        _v2p_delay_time_msg_pub.publish(v2pDelayMsg);
      }
    }
      break;
    case UavLocalVelocityMessageID:
    {
      UavLocalVelocityMessage velocityMsg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,velocityMsg,false);
      if(is_load)
      {
        geometry_msgs::TwistStamped velocityRosMsg;
        velocityRosMsg.twist.linear.x = velocityMsg.linear_velocity_x;
        velocityRosMsg.twist.linear.y = velocityMsg.linear_velocity_y;
        velocityRosMsg.twist.linear.z = velocityMsg.linear_velocity_z;
        velocityRosMsg.twist.angular.x = velocityMsg.anger_velocity_x;
        velocityRosMsg.twist.angular.y = velocityMsg.anger_velocity_y;
        velocityRosMsg.twist.angular.z = velocityMsg.anger_velocity_z;
        _velocity_msg_pub.publish(velocityRosMsg);
      }
    }
      break;

    default:
      break;
    }
  }

}


