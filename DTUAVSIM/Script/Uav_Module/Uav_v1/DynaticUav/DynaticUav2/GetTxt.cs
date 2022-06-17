using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetTxt 
{
   //从Assets的Resource文件夹中读取fileName文件
   public TextAsset readTxt(string fileName)
    {
        // WayPointTxt = Resources.Load("waypoint") as TextAsset;
        TextAsset WayPointTxt = Resources.Load(fileName) as TextAsset;
        if (WayPointTxt == null)
        {
            Debug.Log("读取文件失败");
            return null;
        }
        else
        {
            Debug.Log("读取文件成功");
            return WayPointTxt;
        }
        
    }



    //以换行符为分割点，将该文本分割成若干行字符串，并以数组的形式保存每一行字符串
  public  string[] TexToLine(TextAsset waypointTxt)
    {
        if (waypointTxt == null)
        { 
            return null; }
        else
        {
            string[] str = waypointTxt.text.Split('\n');
            return str;
        }
    }

    //将每行字符串的内容以逗号为分割点，提前每个字符串
  public  List<string[]> LineToStr(string[] str)
    {
        if (str == null)
        {
            return null;
        }
        else
        {
            List<string[]> StrList = new List<string[]>();
            foreach (string strs in str)
            {
                StrList.Add(strs.Split(','));
            }

            return StrList;
        }
    }


    //这个方法读取文件、以换行符、逗号切割保存在List中
 public  List<string[]> GetWayPoint(string fileName)
    {
        return LineToStr(TexToLine(readTxt(fileName)));
    }



}
