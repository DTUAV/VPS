#ifndef MPC_CONTROLLER_H
#define MPC_CONTROLLER_H
#include "Eigen/Eigen"
#include "ros/ros.h"
#include "mpc.h"
#include "dt_message_package/object_predict_position.h"
#include "dt_message_package/update_model_parameter.h"
#include "pthread.h"
#include <mutex>
#include "geometry_msgs/PoseStamped.h"
#include "geometry_msgs/TwistStamped.h"
#include "dt_message_package/predict_time_dt.h"
#include "std_msgs/Float64MultiArray.h"
#include "std_msgs/Int32.h"
class mpc_controller
{
public:
  mpc_controller();
  void current_position_sub_cb(const geometry_msgs::PoseStampedConstPtr& msg);
  void target_position_sub_cb(const dt_message_package::object_predict_positionConstPtr& msg);
  void update_model_parameter_sub_cb(const dt_message_package::update_model_parameterConstPtr& msg);
  void reset_object_sub_cb(const std_msgs::Float64MultiArrayConstPtr& msg);
  void system_running_control_sub_cb(const std_msgs::Int32ConstPtr& msg);

  static void *controller_running(void *args);

  void Init_System_Model(Eigen::MatrixXf A,Eigen::MatrixXf B,Eigen::MatrixXf H,Eigen::MatrixXf omega);
  void Init_System_State(Eigen::MatrixXf X,Eigen::MatrixXf XX);
  void Init_Control_Output(Eigen::MatrixXf U,float max_output,float min_output);
  void Init_MPC_Proble(Eigen::MatrixXf R,Eigen::MatrixXf M);
  void Init_Other_Matrix(Eigen::MatrixXf F, Eigen::MatrixXf PHI,Eigen::MatrixXf W,Eigen::MatrixXf G);

  void Update_Feedback(Eigen::MatrixXf X);
  void Update_Target(Eigen::MatrixXf XX);
  void Update_Controller();
  Eigen::MatrixXf Get_Controller_Results();

private:
  std::string _current_position_sub_topic;//the topic name for subscribe the object current position from meansure sensor
  std::string _target_position_sub_topic;//the topic name for subscribe the object target position from state predictor
  std::string _update_model_parameter_sub_topic;
  std::string _reset_object_sub_topic;
  std::string _system_running_control_sub_topic;

  std::string _controller_output_pub_topic;//the topic name for publish the controller output to inner controller
  std::string _predict_time_dt_pub_topic;//the topic name for publish the controller predict time(B matrix)


  ros::Subscriber _current_position_sub;//the subscriber for object current position
  ros::Subscriber _target_position_sub;//the subscriber for object target position
  ros::Subscriber _update_model_parameter_sub;
  ros::Subscriber _reset_object_sub;
  ros::Subscriber _system_running_control_sub;



  ros::Publisher _controller_output_pub;//the publisher for controller output
  ros::Publisher _predict_time_dt_pub;//the publisher for controller predict time

  float _controller_running_hz;//the running frequency for mpc controller

  std::mutex _feedback_mutex;
  std::mutex _target_mutex;
  pthread_t _mpc_run_thread;

  mpc *controller;

  int _state_dim;//the dimension of system state
  int _observe_dim;//the dimension of observer state
  int _input_dim;//the dimension of control input
  int _control_window;//the window size of control input
  int _predict_window;//the window size of system state predicted




  bool _isInitSystemModel;
  bool _isInitSystemState;
  bool _isInitControlOutput;
  bool _isInitMpcProble;
  bool _isInitOtherMatrix;

  int _system_running_id;

  float _reset_position_x;
  float _reset_position_y;
  float _reset_position_z;


};

#endif // MPC_CONTROLLER_H
