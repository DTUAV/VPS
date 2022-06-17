using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.LidarSensorV3
{
	public class DeapthData : MonoBehaviour
	{
		public Material mat;
		public int width = 512;
		public int height = 512;
		public Texture2D depthImage;
		public Camera depth_cam;
		private RenderTexture rt;
		public Texture2D image;
		public float nearPlanes;
		public float farPlanes;
		void Start()
		{

			depthImage = new Texture2D(width, height, TextureFormat.RGBA32, false);
			depth_cam.depthTextureMode = DepthTextureMode.Depth;

			rt = new RenderTexture(width, height, 24);  // 24 bit depth
			depth_cam.targetTexture = rt;
			nearPlanes = depth_cam.nearClipPlane;
			farPlanes = depth_cam.farClipPlane;

		}

		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			Graphics.Blit(source, destination, mat);
			RenderTexture currentRT = RenderTexture.active;
			RenderTexture.active = destination;
			image = new Texture2D(destination.width, destination.height, TextureFormat.RGBA32, false);
			image.ReadPixels(new Rect(0, 0, destination.width, destination.height), 0, 0);
			image.Apply();
			RenderTexture.active = currentRT; // restore 
			depthImage = image;

		}
		// Update is called once per frame
		void Update()
		{

		}
	}
}
