#include "../include/global_virtual_guide/model_learn_wrapper.h"

model_learn_wrapper::model_learn_wrapper()
{
   ros::NodeHandle n("~");
   n.getParam("controller_output_sub_topic",_controller_output_sub_topic);
   n.getParam("target_cmd_pub_topic",_target_cmd_pub_topic);
   n.getParam("est_move_state_sub_topic",_est_move_state_sub_topic);
   n.getParam("est_velocity_pub_topic",_est_velocity_pub_topic);
   _controller_output_sub = n.subscribe(_controller_output_sub_topic,1,&model_learn_wrapper::controller_output_sub_cb,this);
   _target_cmd_pub = n.advertise<geometry_msgs::Twist>(_target_cmd_pub_topic,1);

   _est_move_state_sub = n.subscribe(_est_move_state_sub_topic,1,&model_learn_wrapper::est_move_state_sub_cb,this);
   _est_velocity_pub = n.advertise<geometry_msgs::TwistStamped>(_est_velocity_pub_topic,1);
}

void model_learn_wrapper::controller_output_sub_cb(const geometry_msgs::TwistStampedConstPtr &msg)
{
  geometry_msgs::Twist targetVelocityMsg;
  targetVelocityMsg.linear.x = msg.get()->twist.linear.x;
  targetVelocityMsg.linear.y = msg.get()->twist.linear.y;
  targetVelocityMsg.linear.z = msg.get()->twist.linear.z;
  _target_cmd_pub.publish(targetVelocityMsg);
}

void model_learn_wrapper::est_move_state_sub_cb(const dt_message_package::object_move_stateConstPtr &msg)
{
  geometry_msgs::TwistStamped estVelocityMsg;
  estVelocityMsg.header.frame_id = "physical_uav";
  estVelocityMsg.header.stamp = ros::Time::now();
  estVelocityMsg.twist.linear.x = msg.get()->velocity_x;
  estVelocityMsg.twist.linear.y = msg.get()->velocity_y;
  estVelocityMsg.twist.linear.z = msg.get()->velocity_z;
  _est_velocity_pub.publish(estVelocityMsg);
}
