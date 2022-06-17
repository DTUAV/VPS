using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using MavLink;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace mavlinkWinformClient.MavClient
{
    public class Mavclient
    {
        public Mavclient()
        {
            this.commStatus = false;

            this._mav = new Mavlink();
            this._mavPack = new MavlinkPacket();
            this._tcpClient = new TcpClient();
            this._mht = new Msg_heartbeat();

            this._mav.PacketReceived += recvMavMsg;
        }

        ~Mavclient()
        {

        }

        private TcpClient _tcpClient;
        private NetworkStream _stream;
        private Mavlink _mav;
        private MavlinkPacket _mavPack;
        private Msg_heartbeat _mht;

        /*  */
        public uint mode;
        public float pos1;
        public float pos2;
        public float pos3;

        public bool commStatus;

        public delegate void sendDataDelegate(NetworkStream stream);

        public event sendDataDelegate sendDataEvent;//用于发送消息



        public event commStatusDelegate commStatusEvent;
        public delegate void commStatusDelegate(bool comm);

        public event commDataDelegate commDataEvent;
        public delegate void commDataDelegate(float dt);

        

        public delegate void MsgHeartBeatDelegate(Msg_heartbeat msg);

        public event MsgHeartBeatDelegate MsgHeartBeatEvent;

        public delegate void MsgSysStatusDelegate(Msg_sys_status msg);

        public event MsgSysStatusDelegate MsgSysStatusEvent;

        public delegate void MsgSystemTimeDelegate(Msg_system_time msg);

        public event MsgSystemTimeDelegate MsgSystemTimeEvent;

        public delegate void MsgPingDelegate(Msg_ping msg);

        public event MsgPingDelegate MsgPingEvent;

        public delegate void MsgChangeOperatorControlDelegate(Msg_change_operator_control msg);

        public event MsgChangeOperatorControlDelegate MsgChangeOperatorControlEvent;

        public delegate void MsgChangeOperatorControlAckDelegate(Msg_change_operator_control_ack msg);

        public event MsgChangeOperatorControlAckDelegate MsgChangeOperatorControlAckEvent;

        public delegate void MsgAuthKeyDelegate(Msg_auth_key msg);

        public event MsgAuthKeyDelegate MsgAuthKeyEvent;

        public delegate void MsgLinkNodeStatusDelegate(Msg_link_node_status msg);

        public event MsgLinkNodeStatusDelegate MsgLinkNodeStatusEvent;

        public delegate void MsgSetModeDelegate(Msg_set_mode msg);

        public event MsgSetModeDelegate MsgSetModeEvent;

        public delegate void MsgParamRequestReadDelegate(Msg_param_request_read msg);

        public event MsgParamRequestReadDelegate MsgParamRequestReadEvent;

        public delegate void MsgParamRequestListDelegate(Msg_param_request_list msg);

        public event MsgParamRequestListDelegate MsgParamRequestListEvent;

        public delegate void MsgParamValueDelegate(Msg_param_value msg);

        public event MsgParamValueDelegate MsgParaValueEvent;

        public delegate void MsgParamSetDelegate(Msg_param_set msg);

        public event MsgParamSetDelegate MsgParamSetEvent;

        public delegate void MsgGpsRawIntDelegate(Msg_gps_raw_int msg);

        public event MsgGpsRawIntDelegate MsgGpsRawIntEvent;

        public delegate void MsgGpsStatusDelegate(Msg_gps_status msg);

        public event MsgGpsStatusDelegate MsgGpsStatusEvent;

        public delegate void MsgScaledImuDelegate(Msg_scaled_imu msg);

        public event MsgScaledImuDelegate MsgScaledImuEvent;

        public delegate void MsgRawImuDelegate(Msg_raw_imu msg);

        public event MsgRawImuDelegate MsgRawImuEvent;

        public delegate void MsgRawPressureDelegate(Msg_raw_pressure msg);

        public event MsgRawPressureDelegate MsgRawPressureEvent;

        public delegate void MsgScaledPressureDelegate(Msg_scaled_pressure msg);

        public event MsgScaledPressureDelegate MsgScaledPressureEvent;

        public delegate void MsgAttitudeDelegate(Msg_attitude msg);

        public event MsgAttitudeDelegate MsgAttitudeEvent;

        public delegate void MsgAttitudeQuaternionDelegate(Msg_attitude_quaternion msg);

        public event MsgAttitudeQuaternionDelegate MsgAttitudeQuaternionEvent;

        public delegate void MsgLocalPositionNedDelegate(Msg_local_position_ned msg);

        public event MsgLocalPositionNedDelegate MsgLocalPositionNedEvent;

        public delegate void MsgGlobalPositionIntDelegate(Msg_global_position_int msg);

        public event MsgGlobalPositionIntDelegate MsgGlobalPositionIntEvent;

        public delegate void MsgRcChannelsScaledDelegate(Msg_rc_channels_scaled msg);

        public event MsgRcChannelsScaledDelegate MsgRcChannelsScaledEvent;

        public delegate void MsgRcChannelsRawDelegate(Msg_rc_channels_raw msg);

        public event MsgRcChannelsRawDelegate MsgRcChannelsRawEvent;

        public delegate void MsgServoOutputRawDelegate(Msg_servo_output_raw msg);

        public event MsgServoOutputRawDelegate MsgServoOutputRawEvent;

        public delegate void MsgMissionRequestPartialListDelegate(Msg_mission_request_partial_list msg);

        public event MsgMissionRequestPartialListDelegate MsgMissionRequestPartialListEvent;

        public delegate void MsgMissionWritePartialListDelegate(Msg_mission_write_partial_list msg);

        public event MsgMissionWritePartialListDelegate MsgMissionWritePartialListEvent;

        public delegate void MsgMissionItemDelegate(Msg_mission_item msg);

        public event MsgMissionItemDelegate MsgMissionItemEvent;

        public delegate void MsgMissionRequestDelegate(Msg_mission_request msg);

        public event MsgMissionRequestDelegate MsgMissionRequestEevent;

        public delegate void MsgMissionSetCurrentDelegate(Msg_mission_set_current msg);

        public event MsgMissionSetCurrentDelegate MsgMissionSetCurrentEvent;

        public delegate void MsgMissionCurrentDelegate(Msg_mission_current msg);

        public event MsgMissionCurrentDelegate MsgMissionCurrentEvent;

        public delegate void MsgMissionRequestListDelegate(Msg_mission_request_list msg);

        public event MsgMissionRequestListDelegate MsgMissionRequestListEvent;

        public delegate void MsgMissionCountDelegate(Msg_mission_count msg);

        public event MsgMissionCountDelegate MsgMissionCountEvent;

        public delegate void MsgMissionClearAllDelegate(Msg_mission_clear_all msg);

        public event MsgMissionClearAllDelegate MsgMissionClearAllEvent;

        public delegate void MsgMissionItemReachedDelegate(Msg_mission_item_reached msg);

        public event MsgMissionItemReachedDelegate MsgMissionItemReachedEvent;

        public delegate void MsgMissionAckDelegate(Msg_mission_ack msg);

        public event MsgMissionAckDelegate MsgMissionAckEvent;

        public delegate void MsgSetGpsGlobalOriginDelegate(Msg_set_gps_global_origin msg);

        public event MsgSetGpsGlobalOriginDelegate MsgSetGpsGlobalOriginEvent;

        public delegate void MsgGpsGlobalOriginDelegate(Msg_gps_global_origin msg);

        public event MsgGpsGlobalOriginDelegate MsgGpsGlobalOriginEvent;

        public delegate void MsgParamMapRcDelegate(Msg_param_map_rc msg);

        public event MsgParamMapRcDelegate MsgParamMapRcEvent;

        public delegate void MsgMissionRequestIntDelegate(Msg_mission_request_int msg);

        public event MsgMissionRequestIntDelegate MsgMissionRequestIntEvent;

        public delegate void MsgMissionChangedDelegate(Msg_mission_changed msg);

        public event MsgMissionChangedDelegate MsgMissionChangedEvent;

        public delegate void MsgSafetySetAllowedAreaDelegate(Msg_safety_set_allowed_area msg);

        public event MsgSafetySetAllowedAreaDelegate MsgSafetySetAllowedAreaEvent;

        public delegate void MsgSafetyAllowedAreaDelegate(Msg_safety_allowed_area msg);

        public event MsgSafetyAllowedAreaDelegate MsgSafetyAllowedAreaEvent;

        public delegate void MsgAttitudeQuaternionCovDelegate(Msg_attitude_quaternion_cov msg);

        public event MsgAttitudeQuaternionCovDelegate MsgAttitudeQuaternionCovEvent;

        public delegate void MsgNavControllerOutputDelegate(Msg_nav_controller_output msg);

        public event MsgNavControllerOutputDelegate MsgNavControllerOutputEvent;

        public delegate void MsgGlobalPositionIntCovDelegate(Msg_global_position_int_cov msg);

        public event MsgGlobalPositionIntCovDelegate MsgGlobalPositionIntCovEvent;

        public delegate void MsgLocalPositionNedCovDelegate(Msg_local_position_ned_cov msg);

        public event MsgLocalPositionNedCovDelegate MsgLocalPositionNedCovEvent;

        public delegate void MsgRcChannelsDelegate(Msg_rc_channels msg);

        public event MsgRcChannelsDelegate MsgRcChannelsEvent;

        public delegate void MsgRequestDataStreamDelegate(Msg_request_data_stream msg);

        public event MsgRequestDataStreamDelegate MsgRequestDataStreamEvent;

        public delegate void MsgDataStreamDelegate(Msg_data_stream msg);

        public event MsgDataStreamDelegate MsgDataStreamEvent;

        public delegate void MsgManualControlDelegate(Msg_manual_control msg);

        public event MsgManualControlDelegate MsgManualControlEvent;

        public delegate void MsgRcChannelsOverrideDelegate(Msg_rc_channels_override msg);

        public event MsgRcChannelsOverrideDelegate MsgRcChannelsOverrideEvent;

        public delegate void MsgMissionItemIntDelegate(Msg_mission_item_int msg);

        public event MsgMissionItemIntDelegate MsgMissionItemIntEvent;

        public delegate void MsgVfrHudDelegate(Msg_vfr_hud msg);

        public event MsgVfrHudDelegate MsgVfrHudEvent;

        public delegate void MsgCommandIntDelegate(Msg_command_int msg);

        public event MsgCommandIntDelegate MsgCommandIntEvent;

        public delegate void MsgCommandLongDelegate(Msg_command_long msg);

        public event MsgCommandLongDelegate MsgCommandLongEvent;

        public delegate void MsgCommandAckDelegate(Msg_command_ack msg);

        public event MsgCommandAckDelegate MsgCommandAckEvent;

        public delegate void MsgCommandCancelDelegate(Msg_command_cancel msg);

        public event MsgCommandCancelDelegate MsgCommandCancelEvent;

        public delegate void MsgManualSetPointDelegate(Msg_manual_setpoint msg);

        public event MsgManualSetPointDelegate MsgManualSetPointEvent;

        public delegate void MsgSetAttitudeTargetDelegate(Msg_set_attitude_target msg);

        public event MsgSetAttitudeTargetDelegate MsgSetAttitudeTargetEvent;

        public delegate void MsgAttitudeTargetDelegate(Msg_attitude_target msg);

        public event MsgAttitudeTargetDelegate MsgAttitudeTargetEvent;

        public delegate void MsgSetPositionTargetLocalNedDelegate(Msg_set_position_target_local_ned msg);

        public event MsgSetPositionTargetLocalNedDelegate MsgSetPositionTargetLocalNedEvent;

        public delegate void MsgPositionTargetLocalNedDelegate(Msg_position_target_local_ned msg);

        public event MsgPositionTargetLocalNedDelegate MsgPositionTargetLocalNedEvent;

        public delegate void MsgSetPositionTargetGlobalIntDelegate(Msg_set_position_target_global_int msg);

        public event MsgSetPositionTargetGlobalIntDelegate MsgSetPositionTargetGlobalIntEvent;

        public delegate void MsgPositionTargetGlobalIntDelegate(Msg_position_target_global_int msg);

        public event MsgPositionTargetGlobalIntDelegate MsgPositionTargetGlobalIntEvent;

        public delegate void MsgLocalPositionNedSystemGlobalOffsetDelegate(
            Msg_local_position_ned_system_global_offset msg);

        public event MsgLocalPositionNedSystemGlobalOffsetDelegate MsgLocalPositionNedSystemGlobalOffsetEvent;

        public delegate void MsgHilStateDelegate(Msg_hil_state msg);

        public event MsgHilStateDelegate MsgHilStateEvent;

        public delegate void MsgHilControlsDelegate(Msg_hil_controls msg);

        public event MsgHilControlsDelegate MsgHilControlsEvent;

        public delegate void MsgHilRcInputRawDelegate(Msg_hil_rc_inputs_raw msg);

        public event MsgHilRcInputRawDelegate MsgHilRcInputRawEvent;

        public delegate void MsgHilActuatorControlsDelegate(Msg_hil_actuator_controls msg);

        public event MsgHilActuatorControlsDelegate MsgHilActuatorControlsEvent;

        public delegate void MsgOpticalFlowDelegate(Msg_optical_flow msg);

        public event MsgOpticalFlowDelegate MsgOpticalFlowEvent;

        public delegate void MsgGlobalVisionPositionEstimateDelegate(Msg_global_vision_position_estimate msg);

        public event MsgGlobalVisionPositionEstimateDelegate MsgGlobalVisionPositionEstimateEvent;

        public delegate void MsgVisionPositionEstimateDelegate(Msg_vision_position_estimate msg);

        public event MsgVisionPositionEstimateDelegate MsgVisionPositionEstimateEvent;

        public delegate void MsgVisionSpeedEstimateDelegate(Msg_vision_speed_estimate msg);

        public event MsgVisionSpeedEstimateDelegate MsgVisionSpeedEstimateEvent;

        public delegate void MsgViconPositionEstimateDelegate(Msg_vicon_position_estimate msg);

        public event MsgViconPositionEstimateDelegate MsgViconPositionEstimateEvent;

        public delegate void MsgHighresImuDelegate(Msg_highres_imu msg);

        public event MsgHighresImuDelegate MsgHighresImuEvent;

        public delegate void MsgOpticalFlowRadDelegate(Msg_optical_flow_rad msg);

        public event MsgOpticalFlowRadDelegate MsgOpticalFlowRadEvent;

        public delegate void MsgHilSensorDelegate(Msg_hil_sensor msg);

        public event MsgHilSensorDelegate MsgHilSensorEvent;

        public delegate void MsgSimStateDelegate(Msg_sim_state msg);

        public event MsgSimStateDelegate MsgSimStateEvent;

        public delegate void MsgRadioStatusDelegate(Msg_radio_status msg);

        public event MsgRadioStatusDelegate MsgRadioStatusEvent;

        public delegate void MsgFileTransferProtocolDelegate(Msg_file_transfer_protocol msg);

        public event MsgFileTransferProtocolDelegate MsgFileTransferProtocolEvent;

        public delegate void MsgTimeSyncDelegate(Msg_timesync msg);

        public event MsgTimeSyncDelegate MsgTimeSyncEvent;

        public delegate void MsgCameraTriggerDelegate(Msg_camera_trigger msg);

        public event MsgCameraTriggerDelegate MsgCameraTriggerEvent;

        public delegate void MsgHilGpsDelegate(Msg_hil_gps msg);

        public event MsgHilGpsDelegate MsgHilGpsEvent;

        public delegate void MsgHilOpticalFlowDelegate(Msg_hil_optical_flow msg);

        public event MsgHilOpticalFlowDelegate MsgHilOpticalFlowEvent;

        public delegate void MsgHilStateQuaternionDelegate(Msg_hil_state_quaternion msg);

        public event MsgHilStateQuaternionDelegate MsgHilStateQuaternionEvent;

        public delegate void MsgScaledImu2Delegate(Msg_scaled_imu2 msg);

        public event MsgScaledImu2Delegate MsgScaledImu2Event;

        public delegate void MsgLogRequestListDelegate(Msg_log_request_list msg);

        public event MsgLogRequestListDelegate MsgLogRequestListEvent;

        public delegate void MsgLogEntryDelegate(Msg_log_entry msg);

        public event MsgLogEntryDelegate MsgLogEntryEvent;

        public delegate void MsgLogRequestDataDelegate(Msg_log_request_data msg);

        public event MsgLogRequestDataDelegate MsgLogRequestDataEvent;

        public delegate void MsgLogDataDelegate(Msg_log_data msg);

        public event MsgLogDataDelegate MsgLogDataEvent;

        public delegate void MsgLogEraseDelegate(Msg_log_erase msg);

        public event MsgLogEraseDelegate MsgLogEraseEvent;

        public delegate void MsgLogRequestEndDelegate(Msg_log_request_end msg);

        public event MsgLogRequestEndDelegate MsgLogRequestEndEvent;

        public delegate void MsgGpsInjectDataDelegate(Msg_gps_inject_data msg);

        public event MsgGpsInjectDataDelegate MsgGpsInjectDataEvent;

        public delegate void MsgGps2RawDelegate(Msg_gps2_raw msg);

        public event MsgGps2RawDelegate MsgGps2RawEvent;

        public delegate void MsgPowerStatusDelegate(Msg_power_status msg);

        public event MsgPowerStatusDelegate MsgPowerStatusEvent;

        public delegate void MsgSerialControlDelegate(Msg_serial_control msg);

        public event MsgSerialControlDelegate MsgSerialControlEvent;


        public delegate void MsgGpsRtkDelegate(Msg_gps_rtk msg);

        public event MsgGpsRtkDelegate MsgGpsRtkEvent;

        public delegate void MsgGps2RtkDelegate(Msg_gps2_rtk msg);

        public event MsgGps2RtkDelegate MsgGps2RtkEvent;

        public delegate void MsgScaledImu3Delegate(Msg_scaled_imu3 msg);

        public event MsgScaledImu3Delegate MsgScaledImu3Event;

        public delegate void MsgDataTransmissionHandshakeDelegate(Msg_data_transmission_handshake msg);

        public event MsgDataTransmissionHandshakeDelegate MsgDataTransmissionHandshakeEvent;

        public delegate void MsgEncapsulatedDataDelegate(Msg_encapsulated_data msg);

        public event MsgEncapsulatedDataDelegate MsgEncapsulatedDataEvent;

        public delegate void MsgDistanceSensorDelegate(Msg_distance_sensor msg);

        public event MsgDistanceSensorDelegate MsgDistanceSensorEvent;

        public delegate void MsgTerrainRequestDelegate(Msg_terrain_request msg);

        public event MsgTerrainRequestDelegate MsgTerrainRequestEvent;

        public delegate void MsgTerrainDataDelegate(Msg_terrain_data msg);

        public event MsgTerrainDataDelegate MsgTerrainDataEvent;

        public delegate void MsgTerrainCheckDelegate(Msg_terrain_check msg);

        public event MsgTerrainCheckDelegate MsgTerrainCheckEvent;

        public delegate void MsgTerrainReportDelegate(Msg_terrain_report msg);

        public event MsgTerrainReportDelegate MsgTerrainReportEvent;

        public delegate void MsgScaledPressure2Delegate(Msg_scaled_pressure2 msg);

        public event MsgScaledPressure2Delegate MsgScaledPressure2Event;

        public delegate void MsgAttPosMocapDelegate(Msg_att_pos_mocap msg);

        public event MsgAttPosMocapDelegate MsgAttPosMocapEvent;

        public delegate void MsgSetActuatorControlTargetDelegate(Msg_set_actuator_control_target msg);

        public event MsgSetActuatorControlTargetDelegate MsgSetActuatorControlTargetEvent;

        public delegate void MsgActuatorControlTargetDelegate(Msg_actuator_control_target msg);

        public event MsgActuatorControlTargetDelegate MsgActuatorControlTargetEvent;

        public delegate void MsgAltitudeDelegate(Msg_altitude msg);

        public event MsgAltitudeDelegate MsgAltitudeEvent;

        public delegate void MsgResourceRequestDelegate(Msg_resource_request msg);

        public event MsgResourceRequestDelegate MsgResourceRequestEvent;

        public delegate void MsgScaledPressure3Delegate(Msg_scaled_pressure3 msg);

        public event MsgScaledPressure3Delegate MsgScaledPressure3Event;

        public delegate void MsgFollowTargetDelegate(Msg_follow_target msg);

        public event MsgFollowTargetDelegate MsgFollowTargetEvent;

        public delegate void MsgControlSystemStateDelegate(Msg_control_system_state msg);

        public event MsgControlSystemStateDelegate MsgControlSystemStateEvent;

        public delegate void MsgBatteryStatusDelegate(Msg_battery_status msg);

        public event MsgBatteryStatusDelegate MsgBatteryStatusEvent;

        public delegate void MsgAutopilotVersionDelegate(Msg_autopilot_version msg);

        public event MsgAutopilotVersionDelegate MsgAutopilotVersionEvent;

        public delegate void MsgLandingTargetDelegate(Msg_landing_target msg);

        public event MsgLandingTargetDelegate MsgLandingTargetEvent;

        public delegate void MsgFenceStatusDelegate(Msg_fence_status msg);

        public event MsgFenceStatusDelegate MsgFenceStatusEvent;

        public delegate void MsgEstimatorStatusDelegate(Msg_estimator_status msg);

        public event MsgEstimatorStatusDelegate MsgEstimatorStatusEvent;

        public delegate void MsgWindCovDelegate(Msg_wind_cov msg);

        public event MsgWindCovDelegate MsgWindCovEvent;

        public delegate void MsgGpsInputDelegate(Msg_gps_input msg);

        public event MsgGpsInputDelegate MsgGpsInputEvent;

        public delegate void MsgGpsRtcmDataDelegate(Msg_gps_rtcm_data msg);

        public event MsgGpsRtcmDataDelegate MsgGpsRtcmDataEvent;

        public delegate void MsgHighLatencyDelegate(Msg_high_latency msg);

        public event MsgHighLatencyDelegate MsgHighLatencyEvent;

        public delegate void MsgHighLatency2Delegate(Msg_high_latency2 msg);

        public event MsgHighLatency2Delegate MsgHighLatency2Event;

        public delegate void MsgVibrationDelegate(Msg_vibration msg);

        public event MsgVibrationDelegate MsgVibrationEvent;

        public delegate void MsgHomePositionDelegate(Msg_home_position msg);

        public event MsgHomePositionDelegate MsgHomePositionEvent;

        public delegate void MsgSetHomePositionDelegate(Msg_set_home_position msg);

        public event MsgSetHomePositionDelegate MsgSetHomePositionEvent;

        public delegate void MsgMessageIntervalDelegate(Msg_message_interval msg);

        public event MsgMessageIntervalDelegate MsgMessageIntervalEvent;

        public delegate void MsgExtendedSysStateDelegate(Msg_extended_sys_state msg);

        public event MsgExtendedSysStateDelegate MsgExtendedSysStateEvent;

        public delegate void MsgAdsbVehicleDelegate(Msg_adsb_vehicle msg);

        public event MsgAdsbVehicleDelegate MsgAdsbVehicleEvent;

        public delegate void MsgCollisionDelegate(Msg_collision msg);

        public event MsgCollisionDelegate MsgCollisionEvent;

        public delegate void MsgV2ExtensionDelegate(Msg_v2_extension msg);

        public event MsgV2ExtensionDelegate MsgV2ExtensionEvent;

        public delegate void MsgMemoryVectDelegate(Msg_memory_vect msg);

        public event MsgMemoryVectDelegate MsgMemoryVectEvent;

        public delegate void MsgDebugVectDelegate(Msg_debug_vect msg);

        public event MsgDebugVectDelegate MsgDebugVectEvent;

        public delegate void MsgNameValueFloatDelegate(Msg_named_value_float msg);

        public event MsgNameValueFloatDelegate MsgNameValueFloatEvent;

        public delegate void MsgNameValueIntDelegate(Msg_named_value_int msg);

        public event MsgNameValueIntDelegate MsgNameValueIntEvent;

        public delegate void MsgStatustextDelegate(Msg_statustext msg);

        public event MsgStatustextDelegate MsgStatustextEvent;

        public delegate void MsgDebugDelegate(Msg_debug msg);

        public event MsgDebugDelegate MsgDebugEvent;
        public void parseMavlink()
        {
            try
            {
                while (_tcpClient.Connected)
                {
                    Byte[] data;

                    _stream = _tcpClient.GetStream();

                    data = new Byte[256];

                    //Read the first batch of the TcpServer response bytes.
                    if (_tcpClient.Connected)
                    {
                        if (_stream.CanRead)//新增yyl
                        {
                            Int32 bytes = _stream.Read(data, 0, data.Length);
                            if (bytes > 0)
                            {
                                //parse mavlink
                                _mav.ParseBytes(data);
                                Debug.Log("ParseBytes: " + Thread.CurrentThread.ManagedThreadId.ToString());
                            }
                        }
                    }
                }

                _stream.Close();
            }
            catch (Exception e)
            {
               Debug.Log(e.Message);
            }
        }


        public void connect(string serverIP, int serverPort)
        {
            try
            {
                _tcpClient.Connect(serverIP, serverPort);
                commStatus = true;
                commStatusEvent(commStatus);
            }
            catch (Exception tcp_con)
            {
               Debug.Log(tcp_con.Message);
                commStatus = false;
                commStatusEvent(commStatus);
            }
            
        }

        public void close()
        {
            try
            {
                _tcpClient.Close();
                commStatus = false;
                commStatusEvent(commStatus);
            }
            catch (Exception tcp_con)
            {
                Debug.Log(tcp_con.Message);
            }
        }

        public void recvMavMsg(object sender,  MavlinkPacket e)
        {
            Debug.Log("recvMavMsg: " + Thread.CurrentThread.ManagedThreadId.ToString());
           

            string str = e.Message.ToString();

            if (str == "MavLink.Msg_heartbeat")
            {
                /* convert */
                Msg_heartbeat ht = (Msg_heartbeat)e.Message;
                if (MsgHeartBeatEvent != null)
                {
                    MsgHeartBeatEvent(ht);
                }
                Debug.Log("Msg_heartbeat_heart"+ht.system_status);
                /* save the incoming msg */
                mode = ht.custom_mode;

                /* publish */
                commDataEvent(mode);

            } 
            else if (str == "MavLink.Msg_sys_status")
            {
                Msg_sys_status syst = (Msg_sys_status) e.Message;
                if (MsgSysStatusEvent != null)
                {
                    MsgSysStatusEvent(syst);
                }
                Debug.Log("Msg_sys_status_current_battery"+syst.current_battery);
            }

            else if (str == "MavLink.Msg_system_time")
            {
                Msg_system_time st = (Msg_system_time) e.Message;
                if (MsgSystemTimeEvent != null)
                {
                    MsgSystemTimeEvent(st);
                }
                Debug.Log("Msg_system_time_time_boot_ms"+st.time_boot_ms);
            }

            else if (str == "MavLink.Msg_ping")
            {
                Msg_ping ping = (Msg_ping) e.Message;
                if (MsgPingEvent != null)
                {
                    MsgPingEvent(ping);
                }
                Debug.Log("Msg_ping_target_component"+ping.target_component);
            }

            else if (str == "MavLink.Msg_change_operator_control")
            {
                Msg_change_operator_control coc = (Msg_change_operator_control) e.Message;
                if (MsgChangeOperatorControlEvent != null)
                {
                    MsgChangeOperatorControlEvent(coc);
                }
                Debug.Log("Msg_change_operator_control_target_system"+coc.target_system);
            }

            else if (str == "MavLink.Msg_change_operator_control_ack")
            {
                Msg_change_operator_control_ack coca = (Msg_change_operator_control_ack) e.Message;
                if (MsgChangeOperatorControlAckEvent != null)
                {
                    MsgChangeOperatorControlAckEvent(coca);
                }
                Debug.Log("Msg_change_operator_control_ack_gcs_system_id" + coca.gcs_system_id);
            }

            else if (str == "MavLink.Msg_auth_key")
            {
                Msg_auth_key ak = (Msg_auth_key) e.Message;
                if (MsgAuthKeyEvent != null)
                {
                    MsgAuthKeyEvent(ak);
                }
                Debug.Log("Msg_auth_key_key[1]" + ak.key[1]);
            }

            else if (str == "MavLink.Msg_link_node_status")
            {
                Msg_link_node_status lns = (Msg_link_node_status) e.Message;
                if (MsgLinkNodeStatusEvent != null)
                {
                    MsgLinkNodeStatusEvent(lns);
                }
                Debug.Log("Msg_link_node_status_messages_received" + lns.messages_received);
            }

            else if (str == "MavLink.Msg_set_mode")
            {
                Msg_set_mode sm = (Msg_set_mode) e.Message;
                if (MsgSetModeEvent != null)
                {
                    MsgSetModeEvent(sm);
                }

            }

            else if (str == "MavLink.Msg_param_request_read")
            {
                Msg_param_request_read sm = (Msg_param_request_read)e.Message;
                if (MsgParamRequestReadEvent != null)
                {
                    MsgParamRequestReadEvent(sm);
                }

            }

            else if (str == "MavLink.Msg_param_request_list")
            {
                Msg_param_request_list msg = (Msg_param_request_list)e.Message;
                if (MsgParamRequestListEvent != null)
                {
                    MsgParamRequestListEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_param_value")
            {
                Msg_param_value msg = (Msg_param_value)e.Message;
                if (MsgParaValueEvent != null)
                {
                    MsgParaValueEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps_status")
            {
                Msg_gps_status msg = (Msg_gps_status)e.Message;
                if (MsgGpsStatusEvent != null)
                {
                    MsgGpsStatusEvent(msg);
                }

            }
            else if (str == "MavLink.Msg_param_set")
            {
                Msg_param_set msg = (Msg_param_set)e.Message;
                if (MsgParamSetEvent != null)
                {
                    MsgParamSetEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps_raw_int")
            {
                Msg_gps_raw_int msg = (Msg_gps_raw_int)e.Message;
                if (MsgGpsRawIntEvent != null)
                {
                    MsgGpsRawIntEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_scaled_imu")
            {
                Msg_scaled_imu msg = (Msg_scaled_imu)e.Message;
                if (MsgScaledImuEvent != null)
                {
                    MsgScaledImuEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_raw_imu")
            {
                Msg_raw_imu msg = (Msg_raw_imu)e.Message;
                if (MsgRawImuEvent != null)
                {
                    MsgRawImuEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_raw_pressure")
            {
                Msg_raw_pressure msg = (Msg_raw_pressure)e.Message;
                if (MsgRawPressureEvent != null)
                {
                    MsgRawPressureEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_scaled_pressure")
            {
                Msg_scaled_pressure msg = (Msg_scaled_pressure)e.Message;
                if (MsgScaledPressureEvent != null)
                {
                    MsgScaledPressureEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_attitude")
            {
                Msg_attitude at = (Msg_attitude)e.Message;
                if (MsgAttitudeEvent != null)
                {
                    MsgAttitudeEvent(at);
                }
                Debug.Log("Msg_attitude_yaw"+at.yaw);
            }


            else if (str == "MavLink.Msg_attitude_quaternion")
            {
                Msg_attitude_quaternion msg = (Msg_attitude_quaternion)e.Message;
                if (MsgAttitudeQuaternionEvent != null)
                {
                    MsgAttitudeQuaternionEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_local_position_ned")
            {
                Msg_local_position_ned msg = (Msg_local_position_ned)e.Message;
                if (MsgLocalPositionNedEvent != null)
                {
                    MsgLocalPositionNedEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_global_position_int")
            {
                Msg_global_position_int msg = (Msg_global_position_int)e.Message;
                if (MsgGlobalPositionIntEvent != null)
                {
                    MsgGlobalPositionIntEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_rc_channels_scaled")
            {
                Msg_rc_channels_scaled msg = (Msg_rc_channels_scaled)e.Message;
                if (MsgRcChannelsScaledEvent != null)
                {
                    MsgRcChannelsScaledEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_rc_channels_raw")
            {
                Msg_rc_channels_raw msg = (Msg_rc_channels_raw)e.Message;
                if (MsgRcChannelsRawEvent != null)
                {
                    MsgRcChannelsRawEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_servo_output_raw")
            {
                Msg_servo_output_raw msg = (Msg_servo_output_raw)e.Message;
                if (MsgServoOutputRawEvent != null)
                {
                    MsgServoOutputRawEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_request_partial_list")
            {
                Msg_mission_request_partial_list msg = (Msg_mission_request_partial_list)e.Message;
                if (MsgMissionRequestPartialListEvent != null)
                {
                    MsgMissionRequestPartialListEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_write_partial_list")
            {
                Msg_mission_write_partial_list msg = (Msg_mission_write_partial_list)e.Message;
                if (MsgMissionWritePartialListEvent != null)
                {
                    MsgMissionWritePartialListEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_item")
            {
                Msg_mission_item msg = (Msg_mission_item)e.Message;
                if (MsgMissionItemEvent != null)
                {
                    MsgMissionItemEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_request")
            {
                Msg_mission_request msg = (Msg_mission_request)e.Message;
                if (MsgMissionRequestEevent != null)
                {
                    MsgMissionRequestEevent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_set_current")
            {
                Msg_mission_set_current msg = (Msg_mission_set_current)e.Message;
                if (MsgMissionSetCurrentEvent != null)
                {
                    MsgMissionSetCurrentEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_current")
            {
                Msg_mission_current msg = (Msg_mission_current)e.Message;
                if (MsgMissionCurrentEvent != null)
                {
                    MsgMissionCurrentEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_request_list")
            {
                Msg_mission_request_list msg = (Msg_mission_request_list)e.Message;
                if (MsgMissionRequestListEvent != null)
                {
                    MsgMissionRequestListEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_count")
            {
                Msg_mission_count msg = (Msg_mission_count)e.Message;
                if (MsgMissionCountEvent != null)
                {
                    MsgMissionCountEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_clear_all")
            {
                Msg_mission_clear_all msg = (Msg_mission_clear_all)e.Message;
                if (MsgMissionClearAllEvent != null)
                {
                    MsgMissionClearAllEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_item_reached")
            {
                Msg_mission_item_reached msg = (Msg_mission_item_reached)e.Message;
                if (MsgMissionItemReachedEvent != null)
                {
                    MsgMissionItemReachedEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_ack")
            {
                Msg_mission_ack msg = (Msg_mission_ack)e.Message;
                if (MsgMissionAckEvent != null)
                {
                    MsgMissionAckEvent(msg);
                }

            }
            else if (str == "MavLink.Msg_set_gps_global_origin")
            {
                Msg_set_gps_global_origin msg = (Msg_set_gps_global_origin)e.Message;
                if (MsgSetGpsGlobalOriginEvent != null)
                {
                    MsgSetGpsGlobalOriginEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps_global_origin")
            {
                Msg_gps_global_origin msg = (Msg_gps_global_origin)e.Message;
                if (MsgGpsGlobalOriginEvent != null)
                {
                    MsgGpsGlobalOriginEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_param_map_rc")
            {
                Msg_param_map_rc msg = (Msg_param_map_rc)e.Message;
                if (MsgParamMapRcEvent != null)
                {
                    MsgParamMapRcEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_request_int")
            {
                Msg_mission_request_int msg = (Msg_mission_request_int)e.Message;
                if (MsgMissionRequestIntEvent != null)
                {
                    MsgMissionRequestIntEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_changed")
            {
                Msg_mission_changed msg = (Msg_mission_changed)e.Message;
                if (MsgMissionChangedEvent != null)
                {
                    MsgMissionChangedEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_safety_set_allowed_area")
            {
                Msg_safety_set_allowed_area msg = (Msg_safety_set_allowed_area)e.Message;
                if (MsgSafetySetAllowedAreaEvent != null)
                {
                    MsgSafetySetAllowedAreaEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_safety_allowed_area")
            {
                Msg_safety_allowed_area msg = (Msg_safety_allowed_area)e.Message;
                if (MsgSafetyAllowedAreaEvent != null)
                {
                    MsgSafetyAllowedAreaEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_attitude_quaternion_cov")
            {
                Msg_attitude_quaternion_cov msg = (Msg_attitude_quaternion_cov)e.Message;
                if (MsgAttitudeQuaternionCovEvent != null)
                {
                    MsgAttitudeQuaternionCovEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_nav_controller_output")
            {
                Msg_nav_controller_output msg = (Msg_nav_controller_output)e.Message;
                if (MsgNavControllerOutputEvent != null)
                {
                    MsgNavControllerOutputEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_global_position_int_cov")
            {
                Msg_global_position_int_cov msg = (Msg_global_position_int_cov)e.Message;
                if (MsgGlobalPositionIntCovEvent != null)
                {
                    MsgGlobalPositionIntCovEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_local_position_ned_cov")
            {
                Msg_local_position_ned_cov msg = (Msg_local_position_ned_cov)e.Message;
                if (MsgLocalPositionNedCovEvent != null)
                {
                    MsgLocalPositionNedCovEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_rc_channels")
            {
                Msg_rc_channels msg = (Msg_rc_channels)e.Message;
                if (MsgRcChannelsEvent != null)
                {
                    MsgRcChannelsEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_request_data_stream")
            {
                Msg_request_data_stream msg = (Msg_request_data_stream)e.Message;
                if (MsgRequestDataStreamEvent != null)
                {
                    MsgRequestDataStreamEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_data_stream")
            {
                Msg_data_stream msg = (Msg_data_stream)e.Message;
                if (MsgDataStreamEvent != null)
                {
                    MsgDataStreamEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_manual_control")
            {
                Msg_manual_control msg = (Msg_manual_control)e.Message;
                if (MsgManualControlEvent != null)
                {
                    MsgManualControlEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_rc_channels_override")
            {
                Msg_rc_channels_override msg = (Msg_rc_channels_override)e.Message;
                if (MsgRcChannelsOverrideEvent != null)
                {
                    MsgRcChannelsOverrideEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_mission_item_int")
            {
                Msg_mission_item_int msg = (Msg_mission_item_int)e.Message;
                if (MsgMissionItemIntEvent != null)
                {
                    MsgMissionItemIntEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_vfr_hud")
            {
                Msg_vfr_hud msg = (Msg_vfr_hud)e.Message;
                if (MsgVfrHudEvent != null)
                {
                    MsgVfrHudEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_command_int")
            {
                Msg_command_int msg = (Msg_command_int)e.Message;
                if (MsgCommandIntEvent != null)
                {
                    MsgCommandIntEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_command_long")
            {
                Msg_command_long msg = (Msg_command_long)e.Message;
                if (MsgCommandLongEvent != null)
                {
                    MsgCommandLongEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_command_ack")
            {
                Msg_command_ack msg = (Msg_command_ack)e.Message;
                if (MsgCommandAckEvent != null)
                {
                    MsgCommandAckEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_command_cancel")
            {
                Msg_command_cancel msg = (Msg_command_cancel)e.Message;
                if (MsgCommandCancelEvent != null)
                {
                    MsgCommandCancelEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_manual_setpoint")
            {
                Msg_manual_setpoint msg = (Msg_manual_setpoint)e.Message;
                if (MsgManualSetPointEvent != null)
                {
                    MsgManualSetPointEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_set_attitude_target")
            {
                Msg_set_attitude_target msg = (Msg_set_attitude_target)e.Message;
                if (MsgSetAttitudeTargetEvent != null)
                {
                    MsgSetAttitudeTargetEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_attitude_target")
            {
                Msg_attitude_target msg = (Msg_attitude_target)e.Message;
                if (MsgAttitudeTargetEvent != null)
                {
                    MsgAttitudeTargetEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_set_position_target_local_ned")
            {
                Msg_set_position_target_local_ned msg = (Msg_set_position_target_local_ned)e.Message;
                if (MsgSetPositionTargetLocalNedEvent != null)
                {
                    MsgSetPositionTargetLocalNedEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_position_target_local_ned")
            {
                Msg_position_target_local_ned msg = (Msg_position_target_local_ned)e.Message;
                if (MsgPositionTargetLocalNedEvent != null)
                {
                    MsgPositionTargetLocalNedEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_set_position_target_global_int")
            {
                Msg_set_position_target_global_int msg = (Msg_set_position_target_global_int)e.Message;
                if (MsgSetPositionTargetGlobalIntEvent != null)
                {
                    MsgSetPositionTargetGlobalIntEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_position_target_global_int")
            {
                Msg_position_target_global_int msg = (Msg_position_target_global_int)e.Message;
                if (MsgPositionTargetGlobalIntEvent != null)
                {
                    MsgPositionTargetGlobalIntEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_local_position_ned_system_global_offset")
            {
                Msg_local_position_ned_system_global_offset msg = (Msg_local_position_ned_system_global_offset)e.Message;
                if (MsgLocalPositionNedSystemGlobalOffsetEvent != null)
                {
                    MsgLocalPositionNedSystemGlobalOffsetEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_hil_state")
            {
                Msg_hil_state msg = (Msg_hil_state)e.Message;
                if (MsgHilStateEvent != null)
                {
                    MsgHilStateEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_hil_controls")
            {
                Msg_hil_controls msg = (Msg_hil_controls)e.Message;
                if (MsgHilControlsEvent != null)
                {
                    MsgHilControlsEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_hil_rc_inputs_raw")
            {
                Msg_hil_rc_inputs_raw msg = (Msg_hil_rc_inputs_raw)e.Message;
                if (MsgHilRcInputRawEvent != null)
                {
                    MsgHilRcInputRawEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_hil_actuator_controls")
            {
                Msg_hil_actuator_controls msg = (Msg_hil_actuator_controls)e.Message;
                if (MsgHilActuatorControlsEvent != null)
                {
                    MsgHilActuatorControlsEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_optical_flow")
            {
                Msg_optical_flow msg = (Msg_optical_flow)e.Message;
                if (MsgOpticalFlowEvent != null)
                {
                    MsgOpticalFlowEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_global_vision_position_estimate")
            {
                Msg_global_vision_position_estimate msg = (Msg_global_vision_position_estimate)e.Message;
                if (MsgGlobalVisionPositionEstimateEvent != null)
                {
                    MsgGlobalVisionPositionEstimateEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_vision_position_estimate")
            {
                Msg_vision_position_estimate msg = (Msg_vision_position_estimate)e.Message;
                if (MsgVisionPositionEstimateEvent != null)
                {
                    MsgVisionPositionEstimateEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_vision_speed_estimate")
            {
                Msg_vision_speed_estimate msg = (Msg_vision_speed_estimate)e.Message;
                if (MsgVisionSpeedEstimateEvent != null)
                {
                    MsgVisionSpeedEstimateEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_vicon_position_estimate")
            {
                Msg_vicon_position_estimate msg = (Msg_vicon_position_estimate)e.Message;
                if (MsgViconPositionEstimateEvent != null)
                {
                    MsgViconPositionEstimateEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_highres_imu")
            {
                Msg_highres_imu msg = (Msg_highres_imu)e.Message;
                if (MsgHighresImuEvent != null)
                {
                    MsgHighresImuEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_optical_flow_rad")
            {
                Msg_optical_flow_rad msg = (Msg_optical_flow_rad)e.Message;
                if (MsgOpticalFlowRadEvent != null)
                {
                    MsgOpticalFlowRadEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_hil_sensor")
            {
                Msg_hil_sensor msg = (Msg_hil_sensor)e.Message;
                if (MsgHilSensorEvent != null)
                {
                    MsgHilSensorEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_sim_state")
            {
                Msg_sim_state msg = (Msg_sim_state)e.Message;
                if (MsgSimStateEvent != null)
                {
                    MsgSimStateEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_radio_status")
            {
                Msg_radio_status msg = (Msg_radio_status)e.Message;
                if (MsgRadioStatusEvent != null)
                {
                    MsgRadioStatusEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_file_transfer_protocol")
            {
                Msg_file_transfer_protocol msg = (Msg_file_transfer_protocol)e.Message;
                if (MsgFileTransferProtocolEvent != null)
                {
                    MsgFileTransferProtocolEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_timesync")
            {
                Msg_timesync msg = (Msg_timesync)e.Message;
                if (MsgTimeSyncEvent != null)
                {
                    MsgTimeSyncEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_camera_trigger")
            {
                Msg_camera_trigger msg = (Msg_camera_trigger)e.Message;
                if (MsgCameraTriggerEvent != null)
                {
                    MsgCameraTriggerEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_hil_gps")
            {
                Msg_hil_gps msg = (Msg_hil_gps)e.Message;
                if (MsgHilGpsEvent != null)
                {
                    MsgHilGpsEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_hil_optical_flow")
            {
                Msg_hil_optical_flow msg = (Msg_hil_optical_flow)e.Message;
                if (MsgHilOpticalFlowEvent != null)
                {
                    MsgHilOpticalFlowEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_hil_state_quaternion")
            {
                Msg_hil_state_quaternion msg = (Msg_hil_state_quaternion)e.Message;
                if (MsgHilStateQuaternionEvent != null)
                {
                    MsgHilStateQuaternionEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_scaled_imu2")
            {
                Msg_scaled_imu2 msg = (Msg_scaled_imu2)e.Message;
                if (MsgScaledImu2Event != null)
                {
                    MsgScaledImu2Event(msg);
                }

            }

            else if (str == "MavLink.Msg_log_request_list")
            {
                Msg_log_request_list msg = (Msg_log_request_list)e.Message;
                if (MsgLogRequestListEvent != null)
                {
                    MsgLogRequestListEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_log_entry")
            {
                Msg_log_entry msg = (Msg_log_entry)e.Message;
                if (MsgLogEntryEvent != null)
                {
                    MsgLogEntryEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_log_request_data")
            {
                Msg_log_request_data msg = (Msg_log_request_data)e.Message;
                if (MsgLogRequestDataEvent != null)
                {
                    MsgLogRequestDataEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_log_data")
            {
                Msg_log_data msg = (Msg_log_data)e.Message;
                if (MsgLogDataEvent != null)
                {
                    MsgLogDataEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_log_erase")
            {
                Msg_log_erase msg = (Msg_log_erase)e.Message;
                if (MsgLogEraseEvent != null)
                {
                    MsgLogEraseEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_log_request_end")
            {
                Msg_log_request_end msg = (Msg_log_request_end)e.Message;
                if (MsgLogRequestEndEvent != null)
                {
                    MsgLogRequestEndEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps_inject_data")
            {
                Msg_gps_inject_data msg = (Msg_gps_inject_data)e.Message;
                if (MsgGpsInjectDataEvent != null)
                {
                    MsgGpsInjectDataEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps2_raw")
            {
                Msg_gps2_raw msg = (Msg_gps2_raw)e.Message;
                if (MsgGps2RawEvent != null)
                {
                    MsgGps2RawEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps2_raw")
            {
                Msg_gps2_raw msg = (Msg_gps2_raw)e.Message;
                if (MsgGps2RawEvent != null)
                {
                    MsgGps2RawEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_power_status")
            {
                Msg_power_status msg = (Msg_power_status)e.Message;
                if (MsgPowerStatusEvent != null)
                {
                    MsgPowerStatusEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_serial_control")
            {
                Msg_serial_control msg = (Msg_serial_control)e.Message;
                if (MsgSerialControlEvent != null)
                {
                    MsgSerialControlEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps_rtk")
            {
                Msg_gps_rtk msg = (Msg_gps_rtk)e.Message;
                if (MsgGpsRtkEvent != null)
                {
                    MsgGpsRtkEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps2_rtk")
            {
                Msg_gps2_rtk msg = (Msg_gps2_rtk)e.Message;
                if (MsgGps2RtkEvent != null)
                {
                    MsgGps2RtkEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_scaled_imu3")
            {
                Msg_scaled_imu3 msg = (Msg_scaled_imu3)e.Message;
                if (MsgScaledImu3Event != null)
                {
                    MsgScaledImu3Event(msg);
                }

            }

            else if (str == "MavLink.Msg_data_transmission_handshake")
            {
                Msg_data_transmission_handshake msg = (Msg_data_transmission_handshake)e.Message;
                if (MsgDataTransmissionHandshakeEvent != null)
                {
                    MsgDataTransmissionHandshakeEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_encapsulated_data")
            {
                Msg_encapsulated_data msg = (Msg_encapsulated_data)e.Message;
                if (MsgEncapsulatedDataEvent != null)
                {
                    MsgEncapsulatedDataEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_distance_sensor")
            {
                Msg_distance_sensor msg = (Msg_distance_sensor)e.Message;
                if (MsgDistanceSensorEvent != null)
                {
                    MsgDistanceSensorEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_terrain_request")
            {
                Msg_terrain_request msg = (Msg_terrain_request)e.Message;
                if (MsgTerrainRequestEvent != null)
                {
                    MsgTerrainRequestEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_terrain_data")
            {
                Msg_terrain_data msg = (Msg_terrain_data)e.Message;
                if (MsgTerrainDataEvent != null)
                {
                    MsgTerrainDataEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_terrain_check")
            {
                Msg_terrain_check msg = (Msg_terrain_check)e.Message;
                if (MsgTerrainCheckEvent != null)
                {
                    MsgTerrainCheckEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_terrain_report")
            {
                Msg_terrain_report msg = (Msg_terrain_report)e.Message;
                if (MsgTerrainReportEvent != null)
                {
                    MsgTerrainReportEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_scaled_pressure2")
            {
                Msg_scaled_pressure2 msg = (Msg_scaled_pressure2)e.Message;
                if (MsgScaledPressure2Event != null)
                {
                    MsgScaledPressure2Event(msg);
                }

            }

            else if (str == "MavLink.Msg_att_pos_mocap")
            {
                Msg_att_pos_mocap msg = (Msg_att_pos_mocap)e.Message;
                if (MsgAttPosMocapEvent != null)
                {
                    MsgAttPosMocapEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_set_actuator_control_target")
            {
                Msg_set_actuator_control_target msg = (Msg_set_actuator_control_target)e.Message;
                if (MsgSetActuatorControlTargetEvent != null)
                {
                    MsgSetActuatorControlTargetEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_actuator_control_target")
            {
                Msg_actuator_control_target msg = (Msg_actuator_control_target)e.Message;
                if (MsgActuatorControlTargetEvent != null)
                {
                    MsgActuatorControlTargetEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_altitude")
            {
                Msg_altitude msg = (Msg_altitude)e.Message;
                if (MsgAltitudeEvent != null)
                {
                    MsgAltitudeEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_resource_request")
            {
                Msg_resource_request msg = (Msg_resource_request)e.Message;
                if (MsgResourceRequestEvent != null)
                {
                    MsgResourceRequestEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_scaled_pressure3")
            {
                Msg_scaled_pressure3 msg = (Msg_scaled_pressure3)e.Message;
                if (MsgScaledPressure3Event != null)
                {
                    MsgScaledPressure3Event(msg);
                }

            }

            else if (str == "MavLink.Msg_follow_target")
            {
                Msg_follow_target msg = (Msg_follow_target)e.Message;
                if (MsgFollowTargetEvent != null)
                {
                    MsgFollowTargetEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_control_system_state")
            {
                Msg_control_system_state msg = (Msg_control_system_state)e.Message;
                if (MsgControlSystemStateEvent != null)
                {
                    MsgControlSystemStateEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_battery_status")
            {
                Msg_battery_status msg = (Msg_battery_status)e.Message;
                if (MsgBatteryStatusEvent != null)
                {
                    MsgBatteryStatusEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_autopilot_version")
            {
                Msg_autopilot_version msg = (Msg_autopilot_version)e.Message;
                if (MsgAutopilotVersionEvent != null)
                {
                    MsgAutopilotVersionEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_landing_target")
            {
                Msg_landing_target msg = (Msg_landing_target)e.Message;
                if (MsgLandingTargetEvent != null)
                {
                    MsgLandingTargetEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_fence_status")
            {
                Msg_fence_status msg = (Msg_fence_status)e.Message;
                if (MsgFenceStatusEvent != null)
                {
                    MsgFenceStatusEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_estimator_status")
            {
                Msg_estimator_status msg = (Msg_estimator_status)e.Message;
                if (MsgEstimatorStatusEvent != null)
                {
                    MsgEstimatorStatusEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_wind_cov")
            {
                Msg_wind_cov msg = (Msg_wind_cov)e.Message;
                if (MsgWindCovEvent != null)
                {
                    MsgWindCovEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps_input")
            {
                Msg_gps_input msg = (Msg_gps_input)e.Message;
                if (MsgGpsInputEvent != null)
                {
                    MsgGpsInputEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_gps_rtcm_data")
            {
                Msg_gps_rtcm_data msg = (Msg_gps_rtcm_data)e.Message;
                if (MsgGpsRtcmDataEvent != null)
                {
                    MsgGpsRtcmDataEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_high_latency")
            {
                Msg_high_latency msg = (Msg_high_latency)e.Message;
                if (MsgHighLatencyEvent != null)
                {
                    MsgHighLatencyEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_high_latency2")
            {
                Msg_high_latency2 msg = (Msg_high_latency2)e.Message;
                if (MsgHighLatency2Event != null)
                {
                    MsgHighLatency2Event(msg);
                }

            }

            else if (str == "MavLink.Msg_vibration")
            {
                Msg_vibration msg = (Msg_vibration)e.Message;
                if (MsgVibrationEvent != null)
                {
                    MsgVibrationEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_home_position")
            {
                Msg_home_position msg = (Msg_home_position)e.Message;
                if (MsgHomePositionEvent != null)
                {
                    MsgHomePositionEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_set_home_position")
            {
                Msg_set_home_position msg = (Msg_set_home_position)e.Message;
                if (MsgSetHomePositionEvent != null)
                {
                    MsgSetHomePositionEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_message_interval")
            {
                Msg_message_interval msg = (Msg_message_interval)e.Message;
                if (MsgMessageIntervalEvent != null)
                {
                    MsgMessageIntervalEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_extended_sys_state")
            {
                Msg_extended_sys_state msg = (Msg_extended_sys_state)e.Message;
                if (MsgExtendedSysStateEvent != null)
                {
                    MsgExtendedSysStateEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_adsb_vehicle")
            {
                Msg_adsb_vehicle msg = (Msg_adsb_vehicle)e.Message;
                if (MsgAdsbVehicleEvent != null)
                {
                    MsgAdsbVehicleEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_collision")
            {
                Msg_collision msg = (Msg_collision)e.Message;
                if (MsgCollisionEvent != null)
                {
                    MsgCollisionEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_v2_extension")
            {
                Msg_v2_extension msg = (Msg_v2_extension)e.Message;
                if (MsgV2ExtensionEvent != null)
                {
                    MsgV2ExtensionEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_memory_vect")
            {
                Msg_memory_vect msg = (Msg_memory_vect)e.Message;
                if (MsgMemoryVectEvent != null)
                {
                    MsgMemoryVectEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_debug_vect")
            {
                Msg_debug_vect msg = (Msg_debug_vect)e.Message;
                if (MsgDebugVectEvent != null)
                {
                    MsgDebugVectEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_named_value_float")
            {
                Msg_named_value_float msg = (Msg_named_value_float)e.Message;
                if (MsgNameValueFloatEvent != null)
                {
                    MsgNameValueFloatEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_named_value_int")
            {
                Msg_named_value_int msg = (Msg_named_value_int)e.Message;
                if (MsgNameValueIntEvent != null)
                {
                    MsgNameValueIntEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_statustext")
            {
                Msg_statustext msg = (Msg_statustext)e.Message;
                if (MsgStatustextEvent != null)
                {
                    MsgStatustextEvent(msg);
                }

            }

            else if (str == "MavLink.Msg_debug")
            {
                Msg_debug msg = (Msg_debug)e.Message;
                if (MsgDebugEvent != null)
                {
                    MsgDebugEvent(msg);
                }

            }
            else
            {
                
            }

        }

        //发送消息到ros
        public void sendData()
        {
            if (sendDataEvent != null)
            {
                sendDataEvent(_stream);
            }
            
        }

        public void sendMavMsg(byte cmd,byte addr,float pos)
        {
            try
            {
                /* mavlink encode */
                _mht.mavlink_version = cmd;
                _mht.custom_mode = (uint)pos;
                _mht.base_mode = addr;

                _mht.system_status = 4;

                _mavPack.ComponentId = 1;
                _mavPack.SystemId = 1;
                _mavPack.SequenceNumber = 0;
                _mavPack.TimeStamp = DateTime.Now;
                _mavPack.Message = _mht;

                byte[] buffer = _mav.Send(_mavPack);
                
                _stream.Write(buffer, 0, buffer.Length);
               Debug.Log("Write: " + Thread.CurrentThread.ManagedThreadId.ToString());
            }
            catch (Exception ee)
            {
                Debug.Log(ee.Message);
            }
        }


    }
}
