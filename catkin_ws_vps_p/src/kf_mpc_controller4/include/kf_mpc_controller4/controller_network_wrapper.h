#ifndef CONTROLLER_NETWORK_WRAPPER_H
#define CONTROLLER_NETWORK_WRAPPER_H
#include "ros/ros.h"
#include "cloud_common/define_common.h"
#include "cloud_common/socket_data_packet_stamp_msg.h"
#include "dt_message_package/object_move_position2.h"
#include "geometry_msgs/TwistStamped.h"
#include "std_msgs/Int8.h"
#include "sys/time.h"
#include "dt_message_package/network_delay_time.h"
#include "pthread.h"
#include <mutex>
#include <boost/circular_buffer.hpp>
#include "dt_message_package/object_predict_position.h"
#include "std_msgs/Bool.h"
class controller_network_wrapper
{
public:
  controller_network_wrapper();
  void network_message_sub_cb(const cloud_common::socket_data_packet_stamp_msgConstPtr& msg);         //The Network Message Callback Function

  static void *network_delay_pub(void* args);
private:

  double getMeanWithRemoveMaxMin(boost::circular_buffer<double> cicl_delay_time);


  std::string _network_message_sub_topic;                                                             //The Message Subscribe Topic Name of Network Message
  std::string _dt_object_position_pub_topic;                                                          //The Message Advertise Topic Name of Object Position
  std::string _virtual_guide_pub_topic;
  std::string _virtual_guide_control_pub_topic;
  std::string _network_delay_pub_topic;
  std::string _targetPositionPubTopic;
  std::string _model_learner_control_pub_topic;

  boost::circular_buffer<double> _cicl_delay_time;

  float _dt_object_position_recv_hz;//The Configure Network Send And Receive Frency
  float _network_delay_pub_hz;
  float _data_times;                                                                                  //The Interval Time of Send And Receive
  double _last_times;                                                                                  //The Last Receive Time(s)
  float _is_first;                                                                                    //The Flag for Indicate First Receive the Data from Network

  double _delay_time;

  bool _is_unity;
  int _push_time;
  bool _start_delay_pub;

  int _predict_window;
  double _predict_time_dt;

  double _current_time_s;
  double _start_time_s;

  ros::Subscriber _network_message_sub;                                                               //The Ros Subscriber of Network Message Receive
  ros::Publisher _model_learner_control_pub;
  ros::Publisher _dt_object_position_pub;                                                             //The Ros Publisher of Object Position
  ros::Publisher _virtual_guide_pub;
  ros::Publisher _virtual_guide_control_pub;
  ros::Publisher _network_delay_pub;
  ros::Publisher _targetPositiongPub;


  pthread_t _delay_pub_thread;
  std::mutex _data_mutex;
};

#endif // CONTROLLER_NETWORK_WRAPPER_H
