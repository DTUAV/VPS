using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimUnity.Sensor.LidarSensorV2
{
	//[ExecuteInEditMode]
	public class LidarSensorDepthCamera : MonoBehaviour
	{
		public Material material;
		public Camera depthCamera;
		//public float Fov;
		//public int SupersampleScale;

		// public void Awake() {
		// 	GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		// 	GetComponent<Camera>().targetTexture.format = RenderTextureFormat.Depth;
		// }
		void Start()
		{
		depthCamera.depthTextureMode = DepthTextureMode.Depth;
		}
		void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			//material.SetFloat("_Fov", Fov);
			//material.SetFloat("_SupersampleScale", SupersampleScale);
			Graphics.Blit(source, destination, material);
		}
	}
}