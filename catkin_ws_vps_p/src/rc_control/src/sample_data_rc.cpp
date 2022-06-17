#include "sample_data_rc.h"
#include "ui_sample_data_rc.h"

sample_data_rc::sample_data_rc(QWidget *parent) :
  QWidget(parent),
  ui(new Ui::sample_data_rc)
{
  ui->setupUi(this);
  ros::NodeHandle n("~");
  n.getParam("start_offboard_pub_topic",_start_offboard_pub_topic);
  n.getParam("target_object_position_pub_topic",_target_object_position_pub_topic);
  n.getParam("start_offboard_ch",_start_offboard_ch);
  n.getParam("land_ch",_land_ch);
  n.getParam("start_sample_data_pub_topic",_start_sample_data_pub_topic);
  n.getParam("end_sample_data_pub_topic",_end_sample_data_pub_topic);
  _init_x = 0;
  _init_y = 0;
  _init_z = 1;
  _start_offboard_pub = n.advertise<mavros_msgs::RCIn>(_start_offboard_pub_topic,1);
  _target_object_position_pub = n.advertise<geometry_msgs::PoseStamped>(_target_object_position_pub_topic,1);
  _start_sample_data_pub = n.advertise<std_msgs::Bool>(_start_sample_data_pub_topic,1);
  _end_sample_data_pub = n.advertise<std_msgs::Bool>(_end_sample_data_pub_topic,1);
  geometry_msgs::PoseStamped pose_msg;
  pose_msg.pose.position.x = _init_x;
  pose_msg.pose.position.y = _init_y;
  pose_msg.pose.position.z = _init_z;
  _target_object_position_pub.publish(pose_msg);
}

sample_data_rc::~sample_data_rc()
{
  delete ui;
}

void sample_data_rc::on_start_sample_data_clicked()
{
  std_msgs::Bool startSampleDataMsg;
  startSampleDataMsg.data = true;
  _start_sample_data_pub.publish(startSampleDataMsg);
}

void sample_data_rc::on_end_sample_data_clicked()
{
  std_msgs::Bool endSampleDataMsg;
  endSampleDataMsg.data = true;
  _end_sample_data_pub.publish(endSampleDataMsg);
}

void sample_data_rc::on_start_offboard_clicked()
{
  mavros_msgs::RCIn rc_msg;
  rc_msg.channels.resize(16);
  rc_msg.channels.at(_start_offboard_ch) = 1900;
  rc_msg.channels.at(_land_ch) = 1000;
  _start_offboard_pub.publish(rc_msg);
}

void sample_data_rc::on_stop_offboard_clicked()
{
  mavros_msgs::RCIn rc_msg;
  rc_msg.channels.resize(16);
  rc_msg.channels.at(_start_offboard_ch) = 1500;
  rc_msg.channels.at(_land_ch) = 1000;
  _start_offboard_pub.publish(rc_msg);
}

void sample_data_rc::on_land_clicked()
{
  mavros_msgs::RCIn rc_msg;
  rc_msg.channels.resize(16);
  rc_msg.channels.at(_start_offboard_ch) = 1500;
  rc_msg.channels.at(_land_ch) = 1900;
  _start_offboard_pub.publish(rc_msg);
}
