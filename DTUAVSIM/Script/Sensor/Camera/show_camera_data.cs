using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DTUAVSIM.Sensor.Camera;
public class show_camera_data : MonoBehaviour
{
    public UnityEngine.Camera CameraInstance;

    public int ImageWidth;

    public int ImageHeight;

    private camera_data cameraDataInstace;
    // Start is called before the first frame update
    void Start()
    {
        cameraDataInstace = new camera_data(CameraInstance,ImageWidth,ImageHeight);
    }

    void OnGUI()
    {
        cameraDataInstace.UpdateCameraData();
        byte[] imageDate = cameraDataInstace.GetCameraImageJPGData();
        Texture2D texture2DData = new Texture2D(ImageWidth,ImageHeight);
        texture2DData.LoadImage(imageDate);
        GUI.Label(new Rect(20,34,123,112),texture2DData);
    }
}
