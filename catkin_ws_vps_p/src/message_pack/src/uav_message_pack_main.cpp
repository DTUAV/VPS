#include "ros/ros.h"
#include "../include/message_pack/uav_message_pack.h"
int main(int argc, char **argv)
{
  ros::init(argc, argv, "uav_message_pack_main");
  ros::NodeHandle nh;
  uav_message_pack uav;
  ros::spin();
  return 0;
}
