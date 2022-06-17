#include "../include/kf_mpc_controller4/state_predictor.h"

state_predictor::state_predictor()
{
  ros::NodeHandle n("~");
  n.getParam("predict_positiong_pub_topic",_predict_positiong_pub_topic);
  n.getParam("state_predictor_info_pub_topic",_state_predictor_info_pub_topic);
  n.getParam("system_state_sub_topic",_system_state_sub_topic);
  n.getParam("predictor_run_hz",_predictor_run_hz);

  n.getParam("predict_time_dt_sub_topic",_predict_time_dt_sub_topic);

  n.getParam("save_data_window",_save_data_window);
  n.getParam("max_predict_data_window",_max_predict_data_window);
  n.getParam("min_predict_data_window",_min_predict_data_window);
  n.getParam("predict_window",_predict_window);
  n.getParam("predict_order",_predict_order);
  n.getParam("predict_time_dt",_predict_time_dt);
  n.getParam("queue_check_error",_queue_check_error);

  n.getParam("max_velocity_x",_max_velocity_x);
  n.getParam("max_velocity_y",_max_velocity_y);
  n.getParam("max_velocity_z",_max_velocity_z);

  _system_state_sub = n.subscribe(_system_state_sub_topic,1,&state_predictor::system_state_sub_cb,this);
  _predict_time_dt_sub = n.subscribe(_predict_time_dt_sub_topic,1,&state_predictor::predict_time_dt_sub_cb,this);
  _predict_position_pub = n.advertise<dt_message_package::object_predict_position>(_predict_positiong_pub_topic,1);
  _state_predictor_info_pub = n.advertise<dt_message_package::state_predictor_info>(_state_predictor_info_pub_topic,1);

  _cicl_queue_t.resize(_save_data_window);
  _cicl_queue_x.resize(_save_data_window);
  _cicl_queue_y.resize(_save_data_window);
  _cicl_queue_z.resize(_save_data_window);


  _predictor_x = new least_square();
  _predictor_y = new least_square();
  _predictor_z = new least_square();

  _predictor_x->set_Order(_predict_order);
  _predictor_y->set_Order(_predict_order);
  _predictor_z->set_Order(_predict_order);

  _predict_data_x_window = _max_predict_data_window;
  _predict_data_y_window = _max_predict_data_window;
  _predict_data_z_window = _max_predict_data_window;

  _predict_time_dt_x = _predict_time_dt;
  _predict_time_dt_y = _predict_time_dt;
  _predict_time_dt_z = _predict_time_dt;

  _x_event_id = 1;
  _y_event_id = 1;
  _z_event_id = 1;

  _running_data_x_t.resize(_predict_data_x_window);
  _running_data_y_t.resize(_predict_data_y_window);
  _running_data_z_t.resize(_predict_data_z_window);
  _running_data_x.resize(_predict_data_x_window);
  _running_data_y.resize(_predict_data_y_window);
  _running_data_z.resize(_predict_data_z_window);

  int flag_thread = pthread_create(&_predictor_run_thread,NULL,&state_predictor::predictor_running,this);
  if (flag_thread < 0)
  {
    ROS_ERROR("pthread_create ros_process_thread failed: %d\n", flag_thread);
  }

}

void state_predictor::system_state_sub_cb(const dt_message_package::object_move_stateConstPtr &msg)
{
  std::lock_guard<std::mutex> lock(_data_mutex);
  _cicl_queue_t.push_back(msg.get()->curren_time);
  _cicl_queue_x.push_back(msg.get()->position_x);
  _cicl_queue_y.push_back(msg.get()->position_y);
  _cicl_queue_z.push_back(msg.get()->position_z);
}

void state_predictor::predict_time_dt_sub_cb(const dt_message_package::predict_time_dtConstPtr &msg)
{
  _predict_time_dt_x = msg.get()->predict_time_dt_x;
  _predict_time_dt_y = msg.get()->predict_time_dt_y;
  _predict_time_dt_z = msg.get()->predict_time_dt_z;

}
predict_state_stu state_predictor::static_motion(float current_times,std::vector<float> state,int predict_window,float predict_time_dt)
{
  predict_state_stu ret;
  ret.states.resize(predict_window);
  ret.times.resize(predict_window);
  float current_state = state.at(state.size()-1);
  for(int i =0;i<predict_window;i++)
  {
    ret.times.at(i) = (i+1)*predict_time_dt + current_times;
    ret.states.at(i) = current_state;
  }
  return ret;
}

predict_state_stu state_predictor::max_velocity_motion(float current_times,std::vector<float> state, int predict_window,float predict_time_dt,float max_velocity)
{
  float move_dir = state.at(state.size()-1) - state.at(state.size()-2);
  float move_velocity = 0;
  if(move_dir > 0.005)
  {
    move_velocity = max_velocity;
  }
  else if(move_dir < 0.005)
  {
    move_velocity = -max_velocity;
  }
  else
  {
    move_velocity = 0;
  }
  //std::cout<<"move_velocity: "<<move_velocity<<std::endl;
  predict_state_stu ret;
  ret.times.resize(predict_window);
  ret.states.resize(predict_window);
  float current_state = state.at(state.size()-1);
  for(int i = 0; i<predict_window; i++)
  {
    ret.times.at(i) = current_times + (i+1)*predict_time_dt;
    ret.states.at(i) = current_state + (i+1)*move_velocity * predict_time_dt;
  }
  return ret;
}
predict_state_stu state_predictor::least_squares_motion(least_square* least_square_predictor,float current_times, std::vector<float> times,std::vector<float> state,int predict_window,float predict_time_dt)
{
  least_square_predictor->set_T(times);
  least_square_predictor->set_X(state);
  Eigen::VectorXf result = least_square_predictor->get_Result2();
  predict_state_stu ret;
  ret.times.resize(predict_window);
  ret.states.resize(predict_window);
  for(int i = 0; i< predict_window; i++)
  {
    float predict_time = current_times + (i+1)*predict_time_dt;
    ret.times.at(i) = predict_time;
    for(int j = result.size()-1;j>=0;j--)
    {
      float predict_time = times.at(times.size()-1) + (i+1)*predict_time_dt;
      ret.states.at(i) = ret.states.at(i) + pow(predict_time,result.size()-1-j)*result[j];
    }
  }
  return ret;
}

double state_predictor::GetMinValue(std::vector<float> data_queue)
{
 // for(int i = 0;i<data_queue.size();i++)
 // {
 //     std::cout<<"data_queue: "<<data_queue.at(i)<<std::endl;
 // }

  float min_value = 100.0;
  for(int i =0;i<data_queue.size();i++)
  {
    if(data_queue.at(i)<min_value)
    {
      min_value = data_queue.at(i);
    }
  }
  return min_value;
}
double state_predictor::GetMaxValue(std::vector<float> data_queue)
{
  float max_value = 0.0;
  for(int i =0;i<data_queue.size();i++)
  {
    if(data_queue.at(i)>max_value)
    {
      max_value = data_queue.at(i);
    }
  }
  return max_value;
}

int state_predictor::event_trigger(std::vector<float> data_queue,float check_err)
{
  std::vector<float> dev_data_queue;
  int event_id = 0;
  dev_data_queue.resize(data_queue.size()-1);
  for(int i = 1;i<data_queue.size();i++)
  {
    dev_data_queue.at(i-1) = fabs(data_queue.at(i) - data_queue.at(i-1));
  }

  double min_value = GetMinValue(dev_data_queue);
  double max_value = GetMaxValue(dev_data_queue);
  if (max_value>check_err && min_value>check_err)
  {
    event_id = 0;
  }
  else if(max_value<=check_err && min_value<= check_err)
  {
    event_id = 1;
  }
  else
  {
    event_id = 2;
  }
  //std::cout<<"min_value: "<<min_value<<std::endl;
  //std::cout<<"max_value: "<<max_value<<std::endl;
  return event_id;

}

int state_predictor::get_predict_window_size(int current_event_id, int last_predict_window_size, int max_window_size, int min_window_size)
{
  int now_predict_window_size = last_predict_window_size;
  if (current_event_id == 0)
  {
    now_predict_window_size = now_predict_window_size + 1;
  }
  else if (current_event_id == 1)
  {
    now_predict_window_size = now_predict_window_size;
  }
  else
  {
    now_predict_window_size = min_window_size;
  }

  if (now_predict_window_size > max_window_size)
  {
    now_predict_window_size = max_window_size;
  }
  else if (now_predict_window_size < min_window_size)
  {
    now_predict_window_size = min_window_size;
  }
  else
  {
    now_predict_window_size = now_predict_window_size;
  }

  return now_predict_window_size;
}

void *state_predictor::predictor_running(void *args)
{
  state_predictor* predictor = (state_predictor*)(args);

  ros::Rate rate(predictor->_predictor_run_hz);
  dt_message_package::object_predict_position object_pre_position;
  object_pre_position.times.resize(predictor->_predict_window);
  object_pre_position.position_xs.resize(predictor->_predict_window);
  object_pre_position.position_ys.resize(predictor->_predict_window);
  object_pre_position.position_zs.resize(predictor->_predict_window);
  while(ros::ok())
  {
    if(predictor->_cicl_queue_t.at(predictor->_cicl_queue_t.size()-1)!=0)
    {
      int j = 0;
      double current_time = predictor->_cicl_queue_t.at(predictor->_cicl_queue_t.size()-1);
      float current_position_x = predictor->_cicl_queue_x.at(predictor->_cicl_queue_x.size()-1);
      float current_position_y = predictor->_cicl_queue_y.at(predictor->_cicl_queue_y.size()-1);
      float current_position_z = predictor->_cicl_queue_z.at(predictor->_cicl_queue_z.size()-1);

      predictor->_running_data_x_t.resize(predictor->_predict_data_x_window);
      predictor->_running_data_y_t.resize(predictor->_predict_data_y_window);
      predictor->_running_data_z_t.resize(predictor->_predict_data_z_window);
      predictor->_running_data_x.resize(predictor->_predict_data_x_window);
      predictor->_running_data_y.resize(predictor->_predict_data_y_window);
      predictor->_running_data_z.resize(predictor->_predict_data_z_window);

      for(int i = predictor->_cicl_queue_t.size()-predictor->_predict_data_x_window;i<predictor->_cicl_queue_t.size();i++)
      {
        predictor->_running_data_x_t.at(j) = predictor->_cicl_queue_t.at(i) - predictor->_cicl_queue_t.at(predictor->_cicl_queue_t.size()-predictor->_predict_data_x_window);
        predictor->_running_data_x.at(j) = predictor->_cicl_queue_x.at(i);
       // std::cout<<"running_data_x_t: "<<j<<": "<<predictor->_running_data_x_t.at(j)<<std::endl;
       // std::cout<<"running_data_x: "<<j<<": "<<predictor->_running_data_x.at(j)<<std::endl;
        // std::cout<<"_cicl_queue_t: "<<j<<": "<<predictor->_cicl_queue_t.at(i)<<std::endl;

        j = j + 1;
      }
      j = 0;

      for(int i = predictor->_cicl_queue_t.size()-predictor->_predict_data_y_window;i<predictor->_cicl_queue_t.size();i++)
      {
        predictor->_running_data_y_t.at(j) = predictor->_cicl_queue_t.at(i) - predictor->_cicl_queue_t.at(predictor->_cicl_queue_t.size()-predictor->_predict_data_y_window);
        // std::cout<<"running_data_t: "<<j<<": "<<predictor->_running_data_t.at(j)<<std::endl;
        //  std::cout<<"_cicl_queue_t: "<<j<<": "<<predictor->_cicl_queue_t.at(i)<<std::endl;
        predictor->_running_data_y.at(j) = predictor->_cicl_queue_y.at(i);
        j = j + 1;
      }
      j = 0;

      for(int i = predictor->_cicl_queue_t.size()-predictor->_predict_data_z_window;i<predictor->_cicl_queue_t.size();i++)
      {
        predictor->_running_data_z_t.at(j) = predictor->_cicl_queue_t.at(i) - predictor->_cicl_queue_t.at(predictor->_cicl_queue_t.size()-predictor->_predict_data_z_window);
        // std::cout<<"running_data_t: "<<j<<": "<<predictor->_running_data_t.at(j)<<std::endl;
        //  std::cout<<"_cicl_queue_t: "<<j<<": "<<predictor->_cicl_queue_t.at(i)<<std::endl;
        predictor->_running_data_z.at(j) = predictor->_cicl_queue_z.at(i);
        j = j + 1;
      }

      predictor->_x_event_id = predictor->event_trigger( predictor->_running_data_x,predictor->_queue_check_error);
      predictor->_y_event_id = predictor->event_trigger( predictor->_running_data_y,predictor->_queue_check_error);
      predictor->_z_event_id = predictor->event_trigger( predictor->_running_data_z,predictor->_queue_check_error);

    //  std::cout<<"x_event_id: "<<predictor->_x_event_id<<std::endl;
    //  std::cout<<"y_event_id: "<<predictor->_y_event_id<<std::endl;
    //  std::cout<<"z_event_id: "<<predictor->_z_event_id<<std::endl;

      predictor->_predict_data_x_window =  predictor->get_predict_window_size( predictor->_x_event_id, predictor->_predict_data_x_window, predictor->_max_predict_data_window, predictor->_min_predict_data_window);
      predictor->_predict_data_y_window =  predictor->get_predict_window_size( predictor->_y_event_id, predictor->_predict_data_y_window, predictor->_max_predict_data_window, predictor->_min_predict_data_window);
      predictor->_predict_data_z_window =  predictor->get_predict_window_size( predictor->_z_event_id, predictor->_predict_data_z_window, predictor->_max_predict_data_window, predictor->_min_predict_data_window);

      predict_state_stu pdt_state_x_stu;
      predict_state_stu pdt_state_y_stu;
      predict_state_stu pdt_state_z_stu;

      switch (predictor->_x_event_id) {
      case 0:
      {
        pdt_state_x_stu = predictor->least_squares_motion(predictor->_predictor_x,current_time, predictor->_running_data_x_t,predictor->_running_data_x,predictor->_predict_window,predictor->_predict_time_dt_x);
      }
        break;
      case 1:
      {
        pdt_state_x_stu = predictor->static_motion(current_time,predictor->_running_data_x,predictor->_predict_window,predictor->_predict_time_dt_x);
      }
        break;
      case 2:
      {
        pdt_state_x_stu = predictor->max_velocity_motion(current_time,predictor->_running_data_x, predictor->_predict_window,predictor->_predict_time_dt_x,predictor->_max_velocity_x);
      }
        break;
      default:
        break;
      }

      switch (predictor->_y_event_id) {
      case 0:
      {
        pdt_state_y_stu = predictor->least_squares_motion(predictor->_predictor_y,current_time, predictor->_running_data_y_t,predictor->_running_data_y,predictor->_predict_window,predictor->_predict_time_dt_y);
      }
        break;
      case 1:
      {
        pdt_state_y_stu = predictor->static_motion(current_time,predictor->_running_data_y,predictor->_predict_window,predictor->_predict_time_dt_y);
      }
        break;
      case 2:
      {
        pdt_state_y_stu = predictor->max_velocity_motion(current_time,predictor->_running_data_y, predictor->_predict_window,predictor->_predict_time_dt_y,predictor->_max_velocity_y);
      }
        break;
      default:
        break;
      }

      switch (predictor->_z_event_id) {
      case 0:
      {
        pdt_state_z_stu = predictor->least_squares_motion(predictor->_predictor_z,current_time, predictor->_running_data_z_t,predictor->_running_data_z,predictor->_predict_window,predictor->_predict_time_dt_z);
      }
        break;
      case 1:
      {
        pdt_state_z_stu = predictor->static_motion(current_time,predictor->_running_data_z,predictor->_predict_window,predictor->_predict_time_dt_z);
      }
        break;
      case 2:
      {
        pdt_state_z_stu = predictor->max_velocity_motion(current_time,predictor->_running_data_z, predictor->_predict_window,predictor->_predict_time_dt_z,predictor->_max_velocity_z);
      }
        break;
      default:
        break;
      }

      for(int i = 0; i< pdt_state_x_stu.states.size();i++)
      {
        object_pre_position.times.at(i) = pdt_state_x_stu.times.at(i);
        object_pre_position.position_xs.at(i) = pdt_state_x_stu.states.at(i);
        object_pre_position.position_ys.at(i) = pdt_state_y_stu.states.at(i);
        object_pre_position.position_zs.at(i) = pdt_state_z_stu.states.at(i);
      }
      object_pre_position.current_position_x = current_position_x;
      object_pre_position.current_time = current_time;
      object_pre_position.current_position_y = current_position_y;
      object_pre_position.current_position_z = current_position_z;
      predictor->_predict_position_pub.publish(object_pre_position);
      //=========================================================out state predictor information=========================================
      {
       dt_message_package::state_predictor_info predictor_info;
       predictor_info.system_time = ros::Time::now().toNSec();
       predictor_info.predict_window = predictor->_predict_window;
       predictor_info.max_predict_data_window = predictor->_max_predict_data_window;
       predictor_info.min_predict_data_window = predictor->_min_predict_data_window;
       predictor_info.predict_time_dt_x = predictor->_predict_time_dt_x;
       predictor_info.predict_time_dt_y = predictor->_predict_time_dt_y;
       predictor_info.predict_time_dt_z = predictor->_predict_time_dt_z;
       predictor_info.predict_data_x_window = predictor->_predict_data_x_window;
       predictor_info.predict_data_y_window = predictor->_predict_data_y_window;
       predictor_info.predict_data_z_window = predictor->_predict_data_z_window;
       predictor_info.x_event_id = predictor->_x_event_id;
       predictor_info.y_event_id = predictor->_y_event_id;
       predictor_info.z_event_id = predictor->_z_event_id;
       predictor_info.queue_check_error = predictor->_queue_check_error;
       predictor_info.max_velocity_x = predictor->_max_velocity_x;
       predictor_info.max_velocity_y = predictor->_max_velocity_y;
       predictor_info.max_velocity_z = predictor->_max_velocity_z;
       predictor_info.running_data_x_t = predictor->_running_data_x_t;
       predictor_info.running_data_y_t = predictor->_running_data_y_t;
       predictor_info.running_data_z_t = predictor->_running_data_z_t;
       predictor_info.running_data_x = predictor->_running_data_x;
       predictor_info.running_data_y = predictor->_running_data_y;
       predictor_info.running_data_z = predictor->_running_data_z;
       predictor->_state_predictor_info_pub.publish(predictor_info);
      }
      //=========================================================end out state predictor information=====================================

    }
    rate.sleep();
  }
  pthread_join(predictor->_predictor_run_thread,NULL);

}
