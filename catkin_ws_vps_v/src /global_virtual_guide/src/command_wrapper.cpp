#include "../include/global_virtual_guide/command_wrapper.h"

command_wrapper::command_wrapper()
{
  ros::NodeHandle n("~");
  n.getParam("system_running_id_sub_topic",_sysRunningIdSubTopic);
  n.getParam("guide_command_sub_topic",_guideCommandSubTopic);
  n.getParam("command_pub_topic",_commandPubTopic);
  n.getParam("controller_cmd_sub_topic",_controllerCmdSubTopic);
  n.getParam("is_simulator",_isSimulator);
  n.getParam("running_frequency",_runningFrequency);
  _sysRunningIdSub = n.subscribe(_sysRunningIdSubTopic,1,&command_wrapper::sysRunningIdSubCallback,this);
  _guideCommandSub = n.subscribe(_guideCommandSubTopic,1,&command_wrapper::guideCommandSubCallback,this);
  _controllerCmdSub = n.subscribe(_controllerCmdSubTopic,1,&command_wrapper::controllerCmdSubCallback,this);
  _commandPub = n.advertise<geometry_msgs::TwistStamped>(_commandPubTopic,1);
  if(_isSimulator)
  {
    _systemRunningId_1 = 1;
    _systemRunningId_2 = 2;
  }
  else
  {
    _systemRunningId_1 = 2;
    _systemRunningId_2 = 2;
  }

  _mpc_output_x = 0;
  _mpc_output_y = 0;
  _mpc_output_z = 0;

  _guide_output_x = 0;
  _guide_output_y = 0;
  _guide_output_z = 0;

  _runningId = 0;

  int flag_thread = pthread_create(&_runthread,NULL,&command_wrapper::run,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }

}

void *command_wrapper::run(void *arg)
{
  command_wrapper* command_wrapper_node = (command_wrapper*)(arg);

  ros::Rate rate(command_wrapper_node->_runningFrequency);
  geometry_msgs::TwistStamped pubMsg;
  while(ros::ok())
  {
    if(command_wrapper_node->_runningId==command_wrapper_node->_systemRunningId_1||command_wrapper_node->_runningId==command_wrapper_node->_systemRunningId_2)
    {
      pubMsg.header.stamp = ros::Time::now();
      pubMsg.twist.linear.x = command_wrapper_node->_guide_output_x;
      pubMsg.twist.linear.y = command_wrapper_node->_guide_output_y;
      pubMsg.twist.linear.z = command_wrapper_node->_guide_output_z;
      pubMsg.twist.angular.x = 0;
      pubMsg.twist.angular.y = 0;
      pubMsg.twist.angular.z = 0;
      command_wrapper_node->_commandPub.publish(pubMsg);
    }
    else
    {
      pubMsg.header.stamp = ros::Time::now();
      pubMsg.twist.linear.x = command_wrapper_node->_mpc_output_x;
      pubMsg.twist.linear.y = command_wrapper_node->_mpc_output_y;
      pubMsg.twist.linear.z = command_wrapper_node->_mpc_output_z;
      pubMsg.twist.angular.x = 0;
      pubMsg.twist.angular.y = 0;
      pubMsg.twist.angular.z = 0;
      command_wrapper_node->_commandPub.publish(pubMsg);
    }
    rate.sleep();
  }
}

void command_wrapper::controllerCmdSubCallback(const geometry_msgs::TwistStampedConstPtr &msg)
{
  _mpc_output_x = msg.get()->twist.linear.x;
  _mpc_output_y = msg.get()->twist.linear.y;
  _mpc_output_z = msg.get()->twist.linear.z;
}

void command_wrapper::guideCommandSubCallback(const geometry_msgs::TwistStampedConstPtr &msg)
{
  _guide_output_x = msg.get()->twist.linear.x;
  _guide_output_y = msg.get()->twist.linear.y;
  _guide_output_z = msg.get()->twist.linear.z;
}

void command_wrapper::sysRunningIdSubCallback(const std_msgs::Int32ConstPtr &msg)
{
  _runningId = msg.get()->data;
}
