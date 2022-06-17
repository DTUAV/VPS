#include "../include/kf_mpc_controller4/least_square.h"

least_square::least_square()
{

}
least_square::least_square(std::vector<float> t, std::vector<float> x, int order)
{
  _t = t;
  _x = x;
  _order = order;
}
//least_square:: ~least_square()
//{

//}
Eigen::VectorXf least_square::get_Result()
{
  Eigen::Map<Eigen::VectorXf> sampleT(_t.data(),_t.size());
  Eigen::Map<Eigen::VectorXf> sampleX(_x.data(),_x.size());
  Eigen::MatrixXf mtxVandermonde(_t.size(),_order+1);
  Eigen::VectorXf colVandermonde = sampleT;
  for (size_t i = 0; i < _order + 1; ++i)
  {
    for (size_t j = 0; j< _order +1; ++j)
    {
      mtxVandermonde(i,j) = pow(colVandermonde(i),j);
    }
  }
  Eigen::VectorXf result = (mtxVandermonde.transpose()*mtxVandermonde).inverse()*(mtxVandermonde.transpose())*sampleX;
  return result;
}
Eigen::VectorXf least_square::get_Result2()
{
  int N = _order;
  // 创建A矩阵
  Eigen::MatrixXd A(_t.size(), N + 1);

  for (unsigned int i = 0; i < _t.size(); ++i) {  // 遍历所有点

    for (int n = N, dex = 0; n >= 1; --n, ++dex) {  // 遍历N到1阶
      A(i, dex) = pow(_t[i], n);
    }

    A(i, N) = 1;  //
  }
  // 创建B矩阵
  Eigen::MatrixXd B(_x.size(), 1);

  for (unsigned int i = 0; i < _x.size(); ++i) {
    B(i, 0) = _x[i];
  }
  // 创建矩阵W
  Eigen::MatrixXd W;
  W = (A.transpose() * A).inverse() * A.transpose() * B;
  // 打印W结果
  Eigen::VectorXf ret;
  ret.resize(N+1);
  for(int i=0;i<N+1;i++)
  {
    ret[i] = W(i);
  }
  //std::cout<<"--------------ret------------"<<ret<<std::endl;
  return ret;//wn--wn-1---wn-2--->w0
}

bool least_square::set_T(std::vector<float> t)
{
  _t = t;
}

bool least_square::set_X(std::vector<float> x)
{
  _x = x;
}

bool least_square::set_Order(int order)
{
  _order =  order;
}

std::vector<float> least_square::get_T()
{
  return _t;
}

std::vector<float> least_square::get_X()
{
  return _x;
}

int least_square::get_Order()
{
  return _order;
}
