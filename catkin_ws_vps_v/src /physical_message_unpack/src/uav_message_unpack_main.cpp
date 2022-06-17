#include "ros/ros.h"
#include "../include/physical_message_unpack/uav_message_unpack.h"

int main(int argc, char **argv)
{
  ros::init(argc, argv, "uav_main");
  ros::NodeHandle nh;
  uav_message_unpack uav;
  ros::MultiThreadedSpinner spinner(2); // Use 2 threads
  spinner.spin();
  return 0;
}
