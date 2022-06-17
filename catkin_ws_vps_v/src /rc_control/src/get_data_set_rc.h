#ifndef GET_DATA_SET_RC_H
#define GET_DATA_SET_RC_H

#include <QWidget>
#include "ros/ros.h"
#include "mavros_msgs/RCIn.h"
#include "geometry_msgs/PoseStamped.h"
namespace Ui {
class get_data_set_rc;
}

class get_data_set_rc : public QWidget
{
  Q_OBJECT

public:
  explicit get_data_set_rc(QWidget *parent = 0);
  ~get_data_set_rc();

private Q_SLOTS:
  void on_start_offboard_clicked();

  void on_stop_offboard_clicked();

  void on_land_clicked();

  void on_start_mission_clicked();

  void on_change_mission_point_clicked();

  void on_stop_mission_clicked();

private:
  std::string _start_offboard_pub_topic;

  ros::Publisher _start_offboard_pub;

  int _start_offboard_ch;
  int _land_ch;
  int _rc_change_mission_ch;
  int _rc_start_mission_ch;

  mavros_msgs::RCIn _rc_msg;


  Ui::get_data_set_rc *ui;
};

#endif // GET_DATA_SET_RC_H
