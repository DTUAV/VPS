#ifndef UAV_MESSAGE_UNPACK_H
#define UAV_MESSAGE_UNPACK_H
#include <cloud_common/socket_data_packet_stamp_msg.h>
#include <cloud_common/define_common.h>
#include "std_msgs/Bool.h"
#include "geometry_msgs/PoseStamped.h"
#include "ros/ros.h"
#include "std_msgs/Float32.h"
#include "dt_message_package/update_model_parameter.h"
#include <boost/circular_buffer.hpp>
#include "dt_message_package/object_move_position2.h"
#include "geometry_msgs/TwistStamped.h"

using namespace DTUAVCARS;

class uav_message_unpack
{
public:
  uav_message_unpack();
  void IotMessageSubCallback(const cloud_common::socket_data_packet_stamp_msgConstPtr& msg);

private:
  double getMeanWithRemoveMaxMin(boost::circular_buffer<double> cicl_delay_time);
  ros::Subscriber _iot_msg_sub;

  ros::Publisher _local_position_msg_pub;
  ros::Publisher _velocity_msg_pub;
  ros::Publisher _update_model_msg_pub;
  ros::Publisher _v2p_delay_time_msg_pub;
  ros::Publisher _p2v_delay_time_msg_pub;
  ros::Publisher _object_move_position_pub;

  std::string _iot_msg_sub_topic;
  std::string _local_position_msg_pub_topic;
  std::string _velocity_msg_pub_topic;
  std::string _v2p_delay_time_msg_pub_topic;
  std::string _p2v_delay_time_msg_pub_topic;
  std::string _update_model_msg_pub_topic;
  std::string _object_move_position_pub_topic;

  float _dt_object_position_recv_hz;//The Configure Network Send And Receive Frency
  float _data_times;                                                                                  //The Interval Time of Send And Receive
  double _last_times;                                                                                  //The Last Receive Time(s)
  float _is_first;                                                                                    //The Flag for Indicate First Receive the Data from Network

  double _delay_time;

  double _p2v_delay_time;

  boost::circular_buffer<double> _cicl_delay_time;
  int _push_time;

};

#endif // R_UAV_MESSAGE_UNPACK_H
