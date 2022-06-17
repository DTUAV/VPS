#include "../include/kf_mpc_controller4/controller_network_wrapper.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "controller_network_wrapper");

  controller_network_wrapper network_wrapper;

  ros::spin();
  return 0;
}
