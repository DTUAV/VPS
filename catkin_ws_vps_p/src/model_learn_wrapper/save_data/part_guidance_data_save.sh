#!/bin/bash
mkdir -p /home/yyl/px4_ws/catkin_ws_vpc_r/src/dt_launch/data/data_save/data_$(date "+%Y%m%d%H%M%S")
cd /home/yyl/px4_ws/catkin_ws_vpc_r/src/dt_launch/data/data_save/data_$(date "+%Y%m%d%H%M%S")
gnome-terminal --window -x bash -c  "rostopic echo -p /uav1/other_controller/target_velocity > mpc_output_velocity.txt &
rostopic echo -p /uav1/virtual_guide/cmd > virtual_guide_cmd.txt &
rostopic echo -p /uav1/virtual_guide/type > virtual_guide_type.txt & 
rostopic echo -p /uav1/mavros/setpoint_velocity/cmd_vel_unstamped > target_velocity.txt &
rostopic echo -p /controller_info > controller_info.txt"
