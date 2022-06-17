using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MavLink;

public class publisher
{
    private string myname;

    public publisher(string str)
    {
        this.myname = str;
    }

    public event myTestDelegate OnChange;

    public delegate void myTestDelegate(string str);

    public void Raise()
    {
        OnChange(this.myname);
    }

}
