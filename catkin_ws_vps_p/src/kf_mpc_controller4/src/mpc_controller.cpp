#include "../include/kf_mpc_controller4/mpc_controller.h"

mpc_controller::mpc_controller()
{
  ros::NodeHandle n("~");

  if(n.getParam("current_position_sub_topic",_current_position_sub_topic))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <current_position_sub_topic>--->%s\033[0m",_current_position_sub_topic.c_str());
  }
  else
  {
    _current_position_sub_topic = "/mpc_controller/current_position";
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <current_position_sub_topic>--defalut: %s",_current_position_sub_topic.c_str());
  }

  if(n.getParam("target_position_sub_topic",_target_position_sub_topic))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <target_position_sub_topic>--->%s\033[0m",_target_position_sub_topic.c_str());
  }
  else
  {
    _target_position_sub_topic = "/mpc_controller/target_position";
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <target_position_sub_topic>--defalut: %s",_target_position_sub_topic.c_str());
  }

  if(n.getParam("controller_output_pub_topic",_controller_output_pub_topic))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <controller_output_pub_topic>--->%s\033[0m",_controller_output_pub_topic.c_str());
  }
  else
  {
    _controller_output_pub_topic = "/mpc_controller/controller_output";
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <controller_output_pub_topic>--defalut: %s",_controller_output_pub_topic.c_str());
  }

  if(n.getParam("controller_running_hz",_controller_running_hz))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <controller_running_hz>--->%f\033[0m",_controller_running_hz);
  }
  else
  {
    _controller_running_hz = 30.0;
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <controller_running_hz>--defalut: %f",_controller_running_hz);
  }

  if(n.getParam("state_dim",_state_dim))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <state_dim>--->%d\033[0m",_state_dim);
  }
  else
  {
    _state_dim = 3;
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <state_dim>--defalut: %d",_state_dim);
  }


  if(n.getParam("observe_dim",_observe_dim))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <observe_dim>--->%d\033[0m",_observe_dim);
  }
  else
  {
    _observe_dim = 3;
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <observe_dim>--defalut: %d",_observe_dim);
  }


  if(n.getParam("input_dim",_input_dim))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <input_dim>--->%d\033[0m",_input_dim);
  }
  else
  {
    _input_dim = 3;
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <input_dim>--defalut: %d",_input_dim);
  }

  if(n.getParam("control_window",_control_window))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <control_window>--->%d\033[0m",_control_window);
  }
  else
  {
    _control_window = 3;
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <control_window>--defalut: %d",_control_window);
  }

  if(n.getParam("predict_window",_predict_window))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <predict_window>--->%d\033[0m",_predict_window);
  }
  else
  {
    _predict_window = 3;
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <predict_window>--defalut: %d",_predict_window);
  }


  if(n.getParam("update_model_parameter_sub_topic",_update_model_parameter_sub_topic))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <update_model_parameter_sub_topic>--->%s\033[0m",_update_model_parameter_sub_topic.c_str());
  }
  else
  {
    _update_model_parameter_sub_topic = "/mpc_controller/update_model_parameter";
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <update_model_parameter_sub_topic>--defalut: %s",_update_model_parameter_sub_topic.c_str());
  }

  if(n.getParam("predict_time_dt_pub_topic",_predict_time_dt_pub_topic))
  {
    ROS_INFO("\033[1;32m Mpc_Controller----Get Topic Name: <predict_time_dt_pub_topic>--->%s\033[0m",_predict_time_dt_pub_topic.c_str());
  }
  else
  {
    _predict_time_dt_pub_topic = "/mpc_controller/predict_time_dt";
    ROS_WARN("Mpc_Controller----Not Get Topic Name: <predict_time_dt_pub_topic>--defalut: %s",_predict_time_dt_pub_topic.c_str());
  }

  _current_position_sub = n.subscribe(_current_position_sub_topic,1,&mpc_controller::current_position_sub_cb,this);
  _target_position_sub = n.subscribe(_target_position_sub_topic,1,&mpc_controller::target_position_sub_cb,this);
  _update_model_parameter_sub = n.subscribe(_update_model_parameter_sub_topic,1,&mpc_controller::update_model_parameter_sub_cb,this);
  _controller_output_pub = n.advertise<geometry_msgs::TwistStamped>(_controller_output_pub_topic,1);
  _predict_time_dt_pub= n.advertise<dt_message_package::predict_time_dt>(_predict_time_dt_pub_topic,1);

  int flag_thread = pthread_create(&_mpc_run_thread,NULL,&mpc_controller::controller_running,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }
  controller = new mpc(_state_dim,_observe_dim,_input_dim,_control_window,_predict_window);

  _isInitSystemModel = false;
  _isInitSystemState = false;
  _isInitControlOutput = false;
  _isInitMpcProble = false;
  _isInitOtherMatrix = false;

}


void *mpc_controller::controller_running(void *args)
{
  mpc_controller* mpc_controller_node = (mpc_controller*)(args);

  ros::Rate rate(mpc_controller_node->_controller_running_hz);
  geometry_msgs::TwistStamped velocity_msg;
  while(ros::ok())
  {
    if(mpc_controller_node->_isInitSystemModel && mpc_controller_node->_isInitSystemState && mpc_controller_node->_isInitControlOutput && mpc_controller_node->_isInitMpcProble && mpc_controller_node->_isInitOtherMatrix)
    {
      mpc_controller_node->Update_Controller();
      Eigen::MatrixXf controller_output = mpc_controller_node->Get_Controller_Results();
      velocity_msg.header.stamp = ros::Time::now();
      velocity_msg.twist.linear.x = controller_output(0,0);
      velocity_msg.twist.linear.y = controller_output(1,0);
      velocity_msg.twist.linear.z = controller_output(2,0);
      mpc_controller_node->_controller_output_pub.publish(velocity_msg);
      rate.sleep();
    }
  }
  pthread_join(mpc_controller_node->_mpc_run_thread,NULL);
}


void mpc_controller::update_model_parameter_sub_cb(const dt_message_package::update_model_parameterConstPtr &msg)
{
  Eigen::MatrixXf B(_state_dim,_input_dim);
  B<<msg.get()->b1,msg.get()->b2,msg.get()->b3,msg.get()->b4,msg.get()->b5,msg.get()->b6,msg.get()->b7,msg.get()->b8,msg.get()->b9;
  Eigen::MatrixXf omega(_state_dim,1);
  omega<<msg.get()->wx,msg.get()->wy,msg.get()->wz;
  Eigen::MatrixXf W = Eigen::MatrixXf::Zero(_predict_window*_state_dim,1);
  for(int i =0;i<_predict_window;i++)
  {
    W.block<3,1>(i*_state_dim,0) = omega;
  }
  Eigen::MatrixXf PHI = Eigen::MatrixXf::Zero(_predict_window*_observe_dim,_input_dim*_control_window);
  Eigen::MatrixXf A = controller->Get_A();
  Eigen::MatrixXf H = controller->Get_H();
  for(int i = 0; i<_predict_window;i++)
  {
    for(int j = 0; j<_control_window;j++)
    {
      if(j<i)
      {
        Eigen::MatrixXf A_temp = A;
        for(int k = 0; k<i-j-1; k++)
        {
          A_temp = A_temp * A;
        }
        // PHI.block<observe_dim,input_dim>(i*observe_dim,j) = H*A_temp*B;
        PHI.block<3,3>(i*_observe_dim,j*_input_dim) = H*A_temp*B;
      }
      else if(j==i)
      {
        // PHI.block<observe_dim,input_dim>(i*observe_dim,j) = H*B;
        PHI.block<3,3>(i*_observe_dim,j*_input_dim) = H*B;
      }
      else
      {
        //PHI.block<observe_dim,input_dim>(i*observe_dim,j) = Eigen::MatrixXf::Zero(observe_dim,input_dim);
        PHI.block<3,3>(i*_observe_dim,j*_input_dim) = Eigen::MatrixXf::Zero(_observe_dim,_input_dim);
      }
    }
  }
  controller->Update_B(B);
  controller->Update_omega(omega);
  controller->Update_W(W);
  controller->Update_PHI(PHI);
  dt_message_package::predict_time_dt predict_time_msg;
  predict_time_msg.predict_time_dt_x = msg.get()->b1;
  predict_time_msg.predict_time_dt_y = msg.get()->b5;
  predict_time_msg.predict_time_dt_z = msg.get()->b9;

  _predict_time_dt_pub.publish(predict_time_msg);

}

void mpc_controller::current_position_sub_cb(const geometry_msgs::PoseStampedConstPtr &msg)
{
  Eigen::MatrixXf temp = Eigen::MatrixXf::Zero(_state_dim,1);
  temp<<msg.get()->pose.position.x,msg.get()->pose.position.y,msg.get()->pose.position.z;
  {
    std::lock_guard<std::mutex> lock(_feedback_mutex);
    Update_Feedback(temp);
  }
}

void mpc_controller::target_position_sub_cb(const dt_message_package::object_predict_positionConstPtr &msg)
{
  Eigen::MatrixXf temp = Eigen::MatrixXf::Zero(_state_dim*_predict_window,1);
  std::lock_guard<std::mutex> lock(_target_mutex);
  if(msg.get()->position_xs.size()>=_predict_window)
  {
    int j =0;
    for(int i = 0; i<_state_dim*_predict_window; i = i+3)
    {
      if(i==0)
      {
        temp(i,0) = msg.get()->current_position_x;
        temp(i+1,0) = msg.get()->current_position_y;
        temp(i+2,0) = msg.get()->current_position_z;
      }
      else
      {
        temp(i,0) = msg.get()->position_xs.at(j);
        temp(i+1,0) = msg.get()->position_ys.at(j);
        temp(i+2,0) = msg.get()->position_zs.at(j);
        j = j + 1;
      }
    }
  }
  else
  {
    int max_j = msg.get()->position_xs.size()-1;
    int j = 0;
    for(int i = 0; i<_state_dim*_predict_window; i = i+3)
    {
      if(i==0)
      {
        temp(i,0) = msg.get()->current_position_x;
        temp(i+1,0) = msg.get()->current_position_y;
        temp(i+2,0) = msg.get()->current_position_z;
      }
      else
      {
        if(j<=max_j)
        {
          temp(i,0) = msg.get()->position_xs.at(j);
          temp(i+1,0) = msg.get()->position_ys.at(j);
          temp(i+2,0) = msg.get()->position_zs.at(j);
        }
        else
        {
          temp(i,0) = msg.get()->position_xs.at(max_j);
          temp(i+1,0) = msg.get()->position_ys.at(max_j);
          temp(i+2,0) = msg.get()->position_zs.at(max_j);
        }
        j = j + 1;
      }
    }
  }
  Update_Target(temp);
}

void mpc_controller::Init_System_Model(Eigen::MatrixXf A, Eigen::MatrixXf B, Eigen::MatrixXf H, Eigen::MatrixXf omega)
{
  controller->Init_A(A);
  controller->Init_B(B);
  controller->Init_H(H);
  controller->Init_omega(omega);
  _isInitSystemModel = true;
}
void mpc_controller::Init_System_State(Eigen::MatrixXf X,Eigen::MatrixXf XX)
{
  controller->Init_X(X);
  controller->Init_XX(XX);
  _isInitSystemState = true;
}
void mpc_controller::Init_Control_Output(Eigen::MatrixXf U,float max_output,float min_output)
{
  controller->Init_U(U);
  controller->Init_Cotrol_Limit(max_output,min_output);
  _isInitControlOutput = true;
}

void mpc_controller::Init_Other_Matrix(Eigen::MatrixXf F, Eigen::MatrixXf PHI, Eigen::MatrixXf W, Eigen::MatrixXf G)
{
  controller->Init_F(F);
  controller->Init_PHI(PHI);
  controller->Init_W(W);
  controller->Init_G(G);
  _isInitOtherMatrix = true;
}

void mpc_controller::Init_MPC_Proble(Eigen::MatrixXf R, Eigen::MatrixXf M)
{
  controller->Init_R(R);
  controller->Init_M(M);

  _isInitMpcProble = true;
}

void mpc_controller::Update_Feedback(Eigen::MatrixXf X)
{
  controller->Update_X(X);
}
void mpc_controller::Update_Target(Eigen::MatrixXf XX)
{
  controller->Update_XX(XX);
}
void mpc_controller::Update_Controller()
{
  controller->Update_MPC();
}
Eigen::MatrixXf mpc_controller::Get_Controller_Results()
{
  return controller->Get_U();
}
