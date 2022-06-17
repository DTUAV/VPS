#include "../include/kf_mpc_controller4/object_move_simulator.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "object_move_simulator");
   ros::NodeHandle n("~");
   float sample_time_tme = 0.33;
   bool if_config_st = n.getParam("sample_time",sample_time_tme);
  int state_dim = 3;//the dimension of system state
  int observe_dim = 3;//the dimension of observer state
  int input_dim = 3;//the dimension of control input
  object_move_simulator *simulator = new object_move_simulator(state_dim,observe_dim,input_dim);
  float sample_time = sample_time_tme;
  Eigen::MatrixXf A(state_dim,state_dim);
  A<<1,0,0,
     0,1,0,
     0,0,1;
  Eigen::MatrixXf B(state_dim,input_dim);
  Eigen::MatrixXf W  = Eigen::MatrixXf::Zero(state_dim,1);
  if(if_config_st)
  {
     B<<sample_time,0,0,
       0,sample_time,0,
       0,0,sample_time;
      W<<0,0,0;
  }
  else
  {
    B<<0.03649265319108963,0,0,       //gazebo uav model
         0,0.036455314606428146,0,   //gazebo uav model
         0,0,0.02114194445312023;   //gazebo uav model
     W<<-0.00013263855362311006,-0.0006406850880011916,-0.006116332020610571;
  }
  Eigen::MatrixXf H(observe_dim,state_dim);
  H<<1,0,0,
     0,1,0,
     0,0,1;
  Eigen::MatrixXf X(state_dim,1);
  X<<0,0,0;
  Eigen::MatrixXf U  = Eigen::MatrixXf::Zero(input_dim,1);
  Eigen::MatrixXf Y  = Eigen::MatrixXf::Zero(observe_dim,1);


  Eigen::MatrixXf V  = Eigen::MatrixXf::Zero(observe_dim,1);



  simulator->Init_Object_Model(A,B,H);
  simulator->Init_Noise(W,V);
  simulator->Init_Object_State(X,Y,U);

  ros::spin();
  return 0;
}
