#ifndef MODEL_LEARN_WRAPPER_H
#define MODEL_LEARN_WRAPPER_H
#include "iostream"
#include "geometry_msgs/TwistStamped.h"
#include "geometry_msgs/Twist.h"
#include "ros/ros.h"
#include "dt_message_package/object_move_state.h"
class model_learn_wrapper
{
public:
  model_learn_wrapper();
  void controller_output_sub_cb(const geometry_msgs::TwistStampedConstPtr& msg);
  void est_move_state_sub_cb(const dt_message_package::object_move_stateConstPtr& msg);
private:
  std::string _controller_output_sub_topic;
  std::string _target_cmd_pub_topic;
  std::string _est_move_state_sub_topic;
  std::string _est_velocity_pub_topic;

  ros::Subscriber _controller_output_sub;
  ros::Subscriber _est_move_state_sub;
  ros::Publisher _est_velocity_pub;
  ros::Publisher _target_cmd_pub;

};

#endif // MODEL_LEARN_WRAPPER_H
