#include "../include/kf_mpc_controller4/state_estimator.h"

int main(int argc, char *argv[])
{
  ros::init(argc, argv, "state_estimator");
  int state_dim = 9;
  int observe_dim = 3;
  int input_dim = 3;
  state_estimator estimator;
  estimator.InitKf(state_dim,observe_dim,input_dim);
  float deta_t = 0.33;//the sameple time
  float deta_t_2 = 0.5*deta_t*deta_t;
  float model_err = 1e-3;
  float measure_err = 1e-4;
  Eigen::MatrixXf A(state_dim,state_dim);
  A<<1,0,0,deta_t,0,0,deta_t_2,0,0,
     0,1,0,0,deta_t,0,0,deta_t_2,0,
     0,0,1,0,0,deta_t,0,0,deta_t_2,
     0,0,0,1,0,0,deta_t,0,0,
     0,0,0,0,1,0,0,deta_t,0,
     0,0,0,0,0,1,0,0,deta_t,
     0,0,0,0,0,0,1,0,0,
     0,0,0,0,0,0,0,1,0,
     0,0,0,0,0,0,0,0,1;
  Eigen::MatrixXf B(state_dim,input_dim);
  B<<0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0;
  Eigen::MatrixXf U(input_dim,1);
  U<<0,0,0;
  Eigen::MatrixXf Y(observe_dim,1);
  Y<<0,0,0;
  Eigen::MatrixXf H(observe_dim,state_dim);
  H<<1,0,0,0,0,0,0,0,0,
     0,1,0,0,0,0,0,0,0,
     0,0,1,0,0,0,0,0,0;
  Eigen::MatrixXf P(state_dim,state_dim);
  P<<1,0,0,0,0,0,0,0,0,
     0,1,0,0,0,0,0,0,0,
     0,0,1,0,0,0,0,0,0,
     0,0,0,1,0,0,0,0,0,
     0,0,0,0,1,0,0,0,0,
     0,0,0,0,0,1,0,0,0,
     0,0,0,0,0,0,1,0,0,
     0,0,0,0,0,0,0,1,0,
     0,0,0,0,0,0,0,0,1;
  Eigen::MatrixXf Q(state_dim,state_dim);
  Q<<model_err,0,0,0,0,0,0,0,0,
     0,model_err,0,0,0,0,0,0,0,
     0,0,model_err,0,0,0,0,0,0,
     0,0,0,model_err,0,0,0,0,0,
     0,0,0,0,model_err,0,0,0,0,
     0,0,0,0,0,model_err,0,0,0,
     0,0,0,0,0,0,model_err,0,0,
     0,0,0,0,0,0,0,model_err,0,
     0,0,0,0,0,0,0,0,model_err;
 Eigen::MatrixXf R(observe_dim,observe_dim);
 R<<measure_err,0,0,
    0,measure_err,0,
    0,0,measure_err;
 Eigen::MatrixXf X(state_dim,1);
 X<<0,0,0,0,0,0,0,0,0;
 estimator.InitSystemModel(A,B,Q);
 estimator.InitObserveModel(H,R);
 estimator.InitEstimateErr(P);
 estimator.InitControlInput(U);
 estimator.InitObserveData(Y);
 estimator.InitSystemState(X);
 estimator.InitMeasureErr(measure_err);
 estimator.InitModelErr(model_err);
  ros::spin();
  return 0;
}
