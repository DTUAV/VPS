#include "get_data_set_rc.h"
#include "ui_get_data_set_rc.h"

get_data_set_rc::get_data_set_rc(QWidget *parent) :
  QWidget(parent),
  ui(new Ui::get_data_set_rc)
{
  ui->setupUi(this);
  ros::NodeHandle n("~");
  n.getParam("start_offboard_pub_topic",_start_offboard_pub_topic);
  n.getParam("start_offboard_ch",_start_offboard_ch);
  n.getParam("land_ch",_land_ch);
  n.getParam("rc_start_mission_ch",_rc_start_mission_ch);
  n.getParam("rc_change_mission_ch",_rc_change_mission_ch);

  _start_offboard_pub = n.advertise<mavros_msgs::RCIn>(_start_offboard_pub_topic,1);

  _rc_msg.channels.resize(16);
}

get_data_set_rc::~get_data_set_rc()
{
  delete ui;
}

void get_data_set_rc::on_start_offboard_clicked()
{
  _rc_msg.channels.at(_start_offboard_ch) = 1900;
  _rc_msg.channels.at(_land_ch) = 1000;
  _start_offboard_pub.publish(_rc_msg);
}

void get_data_set_rc::on_stop_offboard_clicked()
{
  _rc_msg.channels.at(_start_offboard_ch) = 1500;
  _rc_msg.channels.at(_land_ch) = 1000;
  _start_offboard_pub.publish(_rc_msg);
}

void get_data_set_rc::on_land_clicked()
{
  _rc_msg.channels.at(_start_offboard_ch) = 1500;
  _rc_msg.channels.at(_land_ch) = 1900;
  _start_offboard_pub.publish(_rc_msg);
}

void get_data_set_rc::on_start_mission_clicked()
{
_rc_msg.channels.at(_rc_start_mission_ch) = 1900;
_start_offboard_pub.publish(_rc_msg);
}

void get_data_set_rc::on_change_mission_point_clicked()
{
  _rc_msg.channels.at(_rc_change_mission_ch) = 1500;
  _start_offboard_pub.publish(_rc_msg);
  _rc_msg.channels.at(_rc_change_mission_ch) = 1900;
  _start_offboard_pub.publish(_rc_msg);
}

void get_data_set_rc::on_stop_mission_clicked()
{
  _rc_msg.channels.at(_rc_start_mission_ch) = 1500;
  _start_offboard_pub.publish(_rc_msg);
}
