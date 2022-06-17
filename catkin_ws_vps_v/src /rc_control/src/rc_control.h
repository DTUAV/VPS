#ifndef RC_CONTROL_H
#define RC_CONTROL_H

#include <QMainWindow>
#include "ros/ros.h"
#include "mavros_msgs/RCIn.h"
#include "std_msgs/Bool.h"
namespace Ui {
class rc_control;
}

class rc_control : public QMainWindow
{
  Q_OBJECT

public:
  explicit rc_control(QWidget *parent = 0);
  ~rc_control();

private Q_SLOTS:
  void on_takeoff_button_clicked();

  void on_next_button_clicked();

  void on_land_button_clicked();

  void on_switch_button_clicked();

  void on_offboard_control_button_1_clicked();

  void on_start_button_1_clicked();

  void on_offboard_control_button_2_clicked();

  void on_start_button_2_clicked();

  void on_offboard_control_button_s_clicked();

  void on_start_button_s_clicked();

private:
  Ui::rc_control *ui;
  ros::Publisher uav1_rc_pub;
  ros::Publisher uav2_rc_pub;
  ros::Publisher uavs_rc_pub;
  ros::Publisher uav1_start_mission;
  ros::Publisher uav2_start_mission;
  ros::Publisher uavs_start_mission;

  bool is_rc_change;
};

#endif // RC_CONTROL_H
