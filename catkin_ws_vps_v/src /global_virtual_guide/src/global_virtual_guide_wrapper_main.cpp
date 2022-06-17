#include "../include/global_virtual_guide/global_virtual_guide_wrapper.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "global_virtual_guide_wrapper");
  global_virtual_guide_wrapper guide_wrapper;
  ros::MultiThreadedSpinner spinner(1); // Use 1 threads
  spinner.spin();
  return 0;
}
