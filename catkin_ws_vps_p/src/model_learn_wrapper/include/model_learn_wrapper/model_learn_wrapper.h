#ifndef MODEL_LEARN_WRAPPER_H
#define MODEL_LEARN_WRAPPER_H
#include "ros/ros.h"
#include "pthread.h"
#include <mutex>
#include "dt_message_package/object_predict_position.h"
#include "geometry_msgs/PoseStamped.h"
#include <random>
#include <ctime>
#include <cstdlib>
class model_learn_wrapper
{
public:
  model_learn_wrapper();
  void localPosSubCallback(const geometry_msgs::PoseStampedConstPtr& msg);
  static void *run(void *args);


private:
  ros::Subscriber _localPosSub;
  ros::Publisher _targetPosPub;

  float _minRangeX;
  float _minRangeY;
  float _minRangeZ;
  float _maxRangeX;
  float _maxRangeY;
  float _maxRangeZ;

  float _targetPosX;
  float _targetPosY;
  float _targetPosZ;

  float _currentPosX;
  float _currentPosY;
  float _currentPosZ;

  bool _init;

  int _predictedWindow;
  float _checkError;

  float getRandData(float minData, float maxData);
  dt_message_package::object_predict_position targetPositionMsg;

  pthread_t _checkTagetThread;

};

#endif // MODEL_LEARN_WRAPPER_H
