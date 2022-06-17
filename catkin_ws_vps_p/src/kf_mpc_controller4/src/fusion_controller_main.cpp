#include "../include/kf_mpc_controller4/fusion_controller.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "fusion_controller");

  fusion_controller fusionController;

  ros::spin();
  return 0;
}
