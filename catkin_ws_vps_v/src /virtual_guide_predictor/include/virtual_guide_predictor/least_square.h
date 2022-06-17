#ifndef LEAST_SQUARE_H
#define LEAST_SQUARE_H
#include <eigen3/Eigen/Eigen>
#include <iostream>
#include <vector>
#include <math.h>

class least_square
{
public:
  least_square();
  least_square(std::vector<float> t, std::vector<float> x, int order);
  //~least_quare();
  bool set_T(std::vector<float> t);
  bool set_X(std::vector<float> x);
  bool set_Order(int order);
  int get_Order();
  std::vector<float> get_T();
  std::vector<float> get_X();
  Eigen::VectorXf get_Result();
  Eigen::VectorXf get_Result2();

private:
  std::vector<float> _t;
  std::vector<float> _x;
  int _order;


};

#endif // LEAST_SQUARE_H
