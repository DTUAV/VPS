#include "../include/model_learn_wrapper/model_learn_wrapper.h"

int main(int argc, char **argv)
{
  ros::init(argc, argv, "uav_main");
  ros::NodeHandle nh;
  model_learn_wrapper model_learn_wrapper_node;
  ros::spin();
  return 0;
}
