using System;

[Serializable]
public class tk2dSpriteAnimationClip
{
	public enum WrapMode
	{
		Loop = 0,
		LoopSection = 1,
		Once = 2,
		PingPong = 3,
		RandomFrame = 4,
		RandomLoop = 5,
		Single = 6
	}

	public string name = "Default";

	public tk2dSpriteAnimationFrame[] frames;

	public float fps = 30f;

	public int loopStart;

	public WrapMode wrapMode;

	public bool Empty
	{
		get
		{
			return name.Length == 0 || frames == null || frames.Length == 0;
		}
	}

	public void CopyFrom(tk2dSpriteAnimationClip source)
	{
		name = source.name;
		if (source.frames == null)
		{
			frames = null;
		}
		else
		{
			frames = new tk2dSpriteAnimationFrame[source.frames.Length];
			for (int i = 0; i < frames.Length; i++)
			{
				if (source.frames[i] == null)
				{
					frames[i] = null;
					continue;
				}
				frames[i] = new tk2dSpriteAnimationFrame();
				frames[i].CopyFrom(source.frames[i]);
			}
		}
		fps = source.fps;
		loopStart = source.loopStart;
		wrapMode = source.wrapMode;
		if (wrapMode == WrapMode.Single && frames.Length > 1)
		{
			frames = new tk2dSpriteAnimationFrame[1] { frames[0] };
			Debug.LogError(string.Format("Clip: '{0}' Fixed up frames for WrapMode.Single", name));
		}
	}

	public void Clear()
	{
		name = string.Empty;
		frames = new tk2dSpriteAnimationFrame[0];
		fps = 30f;
		loopStart = 0;
		wrapMode = WrapMode.Loop;
	}
}
