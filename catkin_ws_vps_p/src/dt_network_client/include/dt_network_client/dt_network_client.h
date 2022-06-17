#ifndef DT_NETWORK_CLIENT_H
#define DT_NETWORK_CLIENT_H
#include<stdio.h>
#include<sys/types.h>
#include<stdlib.h>
#include<string>
#include<sys/socket.h>
#include<netinet/in.h>
#include<arpa/inet.h>
#include<unistd.h>
#include<iostream>
#include <pthread.h>
#include <ros/ros.h>
#include <iostream>
#include <cloud_common/socket_data_packet_stamp_msg.h>
#include <cloud_common/define_common.h>
#include <cloud_common/function_common.h>
#include <cloud_common/ros_msg_common.h>
#include <cloud_common/socket_data_common.h>

class dt_network_client
{
public:
  dt_network_client(int argc, char **argv);
  ~dt_network_client();
  void rosSubMsgCallback(const cloud_common::socket_data_packet_stamp_msg::ConstPtr& msg);
  static void *data_recv(void *arg);
private:
  int _socketFd;
  int _argc;
  char **_argv;
  int _port;
  string _serverIP;
  pthread_t data_recv_thread;
  std::string ros_sub_topic;
  std::string ros_pub_topic;
  int _data_recv_rate_hz;
};

#endif // DT_NETWORK_CLIENT_H
