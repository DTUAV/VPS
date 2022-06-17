using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RosSharp.RosBridgeClient
{
    public class DepthImagePublisher : UnityPublisher<MessageTypes.Sensor.CompressedImage>
    {
        public getDepthImage Depth_image;
        public string FrameId = "Camera";
        [Range(0, 100)]
        public int qualityLevel = 50;

        private MessageTypes.Sensor.CompressedImage message;
        protected override void Start()
        {
            base.Start();
            InitializeGameObject();
            InitializeMessage();
            Camera.onPostRender += UpdateImage;
           
        }

        private void UpdateImage(Camera _camera)
        {
            
               
                UpdateMessage();
            
        }

        private void InitializeGameObject()
        {
         
        }

        private void InitializeMessage()
        {
            message = new MessageTypes.Sensor.CompressedImage();
            message.header.frame_id = FrameId;
            message.format = "jpeg";

        }
        
        private void UpdateMessage()
        {
            message.header.Update();
            //RenderImage();
            message.data =Depth_image.depthImage.EncodeToJPG(qualityLevel);
           // message.data = texture2D1.EncodeToJPG(qualityLevel);
            Publish(message);
        }

    }
}
