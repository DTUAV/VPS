#include "../include/kf_mpc_controller4/kf.h"
kf::kf()
{

}
kf::kf(int state_dim,int observe_dim,int input_dim)
{
  _state_dim = state_dim;                               //Init the system state dimension
  _observe_dim = observe_dim;                           //Init the sensor measured dimension
  _input_dim = input_dim;                               //Init the control input dimension
  _X = Eigen::MatrixXf::Zero(_state_dim, 1);            //Init the current state of system
  _A = Eigen::MatrixXf::Zero(_state_dim,_state_dim);    //Init the state transition matrix of system
  _B = Eigen::MatrixXf::Zero(_state_dim,_input_dim);    //Init the control matrix of system
  _U = Eigen::MatrixXf::Zero(_input_dim,1);             //Init the control input of system
  _Y = Eigen::MatrixXf::Zero(_observe_dim,1);           //Init the sensor observed data of system
  _H = Eigen::MatrixXf::Zero(_observe_dim,_state_dim);  //Init the sensor measured matrix of system
  _P = Eigen::MatrixXf::Zero(_state_dim,_state_dim);    //Init the state estimated error of system
  _K = Eigen::MatrixXf::Zero(_state_dim,_state_dim);    //Init the state estimated error transfer gain of system
  _Q = Eigen::MatrixXf::Zero(_state_dim,_state_dim);    //Init the model error matrix of system
  _R = Eigen::MatrixXf::Zero(_observe_dim,_observe_dim);//Init the measured error matrix of system
  _current_time_s = 0;
}

void kf::updateKF()
{
  //predict system state by using object model state function
  _X_Predict = _A*_X + _B*_U;
  _P = _A*_P*_A.transpose()  + _Q;

  //correct system state by using measurement data
  _K = (_P*_H.transpose())*(_H*_P*_H.transpose() + _R).inverse();
  _X = _X_Predict + _K*(_Y - _H*_X_Predict);
  _P = _P -_K*_H*_P;
}
Eigen::MatrixXf kf::getK()
{
  return _K;
}


Eigen::MatrixXf kf::getX()
{
  return _X;
}
Eigen::MatrixXf kf::getX_Predict()
{
  return _X_Predict;
}
Eigen::MatrixXf kf::getA()
{
  return _A;
}
Eigen::MatrixXf kf::getB()
{
  return _B;
}
Eigen::MatrixXf kf::getU()
{
  return _U;
}
Eigen::MatrixXf kf::getY()
{
  return _Y;
}
Eigen::MatrixXf kf::getH()
{
  return _H;
}
Eigen::MatrixXf kf::getQ()
{
  return _Q;
}
Eigen::MatrixXf kf::getR()
{
  return _R;
}
Eigen::MatrixXf kf::getP()
{
  return _P;
}
double kf::getTime()
{
  return _current_time_s;
}
bool kf::update_X(Eigen::MatrixXf X)
{
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
float kf::get_model_err()
{
  return _model_err;
}
float kf::get_measure_err()
{
  return _measure_err;
}

bool kf::update_P(Eigen::MatrixXf P)
{
  if(P.cols() ==_state_dim && P.rows() ==_state_dim)
  {
    _P = P;
    return true;
  }
  else
  {
    std::cout<<"Update the Matrix P Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool kf::update_A(Eigen::MatrixXf A)
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
bool kf::update_B(Eigen::MatrixXf B)
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
bool kf::update_U(Eigen::MatrixXf U)
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
bool kf::update_Y(Eigen::MatrixXf Y)
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
bool kf::update_H(Eigen::MatrixXf H)
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
bool kf::update_Q(Eigen::MatrixXf Q)
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
bool kf::update_R(Eigen::MatrixXf R)
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

bool kf::update_model_err(float model_err)
{
  /*
   *[model_err 0 0 0 0
   * 0 model_err 0 0 0
   * 0 0 model_err 0 0
   * 0 0 0 model_err 0
   * 0 0 0 0 model_err]
   *
   *
  */
  for(int i = 0;i<_Q.cols();i++)
  {
    for(int j=0;j<_Q.rows();j++)
    {
      if(i==j)
      {
        _Q(i,j) = model_err;
      }
      else
      {
        _Q(i,j) = 0;
      }
    }
  }
  return true;
}

bool kf::update_measure_err(float measure_err)
{
  /*
   *[measure_err 0 0 0 0
   * 0 measure_err 0 0 0
   * 0 0 measure_err 0 0
   * 0 0 0 measure_err 0
   * 0 0 0 0 measure_err]
   *
   */

  for(int i = 0;i<_R.cols();i++)
  {
    for(int j=0;j<_R.rows();j++)
    {
      if(i==j)
      {
        _R(i,j) = measure_err;
      }
      else
      {
        _R(i,j) = 0;
      }

    }
  }
  return true;
}
bool kf::update_Time(double time_s)
{

  // std::cout<<"time_s: "<<time_s<<std::endl;
  if(time_s >=_current_time_s)
  {
    _current_time_s = time_s;
    return true;
  }
  else
  {
    std::cout<<"Update the System Time Fail, New Time Less Than Old Time !!!"<<std::endl;
    return false;
  }
}




































