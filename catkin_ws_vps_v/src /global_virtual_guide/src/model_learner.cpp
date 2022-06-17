#include "../include/global_virtual_guide/model_learner.h"

model_learner::model_learner()
{

}

model_learner::model_learner(int theta_dim, int y_dim)
{
  ros::NodeHandle n("~");
  if(n.getParam("local_position_sub_topic",_local_position_sub_topic))
  {
    ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <local_position_sub_topic>--->%s\033[0m",_local_position_sub_topic.c_str());
  }
  else
  {
    _local_position_sub_topic = "/mavros/local_position/pose";
    ROS_WARN("Model_Learner----Not Get Topic Name: <local_position_sub_topic>--defalut: %s",_local_position_sub_topic.c_str());
  }

  if(n.getParam("target_velocity_sub_topic",_target_velocity_sub_topic))
  {
    ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <target_velocity_sub_topic>--->%s\033[0m",_target_velocity_sub_topic.c_str());
  }
  else
  {
    _target_velocity_sub_topic = "/target_velocity";
    ROS_WARN("Model_Learner----Not Get Topic Name: <target_velocity_sub_topic>--defalut: %s",_target_velocity_sub_topic.c_str());
  }

  if(n.getParam("update_model_parameter_pub_topic",_update_model_parameter_pub_topic))
  {
    ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <update_model_parameter_pub_topic>--->%s\033[0m",_update_model_parameter_pub_topic.c_str());
  }
  else
  {
    _update_model_parameter_pub_topic = "/update_model_parameter";
    ROS_WARN("Model_Learner----Not Get Topic Name: <update_model_parameter_pub_topic>--defalut: %s",_update_model_parameter_pub_topic.c_str());
  }

  if(n.getParam("model_learner_info_pub_topic",_model_learner_info_pub_topic))
  {
    ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <model_learner_info_pub_topic>--->%s\033[0m",_model_learner_info_pub_topic.c_str());
  }
  else
  {
    _update_model_parameter_pub_topic = "/model_learner_info";
    ROS_WARN("Model_Learner----Not Get Topic Name: <model_learner_info_pub_topic>--defalut: %s",_model_learner_info_pub_topic.c_str());
  }

  if(n.getParam("model_learner_run_hz",_model_learner_run_hz))
  {
    ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <model_learner_run_hz>--->%f\033[0m",_model_learner_run_hz);
  }
  else
  {
    _model_learner_run_hz = 30.0;
    ROS_WARN("Model_Learner----Not Get Topic Name: <model_learner_run_hz>--defalut: %f",_model_learner_run_hz);
  }

  if(n.getParam("is_model_range",_isModelRange))
  {
    if(_isModelRange)
    {
      ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <is_model_range>--->true\033[0m");
    }
    else
    {
      ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <is_model_range>--->false\033[0m");
    }

  }
  else
  {
    _isModelRange = false;
    ROS_WARN("Model_Learner----Not Get Topic Name: <is_model_range>--defalut: false");
  }

  if(n.getParam("model_err_x",_model_err_x))
  {
    ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <model_err_x>--->%f\033[0m",_model_err_x);
  }
  else
  {
    _model_err_x = 0.00018;
    ROS_WARN("Model_Learner----Not Get Topic Name: <model_err_x>--defalut: %f",_model_err_x);
  }

  if(n.getParam("model_err_y",_model_err_y))
  {
    ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <model_err_y>--->%f\033[0m",_model_err_y);
  }
  else
  {
    _model_err_x = 0.00018;
    ROS_WARN("Model_Learner----Not Get Topic Name: <model_err_y>--defalut: %f",_model_err_y);
  }

  if(n.getParam("model_err_z",_model_err_z))
  {
    ROS_INFO("\033[1;32m Model_Learner----Get Topic Name: <model_err_z>--->%f\033[0m",_model_err_z);
  }
  else
  {
    _model_err_x = 0.00018;
    ROS_WARN("Model_Learner----Not Get Topic Name: <model_err_z>--defalut: %f",_model_err_z);
  }

  n.getParam("model_learner_control_sub_topic",_model_learner_control_sub_topic);


  _model_learner_control_sub = n.subscribe(_model_learner_control_sub_topic,1,&model_learner::model_learner_control_sub_cb,this);
  _local_position_sub = n.subscribe(_local_position_sub_topic,1,&model_learner::local_position_sub_cb,this);
  _target_velocity_sub = n.subscribe(_target_velocity_sub_topic,1,&model_learner::target_velocity_sub_cb,this);

  _update_model_parameter_pub = n.advertise<dt_message_package::update_model_parameter>(_update_model_parameter_pub_topic,1);
  _model_learner_info_pub = n.advertise<dt_message_package::model_learner_info>(_model_learner_info_pub_topic,1);

  _theta_dim = theta_dim;
  _y_dim = y_dim;

  _isStopModelLearn = false;
  _isGetModel = false;

  _Ap = Eigen::MatrixXf::Zero(_y_dim,_y_dim);
  _Ap<<1,0,0,0,1,0,0,0,1;
  _rls_estimator = new rls(theta_dim,y_dim);
  _isInitTheta = false;
  _isInitPhi = false;
  _isInitSigma = false;
  _isInitP = false;
  _isInitK = false;
  _isInitY = false;
  _isInitAp = false;
  _is_first_run = true;
  velocity_queue.resize(2);
  velocity_queue.at(0).velocity_x = 0;
  velocity_queue.at(0).velocity_y = 0;
  velocity_queue.at(0).velocity_z = 0;
  velocity_queue.at(1).velocity_x = 0;
  velocity_queue.at(1).velocity_y = 0;
  velocity_queue.at(1).velocity_z = 0;

  int flag_thread = pthread_create(&_model_learner_run_thread,NULL,&model_learner::model_learner_run,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }
}

void model_learner::model_learner_control_sub_cb(const std_msgs::BoolConstPtr &msg)
{
  _isStopModelLearn = msg.get()->data;
  _isGetModel = false;
}

void model_learner::local_position_sub_cb(const geometry_msgs::PoseStampedConstPtr &msg)
{
  if(_is_first_run)
  {
    _last_position_x = msg.get()->pose.position.x;
    _last_position_y = msg.get()->pose.position.y;
    _last_position_z = msg.get()->pose.position.z;
    _is_first_run = false;
  }
  else
  {
    Eigen::MatrixXf y = Eigen::MatrixXf::Zero(_y_dim,1);
    Eigen::MatrixXf y_last = Eigen::MatrixXf::Zero(_y_dim,1);
    y_last<<_last_position_x,_last_position_y,_last_position_z;
    Eigen::MatrixXf y_transfer = _Ap*y_last;
    y(0,0) = msg.get()->pose.position.x - y_transfer(0,0);
    y(1,0) = msg.get()->pose.position.y - y_transfer(1,0);
    y(2,0) = msg.get()->pose.position.z - y_transfer(2,0);
    _last_position_x = msg.get()->pose.position.x;
    _last_position_y = msg.get()->pose.position.y;
    _last_position_z = msg.get()->pose.position.z;
    Update_y(y);
  }
}

void model_learner::target_velocity_sub_cb(const geometry_msgs::TwistStampedConstPtr &msg)
{
  Eigen::MatrixXf phi = Eigen::MatrixXf::Zero(_theta_dim,_y_dim);

  velocity_queue.at(0).velocity_x = velocity_queue.at(1).velocity_x;
  velocity_queue.at(0).velocity_y = velocity_queue.at(1).velocity_y;
  velocity_queue.at(0).velocity_z = velocity_queue.at(1).velocity_z;

  velocity_queue.at(1).velocity_x = msg.get()->twist.linear.x;
  velocity_queue.at(1).velocity_y = msg.get()->twist.linear.y;
  velocity_queue.at(1).velocity_z = msg.get()->twist.linear.z;

  if(_isModelRange)
  {
    phi(0,0) = velocity_queue.at(0).velocity_x;
    phi(1,0) = velocity_queue.at(0).velocity_y;
    phi(2,0) = velocity_queue.at(0).velocity_z;
    phi(3,0) = 1;
    phi(4,0) = 0;
    phi(5,0) = 0;
    phi(6,0) = 0;
    phi(7,0) = 0;
    phi(8,0) = 0;
    phi(9,0) = 0;
    phi(10,0) = 0;
    phi(11,0) = 0;

    phi(0,1) = 0;
    phi(1,1) = 0;
    phi(2,1) = 0;
    phi(3,1) = 0;
    phi(4,1) = velocity_queue.at(0).velocity_x;
    phi(5,1) = velocity_queue.at(0).velocity_y;
    phi(6,1) = velocity_queue.at(0).velocity_z;
    phi(7,1) = 1;
    phi(8,1) = 0;
    phi(9,1) = 0;
    phi(10,1) = 0;
    phi(11,1) = 0;

    phi(0,2) = 0;
    phi(1,2) = 0;
    phi(2,2) = 0;
    phi(3,2) = 0;
    phi(4,2) = 0;
    phi(5,2) = 0;
    phi(6,2) = 0;
    phi(7,2) = 0;
    phi(8,2) = velocity_queue.at(0).velocity_x;
    phi(9,2) = velocity_queue.at(0).velocity_y;
    phi(10,2) = velocity_queue.at(0).velocity_z;
    phi(11,2) = 1;

  }
  else
  {
    phi(0,0) = velocity_queue.at(0).velocity_x;
    phi(1,0) = 0;
    phi(2,0) = 0;
    phi(3,0) = 1;
    phi(4,0) = 0;
    phi(5,0) = 0;
    phi(6,0) = 0;
    phi(7,0) = 0;
    phi(8,0) = 0;
    phi(9,0) = 0;
    phi(10,0) = 0;
    phi(11,0) = 0;

    phi(0,1) = 0;
    phi(1,1) = 0;
    phi(2,1) = 0;
    phi(3,1) = 0;
    phi(4,1) = 0;
    phi(5,1) = velocity_queue.at(0).velocity_y;
    phi(6,1) = 0;
    phi(7,1) = 1;
    phi(8,1) = 0;
    phi(9,1) = 0;
    phi(10,1) = 0;
    phi(11,1) = 0;

    phi(0,2) = 0;
    phi(1,2) = 0;
    phi(2,2) = 0;
    phi(3,2) = 0;
    phi(4,2) = 0;
    phi(5,2) = 0;
    phi(6,2) = 0;
    phi(7,2) = 0;
    phi(8,2) = 0;
    phi(9,2) = 0;
    phi(10,2) = velocity_queue.at(0).velocity_z;
    phi(11,2) = 1;
  }
  Update_phi(phi);
}

void model_learner::reset_model_learn()
{
  Eigen::MatrixXf Ap = Eigen::MatrixXf::Zero(_y_dim,_y_dim);
  Ap<<1,0,0,0,1,0,0,0,1;
  Init_Ap(Ap);
  Eigen::MatrixXf theta = Eigen::MatrixXf::Zero(_theta_dim,1);
  float init_sample_time_x =1.0;
  float init_sample_time_y =1.0;
  float init_sample_time_z =1.0;
  theta <<init_sample_time_x,0,0,0,0,init_sample_time_y,0,0,0,0,init_sample_time_z,0;
  Init_theta(theta);
  Eigen::MatrixXf phi = Eigen::MatrixXf::Zero(_theta_dim,_y_dim);
  phi<<0,0,0,
      0,0,0,
      0,0,0,
      1,0,0,
      0,0,0,
      0,0,0,
      0,0,0,
      0,1,0,
      0,0,0,
      0,0,0,
      0,0,0,
      0,0,1;
  Init_phi(phi);
  Eigen::MatrixXf sigma = Eigen::MatrixXf::Zero(_y_dim,1);
  sigma<<0,0,0;
  Init_sigma(sigma);
  Eigen::MatrixXf p = Eigen::MatrixXf::Zero(_theta_dim,_theta_dim);
  for(int i = 0;i<p.cols();i++)
  {
    for(int j=0;j<p.rows();j++)
    {
      if(i==j)
      {
        p(i,j) = 1;
      }
      else
      {
        p(i,j) = 0;
      }
    }
  }
  Init_p(p);
  Eigen::MatrixXf k = Eigen::MatrixXf::Zero(_theta_dim,_y_dim);
  Init_k(k);
  Eigen::MatrixXf y = Eigen::MatrixXf::Zero(_y_dim,1);
  Init_y(y);
  _isGetModel = false;

}

void *model_learner::model_learner_run(void *args)
{
  model_learner* model_learner_node = (model_learner*)(args);

  ros::Rate rate(model_learner_node->_model_learner_run_hz);
  dt_message_package::update_model_parameter model_parameter_msg;
  //========================================================================
  dt_message_package::model_learner_info learner_info;
  learner_info.theta_dim = model_learner_node->_theta_dim;
  learner_info.y_dim = model_learner_node->_y_dim;
  learner_info.theta.resize(model_learner_node->_theta_dim*1);
  learner_info.phi.resize(model_learner_node->_theta_dim*model_learner_node->_y_dim);
  learner_info.sigma.resize(model_learner_node->_y_dim*1);
  learner_info.p.resize(model_learner_node->_theta_dim*model_learner_node->_theta_dim);
  learner_info.k.resize(model_learner_node->_theta_dim*model_learner_node->_y_dim);
  learner_info.y.resize(model_learner_node->_y_dim*1);
  //=========================================================================
  while(ros::ok())
  {
    if(!model_learner_node->_isStopModelLearn)
    {
      if(model_learner_node->_isInitK&&model_learner_node->_isInitP&&model_learner_node->_isInitPhi&&model_learner_node->_isInitSigma
         &&model_learner_node->_isInitTheta&&model_learner_node->_isInitY&&model_learner_node->_isInitAp)
      {
        if(!model_learner_node->_isGetModel)
        {
          model_learner_node->_rls_estimator->update_rls();
          Eigen::MatrixXf model_parameter_output = model_learner_node->_rls_estimator->get_theta();
          Eigen::MatrixXf model_err = model_learner_node->_rls_estimator->get_p();
         // std::cout<<"model_x_err: "<<model_err(0,0)<<std::endl;
         // std::cout<<"model_y_err: "<<model_err(5,5)<<std::endl;
         // std::cout<<"model_z_err: "<<model_err(10,10)<<std::endl;
         // std::cout<<"model_err_x_ref: "<<model_learner_node->_model_err_x<<std::endl;
         // std::cout<<"model_err_y_ref: "<<model_learner_node->_model_err_y<<std::endl;
         // std::cout<<"model_err_z_ref: "<<model_learner_node->_model_err_z<<std::endl;

          bool isValid = true;
          for(int i=0;i<model_parameter_output.rows();i++)
          {
            if(std::isnan(model_parameter_output(i,0))||std::isinf(model_parameter_output(i,0)))
            {
              isValid = false;
              model_learner_node->reset_model_learn();
            }
          }
          if(isValid)
          {
            if(model_err(0,0)<=model_learner_node->_model_err_x && model_err(5,5)<=model_learner_node->_model_err_y && model_err(10,10)<=model_learner_node->_model_err_z)
            {
              model_parameter_msg.b1 = model_parameter_output(0,0);
              model_parameter_msg.b2 = model_parameter_output(1,0);
              model_parameter_msg.b3 = model_parameter_output(2,0);
              model_parameter_msg.wx = model_parameter_output(3,0);
              model_parameter_msg.b4 = model_parameter_output(4,0);
              model_parameter_msg.b5 = model_parameter_output(5,0);
              model_parameter_msg.b6 = model_parameter_output(6,0);
              model_parameter_msg.wy = model_parameter_output(7,0);
              model_parameter_msg.b7 = model_parameter_output(8,0);
              model_parameter_msg.b8 = model_parameter_output(9,0);
              model_parameter_msg.b9 = model_parameter_output(10,0);
              model_parameter_msg.wz = model_parameter_output(11,0);
              model_learner_node->_isGetModel = true;
              model_learner_node->_isStopModelLearn = true;
            }
          }
        }

        if(model_learner_node->_isGetModel)
        {
         // model_learner_node->_update_model_parameter_pub.publish(model_parameter_msg);//
        }


        //======================================out model learner information===================================
//        {
//          learner_info.system_time = ros::Time::now().toNSec();
//          int k =0;
//          Eigen::MatrixXf theta = model_learner_node->_rls_estimator->get_theta();
//          for(int i = 0; i<theta.rows();i++)
//          {
//            for(int j =0;j<theta.cols();j++)
//            {
//              learner_info.theta.at(k) = theta(i,j);
//              k++;
//            }
//          }
//          k = 0;
//          Eigen::MatrixXf phi = model_learner_node->_rls_estimator->get_phi();
//          for(int i=0;i<phi.rows();i++)
//          {
//            for(int j=0;j<phi.cols();j++)
//            {
//              learner_info.phi.at(k) = phi(i,j);
//              k++;
//            }
//          }
//          k = 0;
//          Eigen::MatrixXf sigma = model_learner_node->_rls_estimator->get_sigma();
//          for(int i=0;i<sigma.rows();i++)
//          {
//            for(int j=0;j<sigma.cols();j++)
//            {
//              learner_info.sigma.at(k) = sigma(i,j);
//              k++;
//            }
//          }
//          k=0;
//          Eigen::MatrixXf p = model_learner_node->_rls_estimator->get_p();
//          for(int i=0;i<p.rows();i++)
//          {
//            for(int j=0;j<p.cols();j++)
//            {
//              learner_info.p.at(k) = p(i,j);
//              k++;
//            }
//          }
//          k=0;
//          Eigen::MatrixXf K = model_learner_node->_rls_estimator->get_k();
//          for(int i=0;i<K.rows();i++)
//          {
//            for(int j=0;j<K.cols();j++)
//            {
//              learner_info.k.at(k) = K(i,j);
//              k++;
//            }
//          }
//          k=0;
//          Eigen::MatrixXf y = model_learner_node->_rls_estimator->get_y();
//          for(int i=0;i<y.rows();i++)
//          {
//            for(int j =0;j<y.cols();j++)
//            {
//              learner_info.y.at(k) = y(i,j);
//              k++;
//            }
//          }
//          model_learner_node->_model_learner_info_pub.publish(learner_info);
//        }
        //=====================================end out model learner information================================

      }

      rate.sleep();

    }
  }
  pthread_join(model_learner_node->_model_learner_run_thread,NULL);

}

bool model_learner::Init_Ap(Eigen::MatrixXf Ap)
{
  if(Ap.rows() == _y_dim && Ap.cols()==_y_dim)
  {
    _Ap = Ap;
    _isInitAp = true;
    return true;
  }
  else
  {
    std::cout<<"Update the Matrix Ap Fail, Inconsistent Dimensions !!!"<<std::endl;
    _isInitAp = false;
    return false;
  }

}

bool model_learner::Init_theta(Eigen::MatrixXf theta)
{
  if(_rls_estimator->update_theta(theta))
  {
    _isInitTheta = true;
    return true;
  }
  else
  {
    _isInitTheta = false;
    return false;
  }
}
bool model_learner::Init_phi(Eigen::MatrixXf phi)
{
  if(_rls_estimator->update_phi(phi))
  {
    _isInitPhi = true;
    return true;
  }
  else
  {
    _isInitPhi = false;
    return false;
  }
}
bool model_learner::Init_sigma(Eigen::MatrixXf sigma)
{
  if(_rls_estimator->update_sigma(sigma))
  {
    _isInitSigma = true;
    return true;
  }
  else
  {
    _isInitSigma = false;
    return false;
  }
}
bool model_learner::Init_p(Eigen::MatrixXf p)
{
  if(_rls_estimator->update_p(p))
  {
    _isInitP = true;
    return true;
  }
  else
  {
    _isInitP = false;
    return false;
  }
}
bool model_learner::Init_k(Eigen::MatrixXf k)
{
  if(_rls_estimator->update_k(k))
  {
    _isInitK = true;
    return true;
  }
  else
  {
    _isInitK = false;
    return false;
  }
}
bool model_learner::Init_y(Eigen::MatrixXf y)
{
  if(_rls_estimator->update_y(y))
  {
    _isInitY = true;
    return true;
  }
  else
  {
    _isInitY = false;
    return false;
  }
}

bool model_learner::Update_phi(Eigen::MatrixXf phi)
{
  return _rls_estimator->update_phi(phi);
}
bool model_learner::Update_y(Eigen::MatrixXf y)
{
  return _rls_estimator->update_y(y);
}





