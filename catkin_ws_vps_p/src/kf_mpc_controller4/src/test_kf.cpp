#include "../include/kf_mpc_controller4/kf.h"
#include "iostream"
#include "../include/kf_mpc_controller4/file_operator.h"
//#include "../include/kf_mpc_controller4/matplotlibcpp.h"
#include "dt_message_package/object_move_position.h"
#include "dt_message_package/object_move_state.h"
#include "ros/ros.h"
//namespace plt = matplotlibcpp;
int main(int argc, char **argv)
{
  ros::init(argc, argv, "test_kalman");
  ros::NodeHandle nh;
  ros::Publisher pub = nh.advertise<dt_message_package::object_move_position>("/virtual/uav0/current_position",10);
 // ros::Publisher state_pub = nh.advertise<dt_message_package::object_move_state>("/virtual/uav0/current_move_state",10);

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
//------------------------------>kf init<------------------------------
  int state_dim = 9;
  int observe_dim = 3;
  int input_dim = 3;
  kf test_kf(state_dim,observe_dim,input_dim);
  float deta_t = 1;//the sameple time
  float deta_t_2 = 0.5*deta_t*deta_t;
  float model_err = 1e-3;
  float measure_err = 1e-4;
  Eigen::MatrixXf A(state_dim,state_dim);
  A<<1,0,0,deta_t,0,0,deta_t_2,0,0,
     0,1,0,0,deta_t,0,0,deta_t_2,0,
     0,0,1,0,0,deta_t,0,0,deta_t_2,
     0,0,0,1,0,0,deta_t,0,0,
     0,0,0,0,1,0,0,deta_t,0,
     0,0,0,0,0,1,0,0,deta_t,
     0,0,0,0,0,0,1,0,0,
     0,0,0,0,0,0,0,1,0,
     0,0,0,0,0,0,0,0,1;
  Eigen::MatrixXf B(state_dim,input_dim);
  B<<0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0,
     0,0,0;
  Eigen::MatrixXf U(input_dim,1);
  U<<0,0,0;
  Eigen::MatrixXf Y(observe_dim,1);
  Y<<0,0,0;
  Eigen::MatrixXf H(observe_dim,state_dim);
  H<<1,0,0,0,0,0,0,0,0,
     0,1,0,0,0,0,0,0,0,
     0,0,1,0,0,0,0,0,0;
  Eigen::MatrixXf P(state_dim,state_dim);
  P<<1,0,0,0,0,0,0,0,0,
     0,1,0,0,0,0,0,0,0,
     0,0,1,0,0,0,0,0,0,
     0,0,0,1,0,0,0,0,0,
     0,0,0,0,1,0,0,0,0,
     0,0,0,0,0,1,0,0,0,
     0,0,0,0,0,0,1,0,0,
     0,0,0,0,0,0,0,1,0,
     0,0,0,0,0,0,0,0,1;
  Eigen::MatrixXf Q(state_dim,state_dim);
  Q<<model_err,0,0,0,0,0,0,0,0,
     0,model_err,0,0,0,0,0,0,0,
     0,0,model_err,0,0,0,0,0,0,
     0,0,0,model_err,0,0,0,0,0,
     0,0,0,0,model_err,0,0,0,0,
     0,0,0,0,0,model_err,0,0,0,
     0,0,0,0,0,0,model_err,0,0,
     0,0,0,0,0,0,0,model_err,0,
     0,0,0,0,0,0,0,0,model_err;
 Eigen::MatrixXf R(observe_dim,observe_dim);
 R<<measure_err,0,0,
    0,measure_err,0,
    0,0,measure_err;
 test_kf.update_A(A);
 test_kf.update_B(B);
 test_kf.update_H(H);
 test_kf.update_P(P);
 test_kf.update_Q(Q);
 test_kf.update_R(R);
 test_kf.update_U(U);
 test_kf.update_Y(Y);

 vector<double> time;

 vector<double> filt_data_x;
 vector<double> filt_data_y;
 vector<double> filt_data_z;

 vector<double> plot_init_x;
 vector<double> plot_init_y;
 vector<double> plot_init_z;

 vector<double> plot_predict_x;
 vector<double> plot_predict_y;
 vector<double> plot_predict_z;
 ros::Rate rate(50);
//std::cout<<"ddddfffdd"<<std::endl;
 for(int i=0;i<data_x.size();i++)
 { //std::cout<<"ddddfffdd"<<data_x.at(i)<<data_y.at(i)<<data_z.at(i)<<std::endl;
   Eigen::MatrixXf measurement(observe_dim,1);
   measurement<<data_x.at(i),data_y.at(i),data_z.at(i);

   test_kf.update_Y(measurement);
   test_kf.updateKF();
   time.push_back(data_t.at(i));
   plot_init_x.push_back(data_x.at(i));
   plot_init_y.push_back(data_y.at(i));
   plot_init_z.push_back(data_z.at(i));

   dt_message_package::object_move_position object_position;
   object_position.time_stamp_ms = data_t.at(i)*1000;
   object_position.position_x = data_x.at(i);
   object_position.position_y = data_y.at(i);
   object_position.position_z = data_z.at(i);
   pub.publish(object_position);

   filt_data_x.push_back((test_kf.getX())(0,0));
   filt_data_y.push_back((test_kf.getX())(1,0));
   filt_data_z.push_back((test_kf.getX())(2,0));
  // std::cout<<"dddddd"<<std::endl;
   plot_predict_x.push_back((test_kf.getX_Predict())(0,0));
   plot_predict_y.push_back((test_kf.getX_Predict())(1,0));
   plot_predict_z.push_back((test_kf.getX_Predict())(2,0));

//   if (i % 10 == 0) {
//     // Clear previous plot
//     plt::clf();
//     // Plot line from given x and y data. Color is selected automatically.
//    // plt::plot(filt_data_x, filt_data_y);
//     // Plot a line whose name will show up as "log(x)" in the legend.
//     plt::named_plot("measure_data_x",time,plot_init_x);
//     plt::named_plot("measure_data_y",time,plot_init_y);
//     plt::named_plot("measure_data_z",time,plot_init_z);

//     plt::named_plot("kf_data_x",time,filt_data_x);
//     plt::named_plot("kf_data_y",time,filt_data_y);
//     plt::named_plot("kf_data_z",time,filt_data_z);

//     plt::named_plot("predict_data_x",time,plot_predict_x);
//     plt::named_plot("predict_data_y",time,plot_predict_y);
//     plt::named_plot("predict_data_z",time,plot_predict_z);

//     // Set x-axis to interval [0,1000000]
//    // plt::xlim(0, 100*100);

//     // Add graph title
//     plt::title("Sample figure");
//     // Enable legend.
//     plt::legend();
//     // Display plot continuously
//     plt::pause(0.1);
//   }
   rate.sleep();

 }
   ros::spin();
return 0;
}





































