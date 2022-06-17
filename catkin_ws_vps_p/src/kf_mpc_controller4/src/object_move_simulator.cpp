#include "../include/kf_mpc_controller4/object_move_simulator.h"

object_move_simulator::object_move_simulator()
{

}

object_move_simulator::object_move_simulator(int state_dim, int observe_dim, int input_dim)
{
  ros::NodeHandle n("~");
  n.getParam("object_input_sub_topic",_object_input_sub_topic);
  n.getParam("sensor_data_pub_topic",_sensor_data_pub_topic);
  n.getParam("simulator_run_hz",_simulator_run_hz);
  n.getParam("model_noise_mu",_model_noise_mu);
  n.getParam("model_noise_sigma",_model_noise_sigma);
  n.getParam("sensor_noise_mu",_sensor_noise_mu);
  n.getParam("sensor_noise_sigma",_sensor_noise_sigma);
  _state_dim = state_dim;
  _observe_dim = observe_dim;
  _input_dim = input_dim;

  _object_input_sub = n.subscribe(_object_input_sub_topic,1,&object_move_simulator::object_input_sub_cb,this);
  _sensor_data_pub = n.advertise<geometry_msgs::PoseStamped>(_sensor_data_pub_topic,1);

  int flag_thread = pthread_create(&_simulator_run_thread,NULL,&object_move_simulator::simulator_running,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }
  _object = new object_move(_state_dim,_observe_dim,_input_dim);

  _isInitObjectModel = false;
  _isInitObjectState = false;
  _isInitNoise = false;
  _seed = 0;

}
double object_move_simulator::GetGuassianNoise(double mu, double sigma)
{
  // generation of two normalized uniform random variables
  double U1 = static_cast<double>(rand_r(&_seed)) / static_cast<double>(RAND_MAX);
  double U2 = static_cast<double>(rand_r(&_seed)) / static_cast<double>(RAND_MAX);

  // using Box-Muller transform to obtain a varaible with a standard normal distribution
  double Z0 = sqrt(-2.0 * ::log(U1)) * cos(2.0*M_PI * U2);

  // scaling
  Z0 = sigma * Z0 + mu;
  return Z0;
}

bool object_move_simulator::Update_Model_Input(Eigen::MatrixXf U)
{
  return _object->update_U(U);
}
bool object_move_simulator::Update_Noise(Eigen::MatrixXf W,Eigen::MatrixXf V)
{
  return (_object->update_Model_Noise(W)&&_object->update_Sensor_Noise(V));
}
bool object_move_simulator::Update_Object()
{
  return _object->update_model();
}
Eigen::MatrixXf object_move_simulator::Get_Sensor_Data()
{
  return _object->getY();
}

bool object_move_simulator::Init_Object_Model(Eigen::MatrixXf A, Eigen::MatrixXf B, Eigen::MatrixXf H)
{
  bool ret = (_object->update_A(A)&&_object->update_B(B)&&_object->update_H(H));
  if(ret)
  {
    _isInitObjectModel = true;
  }

  return ret;
}
bool object_move_simulator::Init_Object_State(Eigen::MatrixXf X, Eigen::MatrixXf Y, Eigen::MatrixXf U)
{
  bool ret = (_object->update_X(X)&&_object->update_Y(Y)&&_object->update_U(U));
  if(ret)
  {
    _isInitObjectState = true;
  }
  return ret;
}
bool object_move_simulator::Init_Noise(Eigen::MatrixXf W,Eigen::MatrixXf V)
{
  bool ret = (_object->update_Model_Noise(W) && _object->update_Sensor_Noise(V));
  if(ret)
  {
    _isInitNoise = true;
  }
  return ret;

}

void object_move_simulator::object_input_sub_cb(const geometry_msgs::TwistStampedConstPtr& msg)
{
  Eigen::MatrixXf temp(_input_dim,1);
  temp(0,0) = msg.get()->twist.linear.x;
  temp(1,0) = msg.get()->twist.linear.y;
  temp(2,0) = msg.get()->twist.linear.z;
  {
    std::lock_guard<std::mutex> lock(_data_mutex);
    Update_Model_Input(temp);
  }
}

void *object_move_simulator::simulator_running(void *args)
{
  object_move_simulator* simulator = (object_move_simulator*)(args);

  ros::Rate rate(simulator->_simulator_run_hz);
  geometry_msgs::PoseStamped current_pose_msg;
  while(ros::ok())
  {
    if(simulator->_isInitNoise && simulator->_isInitObjectModel && simulator->_isInitObjectState)
    {
      float model_noise = simulator->GetGuassianNoise(simulator->_model_noise_mu,simulator->_model_noise_sigma);
      float sensor_noise = simulator->GetGuassianNoise(simulator->_sensor_noise_mu,simulator->_sensor_noise_sigma);
      Eigen::MatrixXf W_temp(simulator->_state_dim,1);
      Eigen::MatrixXf V_temp(simulator->_observe_dim,1);
      for(int i=0; i<simulator->_state_dim; i++)
      {
        W_temp(i,0) = model_noise;
      }
      for(int j =0; j<simulator->_observe_dim; j++)
      {
        V_temp(j,0) = sensor_noise;
      }
      simulator->Update_Noise(W_temp,V_temp);
      simulator->Update_Object();
      Eigen::MatrixXf sensor_data = simulator->Get_Sensor_Data();
      current_pose_msg.pose.position.x = sensor_data(0,0);
      current_pose_msg.pose.position.y = sensor_data(1,0);
      current_pose_msg.pose.position.z = sensor_data(2,0);
      current_pose_msg.header.stamp = ros::Time::now();
      simulator->_sensor_data_pub.publish(current_pose_msg);
      rate.sleep();
    }
  }
  pthread_join(simulator->_simulator_run_thread,NULL);

}
