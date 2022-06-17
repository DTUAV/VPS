#include "../include/kf_mpc_controller_wrapper/kf_mpc_controller_wrapper.h"

kf_mpc_controller_wrapper::kf_mpc_controller_wrapper()
{
  ros::NodeHandle n("~");
  n.getParam("px4_vihecle_pose_sub_topic",_px4_vihecle_pose_sub_topic);
  n.getParam("object_move_position_pub_topic",_object_move_position_pub_topic);

  _px4_vihecle_pose_sub = n.subscribe(_px4_vihecle_pose_sub_topic,1,&kf_mpc_controller_wrapper::px4_vihecle_pose_sub_cb,this);
  _object_move_position_pub = n.advertise<dt_message_package::object_move_position2>(_object_move_position_pub_topic,1);

}

void kf_mpc_controller_wrapper::px4_vihecle_pose_sub_cb(const geometry_msgs::PoseStampedConstPtr &msg)
{
  struct timeval tv;
  gettimeofday(&tv, NULL);
  long rawtime_ms = tv.tv_sec * 1000 + tv.tv_usec / 1000;
  dt_message_package::object_move_position2 move_position_msg;
  move_position_msg.time_stamp_ms = rawtime_ms;
  move_position_msg.position_x = msg.get()->pose.position.x;
  move_position_msg.position_y = msg.get()->pose.position.y;
  move_position_msg.position_z = msg.get()->pose.position.z;
  _object_move_position_pub.publish(move_position_msg);
}
