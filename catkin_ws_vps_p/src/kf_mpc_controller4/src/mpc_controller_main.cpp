#include "../include/kf_mpc_controller4/mpc_controller.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "mpc_controller");
  ros::NodeHandle n("~");
  float max_velocity = 1;
  n.getParam("max_velocity",max_velocity);
  float sample_time = 0.33;//0.33;//0.0002;//1.5;//0.002;//1.5;//0.5//0.2
  float input_sample_time = 0.33;
  float input_out_cost = 0.0002;
  float input_tracking_cost = 0.9;
  float control_out_cost = 0.0002;//
 // float control_out_cost = 0.06;//0.0002;//0.0002;---->model learn
  float tracking_cost = 0.9;//1.1:zhengdong;//0.9;
  if(n.getParam("sample_time",input_sample_time))
  {
    sample_time = input_sample_time;
  }
  if(n.getParam("out_cost",input_out_cost))
  {
    control_out_cost = input_out_cost;
  }
  if(n.getParam("tracking_cost",input_tracking_cost))
  {
    tracking_cost = input_tracking_cost;
  }

  int state_dim = 3;//the dimension of system state
  int observe_dim = 3;//the dimension of observer state
  int input_dim = 3;//the dimension of control input
  int control_window = 5;//the window size of control input
  int predict_window = 8;//the window size of system state predicted
  mpc_controller controller;

  Eigen::MatrixXf A(state_dim,state_dim);
  A<<1,0,0,
     0,1,0,
     0,0,1;
  Eigen::MatrixXf B(state_dim,input_dim);
  B<<sample_time,0,0,
     0,sample_time,0,
     0,0,sample_time;

  /*  model learn
  B<<0.03649265319108963,0,0,//
     0,0.036455314606428146,0,//
     0,0,0.02114194445312023;//
*/
  Eigen::MatrixXf H(observe_dim,state_dim);
  H<<1,0,0,
     0,1,0,
     0,0,1;
  Eigen::MatrixXf omega(state_dim,1);
 omega<<0,0,0;
//omega<<-0.00013263855362311006,-0.0006406850880011916,-0.006116332020610571;//model_learn
  Eigen::MatrixXf W = Eigen::MatrixXf::Zero(predict_window*state_dim,1);
  for(int i =0;i<predict_window;i++)
  {
    W.block<3,1>(i*state_dim,0) = omega;
  }
// std::cout<<"W: "<<W<<std::endl;
  Eigen::MatrixXf G = Eigen::MatrixXf::Zero(predict_window*observe_dim,predict_window*state_dim);
  for(int i = 0; i<predict_window;i++)
  {
    for(int j = 0; j<predict_window;j++)
    {
      if(j<i)
      {
        Eigen::MatrixXf A_temp = A;
        for(int k = 0; k<i-j-1; k++)
        {
          A_temp = A_temp * A;
        }
       // PHI.block<observe_dim,input_dim>(i*observe_dim,j) = H*A_temp*B;
        G.block<3,3>(i*observe_dim,j*state_dim) = H*A_temp;
      }
      else if(j==i)
      {
       // PHI.block<observe_dim,input_dim>(i*observe_dim,j) = H*B;
        G.block<3,3>(i*observe_dim,j*state_dim) = H;
      }
      else
      {
        //PHI.block<observe_dim,input_dim>(i*observe_dim,j) = Eigen::MatrixXf::Zero(observe_dim,input_dim);
        G.block<3,3>(i*observe_dim,j*state_dim) = Eigen::MatrixXf::Zero(observe_dim,state_dim);
      }
    }
  }
 //std::cout<<"G: "<<G<<std::endl;
  Eigen::MatrixXf X(state_dim,1);
  X<<0,0,0;
  Eigen::MatrixXf XX = Eigen::MatrixXf::Zero(predict_window*state_dim,1);
  Eigen::MatrixXf U  = Eigen::MatrixXf::Zero(input_dim,1);
  float max_output = max_velocity;//
  float min_output = -max_velocity;//
  Eigen::MatrixXf R = control_out_cost * Eigen::MatrixXf::Identity(input_dim*control_window,input_dim*control_window);
  Eigen::MatrixXf M = tracking_cost*Eigen::MatrixXf::Identity(observe_dim*predict_window,observe_dim*predict_window);
 // std::cout<<"M: "<<M<<std::endl;
  Eigen::MatrixXf PHI = Eigen::MatrixXf::Zero(predict_window*observe_dim,input_dim*control_window);//the matrix of control matrix: (_predict_window*_observe_dim) * (_input_dim*_control_window)
  Eigen::MatrixXf F = Eigen::MatrixXf::Zero(predict_window*observe_dim,state_dim);//the matrix of predict system state: (_predict_window*_observe_dim) * _state_dim)
  //get the matrix _F
 // std::cout<<"dddddd"<<std::endl;
  for(int i = 0; i<predict_window;i++)
  {
    Eigen::MatrixXf A_temp = A;
    for(int j = 0;j<i;j++)
    {
      A_temp = A_temp * A;
    }
   // F.block<observe_dim,state_dim>(i*observe_dim,0) = H*A_temp;
    F.block<3,3>(i*observe_dim,0) = H*A_temp;
   // std::cout<<"i: "<<i<<std::endl;
  }

 // std::cout<<"F: "<<F<<std::endl;

  //get the matrix _PHI
  for(int i = 0; i<predict_window;i++)
  {
    for(int j = 0; j<control_window;j++)
    {
      if(j<i)
      {
        Eigen::MatrixXf A_temp = A;
        for(int k = 0; k<i-j-1; k++)
        {
          A_temp = A_temp * A;
        }
       // PHI.block<observe_dim,input_dim>(i*observe_dim,j) = H*A_temp*B;
        PHI.block<3,3>(i*observe_dim,j*input_dim) = H*A_temp*B;
      }
      else if(j==i)
      {
       // PHI.block<observe_dim,input_dim>(i*observe_dim,j) = H*B;
        PHI.block<3,3>(i*observe_dim,j*input_dim) = H*B;
      }
      else
      {
        //PHI.block<observe_dim,input_dim>(i*observe_dim,j) = Eigen::MatrixXf::Zero(observe_dim,input_dim);
        PHI.block<3,3>(i*observe_dim,j*input_dim) = Eigen::MatrixXf::Zero(observe_dim,input_dim);
      }
    }
  }
 //std::cout<<"PHI: "<<PHI<<std::endl;
  controller.Init_System_Model(A,B,H,omega);
  controller.Init_System_State(X,XX);
  controller.Init_Control_Output(U,max_output,min_output);
  controller.Init_MPC_Proble(R,M);
  controller.Init_Other_Matrix(F,PHI,W,G);

  ros::spin();
  return 0;
}
