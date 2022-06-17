#include "../include/kf_mpc_controller_wrapper/kf_mpc_controller_wrapper.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "kf_mpc_controller_wrapper");
  kf_mpc_controller_wrapper control_wrapper;
  ros::spin();
  return 0;
}
