#ifndef STATE_PREDICTOR_H
#define STATE_PREDICTOR_H
#include "ros/ros.h"
#include "least_square.h"
#include "dt_message_package/object_predict_position.h"
#include "dt_message_package/object_move_state.h"
#include "dt_message_package/predict_time_dt.h"
#include "dt_message_package/state_predictor_info.h"
#include "pthread.h"
#include <mutex>
#include <boost/circular_buffer.hpp>

struct predict_state_stu
{
  std::vector<float> times;
  std::vector<float> states;
};

class state_predictor
{
public:
  state_predictor();
  void system_state_sub_cb(const dt_message_package::object_move_stateConstPtr& msg);                                                                                                         //the callback function for object state from kalman filter
  void predict_time_dt_sub_cb(const dt_message_package::predict_time_dtConstPtr& msg);
  static void *predictor_running(void *args);                                                                                                                                                 //the running function for least square predict
  int event_trigger(std::vector<float> data_queue,float check_err);                                                                                                                           //the event trigger to determine the ways to predict system state
  int get_predict_window_size(int current_event_id,int last_predict_window_size,int max_window_size,int min_window_size);
  predict_state_stu static_motion(float current_times, std::vector<float> state, int predict_window, float predict_time_dt);
  predict_state_stu max_velocity_motion(float current_times,std::vector<float> state, int predict_window,float predict_time_dt,float max_velocity);
  predict_state_stu least_squares_motion(least_square* least_square_predictor,float current_times, std::vector<float> times,std::vector<float> state,int predict_window,float predict_time_dt);

  double GetMinValue(std::vector<float> data_queue);
  double GetMaxValue(std::vector<float> data_queue);

private:
  std::string _predict_positiong_pub_topic;                                                                                                                                                   //the topic name for publish predicted object position
  std::string _system_state_sub_topic;                                                                                                                                                        //the topic name for subscribe system state from kalman filter
  std::string _predict_time_dt_sub_topic;
  std::string _state_predictor_info_pub_topic;


  ros::Subscriber _system_state_sub;                                                                                                                                                          //the ros subscriber for system state from kalman filter
  ros::Subscriber _predict_time_dt_sub;

  ros::Publisher _predict_position_pub;                                                                                                                                                       //the ros publisher for predicted position from least square
  ros::Publisher _state_predictor_info_pub;

  float _predictor_run_hz;                                                                                                                                                                    //the running frequency for least square predictor

  int _save_data_window;                                                                                                                                                                      //the window size for save data from kalman filter

  int _predict_window;                                                                                                                                                                        //the window size for predicting further system state

  int _max_predict_data_window;                                                                                                                                                               //the max window size for predicting further system state
  int _min_predict_data_window;                                                                                                                                                               //the min window size for predicting further system state

  double _predict_time_dt_x;
  double _predict_time_dt_y;
  double _predict_time_dt_z;

  int _predict_data_x_window;                                                                                                                                                                 //the window size of state data_x for least square predictor
  int _predict_data_y_window;
  int _predict_data_z_window;

  int _x_event_id;
  int _y_event_id;
  int _z_event_id;

  int _predict_order;                                                                                                                                                                         //the predicted function order
  float _predict_time_dt;                                                                                                                                                                     //the predicted time step for least square

  float _queue_check_error;                                                                                                                                                                   //The error value to check the system state queue to used different predict ways

  float _max_velocity_x;                                                                                                                                                                      //The maximum velocity of motion direction of x
  float _max_velocity_y;                                                                                                                                                                      //The maximum velocity of motion direction of y
  float _max_velocity_z;                                                                                                                                                                      //The maximum velocity of motion direction of z

  boost::circular_buffer<double> _cicl_queue_t;                                                                                                                                               //the circular queue for save all time data from kalman filter
  boost::circular_buffer<double> _cicl_queue_x;                                                                                                                                               //the circular queue for save all position x data from kalman filter
  boost::circular_buffer<double> _cicl_queue_y;                                                                                                                                               //the circular queue for save all position y data from kalman filter
  boost::circular_buffer<double> _cicl_queue_z;                                                                                                                                               //the circular queue for save all position z data from kalman filter

  std::vector<float> _running_data_t;                                                                                                                                                         //the vector for save time data to predict system state
  std::vector<float> _running_data_x_t;
  std::vector<float> _running_data_y_t;
  std::vector<float> _running_data_z_t;
  std::vector<float> _running_data_x;                                                                                                                                                         //the vector for save position x data to predict system state
  std::vector<float> _running_data_y;                                                                                                                                                         //the vector for save position y data to predict system state
  std::vector<float> _running_data_z;                                                                                                                                                         //the vector for save position z data to predict system state

  least_square *_predictor_x;                                                                                                                                                                 //the least square predictor for position x
  least_square *_predictor_y;                                                                                                                                                                 //the least square predictor for position y
  least_square *_predictor_z;                                                                                                                                                                 //the least square predictor for position z



  pthread_t _predictor_run_thread;
  std::mutex _data_mutex;

};

#endif // STATE_PREDICTOR_H
