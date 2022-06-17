using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class getDepthImage : MonoBehaviour
{
    public Material mat;
    public int width = 512;
    public int height = 512;
    public Texture2D depthImage;
    public Texture2D image;
    public Camera depth_cam;
    private RenderTexture rt;
    private int image_id = 0;
    public bool getDepthImageFlag = false;
    public float nearPlanes;
    public float farPlanes;
    private bool getDataFlag = false;
    [HideInInspector] public byte[] depthImageData;

    public byte[] depthData;
    private Rect rect;

    void Start()
    {

        depthImage = new Texture2D(width, height, TextureFormat.RGB24, false);
        depth_cam.depthTextureMode = DepthTextureMode.Depth;
        depthImageData = new byte[width * height * 3];
        rt = new RenderTexture(width, height, 24); // 24 bit depth
        depth_cam.targetTexture = rt;
        nearPlanes = depth_cam.nearClipPlane;
        farPlanes = depth_cam.farClipPlane;
        image = new Texture2D(width, height, TextureFormat.RGB24, false);
        rect = new Rect(0, 0, width, height);

    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //if (getDataFlag)
        {
            Graphics.Blit(source, destination, mat);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture.active = destination;
           // image = new Texture2D(destination.width, destination.height, TextureFormat.RGB24, false);
            image.ReadPixels(rect, 0, 0);
            image.Apply();
            RenderTexture.active = currentRT; // restore
            if (getDataFlag)
            {
                depthImage = image;
             
                getDataFlag = false;
            }
            // Debug.Log(image.GetRawTextureData().Length);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!getDepthImageFlag && !getDataFlag)
        {

           // depthImageData = depthImage.GetRawTextureData();
            //  Debug.Log(depthImage.EncodeToJPG(50).Length);
            getDepthImageFlag = true;
            getDataFlag = true;

        }
    }
}

