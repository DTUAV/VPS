#ifndef KF_MPC_CONTROLLER_WRAPPER_H
#define KF_MPC_CONTROLLER_WRAPPER_H
#include "iostream"
#include "dt_message_package/object_move_position2.h"
#include "geometry_msgs/PoseStamped.h"
#include "ros/ros.h"
#include "sys/time.h"

class kf_mpc_controller_wrapper
{
public:
  kf_mpc_controller_wrapper();
  void px4_vihecle_pose_sub_cb(const geometry_msgs::PoseStampedConstPtr& msg);
private:
  std::string _px4_vihecle_pose_sub_topic;

  std::string _object_move_position_pub_topic;

  ros::Subscriber _px4_vihecle_pose_sub;

  ros::Publisher _object_move_position_pub;

};

#endif // KF_MPC_CONTROLLER_WRAPPER_H
