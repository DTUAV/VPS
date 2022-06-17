#ifndef PX4_VELOCITY_CONTROL_WRAPPER_H
#define PX4_VELOCITY_CONTROL_WRAPPER_H
#include <pthread.h>
#include "geometry_msgs/Twist.h"
#include "geometry_msgs/TwistStamped.h"
#include "mavros_msgs/RCIn.h"
#include "mavros_msgs/State.h"
#include <mavros_msgs/CommandBool.h>
#include <mavros_msgs/SetMode.h>
#include <std_msgs/Bool.h>
#include "ros/ros.h"
#include "iostream"
class px4_velocity_rc_control_wrapper
{
public:
  px4_velocity_rc_control_wrapper();
  void tgt_vel_msg_sub_cb(const geometry_msgs::TwistStampedConstPtr& msg);
  void px4_rc_msg_sub_cb(const mavros_msgs::RCInConstPtr& msg);
  static void *offboard_run(void *args);
private:
  std::string _px4_ref_vel_msg_pub_topic;//The Message will Publish to PX4
  std::string _tgt_vel_msg_sub_topic;//The Message from Other Controller
  std::string _px4_rc_msg_sub_topic;//The Message from PX4--about the rc information of px4


  ros::Subscriber _tgt_vel_msg_sub;//The Subscriber for Subscribe Other Control Publish Target Velocity Message
  ros::Subscriber _px4_rc_msg_sub;//The Subscriber for Subscribe RC Information

  ros::Publisher _px4_ref_vel_msg_pub;//The Publisher publish the target velocity to PX4


  int _rc_start_ch;//The RC Channel for Start Arm Vehecle and Enter Offboard Control Mode
  int _rc_land_ch;//The RC Channel for Land the Vehecle

  float _ref_velocity_x;//The Target Value of Velocity X
  float _ref_velocity_y;//The Target Value of Velocity Y
  float _ref_velocity_z;//The Target Value of Velocity Z

  bool _is_land_flag;//The Flag for Land Vehicle
  bool _is_start_other_control_flag;

  geometry_msgs::Twist _px4_ref_vel;


  pthread_t _offboard_control_run;
  float _run_frequen_hz;


};

#endif // PX4_VELOCITY_CONTROL_WRAPPER_H












