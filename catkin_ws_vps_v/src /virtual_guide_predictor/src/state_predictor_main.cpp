#include "../include/virtual_guide_predictor/state_predictor.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "state_predictor");

  state_predictor predictor;

  ros::spin();
  return 0;
}
