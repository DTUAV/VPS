#include "../include/network_delay_model/network_delay_model.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "network_delay_model");
  network_delay_model network;
  ros::MultiThreadedSpinner spinner(4); // Use 2 threads
  spinner.spin();
  return 0;
}
