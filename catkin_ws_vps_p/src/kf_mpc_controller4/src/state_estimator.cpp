#include "../include/kf_mpc_controller4/state_estimator.h"

state_estimator::state_estimator()
{
  ros::NodeHandle n("~");
  n.getParam("dt_obj_position_sub_topic",_dt_obj_position_sub_topic);
  n.getParam("est_move_state_pub_topic",_est_move_state_pub_topic);
  n.getParam("kf_run_hz",_run_hz);
  n.getParam("update_measure_err_k",_updateMeasureErrK);
  n.getParam("kf_information_pub_topic",_kf_information_pub_topic);

  _dt_obj_position_sub = n.subscribe(_dt_obj_position_sub_topic,1,&state_estimator::dt_obj_position_sub_cb,this);

  _est_move_state_pub = n.advertise<dt_message_package::object_move_state>(_est_move_state_pub_topic,1);
  _kf_information_pub = n.advertise<dt_message_package::state_estimator_info>(_kf_information_pub_topic,1);

  int flag_thread = pthread_create(&_kf_run_thread,NULL,&state_estimator::kf_run,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }

  _is_first_measure_data = true;
  _isInitKf = false;
  _isInitSystemModel = false;
  _isInitObserveModel = false;
  _isInitEstimateErr = false;
  _isInitControlInput = false;
  _isInitObserveData = false;
  _isInitSystemState = false;

}

bool state_estimator::InitKf(int state_dim,int observe_dim,int input_dim)
{
  _state_dim = state_dim;//the dimension of system state
  _observe_dim = observe_dim;//the dimension of observer state
  _input_dim = input_dim;//the dimension of control input
  _kf_estimator = new kf(state_dim,observe_dim,input_dim);
  _isInitKf = true;
  _observe_data = Eigen::MatrixXf::Zero(_observe_dim, 1);
}
bool state_estimator::InitSystemModel(Eigen::MatrixXf A,Eigen::MatrixXf B,Eigen::MatrixXf Q)
{
  _kf_estimator->update_A(A);
  _kf_estimator->update_B(B);
  _kf_estimator->update_Q(Q);
  _isInitSystemModel = true;
}
bool state_estimator::InitObserveModel(Eigen::MatrixXf H,Eigen::MatrixXf R)
{
  _kf_estimator->update_H(H);
  _kf_estimator->update_R(R);
  _isInitObserveModel = true;
}
bool state_estimator::InitEstimateErr(Eigen::MatrixXf P)
{
  _kf_estimator->update_P(P);
  _isInitEstimateErr = true;
}
bool state_estimator::InitControlInput(Eigen::MatrixXf U)
{
  _kf_estimator->update_U(U);
  _isInitControlInput = true;

}
bool state_estimator::InitObserveData(Eigen::MatrixXf Y)
{
  _kf_estimator->update_Y(Y);
  _isInitObserveData = true;
}
bool state_estimator::InitSystemState(Eigen::MatrixXf X)
{
  _kf_estimator->update_X(X);
  _isInitSystemState = true;
}
bool state_estimator::InitModelErr(float model_err)
{
  _modelErr = model_err;
  _isInitModelErr = true;
}
bool state_estimator::InitMeasureErr(float measure_err)
{
  _measureErr = measure_err;
  _isInitMeasureErr = true;
}

bool state_estimator::UpdateControlInput(Eigen::MatrixXf U)
{
  return _kf_estimator->update_U(U);
}
bool state_estimator::UpdateObserveData(Eigen::MatrixXf Y)
{
  return _kf_estimator->update_Y(Y);
}

bool state_estimator::UpdateModelErrMatrix(Eigen::MatrixXf Q)
{
  return _kf_estimator->update_Q(Q);
}
bool state_estimator::UpdateObserveErrMatrix(Eigen::MatrixXf R)
{
  return _kf_estimator->update_R(R);
}
bool state_estimator::UpdateMeasureErr(float measure_err)
{
  return _kf_estimator->update_measure_err(measure_err);
}
bool state_estimator::UpdateModelErr(float model_err)
{
  return _kf_estimator->update_model_err(model_err);
}

Eigen::MatrixXf state_estimator::GetSystemState()
{
  return _kf_estimator->getX();
}
Eigen::MatrixXf state_estimator::GetSystemPredictState()
{
  return _kf_estimator->getX_Predict();
}
bool state_estimator::CoutMeasureErrAndUpdate(double delay_time_s)
{
  _measureErr = _modelErr*exp(delay_time_s*_updateMeasureErrK);
  return _kf_estimator->update_measure_err(_measureErr);
}

void *state_estimator::kf_run(void *args)
{
  state_estimator* estimator = (state_estimator*)(args);

  ros::Rate rate(estimator->_run_hz);
  dt_message_package::object_move_state object_state;
  dt_message_package::state_estimator_info estimator_info;
  {
    estimator_info.X.resize(estimator->_state_dim*1);
    estimator_info.X_Predict.resize(estimator->_state_dim*1);
    estimator_info.A.resize(estimator->_state_dim*estimator->_state_dim);
    estimator_info.B.resize(estimator->_state_dim*estimator->_input_dim);
    estimator_info.U.resize(estimator->_input_dim*1);
    estimator_info.Y.resize(estimator->_observe_dim,1);
    estimator_info.H.resize(estimator->_observe_dim*estimator->_state_dim);
    estimator_info.P.resize(estimator->_state_dim*estimator->_state_dim);
    estimator_info.K.resize(estimator->_state_dim*estimator->_state_dim);
    estimator_info.Q.resize(estimator->_state_dim*estimator->_state_dim);
    estimator_info.R.resize(estimator->_observe_dim*estimator->_observe_dim);
    estimator_info.state_dim = estimator->_state_dim;
    estimator_info.input_dim = estimator->_input_dim;
    estimator_info.observe_dim = estimator->_observe_dim;
  }

  while(ros::ok())
  {
    if(estimator->_isInitKf && estimator->_isInitSystemModel && estimator->_isInitObserveModel
       && estimator->_isInitEstimateErr &&estimator->_isInitControlInput && estimator->_isInitObserveData
       && estimator->_isInitSystemState && estimator->_isInitModelErr && estimator->_isInitMeasureErr)
    {
      {
        //  std::lock_guard<std::mutex> lock(estimator->_data_mutex);
        //  estimator->_kf_estimator->update_Y(estimator->_observe_data);
        //  estimator->_kf_estimator->update_Time(estimator->_current_time_s);
      }
      estimator->_kf_estimator->updateKF();
      Eigen::MatrixXf state = estimator->_kf_estimator->getX();
      object_state.curren_time = estimator->_kf_estimator->getTime();
      std::cout<<"object_state.current_time: "<<object_state.curren_time<<std::endl;
      object_state.position_x = state(0,0);
      object_state.position_y = state(1,0);
      object_state.position_z = state(2,0);
      object_state.velocity_x = state(3,0);
      object_state.velocity_y = state(4,0);
      object_state.velocity_z = state(5,0);
      object_state.acceleration_x = state(6,0);
      object_state.acceleration_y = state(7,0);
      object_state.acceleration_z = state(8,0);
      estimator->_est_move_state_pub.publish(object_state);
      //=========================================output the information of kalman filter======================================
      {
        estimator_info.model_err = estimator->_kf_estimator->get_model_err();
        estimator_info.measure_err = estimator->_kf_estimator->get_measure_err();
        estimator_info.system_time = ros::Time::now().toNSec();
        int k =0;
        Eigen::MatrixXf X = estimator->_kf_estimator->getX();
        for(int i=0;i<X.rows();i++)
        {
          for(int j=0;j<X.cols();j++)
          {
            estimator_info.X.at(k) = X(i,j);
            k++;
          }
        }
        k = 0;
        Eigen::MatrixXf X_Predict = estimator->_kf_estimator->getX_Predict();
        for(int i =0;i<X_Predict.rows();i++)
        {
          for(int j=0;j<X_Predict.cols();j++)
          {
            estimator_info.X_Predict.at(k) = X_Predict(i,j);
            k++;
          }
        }
        k = 0;
        Eigen::MatrixXf A = estimator->_kf_estimator->getA();
        for(int i=0;i<A.rows();i++)
        {
          for(int j=0;j<A.cols();j++)
          {
            estimator_info.A.at(k) = A(i,j);
            k++;
          }
        }
        k = 0;
        Eigen::MatrixXf B = estimator->_kf_estimator->getB();
        for(int i=0;i<B.rows();i++)
        {
          for(int j=0;j<B.cols();j++)
          {
            estimator_info.B.at(k) = B(i,j);
            k++;
          }
        }
        k = 0;
        Eigen::MatrixXf U = estimator->_kf_estimator->getU();
        for(int i=0;i<U.rows();i++)
        {
          for(int j=0;j<U.cols();j++)
          {
            estimator_info.U.at(k) = U(i,j);
            k++;
          }
        }

        k = 0;
        Eigen::MatrixXf Y = estimator->_kf_estimator->getY();
        for(int i=0;i<Y.rows();i++)
        {
          for(int j=0;j<Y.cols();j++)
          {
            estimator_info.Y.at(k) = Y(i,j);
            k++;
          }
        }

        k = 0;
        Eigen::MatrixXf H = estimator->_kf_estimator->getH();
        for(int i=0;i<H.rows();i++)
        {
          for(int j=0;j<H.cols();j++)
          {
            estimator_info.H.at(k) = H(i,j);
            k++;
          }
        }

        k = 0;
        Eigen::MatrixXf P = estimator->_kf_estimator->getP();
        for(int i=0;i<P.rows();i++)
        {
          for(int j=0;j<P.cols();j++)
          {
            estimator_info.P.at(k) = P(i,j);
            k++;
          }
        }

        k = 0;
        Eigen::MatrixXf K = estimator->_kf_estimator->getK();
        for(int i=0;i<K.rows();i++)
        {
          for(int j=0;j<K.cols();j++)
          {
            estimator_info.K.at(k) = K(i,j);
            k++;
          }
        }

        k = 0;
        Eigen::MatrixXf Q = estimator->_kf_estimator->getQ();
        for(int i=0;i<Q.rows();i++)
        {
          for(int j=0;j<Q.cols();j++)
          {
            estimator_info.Q.at(k) = Q(i,j);
            k++;
          }
        }

        k = 0;
        Eigen::MatrixXf R = estimator->_kf_estimator->getR();
        for(int i=0;i<R.rows();i++)
        {
          for(int j=0;j<R.cols();j++)
          {
            estimator_info.R.at(k) = R(i,j);
            k++;
          }
        }

        estimator->_kf_information_pub.publish(estimator_info);

      }
      //=============================================================end out information of kalman filter=====================================
      rate.sleep();
    }
  }
  pthread_join(estimator->_kf_run_thread,NULL);

}

void state_estimator::dt_obj_position_sub_cb(const dt_message_package::object_move_position2ConstPtr msg)
{

  if(_is_first_measure_data)
  {
    _start_time_s = msg.get()->time_stamp_ms/1000.0;
    _kf_estimator->update_Time(0.0);
    _is_first_measure_data = false;
  }
  else
  {
    std::lock_guard<std::mutex> lock(_data_mutex);
    if(_observe_dim == 3)
    {
      _current_time_s = msg.get()->time_stamp_ms/1000.0 - _start_time_s;
      // std::cout<<"current_times: "<<_current_time_s<<std::endl;
      Eigen::MatrixXf measurement(_observe_dim,1);
      measurement<<msg.get()->position_x,msg.get()->position_y,msg.get()->position_z;
      _observe_data = measurement;
      CoutMeasureErrAndUpdate(msg.get()->delay_time_ms/1000.0);
      _kf_estimator->update_Time(_current_time_s);
      _kf_estimator->update_Y(_observe_data);
    }

  }
}
