#ifndef COMMAND_WRAPPER_H
#define COMMAND_WRAPPER_H
#include "iostream"
#include "ros/ros.h"
#include "geometry_msgs/TwistStamped.h"
#include "std_msgs/Int32.h"
#include "pthread.h"
class command_wrapper
{
public:
  command_wrapper();
  void sysRunningIdSubCallback(const std_msgs::Int32ConstPtr& msg);
  void guideCommandSubCallback(const geometry_msgs::TwistStampedConstPtr& msg);
  void controllerCmdSubCallback(const geometry_msgs::TwistStampedConstPtr& msg);
  static void *run(void *arg);
private:
  std::string _sysRunningIdSubTopic;
  std::string _guideCommandSubTopic;
  std::string _controllerCmdSubTopic;
  std::string _commandPubTopic;

  ros::Publisher _commandPub;

  ros::Subscriber _sysRunningIdSub;
  ros::Subscriber _guideCommandSub;
  ros::Subscriber _controllerCmdSub;

  bool _isSimulator;

  int _systemRunningId_1;
  int _systemRunningId_2;

  int _runningId;

  float _runningFrequency;

  pthread_t _runthread;

  float _mpc_output_x;
  float _mpc_output_y;
  float _mpc_output_z;

  float _guide_output_x;
  float _guide_output_y;
  float _guide_output_z;

};

#endif // COMMAND_WRAPPER_H
