using UnityEngine;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;

public class transfor_depth_pitch : MonoBehaviour
{
    
    
    // Start is called before the first frame update
    public getDepthImage getDeapthImageData;
    //public Texture2D image;
    public struct ENC
    {
        public double x;
        public double y;
        public double z;
        public double w;
    };

    public struct KD
    {
        public double x;
        public double y;
        public double z;
        public double w;
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
        return (enc.x * kDecodeDot.x + enc.y * kDecodeDot.y + enc.z * kDecodeDot.z + enc.w * kDecodeDot.w);
    }

    void Start()
    {/*
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
        */
    }

    void FixedUpdate()
    {
        if (getDeapthImageData.getDepthImageFlag)
        {
            transfor_piture();
            getDeapthImageData.getDepthImageFlag = false;
        }
    }
    void transfor_piture()
    {
        Mat dst = new Mat(getDeapthImageData.depthImage.height, getDeapthImageData.depthImage.width, CvType.CV_8UC4);
        Utils.texture2DToMat(getDeapthImageData.depthImage, dst);
        Mat t_dst = new Mat(getDeapthImageData.depthImage.height,getDeapthImageData.depthImage.width,CvType.CV_8UC4);
        /*
        for (int i = 0; i < dst.rows(); i++)
        {
            for (int j = 0; j < dst.cols(); j++)
            {
                t_dst.get(i,j)[0] = (dst.get(i,j)[0] / 255) * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes;
                t_dst.get(i, j)[1] = (dst.get(i, j)[1] / 255) * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes;
                t_dst.get(i, j)[2] = (dst.get(i, j)[2] / 255) * (getDeapthImageData.farPlanes - getDeapthImageData.nearPlanes) + getDeapthImageData.nearPlanes;
                t_dst.get(i, j)[3] = 1;
            }
        }
        */
       
        
   // }
        Debug.Log(dst.get(1, 1)[0]);
        //Imgcodecs.imwrite("H:/pitch/test.png", t_dst);
        Imgcodecs.imwrite("H:/pitch/dst.png", dst);
      //  File.WriteAllBytes("H:/pitch/dst.png",dst);
    }

    // Update is called once per frame
   /*
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
    */
}
