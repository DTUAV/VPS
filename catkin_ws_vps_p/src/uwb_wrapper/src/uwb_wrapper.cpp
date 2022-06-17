#include "../include/uwb_wrapper/uwb_wrapper.h"

uwb_wrapper::uwb_wrapper()
{
  ros::NodeHandle n("~");
  n.getParam("UwbPosMsgTopic",_uwbPosPubTopic);//
  n.getParam("UwbInforMsgTopic",_uwbInforSubTopic);//
  n.getParam("UwbInstallHeigh",_uwbInstallHeiht);
  n.getParam("UwbDistanceTopic",_uwbDistanceTopic);
  n.getParam("DistanceMsgTopic",_distanceMsgTopic);
  n.getParam("LocalPositionModel",_isLocalPositionModel);
  n.getParam("VisonPosPubHz",_visionPosPubHz);//no used
  n.getParam("DataValidMsgPubTopic",_dataValidMsgPubTopic);
  _distanceHeiht = 0;
  _distanceMsgSub = n.subscribe(_distanceMsgTopic,1,&uwb_wrapper::distanceMsgCallback,this);
  _uwbPosPub = n.advertise<geometry_msgs::PoseStamped>(_uwbPosPubTopic,1);
  _uwbDistancePub = n.advertise<sensor_msgs::ChannelFloat32>(_uwbDistanceTopic,1);
  _uwbInforSub = n.subscribe(_uwbInforSubTopic,1,&uwb_wrapper::uwbInformationCallback,this);
  _dataValidMsgPub = n.advertise<std_msgs::Bool>(_dataValidMsgPubTopic,1);
  _isInit = false;
  _isHeightInit = false;
  _isUpdatePositionFlag = false;
  _isUpdataHeighFlag = false;
  _sendTimes = 0;

  if(_isLocalPositionModel)
  {
    ROS_INFO("------------Local Position Model--------------------");
  }
  else
  {
    ROS_INFO("-----------Global Position Model--------------------");
  }
  int flag_thread = pthread_create(&_visionPosPubThread,NULL,&uwb_wrapper::visonPosPub,this);
  if (flag_thread < 0)
 {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }
}
void uwb_wrapper::distanceMsgCallback(const sensor_msgs::RangeConstPtr &msg)
{
  if(!_isHeightInit)
  {
    if(msg.get()->range<=0.2)
    {
      _lastDistanceHeight = msg.get()->range;
      _isHeightInit = true;
      ROS_INFO("Get Current Height");
      {
        std::lock_guard<std::mutex> lockguard(_data_mutex);
        _isUpdataHeighFlag = true;
      }
    }
    std::cout<<"Current Height: " << _lastDistanceHeight <<std::endl;
  }
  else
  {
    // if(abs(msg.get()->range - _lastDistanceHeight)<0.2)
    {
      _distanceHeiht = msg.get()->range;
      _lastDistanceHeight = _distanceHeiht;

      {
        std::lock_guard<std::mutex> lockguard(_data_mutex);
        _isUpdataHeighFlag = true;
      }

    }
  }


}

void uwb_wrapper::uwbInformationCallback(const nlink_parser::LinktrackNodeframe2ConstPtr &msg)
{
  sensor_msgs::ChannelFloat32 distance_msg;
  distance_msg.name = "uwb_distance";
  distance_msg.values.resize(4);
  if(_isInit&&_isHeightInit)
  {
    if(msg.get()->eop_3d.at(0)<0.2&&msg.get()->eop_3d.at(1)<0.2&&msg.get()->eop_3d.at(2)<0.2)
    {
      if(_isLocalPositionModel)
      {
        _sendPositionX = msg.get()->pos_3d.at(0) - _homePositionX;
        _sendPositionY = msg.get()->pos_3d.at(1) - _homePositionY;
      }
      else
      {
        _sendPositionX = msg.get()->pos_3d.at(0);
        _sendPositionY = msg.get()->pos_3d.at(1);
      }
      _rotationW = msg.get()->quaternion.at(0);
      _rotationX = msg.get()->quaternion.at(1);
      _rotationY = msg.get()->quaternion.at(2);
      _rotationZ = msg.get()->quaternion.at(3);

      {
        std::lock_guard<std::mutex> lockguard(_data_mutex);
        _isUpdatePositionFlag = true;
      }

    }
  }
  else
  {
    if(msg.get()->eop_3d.at(0)<0.2&&msg.get()->eop_3d.at(1)<0.2&&msg.get()->eop_3d.at(2)<0.2)
    {
      _isInit = true;
      _homePositionX = msg.get()->pos_3d.at(0);
      _homePositionY = msg.get()->pos_3d.at(1);
      _homePositionZ = msg.get()->pos_3d.at(2);
      ROS_INFO("Get Home Position By Uwb");
      std::cout<<"Home Position: " << "( " << _homePositionX << "," << _homePositionY << "," << _homePositionZ << " )" <<std::endl;
      if(_isLocalPositionModel)
      {
        _sendPositionX = msg.get()->pos_3d.at(0) - _homePositionX;
        _sendPositionY = msg.get()->pos_3d.at(1) - _homePositionY;
      }
      else
      {
        _sendPositionX = msg.get()->pos_3d.at(0);
        _sendPositionY = msg.get()->pos_3d.at(1);
      }
      _rotationW = msg.get()->quaternion.at(0);
      _rotationX = msg.get()->quaternion.at(1);
      _rotationY = msg.get()->quaternion.at(2);
      _rotationZ = msg.get()->quaternion.at(3);

      {
        std::lock_guard<std::mutex> lockguard(_data_mutex);
        _isUpdatePositionFlag = true;
      }

    }
  }
      geometry_msgs::PoseStamped pose_msg;
      pose_msg.header.stamp = ros::Time::now();
      pose_msg.pose.position.x = _sendPositionX;
      pose_msg.pose.position.y = _sendPositionY;
      pose_msg.pose.position.z = _distanceHeiht;
      pose_msg.pose.orientation.w =_rotationW;
      pose_msg.pose.orientation.x = _rotationX;
      pose_msg.pose.orientation.y = _rotationY;
      pose_msg.pose.orientation.z = _rotationZ;
      _uwbPosPub.publish(pose_msg);

  if(msg.get()->nodes.size()>=1)
  {
    for(int i = 0; i<msg.get()->nodes.size();i++)
    {
      if(i==0)
      {
        distance_msg.values.at(0) = msg.get()->nodes.at(i).dis;
      }
      else if(i==1)
      {
        distance_msg.values.at(1) = msg.get()->nodes.at(i).dis;
      }
      else if(i==2)
      {
        distance_msg.values.at(2) = msg.get()->nodes.at(i).dis;
      }
      else if(i==3)
      {
        distance_msg.values.at(3) = msg.get()->nodes.at(i).dis;
      }
    }
    _uwbDistancePub.publish(distance_msg);
  }
}

void *uwb_wrapper::visonPosPub(void *args)
{
  uwb_wrapper* wrapper = (uwb_wrapper*)(args);
  ros::Rate rate(wrapper->_visionPosPubHz);
  geometry_msgs::PoseStamped pose_msg;
  std_msgs::Bool data_valid_msg;
  while(ros::ok())
  {
    if(wrapper->_isInit&&wrapper->_isHeightInit)
    {
      if(wrapper->_sendTimes>=10)
      {
        wrapper->_sendTimes = 0;
        if(wrapper->_isUpdataHeighFlag&&wrapper->_isUpdatePositionFlag)
        {
          data_valid_msg.data = true;
          wrapper->_dataValidMsgPub.publish(data_valid_msg);
          {
            std::lock_guard<std::mutex> lockguard(wrapper->_data_mutex);
            wrapper->_isUpdataHeighFlag = false;
            wrapper->_isUpdatePositionFlag = false;
          }
        }
        else
        {
          data_valid_msg.data = false;
          wrapper->_dataValidMsgPub.publish(data_valid_msg);
        }
      }
      else
      {
        ++wrapper->_sendTimes;
      }

      //pose_msg.header.stamp = ros::Time::now();
      //pose_msg.pose.position.x = wrapper->_sendPositionX;
      //pose_msg.pose.position.y = wrapper->_sendPositionY;
      //pose_msg.pose.position.z = wrapper->_distanceHeiht;
      //pose_msg.pose.orientation.w = wrapper->_rotationW;
      //pose_msg.pose.orientation.x = wrapper->_rotationX;
      //pose_msg.pose.orientation.y = wrapper->_rotationY;
      //pose_msg.pose.orientation.z = wrapper->_rotationZ;
      //wrapper->_uwbPosPub.publish(pose_msg);
    }
    rate.sleep();
  }
}
