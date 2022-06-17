#include "../include/kf_mpc_controller4/rls.h"

rls::rls()
{

}
rls::rls(int theta_dim, int y_dim)
{
  _theta_dim = theta_dim;
  _y_dim = y_dim;
  _theta = Eigen::MatrixXf::Zero(_theta_dim, 1);
  _phi =   Eigen::MatrixXf::Zero(_theta_dim,_y_dim);
  _sigma = Eigen::MatrixXf::Zero(_y_dim,1);
  _p =     Eigen::MatrixXf::Zero(_theta_dim,_theta_dim);
  _k =     Eigen::MatrixXf::Zero(_theta_dim,_y_dim);
  _y =     Eigen::MatrixXf::Zero(_y_dim,1);
}
bool rls::update_theta(Eigen::MatrixXf theta)
{
  if(theta.rows() ==_theta_dim && theta.cols() ==1)
  {
    _theta = theta;
    return true;
  }
  else
  {
    std::cout<<"Update the Matrix theta Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool rls::update_phi(Eigen::MatrixXf phi)
{
  if(phi.rows() ==_theta_dim && phi.cols() == _y_dim)
  {
    _phi = phi;
    return true;
  }
  else
  {
    std::cout<<"Update the Matrix phi Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool rls::update_sigma(Eigen::MatrixXf sigma)
{
  if(sigma.rows() ==_y_dim && sigma.cols() ==1)
  {
    _sigma = sigma;
    return true;
  }
  else
  {
    std::cout<<"Update the Matrix sigma Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool rls::update_p(Eigen::MatrixXf p)
{
  if(p.rows() ==_theta_dim && p.cols() ==_theta_dim)
  {
    _p = p;
    return true;
  }
  else
  {
    std::cout<<"Update the Matrix p Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool rls::update_k(Eigen::MatrixXf k)
{
  if(k.rows() ==_theta_dim && k.cols() == _y_dim)
  {
    _k = k;
    return true;
  }
  else
  {
    std::cout<<"Update the Matrix k Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool rls::update_y(Eigen::MatrixXf y)
{
  if(y.rows() ==_y_dim && y.cols() ==1)
  {
    _y = y;
    return true;
  }
  else
  {
    std::cout<<"Update the Matrix y Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

Eigen::MatrixXf rls::get_theta()
{
  return _theta;
}

Eigen::MatrixXf rls::get_sigma()
{
  return _sigma;
}
Eigen::MatrixXf rls::get_p()
{
  return _p;
}
Eigen::MatrixXf rls::get_k()
{
  return _k;
}
Eigen::MatrixXf rls::get_y()
{
  return _y;
}
Eigen::MatrixXf rls::get_phi()
{
  return _phi;
}

bool rls::update_rls()
{
  _sigma = _y - _phi.transpose() * _theta;
  _p = (_p.inverse() + _phi * _phi.transpose()).inverse();
  _k = _p * _phi;
  _theta = _theta + _k * _sigma;
}
