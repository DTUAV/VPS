#ifndef SAMPLE_DATA_RC_H
#define SAMPLE_DATA_RC_H

#include <QWidget>
#include "ros/ros.h"
#include "mavros_msgs/RCIn.h"
#include "geometry_msgs/PoseStamped.h"
#include "std_msgs/Bool.h"
namespace Ui {
class sample_data_rc;
}

class sample_data_rc : public QWidget
{
  Q_OBJECT

public:
  explicit sample_data_rc(QWidget *parent = 0);
  ~sample_data_rc();

private Q_SLOTS:
  void on_start_sample_data_clicked();

  void on_end_sample_data_clicked();

  void on_start_offboard_clicked();

  void on_stop_offboard_clicked();

  void on_land_clicked();

private:
  std::string _start_offboard_pub_topic;
  std::string _target_object_position_pub_topic;
  std::string _start_sample_data_pub_topic;
  std::string _end_sample_data_pub_topic;

  ros::Publisher _start_offboard_pub;
  ros::Publisher _target_object_position_pub;
  ros::Publisher _start_sample_data_pub;
  ros::Publisher _end_sample_data_pub;

  int _start_offboard_ch;
  int _land_ch;

  float _init_x;
  float _init_y;
  float _init_z;
  Ui::sample_data_rc *ui;
};

#endif // SAMPLE_DATA_RC_H
