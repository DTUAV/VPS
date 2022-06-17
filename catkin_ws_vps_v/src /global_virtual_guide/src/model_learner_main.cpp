#include "../include/global_virtual_guide/model_learner.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "model_learner");
  ros::NodeHandle n("~");
  int theta_dim = 12;
  int y_dim = 3;
  model_learner model_learner_node(theta_dim,y_dim);
  Eigen::MatrixXf Ap = Eigen::MatrixXf::Zero(y_dim,y_dim);
  Ap<<1,0,0,0,1,0,0,0,1;
  model_learner_node.Init_Ap(Ap);
  Eigen::MatrixXf theta = Eigen::MatrixXf::Zero(theta_dim,1);
  float init_sample_time_x =1.0;
  n.getParam("init_sample_time_x",init_sample_time_x);
  float init_sample_time_y =1.0;
  n.getParam("init_sample_time_y",init_sample_time_y);
  float init_sample_time_z =1.0;
  n.getParam("init_sample_time_z",init_sample_time_z);
  theta <<init_sample_time_x,0,0,0,0,init_sample_time_y,0,0,0,0,init_sample_time_z,0;
  model_learner_node.Init_theta(theta);
  Eigen::MatrixXf phi = Eigen::MatrixXf::Zero(theta_dim,y_dim);
  phi<<0,0,0,
       0,0,0,
       0,0,0,
       1,0,0,
       0,0,0,
       0,0,0,
       0,0,0,
       0,1,0,
       0,0,0,
       0,0,0,
       0,0,0,
       0,0,1;
  model_learner_node.Init_phi(phi);
  Eigen::MatrixXf sigma = Eigen::MatrixXf::Zero(y_dim,1);
  sigma<<0,0,0;
  model_learner_node.Init_sigma(sigma);
  Eigen::MatrixXf p = Eigen::MatrixXf::Zero(theta_dim,theta_dim);
  for(int i = 0;i<p.cols();i++)
  {
    for(int j=0;j<p.rows();j++)
    {
      if(i==j)
      {
        p(i,j) = 1;
      }
      else
      {
        p(i,j) = 0;
      }
    }
  }
 model_learner_node.Init_p(p);
 Eigen::MatrixXf k = Eigen::MatrixXf::Zero(theta_dim,y_dim);
 model_learner_node.Init_k(k);
 Eigen::MatrixXf y = Eigen::MatrixXf::Zero(y_dim,1);
 model_learner_node.Init_y(y);
  ros::spin();
  return 0;
}
