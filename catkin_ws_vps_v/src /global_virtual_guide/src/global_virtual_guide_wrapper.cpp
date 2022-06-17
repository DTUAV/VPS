#include "../include/global_virtual_guide/global_virtual_guide_wrapper.h"

global_virtual_guide_wrapper::global_virtual_guide_wrapper()
{
  ros::NodeHandle n("~");
  n.getParam("physical_object_position_sub_topic",_pObjPosSubTopic);
  n.getParam("predict_window",_predict_window);
  n.getParam("predict_time_dt",_predict_time_dt);
  n.getParam("target_position_pub_topic",_targetPositionPubTopic);
  _pObjPosSub = n.subscribe(_pObjPosSubTopic,1,&global_virtual_guide_wrapper::pObjPosSubCallback,this);
  _targetPositiongPub = n.advertise<dt_message_package::object_predict_position>(_targetPositionPubTopic,1);

}

void global_virtual_guide_wrapper::pObjPosSubCallback(const geometry_msgs::PoseStampedConstPtr &msg)
{
  if(_is_first_measure_data)
  {
    _start_time_s = ros::Time::now().toNSec()/10e9;
    _is_first_measure_data = false;
  }
  else
  {
    _current_time_s = ros::Time::now().toNSec()/10e9 - _start_time_s;
    dt_message_package::object_predict_position targetPositionMsg;
    targetPositionMsg.current_position_x = msg.get()->pose.position.x;
    targetPositionMsg.current_position_y = msg.get()->pose.position.y;
    targetPositionMsg.current_position_z = msg.get()->pose.position.z;
    targetPositionMsg.current_time = _current_time_s;
    for(int i=0;i<_predict_window;i++)
    {
      targetPositionMsg.position_xs.push_back(msg.get()->pose.position.x);
      targetPositionMsg.position_ys.push_back(msg.get()->pose.position.y);
      targetPositionMsg.position_zs.push_back(msg.get()->pose.position.z);
      targetPositionMsg.times.push_back(_current_time_s+(i+1)*_predict_time_dt);
    }
    _targetPositiongPub.publish(targetPositionMsg);
  }


}
