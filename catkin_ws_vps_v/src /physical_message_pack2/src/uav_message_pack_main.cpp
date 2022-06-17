#include "ros/ros.h"
#include "../include/physical_message_pack2/uav_message_pack.h"
int main(int argc, char **argv)
{
  ros::init(argc, argv, "uav_message_pack_main");
  ros::NodeHandle nh;
  uav_message_pack uav;
  ros::spin();
  return 0;
}
