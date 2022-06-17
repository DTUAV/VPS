#ifndef STATE_ESTIMATOR_H
#define STATE_ESTIMATOR_H
#include "kf.h"
#include "ros/ros.h"
#include "iostream"
#include "geometry_msgs/PoseStamped.h"
#include "dt_message_package/object_move_state.h"
#include "dt_message_package/object_move_position2.h"
#include "dt_message_package/state_estimator_info.h"
#include "pthread.h"
#include <mutex>
class state_estimator
{
public:
  state_estimator();
  bool InitKf(int state_dim,int observe_dim,int input_dim);
  bool InitSystemModel(Eigen::MatrixXf A,Eigen::MatrixXf B,Eigen::MatrixXf Q);
  bool InitObserveModel(Eigen::MatrixXf H,Eigen::MatrixXf R);
  bool InitEstimateErr(Eigen::MatrixXf P);
  bool InitControlInput(Eigen::MatrixXf U);
  bool InitObserveData(Eigen::MatrixXf Y);
  bool InitSystemState(Eigen::MatrixXf X);
  bool InitModelErr(float model_err);
  bool InitMeasureErr(float measure_err);
  bool UpdateModelErr(float model_err);
  bool UpdateMeasureErr(float measure_err);
  bool UpdateControlInput(Eigen::MatrixXf U);
  bool UpdateObserveData(Eigen::MatrixXf Y);
  bool UpdateModelErrMatrix(Eigen::MatrixXf Q);
  bool UpdateObserveErrMatrix(Eigen::MatrixXf R);
  Eigen::MatrixXf GetSystemState();
  Eigen::MatrixXf GetSystemPredictState();

  bool CoutMeasureErrAndUpdate(double delay_time_s);

  static void *kf_run(void* args);

  void dt_obj_position_sub_cb(const dt_message_package::object_move_position2ConstPtr msg);
private:

  float _modelErr;
  float _measureErr;
  float _updateMeasureErrK;

  bool _isInitModelErr;
  bool _isInitMeasureErr;
  bool _isInitKf;
  bool _isInitSystemModel;
  bool _isInitObserveModel;
  bool _isInitEstimateErr;
  bool _isInitControlInput;
  bool _isInitObserveData;
  bool _isInitSystemState;

  std::string _dt_obj_position_sub_topic;
  std::string _est_move_state_pub_topic;
  std::string _kf_information_pub_topic;
  std::string _kf_run_hz;

  ros::Subscriber _dt_obj_position_sub;

  ros::Publisher _est_move_state_pub;
  ros::Publisher _kf_information_pub;


  kf *_kf_estimator;
  int _state_dim;//the dimension of system state
  int _observe_dim;//the dimension of observer state
  int _input_dim;//the dimension of control input

  std::mutex _data_mutex;
  pthread_t _kf_run_thread;

  float _run_hz;

  Eigen::MatrixXf _observe_data;
  Eigen::MatrixXf _control_input_data;

  double _current_time_s;
  double _start_time_s;

  bool _is_first_measure_data;

};

#endif // STATE_ESTIMATOR_H
