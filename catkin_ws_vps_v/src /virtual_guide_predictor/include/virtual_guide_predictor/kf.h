#ifndef KF_H
#define KF_H
#include <eigen3/Eigen/Eigen>
#include <iostream>
/*  Linear System Model  is Described by Space State Function
 * X(k) = AX(k-1) + Bu(k-1) + w(k-1)
 * y(k) = HX(k) + v(k)
 * w(k-1) ~ N(0,Q)
 * v(k) ~ N(0,R)
 */

/* Kalman Filter Running Mode
 * (1)only have control input data
 * (2)only have observe data by sensors
 * (3)control input data and observe data by sensors
 */

/* kalman Filter Using
 * step 1: define kf--> kf(state_dim,observe_dim,input_dim)
 * step 2: init kf --:
 *     (1) set the matrix A -->update_A(A);
 *     (2) set the matrix B -->update_B(B);
 *     (3) set the matrix H -->update_H(H);
 *     (4) set the matrix Q -->update_Q(Q);
 *     (5) set the matrix R -->update_R(R);
 *     (6) set the matrix P -->update_P(P);
 *     (7) set the matrix U -->update_U(U);
 *     (8) set the matrix Y -->update_Y(Y);
 * step 3: running kf
 *     (1) update the control input U -->update_U(U)
 *     (2) update the observe data Y -->update_Y(Y)
 *     (3) update kf                 -->updateKF()
 * step 4: get kf results
 *     (1) get the estimate system state X -->getX()
 *     (2) get the model predict system state X_Predict -->getX_Predict()
 *     (3) get the estimate error P -->getP()
 *
*/
class kf
{
public:
  /*
   * state_dim: the dimension of system state
   * observe_dim: the dimension of observer state - from sensor data
   * input_dim: the dimension of control input
   */
  kf();
  kf(int state_dim,int observe_dim,int input_dim);
  bool update_A(Eigen::MatrixXf A);
  bool update_B(Eigen::MatrixXf B);
  bool update_U(Eigen::MatrixXf U);
  bool update_Y(Eigen::MatrixXf Y);
  bool update_H(Eigen::MatrixXf H);
  bool update_Q(Eigen::MatrixXf Q);
  bool update_R(Eigen::MatrixXf R);
  bool update_P(Eigen::MatrixXf P);
  bool update_X(Eigen::MatrixXf X);
  bool update_model_err(float model_err);
  bool update_measure_err(float measure_err);
  void updateKF();
  bool update_Time(double time_s);
  Eigen::MatrixXf getX();
  Eigen::MatrixXf getX_Predict();
  Eigen::MatrixXf getP();
  Eigen::MatrixXf getA();
  Eigen::MatrixXf getB();
  Eigen::MatrixXf getU();
  Eigen::MatrixXf getY();
  Eigen::MatrixXf getH();
  Eigen::MatrixXf getQ();
  Eigen::MatrixXf getR();
  Eigen::MatrixXf getK();
  float get_model_err();
  float get_measure_err();
  double getTime();



private:
  int _state_dim;//the dimension of system state
  int _observe_dim;//the dimension of observer state
  int _input_dim;//the dimension of control input
  float _current_time_s;//the time of current data
  float _model_err;//the matrix Q
  float _measure_err;//the matrix R
  Eigen::MatrixXf _X;//the matrix of system state: _state_dim * 1
  Eigen::MatrixXf _X_Predict;//the matrix of system predicted state
  Eigen::MatrixXf _A;//the matrix of state transition:: _state_dim * _state_dim
  Eigen::MatrixXf _B;//the matrix of control input gain: _state_dim * _input_dim
  Eigen::MatrixXf _U;//the matrix of control input: _input_dim * 1
  Eigen::MatrixXf _Y;//the matrix of observe state: _observe_dim * 1
  Eigen::MatrixXf _H;//the matrix of observer gain: _observe_dim * _state_dim
  Eigen::MatrixXf _P;//the matrix of estimation error: _state_dim * _state_dim
  Eigen::MatrixXf _K;//the matrix of kalman filter gain: _state_dim * _state_dim
  Eigen::MatrixXf _Q;//the matrix of model error: _state_dim * _state_dim
  Eigen::MatrixXf _R;//the matrix of observe error: _observe_dim * _observe_dim







};

#endif // KF_H
