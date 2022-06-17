#include "kf_mpc_controller_rc.h"
#include "ui_kf_mpc_controller_rc.h"

kf_mpc_controller_rc::kf_mpc_controller_rc(QWidget *parent) :
  QWidget(parent),
  ui(new Ui::kf_mpc_controller_rc)
{
  ros::NodeHandle n("~");
  n.getParam("start_offboard_pub_topic",_start_offboard_pub_topic);
  n.getParam("target_object_position_pub_topic",_target_object_position_pub_topic);
  n.getParam("start_offboard_ch",_start_offboard_ch);
  n.getParam("land_ch",_land_ch);
  _init_x = 0;
  _init_y = 0;
  _init_z = 1;
  _start_offboard_pub = n.advertise<mavros_msgs::RCIn>(_start_offboard_pub_topic,1);
  _target_object_position_pub = n.advertise<geometry_msgs::PoseStamped>(_target_object_position_pub_topic,1);
  geometry_msgs::PoseStamped pose_msg;
  pose_msg.pose.position.x = _init_x;
  pose_msg.pose.position.y = _init_y;
  pose_msg.pose.position.z = _init_z;
  _target_object_position_pub.publish(pose_msg);
  ui->setupUi(this);
}

kf_mpc_controller_rc::~kf_mpc_controller_rc()
{
  delete ui;
}

void kf_mpc_controller_rc::on_start_offboard_clicked()
{
  mavros_msgs::RCIn rc_msg;
  rc_msg.channels.resize(16);
  rc_msg.channels.at(_start_offboard_ch) = 1900;
  rc_msg.channels.at(_land_ch) = 1000;
  _start_offboard_pub.publish(rc_msg);
}

void kf_mpc_controller_rc::on_stop_offboard_clicked()
{
  mavros_msgs::RCIn rc_msg;
  rc_msg.channels.resize(16);
  rc_msg.channels.at(_start_offboard_ch) = 1500;
  rc_msg.channels.at(_land_ch) = 1000;
  _start_offboard_pub.publish(rc_msg);
}

void kf_mpc_controller_rc::on_land_clicked()
{
  mavros_msgs::RCIn rc_msg;
  rc_msg.channels.resize(16);
  rc_msg.channels.at(_start_offboard_ch) = 1500;
  rc_msg.channels.at(_land_ch) = 1900;
  _start_offboard_pub.publish(rc_msg);
}


void kf_mpc_controller_rc::on_next_point_clicked()
{
  _init_x = _init_x + 1;
  _init_y = _init_y + 1;
   geometry_msgs::PoseStamped pose_msg;
   pose_msg.pose.position.x = _init_x;
   pose_msg.pose.position.y = _init_y;
   pose_msg.pose.position.z = _init_z;
   _target_object_position_pub.publish(pose_msg);
}
