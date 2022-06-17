#ifndef KF_MPC_CONTROLLER_RC_H
#define KF_MPC_CONTROLLER_RC_H

#include <QWidget>
#include "ros/ros.h"
#include "mavros_msgs/RCIn.h"
#include "geometry_msgs/PoseStamped.h"
namespace Ui {
class kf_mpc_controller_rc;
}

class kf_mpc_controller_rc : public QWidget
{
  Q_OBJECT

public:
  explicit kf_mpc_controller_rc(QWidget *parent = 0);
  ~kf_mpc_controller_rc();

private Q_SLOTS:

  void on_start_offboard_clicked();

  void on_stop_offboard_clicked();

  void on_land_clicked();

  void on_next_point_clicked();

private:
  std::string _start_offboard_pub_topic;
  std::string _target_object_position_pub_topic;

  ros::Publisher _start_offboard_pub;
  ros::Publisher _target_object_position_pub;

  int _start_offboard_ch;
  int _land_ch;

  float _init_x;
  float _init_y;
  float _init_z;

  Ui::kf_mpc_controller_rc *ui;
};

#endif // KF_MPC_CONTROLLER_RC_H
