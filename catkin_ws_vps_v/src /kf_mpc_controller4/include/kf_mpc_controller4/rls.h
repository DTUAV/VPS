#ifndef RLS_H
#define RLS_H
#include <Eigen/Eigen>
#include "iostream"
/*------------------learn model format-------------------------------
 *****Yk = phi'*theta*******
 * this paper:Xp(k) = Ap*Xp(k-1)+Bp*u(k-1)+w(k-1)
 * this paper:Ap = [1 0 0;0 1 0;0 0 1];
 * this paper:Bp = [b1 b2 b3;b4 b5 b6;b7 b8 b9];
 * this paper:w(k-1) = [wx;wy;wz];
 * this paper:Yk = [Px(k);Py(k);Pz(k)]
 * this paper:phi' = [u(k-1) 1 0 0 0 0;0 0 u(k-1) 1 0 0; 0 0 0 0 u(k-1) 1];
 * this paper:theta = [b1;b2;b3;wx;b4;b5;b6;wy;b7;b8;b9;wz];
*/

class rls
{
public:
  rls();
  rls(int theta_dim,int y_dim);
  bool update_theta(Eigen::MatrixXf theta);
  bool update_phi(Eigen::MatrixXf phi);
  bool update_sigma(Eigen::MatrixXf sigma);
  bool update_p(Eigen::MatrixXf p);
  bool update_k(Eigen::MatrixXf k);
  bool update_y(Eigen::MatrixXf y);

  bool update_rls();

  Eigen::MatrixXf get_theta();
  Eigen::MatrixXf get_sigma();
  Eigen::MatrixXf get_p();
  Eigen::MatrixXf get_k();
  Eigen::MatrixXf get_y();
  Eigen::MatrixXf get_phi();


private:
  int _theta_dim;                           //the dimension of estimated parameter
  int _y_dim;                               //the dimension of observer state
  Eigen::MatrixXf _theta;                   //the matrix of estimated parameter:_theta_dim * 1
  Eigen::MatrixXf _phi;                     //the matrix of parameter weigt: _theta_dim * _y_dim
  Eigen::MatrixXf _sigma;                   //the matrix of model and observer error: _y_dim * 1
  Eigen::MatrixXf _p;                       //the matrix of paramter estimated error: _theta_dim * _theta_dim
  Eigen::MatrixXf _k;                       //the parameter update gain: _theta_dim * _y_dim
  Eigen::MatrixXf _y;                       //the matrix of observer data: _y_dim * 1
};

#endif // RLS_H
