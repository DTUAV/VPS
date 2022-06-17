using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SimUnity.Sensor.LidarSensorV1
{
    public class LidarSensorV1 : MonoBehaviour
    {
		public float RotateFrequency = 20;//旋转的频率
		public float SampleFrequency = 20000;//采样的频率

		public int Channels = 64;//通道，即激光的线数
		public float MaximalVerticalFOV = +0.2f;//FOV 
		public float MinimalVerticalFOV = -24.9f;


		public float MeasurementRange = 120f;//激光的测量范围
		public float MeasurementAccuracy = 0.02f;//激光的测量精度

		public List<LaserSliceData> data = new List<LaserSliceData>();
		int CloudWidth;//点云图片的宽度

		public Texture2D lastImage = null;//上个点云图片
		bool imageRendered;//图片是否需要渲染

		public RawImage rawImage;//UI界面显示的图片

		void Start()
		{
			CloudWidth = Mathf.RoundToInt(SampleFrequency / RotateFrequency);
			lastImage = new Texture2D(CloudWidth, Channels, TextureFormat.RGB24, false);
			imageRendered = false;

			rawImage.texture = lastImage;
		}

		public bool TryRenderPointCloud(out byte[] image)
		{
			if (imageRendered != false)
			{
				image = lastImage.EncodeToJPG();
				//UnityEngine.Object.DestroyImmediate(lastImage);
				imageRendered = false;
				return true;
			}

			image = null;
			return false;
		}

		void TryComposeTexture()
		{
			while (data.Count >= CloudWidth * 2)//剔除超出范围的数据
			{
				data.RemoveRange(0, CloudWidth);
			}

			if (data.Count >= CloudWidth)
			{
				//lastImage = new Texture2D(CloudWidth, Channels, TextureFormat.RGB24, false);
				for (int i = 0; i < CloudWidth; i++)
				{
					for (int j = 0; j < Channels; j++)
					{
						float val = (data[i].Lasers[j].distance / MeasurementRange);

						val = 2f / (1f + Mathf.Exp(-10f * val)) - 1f;

						lastImage.SetPixel(i, j, new Color(0, val, 0));
					}
				}

				lastImage.Apply();
				imageRendered = true;
			}
		}

		void Update()
		{
			if (data.Count > 0)
			{
				LaserSliceData lastSlice = data[data.Count - 1];
				float lastTimeStamp = lastSlice.Timestamp;

				float deltaTime = (Time.time - lastTimeStamp);

				int sampleCount = Mathf.FloorToInt(SampleFrequency * deltaTime);
				Debug.Log(sampleCount);
				int currIdx = data.Count - 1;
				for (int i = 1; i <= sampleCount; i=+6)
				{
					LaserSliceData temp;
					RenderSlice(Mathf.LerpUnclamped(0, 360, (currIdx + i) / (float)CloudWidth), out temp);
					//RenderSlice(i, out temp);
					data.Add(temp);
				}

			}
			else
			{
				LaserSliceData temp;
				RenderSlice(0, out temp);
				data.Add(temp);
			}

			TryComposeTexture();
		}

		void RenderSlice(float horizontalAngle, out LaserSliceData outSlice)
		{
			LaserData[] lasers = new LaserData[Channels];

			for (int i = 0; i < Channels; i++)
			{
				float verticalAngel = -Mathf.Lerp(MinimalVerticalFOV, MaximalVerticalFOV, (i / (float)(Channels - 1)));

				RaycastHit hit;

				float dist;

				//Debug.LogFormat("verticalAngel : {0}, Vector: {1}", verticalAngel, Quaternion.Euler(verticalAngel, 0, 0) * Vector3.forward);

				Vector3 fwd = transform.TransformDirection(Quaternion.Euler(verticalAngel, horizontalAngle, 0) * Vector3.forward);
				if (Physics.Raycast(transform.position, fwd, out hit, MeasurementRange))
				{
					dist = hit.distance + Random.Range(-MeasurementAccuracy, MeasurementAccuracy);
					dist = Mathf.Clamp(dist, 0, MeasurementRange);

					Debug.DrawLine(transform.position, hit.point, Color.green);
					Debug.DrawLine(hit.point - Vector3.up * 0.3f, hit.point + Vector3.up * 0.3f, Color.red, 0, false);
					Debug.DrawLine(hit.point - Vector3.left * 0.3f, hit.point + Vector3.left * 0.3f, Color.red, 0, false);
					Debug.DrawLine(hit.point - Vector3.forward * 0.3f, hit.point + Vector3.forward * 0.3f, Color.red, 0, false);
				}
				else
				{
					dist = MeasurementRange;
					Debug.DrawRay(transform.position, fwd, Color.gray);
				}

				//Debug.LogFormat(dist.ToString());

				lasers[i] = new LaserData()
				{
					distance = dist,
				};
			}

			LaserSliceData laserSliceData = new LaserSliceData()
			{
				RotationalPosition = horizontalAngle,
				Timestamp = Time.time,
				Lasers = lasers,
			};

			outSlice = laserSliceData;
		}

		public struct LaserData
		{
			public float distance;
			public float intensity;
		}

		public struct LaserSliceData
		{

			public float RotationalPosition;
			public LaserData[] Lasers;
			public float Timestamp;
		}

	}
}
