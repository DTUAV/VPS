#include "../include/kf_mpc_controller4/controller_network_wrapper.h"

controller_network_wrapper::controller_network_wrapper()
{
  ros::NodeHandle n("~");
  if(n.getParam("network_message_sub_topic",_network_message_sub_topic))
  {
    ROS_INFO("\033[1;32m Controller_Network_Wrapper----Get Topic Name: <network_message_sub_topic>--->%s\033[0m",_network_message_sub_topic.c_str());
  }
  else
  {
    _network_message_sub_topic = "/network_message";
    ROS_WARN("Controller_Network_Wrapper----Not Get Topic Name: <network_message_sub_topic>--defalut: %s",_network_message_sub_topic.c_str());
  }

  if(n.getParam("dt_object_position_pub_topic",_dt_object_position_pub_topic))
  {
    ROS_INFO("\033[1;32m Controller_Network_Wrapper----Get Topic Name: <dt_object_position_pub_topic>--->%s\033[0m",_dt_object_position_pub_topic.c_str());
  }
  else
  {
    _dt_object_position_pub_topic = "/object_position";
    ROS_WARN("Controller_Network_Wrapper----Not Get Topic Name: <dt_object_position_pub_topic>--defalut: %s",_dt_object_position_pub_topic.c_str());
  }

  if(n.getParam("dt_object_position_recv_hz",_dt_object_position_recv_hz))
  {
    ROS_INFO("\033[1;32m Controller_Network_Wrapper----Get Topic Name: <dt_object_position_recv_hz>--->%f\033[0m",_dt_object_position_recv_hz);
  }
  else
  {
    _dt_object_position_recv_hz = 100.0;
    ROS_WARN("Controller_Network_Wrapper----Not Get Topic Name: <dt_object_position_pub_topic>--defalut: %f",_dt_object_position_recv_hz);
  }

  n.getParam("virtual_guide_pub_topic",_virtual_guide_pub_topic);
  n.getParam("virtual_guide_control_pub_topic",_virtual_guide_control_pub_topic);
  n.getParam("is_unity",_is_unity);
  n.getParam("network_delay_pub_topic",_network_delay_pub_topic);
  n.getParam("network_delay_pub_hz",_network_delay_pub_hz);

  n.getParam("predict_window",_predict_window);
  n.getParam("predict_time_dt",_predict_time_dt);
  n.getParam("target_position_pub_topic",_targetPositionPubTopic);
  n.getParam("model_learner_control_sub_topic",_model_learner_control_pub_topic);

  _network_message_sub = n.subscribe(_network_message_sub_topic,1,&controller_network_wrapper::network_message_sub_cb,this);
  _dt_object_position_pub = n.advertise<dt_message_package::object_move_position2>(_dt_object_position_pub_topic,1);
  _virtual_guide_pub = n.advertise<geometry_msgs::TwistStamped>(_virtual_guide_pub_topic,1);
  _virtual_guide_control_pub = n.advertise<std_msgs::Int8>(_virtual_guide_control_pub_topic,1);
  _network_delay_pub = n.advertise<dt_message_package::network_delay_time>(_network_delay_pub_topic,1);
  _targetPositiongPub = n.advertise<dt_message_package::object_predict_position>(_targetPositionPubTopic,1);
  _model_learner_control_pub = n.advertise<std_msgs::Bool>(_model_learner_control_pub_topic,1);
  _data_times = (1.0/_dt_object_position_recv_hz)*1000;//ms
  _last_times = 0;
  _is_first = true;
  _delay_time = 0;
  _push_time = 0;
  _start_delay_pub = false;

  _cicl_delay_time.resize(10);
  ROS_INFO("The Time Receiving Interval of Message Is %f ms",_data_times);
  int flag_thread = pthread_create(&_delay_pub_thread,NULL,&controller_network_wrapper::network_delay_pub,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }

}

double controller_network_wrapper::getMeanWithRemoveMaxMin(boost::circular_buffer<double> cicl_delay_time)
{
  double maxData = -20000;
  double minData = 20000;
  int maxDataId = 0;
  int minDataId = 0;
  double sum = 0;
  for(int i=0;i<cicl_delay_time.size();i++)
  {
    // std::cout<<i<<" : "<<cicl_delay_time.at(i)<<std::endl;
    if(cicl_delay_time.at(i)>maxData)
    {
      maxDataId = i;
      maxData = cicl_delay_time.at(i);
    }
    if(cicl_delay_time.at(i)<minData)
    {
      minDataId = i;
      minData = cicl_delay_time.at(i);
    }
  }

  for(int i=0;i<cicl_delay_time.size();i++)
  {
    if(i!=maxDataId&&i!=minDataId)
    {
      sum +=cicl_delay_time.at(i);
    }
  }
  return sum/(cicl_delay_time.size()-2);

}

void *controller_network_wrapper::network_delay_pub(void *args)
{
  controller_network_wrapper* wrapper = (controller_network_wrapper*)(args);

  ros::Rate rate(wrapper->_network_delay_pub_hz);
  dt_message_package::network_delay_time delayTimeMsg;
  while(ros::ok())
  {
    if(wrapper->_start_delay_pub)
    {
      delayTimeMsg.delay_time = wrapper->_delay_time;
      wrapper->_network_delay_pub.publish(delayTimeMsg);
      {
        std::lock_guard<std::mutex> lockguard(wrapper->_data_mutex);
        wrapper->_start_delay_pub = false;
      }
    }
    rate.sleep();
  }
}


void controller_network_wrapper::network_message_sub_cb(const cloud_common::socket_data_packet_stamp_msgConstPtr &msg)
{
  bool is_push= false;
  if(!_is_first)
  {
    if(msg.get()->MessageID == UavLocalPositionMessageID)
    {
      DTUAVCARS::UavLocalPositionMessage position_msg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,position_msg,false);
      if(is_load)
      {
        if(_is_unity)
        {
          _last_times = ros::Time::now().toNSec();
          _start_time_s = ros::Time::now().toNSec()/10e9;
          _is_first = false;                             //The First Data To Configure the Last Receive Time
          ROS_INFO("Receive Message From Network Sucessfully");
          ROS_INFO("Start Time is %f ns",_last_times);
        }
        else
        {
          struct timeval tv;
          gettimeofday(&tv, NULL);
          long rawtime_ms = tv.tv_sec * 1000 + tv.tv_usec / 1000;
          _last_times = rawtime_ms;
          _start_time_s = ros::Time::now().toNSec()/10e9;
          _is_first = false;                             //The First Data To Configure the Last Receive Time
          ROS_INFO("Receive Message From Network Sucessfully");
          ROS_INFO("Start Time is %f ns",_last_times);
        }

      }
    }
  }
  else
  {
    if(msg.get()->MessageID == UavLocalPositionMessageID)
    {
      dt_message_package::object_move_position2 object_position_msg;
      DTUAVCARS::UavLocalPositionMessage position_msg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,position_msg,false);
      if(is_load)
      {
        if(_is_unity)
        {
          double current_times = ros::Time::now().toNSec();
          double delay_times_ms = (current_times - _last_times)/1e6;//cout the delay time roughly

          if(delay_times_ms - _data_times > 0.0)
          {
            object_position_msg.delay_time_ms = delay_times_ms - _data_times;
            is_push = true;
          }
          else
          {
            object_position_msg.delay_time_ms = 0.0;
          }
          _last_times = current_times;
          _current_time_s = ros::Time::now().toNSec()/10e9 - _start_time_s;
        }
        else
        {
          struct timeval tv;
          gettimeofday(&tv, NULL);
          long rawtime_ms = tv.tv_sec * 1000 + tv.tv_usec / 1000;
          double delay_times_ms = (rawtime_ms - _last_times);//cout the delay time roughly
          //std::cout<<"delay_times_ms: "<<delay_times_ms<<std::endl;
          if(delay_times_ms - _data_times > 0.0)
          {
            object_position_msg.delay_time_ms = delay_times_ms - _data_times;
            is_push = true;
          }
          else
          {
            object_position_msg.delay_time_ms = 0.0;
          }
          _last_times = rawtime_ms;
          _current_time_s = ros::Time::now().toNSec()/10e9 - _start_time_s;
        }

        object_position_msg.time_stamp_ms = msg.get()->TimeStamp;
        object_position_msg.position_x = position_msg.position_x;
        object_position_msg.position_y = position_msg.position_y;
        object_position_msg.position_z = position_msg.position_z;
        _dt_object_position_pub.publish(object_position_msg);

        dt_message_package::object_predict_position targetPositionMsg;
        targetPositionMsg.current_position_x = position_msg.position_x;
        targetPositionMsg.current_position_y = position_msg.position_y;
        targetPositionMsg.current_position_z = position_msg.position_z;
        targetPositionMsg.current_time = _current_time_s;
        for(int i=0;i<_predict_window;i++)
        {
          targetPositionMsg.position_xs.push_back(position_msg.position_x);
          targetPositionMsg.position_ys.push_back(position_msg.position_y);
          targetPositionMsg.position_zs.push_back(position_msg.position_z);
          targetPositionMsg.times.push_back(_current_time_s+(i+1)*_predict_time_dt);
        }
        _targetPositiongPub.publish(targetPositionMsg);

        if(_push_time<10)
        {
          if(is_push)
          {
            _cicl_delay_time.push_back(object_position_msg.delay_time_ms);
            _push_time++;
          }
        }
        else
        {
          if(is_push)
          {
            _cicl_delay_time.push_back(object_position_msg.delay_time_ms);
            //double all_delay = 0;
            // for(int i=0;i<_cicl_delay_time.size();i++)
            // {
            //   all_delay+=_cicl_delay_time.at(i);
            // }
            //_delay_time = all_delay/_cicl_delay_time.size();
            _delay_time = getMeanWithRemoveMaxMin(_cicl_delay_time);
            //std::cout<<"delay_time: "<<_delay_time<<std::endl;
            {

              std::lock_guard<std::mutex> lockguard(_data_mutex);
              _start_delay_pub = true;
            }
            //_push_time = 0;
          }

        }
      }
    }
    else if(msg.get()->MessageID == VirtualGuideMessageID)
    {
      DTUAVCARS::VirtualGuideMessage virtualGuideMsg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,virtualGuideMsg,false);
      if(is_load)
      {
        geometry_msgs::TwistStamped velocityMsg;
        velocityMsg.twist.linear.x = virtualGuideMsg.linearVelocityX;
        velocityMsg.twist.linear.y = virtualGuideMsg.linearVelocityY;
        velocityMsg.twist.linear.z = virtualGuideMsg.linearVelocityZ;
        _virtual_guide_pub.publish(velocityMsg);
      }
    }
    else if(msg.get()->MessageID == VirtualGuideControlMessageID)
    {
      DTUAVCARS::VirtualGuideControlMessage virtualGuideControlMsg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,virtualGuideControlMsg,false);
      if(is_load)
      {
        std_msgs::Int8 guideControlMsg;
        guideControlMsg.data = virtualGuideControlMsg.guideType;
        _virtual_guide_control_pub.publish(guideControlMsg);
      }
    }
    else if(msg.get()->MessageID == ModelLearnControlMessageID)
    {
      DTUAVCARS::ModelLearnControlMessage modelLearnControlMsg;
      bool is_load = x2struct::X::loadjson(msg.get()->MessageData,modelLearnControlMsg,false);
      if(is_load)
      {
        std_msgs::Bool learnControlMsg;
        learnControlMsg.data = modelLearnControlMsg.learn;
        _model_learner_control_pub.publish(learnControlMsg);
      }
    }
  }
}
