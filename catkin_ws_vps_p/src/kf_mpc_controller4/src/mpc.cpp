#include "../include/kf_mpc_controller4/mpc.h"

mpc::mpc()
{

}

mpc::mpc(int state_dim, int observe_dim, int input_dim, int control_window, int predict_window)
{
  _state_dim = state_dim;
  _observe_dim = observe_dim;
  _input_dim = input_dim;
  _control_window = control_window;
  _predict_window = predict_window;
  _X = Eigen::MatrixXf::Zero(_state_dim,1);
  _A = Eigen::MatrixXf::Zero(_state_dim,_state_dim);
  _B = Eigen::MatrixXf::Zero(_state_dim,_input_dim);
  _U = Eigen::MatrixXf::Zero(_input_dim,1);
  _Y = Eigen::MatrixXf::Zero(_observe_dim,1);
  _H = Eigen::MatrixXf::Zero(_observe_dim,_state_dim);
  _R = Eigen::MatrixXf::Zero(_input_dim*_control_window,_input_dim*_control_window);
  _F = Eigen::MatrixXf::Zero(_predict_window*_observe_dim,_state_dim);
  _PHI = Eigen::MatrixXf::Zero(_predict_window*_observe_dim,_predict_window*_observe_dim);
  _XX = Eigen::MatrixXf::Zero(_predict_window*_state_dim,1);
  _M = Eigen::MatrixXf::Zero(_predict_window*_observe_dim,_predict_window*_observe_dim);
  _omega = Eigen::MatrixXf::Zero(_state_dim,1);
  _G = Eigen::MatrixXf::Zero(_predict_window*_observe_dim,_predict_window*_state_dim);
  _W = Eigen::MatrixXf::Zero(_predict_window*_state_dim,1);

}
bool mpc::Update_W(Eigen::MatrixXf W)
{
  return Init_W(W);
}
bool mpc::Update_G(Eigen::MatrixXf G)
{
  return Init_G(G);
}

bool mpc::Update_omega(Eigen::MatrixXf omega)
{
  return Init_omega(omega);
}


bool mpc::Init_W(Eigen::MatrixXf W)
{
  if(W.rows() == _predict_window*_state_dim && W.cols() == 1)
  {
    _W = W;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix W Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}


bool mpc::Init_G(Eigen::MatrixXf G)
{
  if(G.rows() == _predict_window*_observe_dim && G.cols() == _predict_window*_state_dim)
  {
    _G = G;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix G Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool mpc::Init_omega(Eigen::MatrixXf omega)
{
  if(omega.rows() == _state_dim && omega.cols() == 1)
  {
    _omega = omega;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix omega Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}


bool mpc::Init_M(Eigen::MatrixXf M)
{
  if(M.rows() == _predict_window*_observe_dim && M.cols() == _predict_window*_observe_dim)
  {
    _M = M;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix M Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool mpc::Init_XX(Eigen::MatrixXf XX)
{
  if(XX.rows() == _predict_window*_state_dim && XX.cols() == 1)
  {
    _XX = XX;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix XX Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}


bool mpc::Init_MPC()
{


}




bool mpc::Init_PHI(Eigen::MatrixXf PHI)
{
  if(PHI.rows() == _predict_window*_observe_dim && PHI.cols() == _input_dim*_control_window)
  {
    _PHI = PHI;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix PHI Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}


bool mpc::Init_F(Eigen::MatrixXf F)
{
  if(F.rows() == _predict_window*_observe_dim && F.cols() == _state_dim)
  {
    _F = F;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix F Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool mpc::Init_R(Eigen::MatrixXf R)
{
  if(R.rows() == _input_dim*_control_window && R.cols() == _input_dim*_control_window)
  {
    _R = R;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix R Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool mpc::Init_X(Eigen::MatrixXf X)
{
  if(X.rows() ==_state_dim && X.cols() == 1)
  {
    _X = X;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix X Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool mpc::Init_A(Eigen::MatrixXf A)
{
  if(A.cols() ==_state_dim && A.rows() ==_state_dim)
  {
    _A = A;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix A Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool mpc::Init_B(Eigen::MatrixXf B)
{
  if(B.rows() ==_state_dim && B.cols() ==_input_dim)
  {
    _B = B;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix B Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool mpc::Init_Y(Eigen::MatrixXf Y)
{
  if(Y.rows() ==_observe_dim && Y.cols() == 1)
  {
    _Y = Y;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix Y Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool mpc::Init_U(Eigen::MatrixXf U)
{
  if(U.rows() ==_input_dim && U.cols() == 1)
  {
    _U = U;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix U Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool mpc::Init_H(Eigen::MatrixXf H)
{
  if(H.rows() ==_observe_dim && H.cols() == _state_dim)
  {
    _H = H;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix H Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool mpc::Init_Cotrol_Limit(float max_control_out, float min_control_out)
{
  _max_controller_out = max_control_out;
  _min_controller_out = min_control_out;
  return true;
}


bool mpc::Update_MPC()
{
  //Eigen::MatrixXf U = (((_PHI.transpose())*_PHI + _R).inverse())*(_PHI.transpose())*(_XX - _F*_X);
  Eigen::MatrixXf U = ((_PHI.transpose()*_M*_PHI + _R.transpose()).inverse())*(_PHI.transpose())*_M*(_XX  - _F*_X - _G*_W);
//  for(int i = 0; i<_input_dim;i++)
//  {
//    if(U(i,0)<_min_controller_out)
//    {
//      _U(i,0) = _min_controller_out;
//    }
//    else if(U(i,0)>_max_controller_out)
//    {
//      _U(i,0) = _max_controller_out;
//    }
//    else
//    {
//      _U(i,0) = U(i,0);
//    }
//  }
  bool isValid = true;
   for(int i=0;i<U.rows();i++)
   {
     if(std::isnan(U(i,0))||std::isinf(U(i,0)))
     {
       isValid = false;
       _XX = _F*_X;
     }
   }
   if(isValid)
   {
     for(int i = 0; i<_input_dim;i++)
     {
       if(U(i,0)<_min_controller_out)
       {
         _U(i,0) = _min_controller_out;
       }
       else if(U(i,0)>_max_controller_out)
       {
         _U(i,0) = _max_controller_out;
       }
       else
       {
         _U(i,0) = U(i,0);
       }
     }
   }
}


bool mpc::Update_X(Eigen::MatrixXf X)
{
  if(X.rows() == _state_dim && X.cols() == 1)
  {
    _X = X;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix X Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}


bool mpc::Update_XX(Eigen::MatrixXf XX)
{
  if(XX.rows() == _predict_window*_state_dim && XX.cols() == 1)
  {
    _XX = XX;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix XX Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}


bool mpc::Update_PHI(Eigen::MatrixXf PHI)
{
  if(PHI.rows() == _predict_window*_observe_dim && PHI.cols() == _input_dim*_control_window)
  {
    _PHI = PHI;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix PHI Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool mpc::Update_F(Eigen::MatrixXf F)
{
  if(F.rows() == _predict_window*_observe_dim && F.cols() == _state_dim)
  {
    _F = F;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix F Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool mpc::Update_R(Eigen::MatrixXf R)
{
  if(R.rows() == _input_dim*_control_window && R.cols() == _input_dim*_control_window)
  {
    _R = R;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix R Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}

bool mpc::Update_A(Eigen::MatrixXf A)
{
  if(A.cols() ==_state_dim && A.rows() ==_state_dim)
  {
    _A = A;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix A Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool mpc::Update_B(Eigen::MatrixXf B)
{
  if(B.rows() ==_state_dim && B.cols() ==_input_dim)
  {
    _B = B;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix B Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
bool mpc::Update_H(Eigen::MatrixXf H)
{
  if(H.rows() ==_observe_dim && H.cols() == _state_dim)
  {
    _H = H;
    return true;
  }
  else
  {
    std::cout<<"MPC-->Update the Matrix H Fail, Inconsistent Dimensions !!!"<<std::endl;
    return false;
  }
}
Eigen::MatrixXf mpc::Get_A()
{
  return _A;
}
Eigen::MatrixXf mpc::Get_B()
{
  return _B;
}
Eigen::MatrixXf mpc::Get_H()
{
  return _H;
}
Eigen::MatrixXf mpc::Get_X()
{
  return _X;
}
Eigen::MatrixXf mpc::Get_U()
{
  return _U;
}
Eigen::MatrixXf mpc::Get_Y()
{
  return _Y;
}
Eigen::MatrixXf mpc::Get_R()
{
  return _R;
}
Eigen::MatrixXf mpc::Get_F()
{
  return _F;
}
Eigen::MatrixXf mpc::Get_PHI()
{
  return _PHI;
}
Eigen::MatrixXf mpc::Get_XX()
{
  return _XX;
}










