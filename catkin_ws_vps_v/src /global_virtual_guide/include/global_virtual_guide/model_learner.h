#ifndef MODEL_LEARNER_H
#define MODEL_LEARNER_H
#include "eigen3/Eigen/Eigen"
#include "rls.h"
#include "ros/ros.h"
#include "geometry_msgs/PoseStamped.h"
#include "geometry_msgs/TwistStamped.h"
#include "dt_message_package/update_model_parameter.h"
#include "dt_message_package/model_learner_info.h"
#include "std_msgs/Bool.h"
#include "iostream"
#include "pthread.h"
#include <mutex>
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
struct velocity_stu
{
  float velocity_x;
  float velocity_y;
  float velocity_z;
};

class model_learner
{
public:
  model_learner();
  model_learner(int theta_dim,int y_dim);
  bool Init_Ap(Eigen::MatrixXf Ap);
  bool Init_theta(Eigen::MatrixXf theta);
  bool Init_phi(Eigen::MatrixXf phi);
  bool Init_sigma(Eigen::MatrixXf sigma);
  bool Init_p(Eigen::MatrixXf p);
  bool Init_k(Eigen::MatrixXf k);
  bool Init_y(Eigen::MatrixXf y);

  bool Update_phi(Eigen::MatrixXf phi);
  bool Update_y(Eigen::MatrixXf y);

  void reset_model_learn();

  void local_position_sub_cb(const geometry_msgs::PoseStampedConstPtr& msg);
  void target_velocity_sub_cb(const geometry_msgs::TwistStampedConstPtr& msg);
  void model_learner_control_sub_cb(const std_msgs::BoolConstPtr& msg);

  static void *model_learner_run(void *args);
private:

  Eigen::MatrixXf _Ap;//The System State Transfer Matrix

  std::string _local_position_sub_topic;
  std::string _target_velocity_sub_topic;
  std::string _model_learner_control_sub_topic;
  std::string _update_model_parameter_pub_topic;
  std::string _model_learner_info_pub_topic;

  ros::Subscriber _local_position_sub;
  ros::Subscriber _target_velocity_sub;
  ros::Subscriber _model_learner_control_sub;

  ros::Publisher _update_model_parameter_pub;
  ros::Publisher _model_learner_info_pub;

  pthread_t _model_learner_run_thread;

  float _model_learner_run_hz;

  float _last_position_x;
  float _last_position_y;
  float _last_position_z;

  double _model_err_x;
  double _model_err_y;
  double _model_err_z;

  std::vector<velocity_stu> velocity_queue;
  bool _is_first_run;


  bool _isInitTheta;
  bool _isInitPhi;
  bool _isInitSigma;
  bool _isInitP;
  bool _isInitK;
  bool _isInitY;
  bool _isInitAp;

  bool _isModelRange;//false:only estimated the diagonal matrix of B. true:estimate all the matrix of B

  int _theta_dim;                           //the dimension of estimated parameter
  int _y_dim;                               //the dimension of observer state

  bool _isStopModelLearn;
  bool _isGetModel;
  rls *_rls_estimator;
};

#endif // MODEL_LEARNER_H
