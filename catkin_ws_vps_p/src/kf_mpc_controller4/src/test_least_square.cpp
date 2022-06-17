#include "iostream"
#include "../include/kf_mpc_controller4/file_operator.h"
#include "../include/kf_mpc_controller4/matplotlibcpp.h"
#include "../include/kf_mpc_controller4/least_square.h"
#include "ros/ros.h"
#include <boost/circular_buffer.hpp>
namespace plt = matplotlibcpp;
int main(int argc, char **argv)
{
  ros::init(argc, argv, "test_kalman");
  ros::NodeHandle nh;

  //---------------------->get data<-----------------------------
  std::string data_file_name = "sample_data.csv";
  std::vector<std::vector<std::string> > data;
  ReadCsvFile(data,data_file_name);
  std::vector<vector<float> > float_data;
  StringToFloat(data,float_data);
  vector<float> data_t;
  vector<float> data_x;
  vector<float> data_y;
  vector<float> data_z;
  for(int i = 0; i<float_data.size();i++)
  {
    for(int j =0;j<float_data.at(i).size();j++)
    {
      if(j==0)
      {
        data_t.push_back(float_data.at(i).at(j));
      }
      else if(j==1)
      {
        data_x.push_back(float_data.at(i).at(j));
      }
      else if (j==2)
      {
       data_y.push_back(float_data.at(i).at(j));
      }
      else if(j==3)
      {
        data_z.push_back(float_data.at(i).at(j));
      }
    }
  }

 least_square least_square_x;
 least_square least_square_y;
 least_square least_square_z;

 least_square_x.set_Order(3);
 least_square_y.set_Order(3);
 least_square_z.set_Order(3);

 //vector<float> running_t;
 //vector<float> running_x;
// vector<float> running_y;
// vector<float> running_z;
 //running_t.push_back(0);
 //running_x.push_back(0);
 //running_y.push_back(0);
 //running_z.push_back(0);
 int data_window = 10;
 boost::circular_buffer<float> queue_t;
 queue_t.resize(data_window);
 boost::circular_buffer<float> queue_x;
 queue_x.resize(data_window);
 for(int i =0; i< data_x.size();i++)
 {
   queue_t.push_back(data_t.at(i));
   queue_x.push_back(data_x.at(i));
   if(queue_t.size() >= data_window)
   {
     vector<float> running_t;
     vector<float> running_x;
     for(int j=0;j<data_window;j++)
     {
       running_t.push_back(queue_t.at(j) - queue_t.at(0));
       running_x.push_back(queue_x.at(j));
     }

     least_square_x.set_T(running_t);
     least_square_x.set_X(running_x);
     vector<float> ls_result;
     ls_result.resize(queue_t.size());
     Eigen::VectorXf result = least_square_x.get_Result2();
     for(int l=0;l<queue_t.size();l++)
     {
       for(int k = result.size()-1;k>=0;k--)
       {
         ls_result.at(l) = ls_result.at(l) + pow(running_t.at(l),result.size()-1-k)*result[k];
       }
        std::cout<<"predict_data: "<<ls_result.at(l)<<std::endl;
        std::cout<<"origin_data: "<<running_x.at(l)<<std::endl;
     }
   }
 }
  ros::spin();
  return 0;
}























