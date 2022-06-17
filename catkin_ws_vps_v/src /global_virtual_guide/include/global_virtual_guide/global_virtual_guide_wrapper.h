#ifndef GLOBAL_VIRTUAL_GUIDE_WRAPPER_H
#define GLOBAL_VIRTUAL_GUIDE_WRAPPER_H
#include "dt_message_package/object_predict_position.h"
#include "iostream"
#include "ros/ros.h"
#include "geometry_msgs/PoseStamped.h"

class global_virtual_guide_wrapper
{
public:
  global_virtual_guide_wrapper();
  void pObjPosSubCallback(const geometry_msgs::PoseStampedConstPtr& msg);

private:
  std::string _pObjPosSubTopic;
  std::string _targetPositionPubTopic;

  ros::Subscriber _pObjPosSub;

  ros::Publisher _targetPositiongPub;

  int _predict_window;
  double _predict_time_dt;

  bool _is_first_measure_data;
  double _current_time_s;
  double _start_time_s;

};

#endif // GLOBAL_VIRTUAL_GUIDE_WRAPPER_H
