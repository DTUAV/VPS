using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DTUAVSIM.Sensor.Camera
{
    public class camera_data
    {
        private UnityEngine.Camera _camera;                    //the camera object
        private int _imageWidth;                               //the width of image
        private int _imageHeight;                              //the height of image
        private Texture2D _cameraTextureData;                  //the data of camera texture
        private Rect _rect;                                    //the rect of camera image
        private RenderTexture _rt;                             //the render texture of camera
        private byte[] _cameraImageRawData;                    //the raw data of camera image
        private byte[] _cameraImageJPGDate;                    //the jpg format data of camera image
        private byte[] _cameraImagePNGData;                    //the png format data of camera image
        private byte[] _cameraImageEXRData;                    //the exr format data of camera image
        private byte[] _cameraImageTGAData;                    //the tga format data of camera image
       public camera_data(UnityEngine.Camera camera, int imageWidth, int imageHeight)
        {
            _camera = camera;
            _imageHeight = imageHeight;
            _imageWidth = imageWidth;
            _cameraImageRawData = new byte[_imageWidth * _imageHeight * 3];
            _cameraTextureData = new Texture2D(_imageWidth, _imageHeight, TextureFormat.RGB24, false);
            _rect = new Rect(0, 0, _imageWidth, _imageHeight);
            _camera.targetTexture = new RenderTexture(_imageWidth, _imageHeight, 24);
            _rt = new RenderTexture(_imageWidth, _imageHeight, 24);
            _camera.targetTexture = _rt;
        }

        public void UpdateCameraData()
        {
            RenderTexture.active = _rt;
            _cameraTextureData.ReadPixels(_rect, 0, 0);
            _cameraTextureData.Apply();
            RenderTexture.active = null;

            _cameraImageRawData = _cameraTextureData.GetRawTextureData();
            _cameraImageJPGDate = _cameraTextureData.EncodeToJPG();
           // _cameraImageEXRData = _cameraTextureData.EncodeToEXR();
            _cameraImagePNGData = _cameraTextureData.EncodeToPNG();
            _cameraImageTGAData = _cameraTextureData.EncodeToTGA();
        }

        public byte[] GetCameraImageRawData()
        {
            return _cameraImageRawData;
        }

        public byte[] GetCameraImageJPGData()
        {
            return _cameraImageJPGDate;
        }

        public byte[] GetCameraImageEXRData()
        {
            return _cameraImageEXRData;
        }

        public byte[] GetCameraImagePNGData()
        {
            return _cameraImagePNGData;
        }

        public byte[] GetCameraImageTGAData()
        {
            return _cameraImageTGAData;
        }

    }
}
