using System;
using UnityEngine;

[Serializable]
public class tk2dCameraResolutionOverride
{
	public enum AutoScaleMode
	{
		None = 0,
		FitWidth = 1,
		FitHeight = 2,
		FitVisible = 3,
		StretchToFit = 4,
		PixelPerfectFit = 5
	}

	public enum FitMode
	{
		Constant = 0,
		Center = 1
	}

	public string name;

	public int width;

	public int height;

	public float scale = 1f;

	public Vector2 offsetPixels = new Vector2(0f, 0f);

	public AutoScaleMode autoScaleMode;

	public FitMode fitMode;

	public bool Match(int pixelWidth, int pixelHeight)
	{
		return (width == -1 || pixelWidth == width) && (height == -1 || pixelHeight == height);
	}
}
