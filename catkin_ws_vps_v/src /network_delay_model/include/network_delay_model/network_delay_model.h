#ifndef NETWORK_DELAY_MODEL_H
#define NETWORK_DELAY_MODEL_H
#include "iostream"
#include "ros/ros.h"
#include "geometry_msgs/PoseStamped.h"
#include "dt_message_package/object_move_position2.h"
#include "std_msgs/Float32.h"
#include "std_msgs/Int32.h"
#include "std_msgs/Float64MultiArray.h"
#include "geometry_msgs/TwistStamped.h"
#include "dt_message_package/object_predict_position.h"
class network_delay_model
{
public:
  network_delay_model();
  void vObjPosSubCallback(const geometry_msgs::PoseStampedConstPtr& msg);
  void pObjPosSubCallback(const geometry_msgs::PoseStampedConstPtr& msg);
  void v2pDelaySubCallback(const std_msgs::Float32ConstPtr& msg);
  void p2vDelaySubCallback(const std_msgs::Float32ConstPtr& msg);
  void virtualGuideTypeSubCallback(const std_msgs::Int32ConstPtr& msg);
  void virtualGuideCmdSubCallback(const geometry_msgs::TwistStampedConstPtr& msg);
  void physicalUavPositionSubCallback(const geometry_msgs::PoseStampedConstPtr& msg);
  double GetGuassianNoise(double mu, double sigma);
  void reset_object_sub_cb(const std_msgs::Float64MultiArrayConstPtr& msg);
private:
  std::string _vObjPosSubTopic;
  std::string _vObjPosPubTopic;
  std::string _pObjPosSubTopic;
  std::string _pObjPosPubTopic;
  std::string _v2pDelaySubTopic;
  std::string _p2vDelaySubTopic;
  std::string _reset_object_sub_topic;
  std::string _targetPositionPubTopic;

  std::string _virtualGuideTypeSubTopic;
  std::string _virtualGuideCmdSubTopic;
  std::string _virtualGuideTypePubTopic;
  std::string _virtualGuideCmdPubTopic;

  std::string _physicalUavPositionSubTopic;
  std::string _physicalUavPositionPubTopic;
  std::string _physicalUavMoveStatePubTopic;

  ros::Subscriber _virtualGuideTypeSub;
  ros::Subscriber _virtualGuideCmdSub;
  ros::Subscriber _physicalUavPositionSub;

  ros::Publisher _virtualGuideTypePub;
  ros::Publisher _virtualGuideCmdPub;
  ros::Publisher _physicalUavPositionPub;
  ros::Publisher _physicalUavMoveStatePub;

  ros::Subscriber _vObjPosSub;
  ros::Subscriber _pObjPosSub;
  ros::Subscriber _v2pDelaySub;
  ros::Subscriber _p2vDelaySub;
  ros::Subscriber _reset_object_sub;

  ros::Publisher _vObjPosPub;
  ros::Publisher _pObjPosPub;
  ros::Publisher _targetPositiongPub;

  double _v2pDelay_noise_mu;
  double _v2pDelay_noise_sigma;

  double _p2vDelay_noise_mu;
  double _p2vDelay_noise_sigma;

  double _v2pDelay;//The delay time from virtual object to physical object
  double _p2vDelay;//The delay time from physical object to virtual object

  unsigned int _seed;

  bool _is_first_measure_data;
  bool _is_first_p_measure_data;
  bool _is_reset_object;
  double _current_time_s;
  double _p_current_time_s;
  double _start_time_s;
  double _p_start_time_s;
  int _predict_window;
  double _predict_time_dt;
};

#endif // NETWORK_DELAY_MODEL_H
