using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mytest.Detatil;
using System.IO;
using Google.Protobuf;
public class testProtoBuf : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Test_Person _Person = new Test_Person
        {
            Name = "老王",
            Level =12,
            Profession = "法师",

            All = { new Test_Person.Types.Person_all { Num = "123", Type = 0 } }

        };

        using (var output = File.Create("Test.txt"))
        { //写入stream
            _Person.WriteTo(output);
        }
        //转json
        var json = _Person.ToString();
        //转bytestring
        var byteStr = _Person.ToByteString();
        //转byte数组
        var byteArr = _Person.ToByteArray();


        //解析
        using(var input = File.OpenRead("Test.txt"))
        {
            //stream 解析
            _Person = Test_Person.Parser.ParseFrom(input);
            //字节串解析
            _Person = Test_Person.Parser.ParseFrom(byteStr);
            //字节数组解析
            _Person = Test_Person.Parser.ParseFrom(byteArr);
            //json解析
            _Person = Test_Person.Parser.ParseJson(json);
        }



        }

    // Update is called once per frame
    void Update()
    {

        
    }
}
