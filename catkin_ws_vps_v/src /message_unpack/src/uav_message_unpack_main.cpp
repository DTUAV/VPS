#include "ros/ros.h"
#include "../include/message_unpack/uav_message_unpack.h"

int main(int argc, char **argv)
{
  ros::init(argc, argv, "uav_main");
  ros::NodeHandle nh;
  uav_message_unpack uav;
  ros::spin();
  return 0;
}
