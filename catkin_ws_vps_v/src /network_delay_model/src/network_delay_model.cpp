#include "../include/network_delay_model/network_delay_model.h"

network_delay_model::network_delay_model()
{
  ros::NodeHandle n("~");
  n.getParam("virtual_object_position_sub_topic",_vObjPosSubTopic);
  n.getParam("virtual_object_position_pub_topic",_vObjPosPubTopic);
  n.getParam("physical_object_position_sub_topic",_pObjPosSubTopic);
  n.getParam("physical_object_position_pub_topic",_pObjPosPubTopic);
  n.getParam("virtual2physical_delay_time_sub_topic",_v2pDelaySubTopic);
  n.getParam("physical2virtual_delay_time_sub_topic",_p2vDelaySubTopic);
  n.getParam("v2p_noise_mu",_v2pDelay_noise_mu);
  n.getParam("v2p_noise_sigma",_v2pDelay_noise_sigma);
  n.getParam("p2v_noise_mu",_p2vDelay_noise_mu);
  n.getParam("p2v_noise_sigma",_p2vDelay_noise_sigma);
  n.getParam("reset_object_sub_topic",_reset_object_sub_topic);
  n.getParam("predict_window",_predict_window);
  n.getParam("predict_time_dt",_predict_time_dt);
  n.getParam("target_position_pub_topic",_targetPositionPubTopic);
  n.getParam("virtual_guide_type_sub_topic",_virtualGuideTypeSubTopic);
  n.getParam("virtual_guide_type_pub_topic",_virtualGuideTypePubTopic);
  n.getParam("virtual_guide_cmd_sub_topic",_virtualGuideCmdSubTopic);
  n.getParam("virtual_guide_cmd_pub_topic",_virtualGuideCmdPubTopic);
  n.getParam("physical_uav_position_sub_topic",_physicalUavPositionSubTopic);
  n.getParam("physical_uav_position_pub_topic",_physicalUavPositionPubTopic);
  n.getParam("physical_uav_move_state_pub_topic",_physicalUavMoveStatePubTopic);

  _vObjPosSub = n.subscribe(_vObjPosSubTopic,1,&network_delay_model::vObjPosSubCallback,this);
  _vObjPosPub = n.advertise<dt_message_package::object_move_position2>(_vObjPosPubTopic,1);
  _pObjPosSub = n.subscribe(_pObjPosSubTopic,1,&network_delay_model::pObjPosSubCallback,this);
  _pObjPosPub = n.advertise<geometry_msgs::PoseStamped>(_pObjPosPubTopic,1);
  _targetPositiongPub = n.advertise<dt_message_package::object_predict_position>(_targetPositionPubTopic,1);
  _v2pDelaySub = n.subscribe(_v2pDelaySubTopic,1,&network_delay_model::v2pDelaySubCallback,this);
  _p2vDelaySub = n.subscribe(_p2vDelaySubTopic,1,&network_delay_model::p2vDelaySubCallback,this);
  _reset_object_sub = n.subscribe(_reset_object_sub_topic,1,&network_delay_model::reset_object_sub_cb,this);

  _virtualGuideCmdPub = n.advertise<geometry_msgs::TwistStamped>(_virtualGuideCmdPubTopic,1);
  _virtualGuideTypePub = n.advertise<std_msgs::Int32>(_virtualGuideTypePubTopic,1);
  _virtualGuideCmdSub = n.subscribe(_virtualGuideCmdSubTopic,1,&network_delay_model::virtualGuideCmdSubCallback,this);
  _virtualGuideTypeSub = n.subscribe(_virtualGuideTypeSubTopic,1,&network_delay_model::virtualGuideTypeSubCallback,this);

  _physicalUavPositionPub = n.advertise<geometry_msgs::PoseStamped>(_physicalUavPositionPubTopic,1);
  _physicalUavPositionSub = n.subscribe(_physicalUavPositionSubTopic,1,&network_delay_model::physicalUavPositionSubCallback,this);
  _physicalUavMoveStatePub = n.advertise<dt_message_package::object_move_position2>(_physicalUavMoveStatePubTopic,1);

  _v2pDelay = 0.0;
  _p2vDelay = 0.0;
  _start_time_s = 0.0;
  _current_time_s = 0.0;
  _is_reset_object = false;
  _seed = 0;

}

void network_delay_model::physicalUavPositionSubCallback(const geometry_msgs::PoseStampedConstPtr &msg)
{
  double delay_time = GetGuassianNoise(_p2vDelay,_p2vDelay_noise_sigma);//ms
  geometry_msgs::PoseStamped pub_msg;
  pub_msg.header.stamp = ros::Time::now();
  pub_msg.pose.position.x = msg.get()->pose.position.x;
  pub_msg.pose.position.y = msg.get()->pose.position.y;
  pub_msg.pose.position.z = msg.get()->pose.position.z;
  dt_message_package::object_move_position2 pub_move_msg;
  pub_move_msg.time_stamp_ms = ros::Time::now().toNSec()/10e6;
  pub_move_msg.delay_time_ms = delay_time;
  pub_move_msg.position_x = msg.get()->pose.position.x;
  pub_move_msg.position_y = msg.get()->pose.position.y;
  pub_move_msg.position_z = msg.get()->pose.position.z;
  double now_time = ros::Time::now().toNSec();//ns
  double next_time = now_time + delay_time*10e6;
  while(next_time - ros::Time::now().toNSec()>=0);
  _physicalUavPositionPub.publish(pub_msg);
  _physicalUavMoveStatePub.publish(pub_move_msg);

}

void network_delay_model::virtualGuideCmdSubCallback(const geometry_msgs::TwistStampedConstPtr &msg)
{
  double delay_time = GetGuassianNoise(_v2pDelay,_v2pDelay_noise_sigma);//ms
  geometry_msgs::TwistStamped pub_msg;
  pub_msg.twist.linear.x = msg.get()->twist.linear.x;
  pub_msg.twist.linear.y = msg.get()->twist.linear.y;
  pub_msg.twist.linear.z = msg.get()->twist.linear.z;
  double now_time = ros::Time::now().toNSec();//ns
  double next_time = now_time + delay_time*10e6;
  while(next_time - ros::Time::now().toNSec()>=0);
  _virtualGuideCmdPub.publish(pub_msg);
}

void network_delay_model::virtualGuideTypeSubCallback(const std_msgs::Int32ConstPtr &msg)
{
  double delay_time = GetGuassianNoise(_v2pDelay,_v2pDelay_noise_sigma);//ms
  std_msgs::Int32 pub_msg;
  pub_msg.data = msg.get()->data;
  double now_time = ros::Time::now().toNSec();//ns
  double next_time = now_time + delay_time*10e6;
  while(next_time - ros::Time::now().toNSec()>=0);
  _virtualGuideTypePub.publish(pub_msg);

}

void network_delay_model::reset_object_sub_cb(const std_msgs::Float64MultiArrayConstPtr &msg)
{
  _is_reset_object = true;
  _start_time_s = 0.0;
  _is_first_measure_data = true;

  _is_reset_object = false;
}

void network_delay_model::vObjPosSubCallback(const geometry_msgs::PoseStampedConstPtr &msg)
{
  if(!_is_reset_object)
  {

    if(_is_first_measure_data)
    {
      _start_time_s = ros::Time::now().toNSec()/10e9;
      _is_first_measure_data = false;
    }
    else
    {
        _current_time_s = ros::Time::now().toNSec()/10e9 - _start_time_s;

        dt_message_package::object_predict_position targetPositionMsg;
        targetPositionMsg.current_position_x = msg.get()->pose.position.x;
        targetPositionMsg.current_position_y = msg.get()->pose.position.y;
        targetPositionMsg.current_position_z = msg.get()->pose.position.z;
        targetPositionMsg.current_time = _current_time_s;
        for(int i=0;i<_predict_window;i++)
        {
          targetPositionMsg.position_xs.push_back(msg.get()->pose.position.x);
          targetPositionMsg.position_ys.push_back(msg.get()->pose.position.y);
          targetPositionMsg.position_zs.push_back(msg.get()->pose.position.z);
          targetPositionMsg.times.push_back(_current_time_s+(i+1)*_predict_time_dt);
        }
        double delay_time = GetGuassianNoise(_v2pDelay,_v2pDelay_noise_sigma);//ms
       // std::cout<<"delay_time: "<<delay_time<<std::endl;
        dt_message_package::object_move_position2 pub_msg;
        pub_msg.time_stamp_ms = ros::Time::now().toNSec()/10e6;
        pub_msg.delay_time_ms = delay_time;
        pub_msg.position_x = msg.get()->pose.position.x;
        pub_msg.position_y = msg.get()->pose.position.y;
        pub_msg.position_z = msg.get()->pose.position.z;
        double now_time = ros::Time::now().toNSec();//ns
        double next_time = now_time + delay_time*10e6;
        while(next_time - ros::Time::now().toNSec()>=0);
        _vObjPosPub.publish(pub_msg);
        _targetPositiongPub.publish(targetPositionMsg);
      }
    }
  }

void network_delay_model::pObjPosSubCallback(const geometry_msgs::PoseStampedConstPtr &msg)
{
  double delay_time = GetGuassianNoise(_p2vDelay,_p2vDelay_noise_sigma);//ms
  geometry_msgs::PoseStamped pub_msg;
  pub_msg.header.stamp = ros::Time::now();
  pub_msg.pose.position.x = msg.get()->pose.position.x;
  pub_msg.pose.position.y = msg.get()->pose.position.y;
  pub_msg.pose.position.z = msg.get()->pose.position.z;
  double now_time = ros::Time::now().toNSec();//ns
  double next_time = now_time + delay_time*10e6;
  while(next_time - ros::Time::now().toNSec()>=0);
  _pObjPosPub.publish(pub_msg);
}

void network_delay_model::v2pDelaySubCallback(const std_msgs::Float32ConstPtr &msg)
{
  _v2pDelay = msg.get()->data;
}

void network_delay_model::p2vDelaySubCallback(const std_msgs::Float32ConstPtr &msg)
{
  _p2vDelay = msg.get()->data;
}


double network_delay_model::GetGuassianNoise(double mu, double sigma)
{
  // generation of two normalized uniform random variables
  double U1 = static_cast<double>(rand_r(&_seed)) / static_cast<double>(RAND_MAX);
  double U2 = static_cast<double>(rand_r(&_seed)) / static_cast<double>(RAND_MAX);

  // using Box-Muller transform to obtain a varaible with a standard normal distribution
  double Z0 = sqrt(-2.0 * ::log(U1)) * cos(2.0*M_PI * U2);

  // scaling
  Z0 = sigma * Z0 + mu;
  return Z0;
}
