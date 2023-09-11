using System;

[Serializable]
public class SpriteAnimationClip
{
	public enum WrapMode
	{
		Loop = 0,
		Once = 1
	}

	public string clipName = "Default";

	public float fps = 30f;

	public int loopStart;

	public WrapMode wrapMode;

	public SpriteAnimationFrame[] frames;
}
