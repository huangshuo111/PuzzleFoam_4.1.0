using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("2D Toolkit/Camera/tk2dCamera")]
public class tk2dCamera : MonoBehaviour
{
	public tk2dCameraResolutionOverride[] resolutionOverride;

	private tk2dCameraResolutionOverride currentResolutionOverride;

	public tk2dCamera inheritSettings;

	public int nativeResolutionWidth = 960;

	public int nativeResolutionHeight = 640;

	public bool enableResolutionOverrides = true;

	[HideInInspector]
	public Camera mainCamera;

	public static tk2dCamera inst;

	[NonSerialized]
	public float orthoSize = 1f;

	public bool viewportClippingEnabled;

	public Vector4 viewportRegion = new Vector4(0f, 0f, 100f, 100f);

	public Camera screenCamera;

	private Vector2 _targetResolution = Vector2.zero;

	private Vector2 _scaledResolution = Vector2.zero;

	private Vector2 _screenOffset = Vector2.zero;

	[NonSerialized]
	public float zoomScale = 1f;

	[HideInInspector]
	public bool forceResolutionInEditor;

	[HideInInspector]
	public Vector2 forceResolution = new Vector2(960f, 640f);

	private Rect _screenExtents;

	private Rect unitRect = new Rect(0f, 0f, 1f, 1f);

	public Vector2 dumbOffset = Vector2.zero;

	public Vector2 ScaledResolution
	{
		get
		{
			return _scaledResolution;
		}
	}

	public Rect ScreenExtents
	{
		get
		{
			return _screenExtents;
		}
	}

	public Vector2 ScreenOffset
	{
		get
		{
			return _screenOffset;
		}
	}

	[Obsolete]
	public Vector2 resolution
	{
		get
		{
			return ScaledResolution;
		}
	}

	public Vector2 TargetResolution
	{
		get
		{
			return _targetResolution;
		}
	}

	private tk2dCamera Settings
	{
		get
		{
			return (!(inheritSettings == null) && !(inheritSettings == this)) ? inheritSettings.Settings : this;
		}
	}

	private void Awake()
	{
		mainCamera = GetComponent<Camera>();
		if (mainCamera != null)
		{
			UpdateCameraMatrix();
		}
		if (!viewportClippingEnabled)
		{
			inst = this;
		}
	}

	private void LateUpdate()
	{
		UpdateCameraMatrix();
	}

	public void UpdateCameraMatrix()
	{
		if (!viewportClippingEnabled)
		{
			inst = this;
		}
		if (!mainCamera.orthographic)
		{
			Debug.LogError("tk2dCamera must be orthographic");
			mainCamera.orthographic = true;
		}
		tk2dCamera settings = Settings;
		bool flag = viewportClippingEnabled && screenCamera != null && screenCamera.rect == unitRect;
		Camera camera = ((!flag) ? mainCamera : screenCamera);
		float pixelWidth = camera.pixelWidth;
		float pixelHeight = camera.pixelHeight;
		_targetResolution = new Vector2(pixelWidth, pixelHeight);
		if (!settings.enableResolutionOverrides)
		{
			currentResolutionOverride = null;
		}
		if (settings.enableResolutionOverrides && (currentResolutionOverride == null || (currentResolutionOverride != null && ((float)currentResolutionOverride.width != pixelWidth || (float)currentResolutionOverride.height != pixelHeight))))
		{
			currentResolutionOverride = null;
			if (settings.resolutionOverride != null)
			{
				tk2dCameraResolutionOverride[] array = settings.resolutionOverride;
				foreach (tk2dCameraResolutionOverride tk2dCameraResolutionOverride2 in array)
				{
					if (tk2dCameraResolutionOverride2.Match((int)pixelWidth, (int)pixelHeight))
					{
						currentResolutionOverride = tk2dCameraResolutionOverride2;
						break;
					}
				}
			}
		}
		Vector2 vector = new Vector2(1f, 1f);
		Vector2 screenOffset = new Vector2(0f, 0f);
		float num = 0f;
		if (currentResolutionOverride != null)
		{
			switch (currentResolutionOverride.autoScaleMode)
			{
			case tk2dCameraResolutionOverride.AutoScaleMode.FitHeight:
				num = pixelHeight / (float)settings.nativeResolutionHeight;
				vector.Set(num, num);
				break;
			case tk2dCameraResolutionOverride.AutoScaleMode.FitWidth:
				num = pixelWidth / (float)settings.nativeResolutionWidth;
				vector.Set(num, num);
				break;
			case tk2dCameraResolutionOverride.AutoScaleMode.FitVisible:
			case tk2dCameraResolutionOverride.AutoScaleMode.PixelPerfectFit:
			{
				float num2 = (float)settings.nativeResolutionWidth / (float)settings.nativeResolutionHeight;
				float num3 = pixelWidth / pixelHeight;
				num = ((!(num3 < num2)) ? (pixelHeight / (float)settings.nativeResolutionHeight) : (pixelWidth / (float)settings.nativeResolutionWidth));
				if (currentResolutionOverride.autoScaleMode == tk2dCameraResolutionOverride.AutoScaleMode.PixelPerfectFit)
				{
					num = ((!(num > 1f)) ? Mathf.Pow(2f, Mathf.Floor(Mathf.Log(num, 2f))) : Mathf.Floor(num));
				}
				vector.Set(num, num);
				break;
			}
			case tk2dCameraResolutionOverride.AutoScaleMode.StretchToFit:
				vector.Set(pixelWidth / (float)settings.nativeResolutionWidth, pixelHeight / (float)settings.nativeResolutionHeight);
				break;
			default:
				num = currentResolutionOverride.scale;
				vector.Set(num, num);
				break;
			}
			vector *= zoomScale;
			if (currentResolutionOverride.autoScaleMode != tk2dCameraResolutionOverride.AutoScaleMode.StretchToFit)
			{
				tk2dCameraResolutionOverride.FitMode fitMode = currentResolutionOverride.fitMode;
				screenOffset = ((fitMode == tk2dCameraResolutionOverride.FitMode.Constant || fitMode != tk2dCameraResolutionOverride.FitMode.Center) ? (-currentResolutionOverride.offsetPixels) : new Vector2(Mathf.Round(((float)settings.nativeResolutionWidth * vector.x - pixelWidth) / 2f), Mathf.Round(((float)settings.nativeResolutionHeight * vector.y - pixelHeight) / 2f)));
			}
		}
		float num4 = screenOffset.x;
		float num5 = screenOffset.y;
		float num6 = pixelWidth + screenOffset.x;
		float num7 = pixelHeight + screenOffset.y;
		if (flag)
		{
			float num8 = (num6 - num4) / vector.x;
			float num9 = (num7 - num5) / vector.y;
			Vector4 vector2 = new Vector4((int)viewportRegion.x, (int)viewportRegion.y, (int)viewportRegion.z, (int)viewportRegion.w);
			float num10 = (0f - screenOffset.x) / pixelWidth + vector2.x / num8;
			float num11 = (0f - screenOffset.y) / pixelHeight + vector2.y / num9;
			float num12 = vector2.z / num8;
			float num13 = vector2.w / num9;
			Rect rect = new Rect(num10, num11, num12, num13);
			if (mainCamera.rect.x != num10 || mainCamera.rect.y != num11 || mainCamera.rect.width != num12 || mainCamera.rect.height != num13)
			{
				mainCamera.rect = rect;
			}
			float num14 = Mathf.Min(1f - rect.x, rect.width);
			float num15 = Mathf.Min(1f - rect.y, rect.height);
			float num16 = vector2.x * vector.x - screenOffset.x;
			float num17 = vector2.y * vector.y - screenOffset.y;
			if (rect.x < 0f)
			{
				num16 += (0f - rect.x) * pixelWidth;
				num14 = rect.x + rect.width;
			}
			if (rect.y < 0f)
			{
				num17 += (0f - rect.y) * pixelHeight;
				num15 = rect.y + rect.height;
			}
			num4 += num16;
			num5 += num17;
			num6 = pixelWidth * num14 + screenOffset.x + num16;
			num7 = pixelHeight * num15 + screenOffset.y + num17;
		}
		else
		{
			mainCamera.rect = new Rect(0f, 0f, 1f, 1f);
		}
		_screenExtents.Set(num4 / vector.x, num7 / vector.y, (num6 - num4) / vector.x, (num5 - num7) / vector.y);
		float farClipPlane = mainCamera.farClipPlane;
		float near = mainCamera.near;
		orthoSize = (num7 - num5) / 2f;
		_scaledResolution = new Vector2((num6 - num4) / vector.x, (num7 - num5) / vector.y);
		_screenOffset = screenOffset;
		bool flag2 = false;
		float num18 = ((Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsWebPlayer && Application.platform != RuntimePlatform.WindowsEditor) ? 0f : 1f);
		float value = 2f / (num6 - num4) * vector.x;
		float value2 = 2f / (num7 - num5) * vector.y;
		float value3 = -2f / (farClipPlane - near);
		float value4 = (0f - (num6 + num4 + num18)) / (num6 - num4);
		float value5 = (0f - (num5 + num7 - num18)) / (num7 - num5);
		float value6 = (0f - (farClipPlane + near)) / (farClipPlane - near);
		Matrix4x4 projectionMatrix = default(Matrix4x4);
		projectionMatrix[0, 0] = value;
		projectionMatrix[0, 1] = 0f;
		projectionMatrix[0, 2] = 0f;
		projectionMatrix[0, 3] = value4;
		projectionMatrix[1, 0] = 0f;
		projectionMatrix[1, 1] = value2;
		projectionMatrix[1, 2] = 0f;
		projectionMatrix[1, 3] = value5;
		projectionMatrix[2, 0] = 0f;
		projectionMatrix[2, 1] = 0f;
		projectionMatrix[2, 2] = value3;
		projectionMatrix[2, 3] = value6;
		projectionMatrix[3, 0] = 0f;
		projectionMatrix[3, 1] = 0f;
		projectionMatrix[3, 2] = 0f;
		projectionMatrix[3, 3] = 1f;
		mainCamera.projectionMatrix = projectionMatrix;
	}
}
