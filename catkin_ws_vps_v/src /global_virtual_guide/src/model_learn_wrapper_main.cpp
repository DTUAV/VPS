#include "../include/global_virtual_guide/model_learn_wrapper.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "model_learn_wrapper");
  model_learn_wrapper learn_warpper;
  ros::MultiThreadedSpinner spinner(1); // Use 1 threads
  spinner.spin();
  return 0;
}
