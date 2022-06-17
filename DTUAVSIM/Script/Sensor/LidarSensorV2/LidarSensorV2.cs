using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimUnity.Sensor.LidarSensorV2;
namespace SimUnity.Sensor.LidarSensorV2
{
	public class LidarSensorV2 : MonoBehaviour
	{
		public Camera DepthCamera;//一个相机
		public LidarSensorDepthCamera LidarV2DepthCameraObject;//挂载LidarSensorDepthCamera脚本的相机

		public float RotateFrequency = 20;//旋转的频率


		public float SampleFrequency = 20000;//采样频率

		public int Channels = 64;//激光雷达的线束
		public float MaximalVerticalFOV = +0.2f;//垂直方向最大角度
		public float MinimalVerticalFOV = -24.9f;//垂直方向最小角度


		public float MeasurementRange = 120f;//测量的范围
		public float MeasurementAccuracy = 0.02f;//测量的精度

		public int SupersampleScale = 2;//图片缩放

		int CloudWidth;//点云图像的宽度

		//public Queue<Texture2D> imageQueue = new Queue<Texture2D>();
		public Texture2D lastImage;//上一张图像
		public Texture2D nextImage;//当前图像

		public RawImage rawImage;//显示在UI界面的图像

		public Texture2D scaledImage;//缩放的图像

		int nextStartColumns = 0;//下一个开始的列
		int frameRenderCounter = 0;//渲染帧的总数
		int frameActualRenderTimes = 0;//实际渲染帧的时间

		float currCamTheta;//当前相机的
		int maxCamRenderWidth;//渲染相机的最大宽度
		int maxCamRenderHeight;//渲染相机的最大高度

		public bool TryRenderPointCloud(out byte[] image)
		{
			if (lastImage == null)
			{
				image = null;
				return false;
			}

			image = lastImage.EncodeToJPG();//将上一帧图像编码为JPG显示
			lastImage = null;
			return true;
		}

		void Start()
		{

			CloudWidth = Mathf.RoundToInt(SampleFrequency / RotateFrequency);//点云图的宽度由采样频率和旋转频率决定
			lastImage = new Texture2D(CloudWidth, Channels, TextureFormat.RGB24, false);
			nextImage = new Texture2D(CloudWidth, Channels, TextureFormat.RGB24, false);

			currCamTheta = Mathf.Rad2Deg * Mathf.Atan((Mathf.Tan(Mathf.Deg2Rad * DepthCamera.fieldOfView / 2) / Mathf.Sqrt(2f)));
			maxCamRenderWidth = Mathf.FloorToInt((DepthCamera.fieldOfView / 360) * CloudWidth);
			maxCamRenderHeight = Mathf.RoundToInt(2 * currCamTheta * Channels / (MaximalVerticalFOV - MinimalVerticalFOV));
			DepthCamera.targetTexture = new RenderTexture(SupersampleScale * maxCamRenderWidth, SupersampleScale * maxCamRenderHeight, 24);//设置相机的目标texture
			DepthCamera.targetTexture.Create();
			DepthCamera.aspect = 1;

			//LidarV2DepthCameraObject.Fov = DepthCamera.fieldOfView;
			//LidarV2DepthCameraObject.SupersampleScale = SupersampleScale;
		}

		void Update()
		{
			int sampleCount = Mathf.FloorToInt(SampleFrequency * Time.deltaTime);//采样的点跟上一帧运行的实际有关//采样的点数

			// theta is the angle of the diag

			frameRenderCounter = 0;//渲染的帧数
			frameActualRenderTimes = 0;//实际渲染帧的次数
			Render(ref nextImage, ref nextStartColumns, ref sampleCount);

			while (sampleCount > 0)
			{
				nextImage.Apply();
				lastImage = nextImage;
				rawImage.texture = lastImage;
				nextImage = new Texture2D(CloudWidth, Channels, TextureFormat.RGB24, false);
				Render(ref nextImage, ref nextStartColumns, ref sampleCount);
			}

			Debug.LogFormat("DeltaTime:{0}, RenderTimes:{1}, ActualRenderTiems:{2}", Time.deltaTime, frameRenderCounter, frameActualRenderTimes);
		}

		// return successfully rendered fragment width
		void Render(ref Texture2D targetImage, ref int imgHorizontalPixelStart, ref int sampleCount)
		{
			frameRenderCounter++;


			while (maxCamRenderWidth < sampleCount && imgHorizontalPixelStart + maxCamRenderWidth < CloudWidth)//只有采样的点达到点云图像的大小才渲染
			{
				// render a whole camera
				ExecuteRender(ref targetImage, maxCamRenderWidth, ref imgHorizontalPixelStart, ref sampleCount);
			}

			int renderWidth = Mathf.Min(sampleCount, CloudWidth - imgHorizontalPixelStart);
			ExecuteRender(ref targetImage, renderWidth, ref imgHorizontalPixelStart, ref sampleCount);

		}


		void ExecuteRender(ref Texture2D targetImage, int renderWidth, ref int imgHorizontalPixelStart, ref int sampleCount)
		{

			// Rotate Camera to target angle and render
			DepthCamera.transform.localEulerAngles = Vector3.up * Mathf.LerpUnclamped(0, 360, (imgHorizontalPixelStart + 0.5f * renderWidth) / (float)(CloudWidth));
			DepthCamera.Render();

			// copy camera render texture to "readRenderTex"
			Texture2D readRenderTex = new Texture2D(maxCamRenderWidth, maxCamRenderHeight, TextureFormat.RGB24, false);
			RenderTexture.active = DepthCamera.targetTexture;
			readRenderTex.ReadPixels(new Rect(maxCamRenderWidth * (SupersampleScale * 0.5f - 0.5f), maxCamRenderHeight * (SupersampleScale * 0.5f - 0.5f), maxCamRenderWidth, maxCamRenderHeight), 0, 0);
			readRenderTex.Apply();

			// copy texture from "readRenderTex" to related area in "targetImage"
			int srcX = (maxCamRenderWidth - renderWidth) / 2;
			int srcY = Mathf.RoundToInt(maxCamRenderHeight * (MinimalVerticalFOV + currCamTheta) / (currCamTheta + currCamTheta));
			Graphics.CopyTexture(readRenderTex, 0, 0, srcX, srcY, renderWidth, Channels, targetImage, 0, 0, imgHorizontalPixelStart, 0);

			sampleCount -= renderWidth;
			imgHorizontalPixelStart += renderWidth;
			imgHorizontalPixelStart %= CloudWidth;
			frameActualRenderTimes++;
		}
	}
}
