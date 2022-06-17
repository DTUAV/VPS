#include "rc_control.h"
#include "ui_rc_control.h"

rc_control::rc_control(QWidget *parent) :
  QMainWindow(parent),
  ui(new Ui::rc_control)
{
  ui->setupUi(this);
  ros::NodeHandle n("~");
  uav1_rc_pub = n.advertise<mavros_msgs::RCIn>("/uav_1/mavros/rc/in",1);
  uav2_rc_pub = n.advertise<mavros_msgs::RCIn>("/uav_2/mavros/rc/in",1);
  uavs_rc_pub = n.advertise<mavros_msgs::RCIn>("/uav_3/mavros/rc/in",1);
  uav1_start_mission = n.advertise<std_msgs::Bool>("/uav_1/offboard_control/uav_start",1);
  uav2_start_mission = n.advertise<std_msgs::Bool>("/uav_2/offboard_control/uav_start",1);
  uavs_start_mission = n.advertise<std_msgs::Bool>("/uav_3/offboard_control/uav_start",1);
  is_rc_change = false;
}

rc_control::~rc_control()
{
  delete ui;
}

void rc_control::on_takeoff_button_clicked()
{
  mavros_msgs::RCIn uav0_rc_msg;
  mavros_msgs::RCIn uav1_rc_msg;
  uav0_rc_msg.channels.resize(16);
  uav1_rc_msg.channels.resize(16);
  for(int i =0;i<16;i++)
  {
    uav0_rc_msg.channels.at(i) = 0;
    uav1_rc_msg.channels.at(i) = 0;
  }
  uav0_rc_msg.channels.at(4) = 1400;
  uav1_rc_msg.channels.at(4) = 1400;
  uav1_rc_pub.publish(uav0_rc_msg);
  uav2_rc_pub.publish(uav1_rc_msg);

}

void rc_control::on_next_button_clicked()
{
  mavros_msgs::RCIn uav0_rc_msg;
  mavros_msgs::RCIn uav1_rc_msg;
  uav0_rc_msg.channels.resize(16);
  uav1_rc_msg.channels.resize(16);
  for(int i =0;i<16;i++)
  {
    uav0_rc_msg.channels.at(i) = 0;
    uav1_rc_msg.channels.at(i) = 0;
  }
  uav0_rc_msg.channels.at(8) = 1300;
  uav1_rc_msg.channels.at(8) = 1300;
  uav1_rc_pub.publish(uav0_rc_msg);
  uav2_rc_pub.publish(uav1_rc_msg);
}

void rc_control::on_land_button_clicked()
{
  mavros_msgs::RCIn uav0_rc_msg;
  mavros_msgs::RCIn uav1_rc_msg;
  uav0_rc_msg.channels.resize(16);
  uav1_rc_msg.channels.resize(16);
  for(int i =0;i<16;i++)
  {
    uav0_rc_msg.channels.at(i) = 0;
    uav1_rc_msg.channels.at(i) = 0;
  }
  uav0_rc_msg.channels.at(7) = 1600;
  uav1_rc_msg.channels.at(7) = 1600;
  uav1_rc_pub.publish(uav0_rc_msg);
  uav2_rc_pub.publish(uav1_rc_msg);
}

void rc_control::on_switch_button_clicked()
{
  if(!is_rc_change)
  {
    mavros_msgs::RCIn uav1_rc_msg;
    mavros_msgs::RCIn uav2_rc_msg;
    mavros_msgs::RCIn uavs_rc_msg;
    uav1_rc_msg.channels.resize(16);
    uav2_rc_msg.channels.resize(16);
    uavs_rc_msg.channels.resize(16);
    for(int i =0;i<16;i++)
    {
      uav1_rc_msg.channels.at(i) = 0;
      uav2_rc_msg.channels.at(i) = 0;
      uavs_rc_msg.channels.at(i) = 0;
    }
    uav1_rc_msg.channels.at(9) = 1300;
    uav2_rc_msg.channels.at(9) = 1300;
    uavs_rc_msg.channels.at(9) = 1300;
    uav1_rc_msg.channels.at(5) = 1500;
    uav2_rc_msg.channels.at(5) = 1500;
    uavs_rc_msg.channels.at(5) = 1500;
    uav1_rc_msg.channels.at(11) = 1300;
    uav2_rc_msg.channels.at(11) = 1300;
    uavs_rc_msg.channels.at(11) = 1300;
    uav1_rc_pub.publish(uav1_rc_msg);
    uav2_rc_pub.publish(uav2_rc_msg);
    uavs_rc_pub.publish(uavs_rc_msg);

    is_rc_change = true;
  }
  else
  {
    mavros_msgs::RCIn uav1_rc_msg;
    mavros_msgs::RCIn uav2_rc_msg;
    mavros_msgs::RCIn uavs_rc_msg;
    uav1_rc_msg.channels.resize(16);
    uav2_rc_msg.channels.resize(16);
    uavs_rc_msg.channels.resize(16);
    for(int i =0;i<16;i++)
    {
      uav1_rc_msg.channels.at(i) = 0;
      uav2_rc_msg.channels.at(i) = 0;
      uavs_rc_msg.channels.at(i) = 0;
    }
    uav1_rc_msg.channels.at(9) = 1300;
    uav2_rc_msg.channels.at(9) = 1300;
    uavs_rc_msg.channels.at(9) = 1300;
    uav1_rc_msg.channels.at(5) = 1500;
    uav2_rc_msg.channels.at(5) = 1500;
    uavs_rc_msg.channels.at(5) = 1500;
    uav1_rc_msg.channels.at(11) = 1500;
    uav2_rc_msg.channels.at(11) = 1500;
    uavs_rc_msg.channels.at(11) = 1500;
    uav1_rc_pub.publish(uav1_rc_msg);
    uav2_rc_pub.publish(uav2_rc_msg);
    uavs_rc_pub.publish(uavs_rc_msg);

    is_rc_change = false;
  }

}

void rc_control::on_offboard_control_button_1_clicked()
{
  mavros_msgs::RCIn uav1_rc_msg;

  uav1_rc_msg.channels.resize(16);

  for(int i =0;i<16;i++)
  {
    uav1_rc_msg.channels.at(i) = 0;

  }
  uav1_rc_msg.channels.at(5) = 1500;

  uav1_rc_pub.publish(uav1_rc_msg);

}

void rc_control::on_start_button_1_clicked()
{
  mavros_msgs::RCIn uav1_rc_msg;

  uav1_rc_msg.channels.resize(16);

  for(int i =0;i<16;i++)
  {
    uav1_rc_msg.channels.at(i) = 0;

  }
  uav1_rc_msg.channels.at(9) = 1300;

  uav1_rc_msg.channels.at(5) = 1500;
  uav1_rc_msg.channels.at(11) = 1300;

  uav1_rc_pub.publish(uav1_rc_msg);


  uav1_rc_msg.channels.at(11) = 1500;


  uav1_rc_pub.publish(uav1_rc_msg);



  std_msgs::Bool startMsg;
  startMsg.data = true;
  uav1_start_mission.publish(startMsg);

}

void rc_control::on_offboard_control_button_2_clicked()
{

  mavros_msgs::RCIn uav2_rc_msg;


  uav2_rc_msg.channels.resize(16);

  for(int i =0;i<16;i++)
  {

    uav2_rc_msg.channels.at(i) = 0;

  }

  uav2_rc_msg.channels.at(5) = 1500;

  uav2_rc_pub.publish(uav2_rc_msg);

}

void rc_control::on_start_button_2_clicked()
{

  mavros_msgs::RCIn uav2_rc_msg;


  uav2_rc_msg.channels.resize(16);

  for(int i =0;i<16;i++)
  {

    uav2_rc_msg.channels.at(i) = 0;

  }

  uav2_rc_msg.channels.at(9) = 1300;

  uav2_rc_msg.channels.at(5) = 1500;

  uav2_rc_msg.channels.at(11) = 1300;

  uav2_rc_pub.publish(uav2_rc_msg);


  uav2_rc_msg.channels.at(11) = 1500;


  uav2_rc_pub.publish(uav2_rc_msg);

  std_msgs::Bool startMsg;
  startMsg.data = true;

  uav2_start_mission.publish(startMsg);

}

void rc_control::on_offboard_control_button_s_clicked()
{

  mavros_msgs::RCIn uavs_rc_msg;

  uavs_rc_msg.channels.resize(16);
  for(int i =0;i<16;i++)
  {

    uavs_rc_msg.channels.at(i) = 0;
  }

  uavs_rc_msg.channels.at(5) = 1500;

  uavs_rc_pub.publish(uavs_rc_msg);
}

void rc_control::on_start_button_s_clicked()
{

  mavros_msgs::RCIn uavs_rc_msg;

  uavs_rc_msg.channels.resize(16);
  for(int i =0;i<16;i++)
  {

    uavs_rc_msg.channels.at(i) = 0;
  }

  uavs_rc_msg.channels.at(9) = 1300;

  uavs_rc_msg.channels.at(5) = 1500;

  uavs_rc_msg.channels.at(11) = 1300;

  uavs_rc_pub.publish(uavs_rc_msg);


  uavs_rc_msg.channels.at(11) = 1500;


  uavs_rc_pub.publish(uavs_rc_msg);

  std_msgs::Bool startMsg;
  startMsg.data = true;

  uavs_start_mission.publish(startMsg);
}
