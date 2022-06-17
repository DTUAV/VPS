#include "../include/kf_mpc_controller4/state_predictor.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "state_predictor");

  state_predictor predictor;

  ros::spin();
  return 0;
}
