#include "../include/model_learn_wrapper/model_learn_wrapper.h"

model_learn_wrapper::model_learn_wrapper()
{
  ros::NodeHandle n("~");
  std::string localPosSubTopic = "/mavros/local_position/pose";
  std::string targetPosPubTopic = "/target/position";
  n.getParam("LocalPositionSubTopic",localPosSubTopic);
  n.getParam("TargetPositionPubTopic",targetPosPubTopic);
  n.getParam("MinRangeX",_minRangeX);
  n.getParam("MinRangeY",_minRangeY);
  n.getParam("MinRangeZ",_minRangeZ);
  n.getParam("MaxRangeX",_maxRangeX);
  n.getParam("MaxRangeY",_maxRangeY);
  n.getParam("MaxRangeZ",_maxRangeZ);
  n.getParam("PredictedWindow",_predictedWindow);
  n.getParam("CheckError",_checkError);

  _targetPosX = 0;
  _targetPosY = 0;
  _targetPosZ = 1;

  _init = false;

  _localPosSub = n.subscribe(localPosSubTopic,1,&model_learn_wrapper::localPosSubCallback,this);
  _targetPosPub = n.advertise<dt_message_package::object_predict_position>(targetPosPubTopic,1);

  int flag_thread = pthread_create(&_checkTagetThread,NULL,&model_learn_wrapper::run,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }
}

void *model_learn_wrapper::run(void *args)
{
  model_learn_wrapper* wrapper = (model_learn_wrapper*)(args);
  ros::Rate rate(1);
  while(ros::ok())
  {
    if(fabs(wrapper->_currentPosX-wrapper->_targetPosX)<=wrapper->_checkError
        &&fabs(wrapper->_currentPosY-wrapper->_targetPosY)<=wrapper->_checkError
        &&fabs(wrapper->_currentPosZ-wrapper->_targetPosZ)<=wrapper->_checkError)
     {
       wrapper->_targetPosX = wrapper->getRandData(wrapper->_minRangeX,wrapper->_maxRangeX);
       wrapper->_targetPosY = wrapper->getRandData(wrapper->_minRangeY,wrapper->_maxRangeY);
       wrapper->_targetPosZ = wrapper->getRandData(wrapper->_minRangeZ,wrapper->_maxRangeZ);
    }
      rate.sleep();

  }
  pthread_join(wrapper->_checkTagetThread,NULL);
}

void model_learn_wrapper::localPosSubCallback(const geometry_msgs::PoseStampedConstPtr &msg)
{
  if(!_init)
  {
    _targetPosX = msg.get()->pose.position.x;
    _targetPosY = msg.get()->pose.position.y;
    _targetPosZ = 1;
    targetPositionMsg.current_position_x = _targetPosX;
    targetPositionMsg.current_position_y = _targetPosY;
    targetPositionMsg.current_position_z = _targetPosZ;
    targetPositionMsg.current_time = ros::Time::now().toNSec()/10e9;
    for(int i=0;i<_predictedWindow;i++)
    {
      targetPositionMsg.position_xs.push_back(_targetPosX);
      targetPositionMsg.position_ys.push_back(_targetPosY);
      targetPositionMsg.position_zs.push_back(_targetPosZ);
      targetPositionMsg.times.push_back(_targetPosY+(i+1)*0.033);
    }
    _targetPosPub.publish(targetPositionMsg);
    std::cout<<"target_position: "<<_targetPosX<<","<<_targetPosY<<","<<_targetPosZ<<std::endl;
    _init = true;
  }
  else
  {
      targetPositionMsg.current_position_x = _targetPosX;
      targetPositionMsg.current_position_y = _targetPosY;
      targetPositionMsg.current_position_z = _targetPosZ;
      targetPositionMsg.current_time = ros::Time::now().toNSec()/10e9;
      for(int i=0;i<_predictedWindow;i++)
      {
        targetPositionMsg.position_xs.push_back(_targetPosX);
        targetPositionMsg.position_ys.push_back(_targetPosY);
        targetPositionMsg.position_zs.push_back(_targetPosZ);
        targetPositionMsg.times.push_back(_targetPosY+(i+1)*0.033);
      }
      _targetPosPub.publish(targetPositionMsg);
  }

  _currentPosX = msg.get()->pose.position.x;
  _currentPosY = msg.get()->pose.position.y;
  _currentPosZ = msg.get()->pose.position.z;
}
float model_learn_wrapper::getRandData(float minData, float maxData)
{
  //std::default_random_engine e(std::time(0));
 // std::uniform_real_distribution<float> u(minData,maxData);
  srand (static_cast <unsigned> (std::time(0)));
  float ret = minData + static_cast <float> (rand()) /( static_cast <float> (RAND_MAX/(maxData-minData)));
  return ret;
}
