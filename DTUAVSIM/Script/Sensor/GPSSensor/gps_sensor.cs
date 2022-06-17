using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.Gps
{
    public class gps_sensor
    { 
        private GameObject target_obj;//gps传感器挂载的对象
        private string curr_data;//当前的日期
        private string curr_time;//当前的时间
        private string curr_dirction_lat;//当前纬度方向
        private string curr_dirction_long;//当前经度方向
        private string curr_status;//当前数据状态，是否有效
        private double latitude;//当前纬度
        private double longitude;//当前经度
        private double high;//当前高度

        void get_curr_data()
        {
            int hour = DateTime.Now.Hour;
            int minute = DateTime.Now.Minute;
            int second = DateTime.Now.Second;
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            curr_data = year.ToString() + "-" + month.ToString() + "-" + day.ToString();
            curr_time = hour.ToString() + ":" + minute.ToString() + ":" + second.ToString();
            curr_status = "A";
        }


        private struct globa_position
        {
            public float latitude;
            public float longitude;
            public float altitude;
        }

        private struct groud_position
        {
            public float x;
            public float y;
            public float z;
        }

        private struct enu_position
        {
            public float x;
            public float y;
            public float z;
        }
     public   gps_sensor(GameObject obj)
        {
            this.target_obj = obj;
            curr_data = string.Empty;
            curr_time = string.Empty;
            curr_dirction_lat = string.Empty;
            curr_dirction_long = string.Empty;
            curr_status = string.Empty;
            latitude = 0;
            longitude = 0;
            high = 0;

        }

     public void update_gps()
        {
            groud_position gro_position;
            gro_position.x = this.target_obj.transform.position.x;
            gro_position.y = this.target_obj.transform.position.y;
            gro_position.z = this.target_obj.transform.position.z;
            globa_position glo_position;
            glo_position = groud_to_global(gro_position);
            Debug.Log(glo_position.latitude);
        }

     //wgs84坐标-->球心坐标
        private groud_position global_to_groud(globa_position glo_position)
        {
            float a = 6378137;//a为椭球的长半轴:a=6378.137km
            float b = 6356752.3141f;//b为椭球的短半轴:a=6356.7523141km
            float H = glo_position.altitude;
            float e = Mathf.Sqrt(1 - Mathf.Pow(b, 2) / Mathf.Pow(a, 2));//e为椭球的第一偏心率
                // double e=sqrt(0.006693421622966); //克拉索夫斯基椭球
               // double e=sqrt(0.006694384999588); //1975年国际椭球
              // double e=sqrt(0.0066943799013); //WGS-84椭球
              groud_position gro_position;
              float m = Mathf.PI / 180;//经度维度需要转换成弧度.
              float B = glo_position.latitude * m;
              float L = glo_position.longitude * m;
              float W = Mathf.Sqrt(1 - Mathf.Pow(e, 2) * Mathf.Pow(Mathf.Sin(B), 2));
              float N = a / W;//N为椭球的卯酉圈曲率半径
              gro_position.x = (N + H) * Mathf.Cos(B) * Mathf.Cos(L);
              gro_position.y = (N + H) * Mathf.Cos(B) * Mathf.Sin(L);
              gro_position.z = (N * (1 - Mathf.Pow(e, 2)) + H) * Mathf.Sin(B);
              return gro_position;
        }
        //球心坐标-->wgs84坐标
        private globa_position groud_to_global(groud_position gro_position)
        { 
            float v0 = gro_position.z / Mathf.Sqrt(Mathf.Pow(gro_position.x, 2) + Mathf.Pow(gro_position.y, 2));
            float a = 6378137;//a为椭球的长半轴:a=6378.137km
            float b = 6356752;
            float e = Mathf.Sqrt(1 - Mathf.Pow(b, 2) / Mathf.Pow(a, 2));
                // double e=sqrt(0.006693421622966); //克拉索夫斯基椭球
               // double e=sqrt(0.006694384999588); //1975年国际椭球
              // double e=sqrt(0.0066943799013); //WGS-84椭球
             // double W=sqrt(1-pow(e ,2)*pow(sin(B) ,2));
             float N = 0;//N为椭球的卯酉圈曲率半径
             float B1 = Mathf.Atan(v0), B2 = 0;
             float H = 0;
             while(Mathf.Abs((B2-B1))> Mathf.Pow(10,-5))
             {
                 N = a / Mathf.Sqrt(1 - Mathf.Pow(e, 2) * Mathf.Pow(Mathf.Sin(B1), 2));
                 H = gro_position.z / Mathf.Sin(B1) - N * (1 - Mathf.Pow(e, 2));
                 B2 = Mathf.Atan(gro_position.z * (N + H) /
                                 Mathf.Sqrt((Mathf.Pow(gro_position.x, 2) + Mathf.Pow(gro_position.y, 2)) *
                                            (N * (1 - Mathf.Pow(e, 2))+H)));
                 B1 = B2;
             }

             float m = Mathf.PI / 180;
             globa_position glo_position;
             glo_position.latitude = B1 / m;
             glo_position.longitude = Mathf.Atan(gro_position.y / gro_position.x) / m;
             glo_position.altitude = H - a;
             return glo_position;
        }
       //球心-->站心
       private enu_position groud_to_enu(groud_position pos_XYZ, globa_position Center)
       {
           float a = 6378137;//a为椭球的长半轴:a=6378.137km
            enu_position pos_xyz;
           groud_position tmp_XYZ;
           groud_position Center_XYZ = global_to_groud(Center);
           tmp_XYZ.x = pos_XYZ.x - Center_XYZ.x;
           tmp_XYZ.y = pos_XYZ.y - Center_XYZ.y;
           tmp_XYZ.z = pos_XYZ.z - Center_XYZ.z;
           float m = Mathf.PI / 180;
           pos_xyz.x = -Mathf.Sin(Center.latitude * m) * Mathf.Cos(Center.longitude * m) * tmp_XYZ.x -
                       Mathf.Sin(Center.latitude * m) * Mathf.Sin(Center.longitude * m) * tmp_XYZ.y +
                       Mathf.Cos(Center.latitude * m) * tmp_XYZ.z;
           pos_xyz.y = -Mathf.Sin(Center.longitude * m) * tmp_XYZ.x + Mathf.Cos(Center.longitude * m) * tmp_XYZ.y;
           pos_xyz.z = Mathf.Cos(Center.latitude * m) * Mathf.Cos(Center.longitude * m) * tmp_XYZ.x +Mathf.Cos(Center.latitude * m) * Mathf.Sin(Center.longitude * m) * tmp_XYZ.y + Mathf.Sin(Center.latitude * m) * tmp_XYZ.z - a;
           return pos_xyz;
       }
        //站心-->球心
        private groud_position enu_to_groud(enu_position pos_xyz, globa_position Center)
        {
            float a = 6378137;//a为椭球的长半轴:a=6378.137km
            float b = 6356752.3141f;//b为椭球的短半轴:a=6356.7523141km
            float H0 = Center.altitude + a;
            float e = Mathf.Sqrt(1 - Mathf.Pow(b, 2) / Mathf.Pow(a, 2)); //e为椭球的第一偏心率
            // double e=sqrt(0.006693421622966); //克拉索夫斯基椭球
            // double e=sqrt(0.006694384999588); //1975年国际椭球
            // double e=sqrt(0.0066943799013); //WGS-84椭球
            float m = Mathf.PI / 180;//经度维度需要转换成弧度.
            float B0 = Center.latitude * m;
            float L0 = Center.longitude * m;
            float W = Mathf.Sqrt(1 - Mathf.Pow(e, 2) * Mathf.Pow(Mathf.Sin(B0), 2));
            float N0 = a / W; //N为椭球的卯酉圈曲率半径
            groud_position pos_XYZ;
            pos_XYZ.x = (N0 + H0) * Mathf.Cos(B0) * Mathf.Cos(L0)
                - Mathf.Sin(B0) * Mathf.Cos(L0) * pos_xyz.x - Mathf.Sin(L0) * pos_xyz.y + Mathf.Cos(B0) * Mathf.Cos(L0) * pos_xyz.z;
            pos_XYZ.y = (N0 + H0) * Mathf.Cos(B0) * Mathf.Sin(L0)
                - Mathf.Sin(B0) * Mathf.Sin(L0) * pos_xyz.x - Mathf.Cos(L0) * pos_xyz.y + Mathf.Cos(B0) * Mathf.Sin(L0) * pos_xyz.z;
            pos_XYZ.z = (N0 * (1 -Mathf.Pow(e, 2)) + H0) * Mathf.Sin(B0)
                - Mathf.Cos(B0) * pos_xyz.x + Mathf.Sin(B0) * pos_xyz.z;
            return pos_XYZ;
                
        }




    }

}