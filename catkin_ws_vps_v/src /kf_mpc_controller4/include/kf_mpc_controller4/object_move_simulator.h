#ifndef OBJECT_MOVE_SIMULATOR_H
#define OBJECT_MOVE_SIMULATOR_H
#include "Eigen/Eigen"
#include "ros/ros.h"
#include "geometry_msgs/TwistStamped.h"
#include "geometry_msgs/PoseStamped.h"
#include "object_move.h"
#include "pthread.h"
#include <mutex>
#include "std_msgs/Float64MultiArray.h"
#include "std_msgs/Bool.h"
class object_move_simulator
{
public:
  object_move_simulator();
  object_move_simulator(int state_dim, int observe_dim, int input_dim);
  void object_input_sub_cb(const geometry_msgs::TwistStampedConstPtr& msg);
  void reset_object_sub_cb(const std_msgs::Float64MultiArrayConstPtr& msg);
  static void *simulator_running(void *args);
  bool Init_Object_Model(Eigen::MatrixXf A,Eigen::MatrixXf B,Eigen::MatrixXf H);
  bool Init_Object_State(Eigen::MatrixXf X,Eigen::MatrixXf Y,Eigen::MatrixXf U);
  bool Init_Noise(Eigen::MatrixXf W,Eigen::MatrixXf V);
  bool Update_Model_Input(Eigen::MatrixXf U);
  bool Update_Noise(Eigen::MatrixXf W,Eigen::MatrixXf V);
  bool Update_Object();
  bool ResetObject(Eigen::MatrixXf X);
  double GetGuassianNoise(double mu, double sigma);
  Eigen::MatrixXf Get_Sensor_Data();


private:

  std::string _object_input_sub_topic;
  std::string _reset_object_sub_topic;

  std::string _sensor_data_pub_topic;
  std::string _start_simulator_pub_topic;

  ros::Publisher _sensor_data_pub;
  ros::Publisher _start_simulator_pub;

  ros::Subscriber _object_input_sub;
  ros::Subscriber _reset_object_sub;

  float _simulator_run_hz;
  std::mutex _data_mutex;
  pthread_t _simulator_run_thread;

  int _state_dim;//the dimension of system state
  int _observe_dim;//the dimension of observer state
  int _input_dim;//the dimension of control input

  object_move *_object;

  bool _isInitObjectModel;
  bool _isInitObjectState;
  bool _isInitNoise;
  bool _isResetObject;

  float _model_noise_mu;
  float _model_noise_sigma;
  float _sensor_noise_mu;
  float _sensor_noise_sigma;

  unsigned int _seed;

};

#endif // OBJECT_MOVE_SIMULATOR_H
