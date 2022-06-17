#ifndef MPC_H
#define MPC_H
#include "Eigen/Eigen"
#include "iostream"
/*
 * (1) System model : described by system state function: X(t) = AX(t-1)+BU(t-1)+w(t-1)  Y(t) = H(t)X(t)
 * (2) State Predict: system model based
 * (3) Rolling optimization: get the optimal control input
 *
*/

/* --------------->mpc using<-------------------------------------
 * ---->Init MPC Controller<-----
 * (1) Init System : define mpc--mpc *mpc_node = new mpc(state_dim,observe_dim,input_dim,control_window,predict_window)
 * (2) Init System Model: Init_A,Init_B,Init_H
 * (3) Init System State: Init_X
 * (4) Init System Target State: Init_XX
 * (5) Init Control Limit: Init_Control_Limit
 * (6) Init Optimal Problem Controller Output Cost: Init_R
 * (6) Init MPC other paramter: Init_MPC
 * (7) Init Control Output: Init_U
 * (8) Init other Matrix : Init_F,Init_PHI
 *
 *--->Running MPC Controller
 * (1) Update System State: Update_X
 * (2) Update System State Target: Update_XX
 * (3) Update MPC Controller: Update_MPC
 * (4) Get Controller Output: Get_U
*/
class mpc
{
public:
  mpc();
  mpc(int state_dim, int observe_dim, int input_dim, int control_window, int predict_window);
  bool Init_X(Eigen::MatrixXf X);
  bool Init_A(Eigen::MatrixXf A);
  bool Init_B(Eigen::MatrixXf B);
  bool Init_Y(Eigen::MatrixXf Y);
  bool Init_U(Eigen::MatrixXf U);
  bool Init_H(Eigen::MatrixXf H);
  bool Init_R(Eigen::MatrixXf R);
  bool Init_M(Eigen::MatrixXf M);//the tracking error weigt//
  bool Init_omega(Eigen::MatrixXf omega);//the model error(wx;wy;wz//)
  bool Init_XX(Eigen::MatrixXf XX);
  bool Init_Cotrol_Limit(float max_control_out, float min_control_out);
  bool Init_MPC();

  bool Init_F(Eigen::MatrixXf F);//
  bool Init_PHI(Eigen::MatrixXf PHI);
  bool Init_G(Eigen::MatrixXf G);//the omega-based predict matrix
  bool Init_W(Eigen::MatrixXf W);//the omega matrix with target predicted window

  bool Update_A(Eigen::MatrixXf A);
  bool Update_B(Eigen::MatrixXf B);
  bool Update_H(Eigen::MatrixXf H);
  bool Update_R(Eigen::MatrixXf R);
  bool Update_X(Eigen::MatrixXf X);
  bool Update_XX(Eigen::MatrixXf XX);
  bool Update_F(Eigen::MatrixXf F);
  bool Update_PHI(Eigen::MatrixXf PHI);

  bool Update_W(Eigen::MatrixXf W);
  bool Update_G(Eigen::MatrixXf G);
  bool Update_omega(Eigen::MatrixXf omega);


  bool Update_MPC();

  Eigen::MatrixXf Get_A();
  Eigen::MatrixXf Get_B();
  Eigen::MatrixXf Get_H();
  Eigen::MatrixXf Get_X();
  Eigen::MatrixXf Get_U();
  Eigen::MatrixXf Get_Y();
  Eigen::MatrixXf Get_R();
  Eigen::MatrixXf Get_XX();
  Eigen::MatrixXf Get_F();
  Eigen::MatrixXf Get_PHI();

//Y=F*x(k)+PHI*U
private:
  int _state_dim;//the dimension of system state
  int _observe_dim;//the dimension of observer state
  int _input_dim;//the dimension of control input
  int _control_window;//the window size of control input
  int _predict_window;//the window size of system state predicted
  float _max_controller_out;
  float _min_controller_out;

  Eigen::MatrixXf _X;//the matrix of system state: _state_dim * 1
  Eigen::MatrixXf _A;//the matrix of state transition:: _state_dim * _state_dim
  Eigen::MatrixXf _B;//the matrix of control input gain: _state_dim * _input_dim
  Eigen::MatrixXf _U;//the matrix of control input: _input_dim * 1
  Eigen::MatrixXf _Y;//the matrix of observe state: _observe_dim * 1
  Eigen::MatrixXf _H;//the matrix of observer gain: _observe_dim * _state_dim
  Eigen::MatrixXf _R;//the matrix of control input cost or gain: (_control_window * _input_dim) * (_control_window * _input_dim)
  Eigen::MatrixXf _F;//the matrix of predict system state: (_predict_window*_observe_dim) * _state_dim)
  Eigen::MatrixXf _PHI;//the matrix of control matrix: (_predict_window*_observe_dim) * (_input_dim*_control_window)
  Eigen::MatrixXf _XX;//the matrix of target system state: (_predict_window*_state_dim) * 1//[x0;y0;z0;x1;y1;y2;x3;y3;z3;x4;y4;z4...]

  Eigen::MatrixXf _M;//the matrix of tracking error cost or gain:(_predict_window*_observe_dim)*(predict_window*_observe_dim)
  Eigen::MatrixXf _omega;//the matrix of model error(wx;wy;wz):(_state_dim)*1
  Eigen::MatrixXf _G;//the omega-based predict matrix:(_predict_window*_observe_dim)*(_predict_window*_state_dim)
  Eigen::MatrixXf _W;//the omega matrix with target predicted window:(_predict_window*_state_dim)*(1)



};

#endif // MPC_H
