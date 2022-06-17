#include "../include/px4_control_wrapper/px4_velocity_rc_control_wrapper.h"

void px4_velocity_rc_control_wrapper::tgt_vel_msg_sub_cb(const geometry_msgs::TwistStampedConstPtr &msg)
{
  if(!_is_land_flag&&_is_start_other_control_flag)
  {
    _ref_velocity_x = msg.get()->twist.linear.x;
    _ref_velocity_y = msg.get()->twist.linear.y;
    _ref_velocity_z = msg.get()->twist.linear.z;
  }
  else
  {
    _ref_velocity_x = 0;
    _ref_velocity_y = 0;
    _ref_velocity_z = 0;
  }
}

void px4_velocity_rc_control_wrapper::px4_rc_msg_sub_cb(const mavros_msgs::RCInConstPtr &msg)
{
  if(msg.get()->channels.at(_rc_start_ch)>1800&&msg.get()->channels.at(_rc_start_ch)<2000)
  {
   _is_start_other_control_flag = true;
  }
  else
  {
    _is_start_other_control_flag = false;
  }
  if(msg.get()->channels.at(_rc_land_ch)>1800&&msg.get()->channels.at(_rc_land_ch)<2000)
  {
    _is_land_flag = true;
    _ref_velocity_x = 0;
    _ref_velocity_y = 0;
    _ref_velocity_z = -0.6;
  }
  else
  {
    _is_land_flag = false;
  }

}

void *px4_velocity_rc_control_wrapper::offboard_run(void *args)
{
  px4_velocity_rc_control_wrapper* control_wrapper = (px4_velocity_rc_control_wrapper*)(args);
  ros::Rate rate(20);
  while(ros::ok())
  {
      control_wrapper->_px4_ref_vel.linear.x = control_wrapper->_ref_velocity_x;
      control_wrapper->_px4_ref_vel.linear.y = control_wrapper->_ref_velocity_y;
      control_wrapper->_px4_ref_vel.linear.z = control_wrapper->_ref_velocity_z;
      control_wrapper->_px4_ref_vel_msg_pub.publish(control_wrapper->_px4_ref_vel);
      rate.sleep();

  }
  pthread_join(control_wrapper->_offboard_control_run,NULL);
}

px4_velocity_rc_control_wrapper::px4_velocity_rc_control_wrapper()
{
  ros::NodeHandle n("~");
  n.getParam("px4_ref_vel_msg_pub_topic",_px4_ref_vel_msg_pub_topic);
  n.getParam("tgt_vel_msg_sub_topic",_tgt_vel_msg_sub_topic);
  n.getParam("px4_rc_msg_sub_topic",_px4_rc_msg_sub_topic);
  n.getParam("run_frequen_hz",_run_frequen_hz);
  n.getParam("rc_start_ch",_rc_start_ch);
  n.getParam("rc_land_ch",_rc_land_ch);

  _tgt_vel_msg_sub = n.subscribe(_tgt_vel_msg_sub_topic,1,&px4_velocity_rc_control_wrapper::tgt_vel_msg_sub_cb,this);
  _px4_rc_msg_sub = n.subscribe(_px4_rc_msg_sub_topic,1,&px4_velocity_rc_control_wrapper::px4_rc_msg_sub_cb,this);

  _px4_ref_vel_msg_pub = n.advertise<geometry_msgs::Twist>(_px4_ref_vel_msg_pub_topic,1);

  _px4_ref_vel.linear.x = 0;
  _px4_ref_vel.linear.y = 0;
  _px4_ref_vel.linear.z = 0;

  _ref_velocity_x = 0;
  _ref_velocity_y = 0;
  _ref_velocity_z = 0;

  _is_start_other_control_flag = false;
  _is_land_flag = false;


  int flag_thread = pthread_create(&_offboard_control_run,NULL,&px4_velocity_rc_control_wrapper::offboard_run,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }
}
