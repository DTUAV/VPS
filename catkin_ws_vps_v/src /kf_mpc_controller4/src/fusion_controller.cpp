#include "../include/kf_mpc_controller4/fusion_controller.h"

fusion_controller::fusion_controller()
{
  ros::NodeHandle n("~");
  n.getParam("mpc_controller_output_sub_topic",_mpc_controller_output_sub_topic);
  n.getParam("virtual_guide_output_sub_topic",_virtual_guide_output_sub_topic);
  n.getParam("target_velocity_pub_topic",_target_output_pub_topic);
  n.getParam("virtual_guide_control_sub_topic",_virtual_guide_control_sub_topic);
  n.getParam("virtual_guide_hz",_virtual_guide_hz);
  n.getParam("min_velocity",_minVelocity);
  n.getParam("max_velocity",_maxVelocity);
  _mpc_controller_output_sub = n.subscribe(_mpc_controller_output_sub_topic,1,&fusion_controller::mpc_controller_output_sub_cb,this);
  _virtual_guide_output_sub = n.subscribe(_virtual_guide_output_sub_topic,1,&fusion_controller::virtual_guide_output_sub_cb,this);
  _virtual_guide_control_sub = n.subscribe(_virtual_guide_control_sub_topic,1,&fusion_controller::virtual_guide_control_sub_cb,this);
  _target_output_pub = n.advertise<geometry_msgs::TwistStamped>(_target_output_pub_topic,1);

  _virtual_guide_output_x = 0;
  _virtual_guide_output_y = 0;
  _virtual_guide_output_z = 0;

  _target_cmd_x = 0;
  _target_cmd_y = 0;
  _target_cmd_z = 0;

  _mpc_controller_output_x = 0;
  _mpc_controller_output_y = 0;
  _mpc_controller_output_z = 0;

  _guide_type = 0;//0:no guide 1: part guide 2:all guide
  _is_new_guide = false;
  int flag_thread = pthread_create(&_guide_change_thread,NULL,&fusion_controller::guide_change_run,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }
}

void fusion_controller::virtual_guide_control_sub_cb(const std_msgs::Int32::ConstPtr &msg)
{
  _guide_type = msg.get()->data;

}

void fusion_controller::mpc_controller_output_sub_cb(const geometry_msgs::TwistStamped::ConstPtr &msg)
{

  _mpc_controller_output_x = msg.get()->twist.linear.x;
  _mpc_controller_output_y = msg.get()->twist.linear.y;
  _mpc_controller_output_z = msg.get()->twist.linear.z;

}

void fusion_controller::virtual_guide_output_sub_cb(const geometry_msgs::TwistStamped::ConstPtr &msg)
{
  _virtual_guide_output_x = msg.get()->twist.linear.x;
  _virtual_guide_output_y = msg.get()->twist.linear.y;
  _virtual_guide_output_z = msg.get()->twist.linear.z;
  {
    std::lock_guard<std::mutex> lock(_data_mutex);
    _is_new_guide = true;
  }
}

void *fusion_controller::guide_change_run(void *args)
{
  fusion_controller* fusion = (fusion_controller*)(args);
  ros::Rate rate(fusion->_virtual_guide_hz);
  geometry_msgs::TwistStamped targetVelocityMsg;
  while(ros::ok())
  {
    switch (fusion->_guide_type)
    {
    case 0:
    {
      fusion->_target_cmd_x = fusion->_mpc_controller_output_x;
      fusion->_target_cmd_y = fusion->_mpc_controller_output_y;
      fusion->_target_cmd_z = fusion->_mpc_controller_output_z;
    }
      break;
    case 1:
    {
      if(fusion->_is_new_guide)
      {
        fusion->_target_cmd_x =  fusion->_mpc_controller_output_x+fusion->_virtual_guide_output_x;
        fusion->_target_cmd_y =  fusion->_mpc_controller_output_y+fusion->_virtual_guide_output_y;
        fusion->_target_cmd_z =  fusion->_mpc_controller_output_z+fusion->_virtual_guide_output_z;
        {
          std::lock_guard<std::mutex> lock(fusion->_data_mutex);
          fusion->_is_new_guide = false;
        }
      }
      else
      {
        fusion->_target_cmd_x = fusion->_mpc_controller_output_x;
        fusion->_target_cmd_y = fusion->_mpc_controller_output_y;
        fusion->_target_cmd_z = fusion->_mpc_controller_output_z;
      }
    }
      break;
    case 2:
    {
      if(fusion->_is_new_guide)
      {
        fusion->_target_cmd_x = fusion->_virtual_guide_output_x;
        fusion->_target_cmd_y = fusion->_virtual_guide_output_y;
        fusion->_target_cmd_z = fusion->_virtual_guide_output_z;

        {
          std::lock_guard<std::mutex> lock(fusion->_data_mutex);
          fusion->_is_new_guide = false;
        }
      }
      else
      {
        fusion->_target_cmd_x = 0;
        fusion->_target_cmd_y = 0;
        fusion->_target_cmd_z = 0;
      }
    }
      break;
    default:
      break;
    }

    fusion->_target_cmd_x = fusion->_target_cmd_x<fusion->_minVelocity?fusion->_minVelocity:fusion->_target_cmd_x>fusion->_maxVelocity?fusion->_maxVelocity:fusion->_target_cmd_x;
    fusion->_target_cmd_y = fusion->_target_cmd_y<fusion->_minVelocity?fusion->_minVelocity:fusion->_target_cmd_y>fusion->_maxVelocity?fusion->_maxVelocity:fusion->_target_cmd_y;
    fusion->_target_cmd_z = fusion->_target_cmd_z<fusion->_minVelocity?fusion->_minVelocity:fusion->_target_cmd_z>fusion->_maxVelocity?fusion->_maxVelocity:fusion->_target_cmd_z;

    targetVelocityMsg.header.stamp = ros::Time::now();
    targetVelocityMsg.twist.linear.x = fusion->_target_cmd_x;
    targetVelocityMsg.twist.linear.y = fusion->_target_cmd_y;
    targetVelocityMsg.twist.linear.z = fusion->_target_cmd_z;
    fusion->_target_output_pub.publish(targetVelocityMsg);
    rate.sleep();
  }
}

























