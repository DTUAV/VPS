/*
 * Update Information:
 *                    First: 2021-8-4 In Guangdong University of Technology By Yang Yuanlin
 *
 */

using UnityEngine;

namespace DTUAVCARS.DTNetwok.Message
{
    public class MessageProcess
    {
        private static string CheckRecvData(string msg)
        {
            string ret = "";
            if (msg[0] != '{')
            {
                ret = "";
            }
            else
            {
                int num = 0;
                for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] == '}')
                    {
                        num++;
                    }

                    if (num == 2)
                    {
                        ret = msg.Substring(0, i + 1);
                        break;
                    }
                }
            }

            return ret;
        }

        public static IotMessage GetRecvData(string msg)
        {
            string dataMsg = CheckRecvData(msg);
            if (dataMsg != "")
            {
                IotMessage ret = JsonUtility.FromJson<IotMessage>(dataMsg);
                return ret;
            } 
            else
            {
                return null;
            }
        }


    }
}
