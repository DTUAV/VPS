#ifndef FUSION_CONTROLLER_H
#define FUSION_CONTROLLER_H
#include "iostream"
#include "geometry_msgs/TwistStamped.h"
#include "ros/ros.h"
#include "pthread.h"
#include "std_msgs/Int8.h"
#include <mutex>
#include "dt_message_package/part_guide_controller_info.h"
class fusion_controller
{
public:
  fusion_controller();
  void mpc_controller_output_sub_cb(const geometry_msgs::TwistStamped::ConstPtr& msg);
  void virtual_guide_output_sub_cb(const geometry_msgs::TwistStamped::ConstPtr& msg);
  void virtual_guide_control_sub_cb(const std_msgs::Int8::ConstPtr& msg);
  static void *guide_change_run(void* args);
private:
  std::string _mpc_controller_output_sub_topic;
  std::string _virtual_guide_output_sub_topic;
  std::string _target_output_pub_topic;
  std::string _virtual_guide_control_sub_topic;
  std::string _controller_info_pub_topic;

  ros::Subscriber _mpc_controller_output_sub;
  ros::Subscriber _virtual_guide_output_sub;
  ros::Subscriber _virtual_guide_control_sub;
  ros::Publisher _target_output_pub;
  ros::Publisher _controller_info_pub;

  float _virtual_guide_output_x;
  float _virtual_guide_output_y;
  float _virtual_guide_output_z;

  float _mpc_controller_output_x;
  float _mpc_controller_output_y;
  float _mpc_controller_output_z;

  float _target_cmd_x;
  float _target_cmd_y;
  float _target_cmd_z;

  float _maxVelocity;
  float _minVelocity;

  float _virtual_guide_hz;

  int _guide_type;
  bool _is_new_guide;

  pthread_t _guide_change_thread;
  std::mutex _data_mutex;

};

#endif // FUSION_CONTROLLER_H
