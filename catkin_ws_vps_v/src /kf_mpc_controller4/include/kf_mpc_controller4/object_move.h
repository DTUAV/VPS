#ifndef OBJECT_MOVE_H
#define OBJECT_MOVE_H
#include "Eigen/Eigen"
#include "iostream"

/*  Linear System Model  is Described by Space State Function
 * X(k) = AX(k-1) + Bu(k-1) + w(k-1)
 * y(k) = HX(k) + v(k)
 * w(k-1) ~ N(0,Q)
 * v(k) ~ N(0,R)
 */

/*  object_move using
 * (1) define object_move instance: object_move *object = new object_move(state_dim,observe_dim,input_dim)
 * (2) Init object Model:update_A,update_B,update_H
 * (3) Init object State and Sensor Output:update_X,update_Y,update_U
 * (4) Init Model Noise and Sensor Noise: update_Model_Noise,update_Sensor_Noise
 *
 * (5) running Model:update_U,update_Model_Noise,update_Sensor_Noise,update_model
 * (6) get the object sensor output: getY
*/


class object_move
{
public:
  object_move();
  object_move(int state_dim,int observe_dim,int input_dim);
  bool update_X(Eigen::MatrixXf X);
  bool update_A(Eigen::MatrixXf A);
  bool update_B(Eigen::MatrixXf B);
  bool update_U(Eigen::MatrixXf U);
  bool update_Y(Eigen::MatrixXf Y);
  bool update_H(Eigen::MatrixXf H);
  bool update_Model_Noise(Eigen::MatrixXf W);
  bool update_Sensor_Noise(Eigen::MatrixXf V);

  bool update_Q(Eigen::MatrixXf Q);//
  bool update_R(Eigen::MatrixXf R);//

  bool reset_Object_State(Eigen::MatrixXf X);//



  bool update_model();

  Eigen::MatrixXf getY();
private:
  int _state_dim;//the dimension of system state
  int _observe_dim;//the dimension of observer state
  int _input_dim;//the dimension of control input

  Eigen::MatrixXf _X;//the matrix of system state: _state_dim * 1

  Eigen::MatrixXf _A;//the matrix of state transition:: _state_dim * _state_dim
  Eigen::MatrixXf _B;//the matrix of control input gain: _state_dim * _input_dim
  Eigen::MatrixXf _U;//the matrix of control input: _input_dim * 1
  Eigen::MatrixXf _Y;//the matrix of observe state: _observe_dim * 1
  Eigen::MatrixXf _H;//the matrix of observer gain: _observe_dim * _state_dim
  Eigen::MatrixXf _W;//the matrix of model noise:  _state_dim * 1
  Eigen::MatrixXf _V;//the matrix of sensor noise; _observe_dim * 1


  Eigen::MatrixXf _Q;//the matrix of model error: _state_dim * _state_dim //
  Eigen::MatrixXf _R;//the matrix of observe error: _observe_dim * _observe_dim //
};

#endif // OBJECT_MOVE_H
