#include "../include/global_virtual_guide/command_wrapper.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "command_wrapper");
  command_wrapper command_wrapper_node;
  ros::MultiThreadedSpinner spinner(1); // Use 1 threads
  spinner.spin();
  return 0;
}
