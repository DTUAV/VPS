#include "../include/kf_mpc_controller4/object_move.h"

object_move::object_move()
{

}
object_move::object_move(int state_dim, int observe_dim, int input_dim)
{
  _state_dim = state_dim;
  _observe_dim = observe_dim;
  _input_dim = input_dim;
  _X = Eigen::MatrixXf::Zero(_state_dim, 1);
  _A = Eigen::MatrixXf::Zero(_state_dim,_state_dim);
  _B = Eigen::MatrixXf::Zero(_state_dim,_input_dim);
  _U = Eigen::MatrixXf::Zero(_input_dim,1);
  _Y = Eigen::MatrixXf::Zero(_observe_dim,1);
  _H = Eigen::MatrixXf::Zero(_observe_dim,_state_dim);
  _Q = Eigen::MatrixXf::Zero(_state_dim,_state_dim);
  _R = Eigen::MatrixXf::Zero(_observe_dim,_observe_dim);
  _W = Eigen::MatrixXf::Zero(_state_dim,1);
  _V = Eigen::MatrixXf::Zero(_observe_dim,1);
}

bool object_move::reset_Object_State(Eigen::MatrixXf X)
{
  _U = Eigen::MatrixXf::Zero(_input_dim,1);
  bool ret = update_X(X) ;
  if(ret)
  {
    _Y = _H*_X;
    return true;
  }
  else
  {
    return false;
  }
}

bool object_move::update_model()
{
   _X = _A*_X + _B*_U + _W;
   _Y = _H*_X + _V;
   return true;
}

Eigen::MatrixXf object_move::getY()
{
  return _Y;
}

bool object_move::update_X(Eigen::MatrixXf X)
{
  std::cout<<"X.rows(): "<<X.rows()<<std::endl;
  if(X.rows() ==_state_dim && X.cols() == 1)
  {
    _X = X;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix X Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}

bool object_move::update_A(Eigen::MatrixXf A)
{
  if(A.cols() ==_state_dim && A.rows() ==_state_dim)
  {
    _A = A;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix A Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}
bool object_move::update_B(Eigen::MatrixXf B)
{
 // std::cout<<"ssss"<<B.cols()<<std::endl;
  if(B.rows() ==_state_dim && B.cols() ==_input_dim)
  {
    _B = B;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix B Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}
bool object_move::update_U(Eigen::MatrixXf U)
{
  if(U.rows() ==_input_dim && U.cols() == 1)
  {
    _U = U;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix U Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}
bool object_move::update_Y(Eigen::MatrixXf Y)
{
  if(Y.rows() ==_observe_dim && Y.cols() == 1)
  {
    _Y = Y;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix Y Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}
bool object_move::update_H(Eigen::MatrixXf H)
{
  if(H.rows() ==_observe_dim && H.cols() == _state_dim)
  {
    _H = H;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix H Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}
bool object_move::update_Q(Eigen::MatrixXf Q)
{
  if(Q.cols() == _state_dim && Q.rows() == _state_dim)
  {
    _Q = Q;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix Q Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }

}
bool object_move::update_R(Eigen::MatrixXf R)
{
  if(R.cols() == _observe_dim && R.rows() == _observe_dim)
  {
    _R = R;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix R Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}
bool object_move::update_Model_Noise(Eigen::MatrixXf W)
{
  if(W.rows() == _state_dim && W.cols() == 1)
  {
    _W = W;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix W Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}
bool object_move::update_Sensor_Noise(Eigen::MatrixXf V)
{
  if(V.rows() == _observe_dim && V.cols() == 1)
  {
    _V = V;
    return true;
  }
  else
  {
   std::cout<<"Update the Matrix V Fail, Inconsistent Dimensions !!!"<<std::endl;
   return false;
  }
}
