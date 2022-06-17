using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;

public class getDeapthData : MonoBehaviour
{
    // Start is called before the first frame update
    public getDepthImage getDeapthImageData;
    //public Texture2D image;
   
    public  struct ENC
    {
      public  double x;
      public  double y;
      public  double z;
      public  double w;
    };

  public  struct KD
    { public  double x;
      public  double y;
      public  double z;
      public  double w;
    };
   public double DecodeFloatRGBA(ENC enc)
    {
        KD kDecodeDot;

        kDecodeDot.x = 1.0;
        kDecodeDot.y = 1 / 255.0;
        kDecodeDot.z = 1 / 65025.0;
        kDecodeDot.w = 1 / 160581375.0;
        return dot(enc, kDecodeDot);
    }

    private double dot(ENC enc, KD kDecodeDot)
    {
        return (enc.x *kDecodeDot.x+enc.y*kDecodeDot.y+enc.z*kDecodeDot.z+enc.w*kDecodeDot.w);
    }

    void Start()
    {
      Mat dst = new Mat(getDeapthImageData.depthImage.height, getDeapthImageData.depthImage.width, CvType.CV_8UC4);
        Utils.texture2DToMat(getDeapthImageData.depthImage, dst);
        
         if (dst == null)
        {
            Debug.Log("ddd");
        }
         else
        {
            Debug.Log("XXXXXXXXXX"+dst.channels());
            
        
                    
          Debug.Log("XXXX" + dst.get(1, 1)[0]+dst.get(1,1)[1]+ dst.get(1, 1)[2] + dst.get(1, 1)[3]);
            ENC a;
            a.x = dst.get(1, 2)[0];
            a.y = dst.get(1, 2)[1];
            a.z = dst.get(1, 2)[2];
            a.w = dst.get(1, 2)[3];

            Debug.Log("aaaa"+(dst.get(1, 2)[0]/255)*(getDeapthImageData.farPlanes-getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(1, 3)[0]/255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(2, 4)[0]/255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(3, 5)[0]/255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(4, 6)[0]/255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(5, 7)[0]/255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);



        }
    }


    // Update is called once per frame
    void Update()
    {
        Mat dst = new Mat(getDeapthImageData.depthImage.height, getDeapthImageData.depthImage.width, CvType.CV_8UC4);
        Utils.texture2DToMat(getDeapthImageData.depthImage, dst);

        if (dst == null)
        {
            Debug.Log("ddd");
        }
        else
        {
            Debug.Log("XXXXXXXXXX" + dst.channels());



            Debug.Log("XXXX" + dst.get(1, 1)[0] + dst.get(1, 1)[1] + dst.get(1, 1)[2] + dst.get(1, 1)[3]);
            ENC a;
            a.x = dst.get(1, 2)[0];
            a.y = dst.get(1, 2)[1];
            a.z = dst.get(1, 2)[2];
            a.w = dst.get(1, 2)[3];

            Debug.Log("aaaa" + (dst.get(1, 2)[0] / 255) * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(1, 3)[0] / 255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(1, 4)[0] / 255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(1, 5)[0] / 255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(1, 6)[0] / 255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
            Debug.Log("aaaa" + dst.get(1, 7)[0] / 255 * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes);
        }
    }
}
