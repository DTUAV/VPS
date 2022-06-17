#include "ros/ros.h"
#include "std_msgs/String.h"
#include "../include/kf_mpc_controller4/kalman.h"
#include "../include/kf_mpc_controller4/file_operator.h"
#include <eigen3/Eigen/Dense>
#include <eigen3/Eigen/Eigen>
#include <cmath>
#include "../include/kf_mpc_controller4/matplotlibcpp.h"

namespace plt = matplotlibcpp;
using namespace Eigen;
int main(int argc, char **argv)
{
  ros::init(argc, argv, "test_kalman");
  ros::NodeHandle nh;
  std::string data_file_name = "test_data.csv";
  std::vector<std::vector<std::string> > data;
  ReadCsvFile(data,data_file_name);
  std::vector<vector<float> > float_data;
  StringToFloat(data,float_data);
  vector<float> data_x;
  vector<float> data_y;
  vector<float> data_z;
  for(int i = 0; i<float_data.size();i++)
  {
    for(int j =0;j<float_data.at(i).size();j++)
    {
      if(j==0)
      {
        data_x.push_back(float_data.at(i).at(j));
      }
      else if (j==1)
      {
       data_y.push_back(float_data.at(i).at(j));
      }
      else if(j==2)
      {
        data_z.push_back(float_data.at(i).at(j));
      }
    }
  }


  Kalman kf;
  int dim_observer = 3;//dim_meansure:3:x,y,z
  int n = 3*dim_observer;//dim_state:9:x,y,z,vx,vy,vz,ax,ay,az
  kf.init(n,dim_observer);
  float deta_t = 1;//sameple time
  float deta_t_2 = 0.5*deta_t*deta_t;
/*
 * x(k) = Ax(k-1)+Bu(k-1)+w(k-1)
 * y(K) = Cx(k)+v(k)
 * w(k-1)~(0,Q)
 * v(k)~(0,R)
 * or
 * x(k) = Ax(k-1)
 * y(k) = Cx(k)
*/
  //init A
  kf.transMat<<1,0,0,deta_t,0,0,deta_t_2,0,0,
               0,1,0,0,deta_t,0,0,deta_t_2,0,
               0,0,1,0,0,deta_t,0,0,deta_t_2,
               0,0,0,1,0,0,deta_t,0,0,
               0,0,0,0,1,0,0,deta_t,0,
               0,0,0,0,0,1,0,0,deta_t,
               0,0,0,0,0,0,1,0,0,
               0,0,0,0,0,0,0,1,0,
               0,0,0,0,0,0,0,0,1;
  //init C
 kf.measureMat<<1,0,0,0,0,0,0,0,0,
                0,1,0,0,0,0,0,0,0,
                0,0,1,0,0,0,0,0,0;
 //init Q
 double q = 1; //
 double q1 = 0.00085, q2 = 98;
   kf.processNoiseCov << q,0,0,0,0,0,0,0,0,
                         0,q,0,0,0,0,0,0,0,
                         0,0,q,0,0,0,0,0,0,
                         0,0,0,q,0,0,0,0,0,
                         0,0,0,0,q,0,0,0,0,
                         0,0,0,0,0,q,0,0,0,
                         0,0,0,0,0,0,q,0,0,
                         0,0,0,0,0,0,0,q,0,
                         0,0,0,0,0,0,0,0,q;

   kf.processNoiseCov_Slow << q1,0,0,0,0,0,0,0,0,
                              0,q1,0,0,0,0,0,0,0,
                              0,0,q1,0,0,0,0,0,0,
                              0,0,0,q1,0,0,0,0,0,
                              0,0,0,0,q1,0,0,0,0,
                              0,0,0,0,0,q1,0,0,0,
                              0,0,0,0,0,0,q1,0,0,
                              0,0,0,0,0,0,0,q1,0,
                              0,0,0,0,0,0,0,0,q1;

   kf.processNoiseCov_Fast << q2,0,0,0,0,0,0,0,0,
                              0,q2,0,0,0,0,0,0,0,
                              0,0,q2,0,0,0,0,0,0,
                              0,0,0,q2,0,0,0,0,0,
                              0,0,0,0,q2,0,0,0,0,
                              0,0,0,0,0,q2,0,0,0,
                              0,0,0,0,0,0,q2,0,0,
                              0,0,0,0,0,0,0,q2,0,
                              0,0,0,0,0,0,0,0,q2;
  //init R
   double r = 100; //100 ~ 500
   double r_fast = 255;
   double r_slow = 100;

   kf.measureNosiseCov <<r,0,0,
                         0,r,0,
                         0,0,r;
   kf.measureNosiseCov_Slow << r_slow,0,0,
                               0,r_slow,0,
                               0,0,r_slow;
   kf.measureNosiseCov_Fast << r_fast,0,0,
                               0,r_fast,0,
                               0,0,r_fast;

   kf.errorCovOpt << 1,0,0,0,0,0,0,0,0,
                     0,1,0,0,0,0,0,0,0,
                     0,0,1,0,0,0,0,0,0,
                     0,0,0,1,0,0,0,0,0,
                     0,0,0,0,1,0,0,0,0,
                     0,0,0,0,0,1,0,0,0,
                     0,0,0,0,0,0,1,0,0,
                     0,0,0,0,0,0,0,1,0,
                     0,0,0,0,0,0,0,0,1;

   kf.eps = 0.01;
   kf.rho = 1;
   vector<double> filt_data_x;
   vector<double> filt_data_y;
   vector<double> filt_data_z;
   vector<double> plot_init_x;
   vector<double> plot_init_y;
   vector<double> plot_predict_x;
   vector<double> plot_predict_y;
   MatrixXf last_measurement(3,1);
   last_measurement<<0,0,0;
   for(int i=0;i<data_x.size();i++)
   {
     MatrixXf measurement(3,1);
     if(i%50 == 0)
     {
       measurement<<data_x.at(i),data_y.at(i),data_z.at(i);
       last_measurement = measurement;
     }
     else
     {
      measurement = last_measurement;
     }
     kf.predict();
     kf.correct(measurement);
     plot_init_x.push_back(data_x.at(i));
     plot_init_y.push_back(data_y.at(i));
     filt_data_x.push_back(kf.stateOpt(0,0));
     filt_data_y.push_back(kf.stateOpt(1,0));
     filt_data_z.push_back(kf.stateOpt(2,0));
     plot_predict_x.push_back(kf.statePre(0,0));
     plot_predict_y.push_back(kf.statePre(1,0));
       if (i % 10 == 0) {
         // Clear previous plot
         plt::clf();
         // Plot line from given x and y data. Color is selected automatically.
         plt::plot(filt_data_x, filt_data_y);
         // Plot a line whose name will show up as "log(x)" in the legend.
         plt::named_plot("measure_data", plot_init_x, plot_init_y);
         plt::named_plot("predict_data",plot_predict_x,plot_predict_y);

         // Set x-axis to interval [0,1000000]
        // plt::xlim(0, 100*100);

         // Add graph title
         plt::title("Sample figure");
         // Enable legend.
         plt::legend();
         // Display plot continuously
         plt::pause(0.01);
       }
   }
      cout<<filt_data_x.size()<<endl;
    cout<<"end"<<endl;




  ros::spin();

  return 0;
}

/*
首先，你必须实例化一个KF对象，然后调用kf.init()，初始化各个参数。
根据自己的需求定义状态转移矩阵A
定义状态测量矩阵H
定义  预测噪声协方差矩阵Q和测量噪声协方差矩阵R
初始化后验估计值协方差矩阵P

2预测

调用predict();
3矫正

传入测量值
调用correct();
输出结果。
*/
