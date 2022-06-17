#ifndef UWB_WRAPPER_H
#define UWB_WRAPPER_H
#include "ros/ros.h"
#include "iostream"
#include "geometry_msgs/PoseStamped.h"
#include "nlink_parser/LinktrackNodeframe2.h"
#include "sensor_msgs/ChannelFloat32.h"
#include "sensor_msgs/Range.h"
#include "std_msgs/Bool.h"
#include "pthread.h"
#include <mutex>
using namespace std;
class uwb_wrapper
{
public:
  uwb_wrapper();
  void uwbInformationCallback(const nlink_parser::LinktrackNodeframe2ConstPtr& msg);
  void distanceMsgCallback(const sensor_msgs::RangeConstPtr& msg);
  static void *visonPosPub(void* args);
private:
  string _uwbInforSubTopic;
  string _uwbPosPubTopic;
  string _uwbDistanceTopic;
  string _distanceMsgTopic;
  string _dataValidMsgPubTopic;

  float _uwbInstallHeiht;
  float _distanceHeiht;
  float _lastDistanceHeight;

  float _visionPosPubHz;
  bool _isLocalPositionModel;
  bool _isInit;
  bool _isHeightInit;

  bool _isUpdatePositionFlag;
  bool _isUpdataHeighFlag;

  int _sendTimes;

  float _homePositionX;
  float _homePositionY;
  float _homePositionZ;

  float _sendPositionX;
  float _sendPositionY;
  float _sendPositionZ;

  float _rotationX;
  float _rotationY;
  float _rotationZ;
  float _rotationW;


  ros::Subscriber _uwbInforSub;
  ros::Subscriber _distanceMsgSub;
  ros::Publisher _uwbPosPub;
  ros::Publisher _uwbDistancePub;
  ros::Publisher _dataValidMsgPub;

  pthread_t _visionPosPubThread;
  std::mutex _data_mutex;

};

#endif // UWB_WRAPPER_H
