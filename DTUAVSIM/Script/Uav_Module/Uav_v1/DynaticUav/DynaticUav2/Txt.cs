using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Txt : MonoBehaviour
{
    public static TextAsset WayPointTxt;
    static public List<string[]> WayPointList;




    //以换行符为分割点，将该文本分割成若干行字符串，并以数组的形式保存每一行字符串
    string[] TexToLine(TextAsset waypointTxt)
    {
        string[] str = waypointTxt.text.Split('\n') ;
        return str;
    }

    //将每行字符串的内容以逗号为分割点，提前每个字符串
     List<string[]> LineToStr(string[] str)
    {
        List<string[]> StrList = new List<string[]>();
        foreach(string strs in str)
        {
            StrList.Add(strs.Split(','));
        }

        return StrList;
    }


    // Start is called before the first frame update
    void Start()
    {
        WayPointTxt = Resources.Load("waypoint") as TextAsset;
        if(WayPointTxt == null)
        {
            Debug.Log("读取文件失败");
        }
        else
        {
            Debug.Log("读取文件成功");
           // Debug.Log(WayPointTxt);

            WayPointList = LineToStr(TexToLine(WayPointTxt));
           // string[] ss = WayPointList[0];
           //  Debug.Log(ss[1]);
        }
       


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
