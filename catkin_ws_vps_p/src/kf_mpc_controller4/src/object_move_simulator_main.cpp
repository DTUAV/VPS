#include "../include/kf_mpc_controller4/object_move_simulator.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "object_move_simulator");
  int state_dim = 3;//the dimension of system state
  int observe_dim = 3;//the dimension of observer state
  int input_dim = 3;//the dimension of control input
  object_move_simulator *simulator = new object_move_simulator(state_dim,observe_dim,input_dim);
  float sample_time = 0.1;
  Eigen::MatrixXf A(state_dim,state_dim);
  A<<1,0,0,
     0,1,0,
     0,0,1;
  Eigen::MatrixXf B(state_dim,input_dim);
  B<<sample_time,0,0,
     0,sample_time,0,
     0,0,sample_time;
  Eigen::MatrixXf H(observe_dim,state_dim);
  H<<1,0,0,
     0,1,0,
     0,0,1;
  Eigen::MatrixXf X(state_dim,1);
  X<<0,0,0;
  Eigen::MatrixXf U  = Eigen::MatrixXf::Zero(input_dim,1);
  Eigen::MatrixXf Y  = Eigen::MatrixXf::Zero(observe_dim,1);
  Eigen::MatrixXf W  = Eigen::MatrixXf::Zero(state_dim,1);
  Eigen::MatrixXf V  = Eigen::MatrixXf::Zero(observe_dim,1);

  simulator->Init_Object_Model(A,B,H);
  simulator->Init_Noise(W,V);
  simulator->Init_Object_State(X,Y,U);

  ros::spin();
  return 0;
}
