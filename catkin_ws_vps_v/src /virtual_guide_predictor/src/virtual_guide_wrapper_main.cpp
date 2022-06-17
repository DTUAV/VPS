#include "../include/virtual_guide_predictor/virtual_guide_wrapper.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "virtual_guide_wrapper");
  virtual_guide_wrapper guide_wrapper;

  ros::spin();
  return 0;
}
