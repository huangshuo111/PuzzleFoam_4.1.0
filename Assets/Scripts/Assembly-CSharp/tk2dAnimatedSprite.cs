using System;
using UnityEngine;

[AddComponentMenu("2D Toolkit/Sprite/tk2dAnimatedSprite")]
public class tk2dAnimatedSprite : tk2dSprite
{
	private enum State
	{
		Init = 0,
		Playing = 1,
		Paused = 2
	}

	public delegate void AnimationCompleteDelegate(tk2dAnimatedSprite sprite, int clipId);

	public delegate void AnimationEventDelegate(tk2dAnimatedSprite sprite, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum);

	public tk2dSpriteAnimation anim;

	public int clipId;

	public bool playAutomatically;

	private static State globalState;

	public bool createCollider;

	private tk2dSpriteAnimationClip currentClip;

	private float clipTime;

	private float clipFps = -1f;

	private int previousFrame = -1;

	public AnimationCompleteDelegate animationCompleteDelegate;

	public AnimationEventDelegate animationEventDelegate;

	private State state;

	public static bool g_paused
	{
		get
		{
			return (globalState & State.Paused) != 0;
		}
		set
		{
			globalState = (value ? State.Paused : State.Init);
		}
	}

	public bool Paused
	{
		get
		{
			return (state & State.Paused) != 0;
		}
		set
		{
			if (value)
			{
				state |= State.Paused;
			}
			else
			{
				state &= (State)(-3);
			}
		}
	}

	public tk2dSpriteAnimationClip CurrentClip
	{
		get
		{
			return currentClip;
		}
	}

	public float ClipTimeSeconds
	{
		get
		{
			return (!(clipFps > 0f)) ? (clipTime / currentClip.fps) : (clipTime / clipFps);
		}
	}

	public float ClipFps
	{
		get
		{
			return clipFps;
		}
		set
		{
			if (currentClip != null)
			{
				clipFps = ((!(value > 0f)) ? currentClip.fps : value);
			}
		}
	}

	public bool Playing
	{
		get
		{
			return (state & State.Playing) != 0;
		}
	}

	public static float DefaultFps
	{
		get
		{
			return 0f;
		}
	}

	private new void Start()
	{
		base.Start();
		if (playAutomatically)
		{
			Play(clipId);
		}
	}

	public static tk2dAnimatedSprite AddComponent(GameObject go, tk2dSpriteAnimation anim, int clipId)
	{
		tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = anim.clips[clipId];
		tk2dAnimatedSprite tk2dAnimatedSprite2 = go.AddComponent<tk2dAnimatedSprite>();
		tk2dAnimatedSprite2.SwitchCollectionAndSprite(tk2dSpriteAnimationClip2.frames[0].spriteCollection, tk2dSpriteAnimationClip2.frames[0].spriteId);
		tk2dAnimatedSprite2.anim = anim;
		return tk2dAnimatedSprite2;
	}

	public void Play()
	{
		Play(clipId);
	}

	public void Play(float clipStartTime)
	{
		Play(clipId, clipStartTime);
	}

	public void PlayFromFrame(int frame)
	{
		PlayFromFrame(clipId, frame);
	}

	public void Play(string name)
	{
		int id = ((!anim) ? (-1) : anim.GetClipIdByName(name));
		Play(id);
	}

	public void PlayFromFrame(string name, int frame)
	{
		int id = ((!anim) ? (-1) : anim.GetClipIdByName(name));
		PlayFromFrame(id, frame);
	}

	public void Play(string name, float clipStartTime)
	{
		int num = ((!anim) ? (-1) : anim.GetClipIdByName(name));
		Play(num, clipStartTime);
	}

	public void Stop()
	{
		state &= (State)(-2);
	}

	public void StopAndResetFrame()
	{
		if (currentClip != null)
		{
			SwitchCollectionAndSprite(currentClip.frames[0].spriteCollection, currentClip.frames[0].spriteId);
		}
		Stop();
	}

	[Obsolete]
	public bool isPlaying()
	{
		return Playing;
	}

	public bool IsPlaying(string name)
	{
		return Playing && CurrentClip != null && CurrentClip.name == name;
	}

	public bool IsPlaying(tk2dSpriteAnimationClip clip)
	{
		return Playing && CurrentClip != null && CurrentClip == clip;
	}

	protected override bool NeedBoxCollider()
	{
		return createCollider;
	}

	public int GetClipIdByName(string name)
	{
		return (!anim) ? (-1) : anim.GetClipIdByName(name);
	}

	public tk2dSpriteAnimationClip GetClipByName(string name)
	{
		return (!anim) ? null : anim.GetClipByName(name);
	}

	public void Play(int id)
	{
		Play(id, 0f);
	}

	public void PlayFromFrame(int id, int frame)
	{
		tk2dSpriteAnimationClip tk2dSpriteAnimationClip2 = anim.clips[id];
		Play(id, ((float)frame + 0.001f) / tk2dSpriteAnimationClip2.fps);
	}

	private void WarpClipToLocalTime(tk2dSpriteAnimationClip clip, float time)
	{
		clipTime = time;
		int num = (int)clipTime % clip.frames.Length;
		tk2dSpriteAnimationFrame tk2dSpriteAnimationFrame2 = clip.frames[num];
		SwitchCollectionAndSprite(tk2dSpriteAnimationFrame2.spriteCollection, tk2dSpriteAnimationFrame2.spriteId);
		if (tk2dSpriteAnimationFrame2.triggerEvent && animationEventDelegate != null)
		{
			animationEventDelegate(this, clip, tk2dSpriteAnimationFrame2, num);
		}
		previousFrame = num;
	}

	public void Play(int clipId, float clipStartTime)
	{
		this.clipId = clipId;
		Play(anim.clips[clipId], clipStartTime, DefaultFps);
	}

	public void Play(tk2dSpriteAnimationClip clip, float clipStartTime)
	{
		Play(clip, clipStartTime, DefaultFps);
	}

	public void Play(tk2dSpriteAnimationClip clip, float clipStartTime, float overrideFps)
	{
		if (clip != null)
		{
			state |= State.Playing;
			currentClip = clip;
			clipFps = ((!(overrideFps > 0f)) ? currentClip.fps : overrideFps);
			if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Single || currentClip.frames == null)
			{
				WarpClipToLocalTime(currentClip, 0f);
				state &= (State)(-2);
			}
			else if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame || currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomLoop)
			{
				int num = UnityEngine.Random.Range(0, currentClip.frames.Length);
				WarpClipToLocalTime(currentClip, num);
				if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.RandomFrame)
				{
					previousFrame = -1;
					state &= (State)(-2);
				}
			}
			else
			{
				float num2 = clipStartTime * clipFps;
				if (currentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Once && num2 >= clipFps * (float)currentClip.frames.Length)
				{
					WarpClipToLocalTime(currentClip, currentClip.frames.Length - 1);
					state &= (State)(-2);
				}
				else
				{
					WarpClipToLocalTime(currentClip, num2);
					clipTime = num2;
				}
			}
		}
		else
		{
			OnCompleteAnimation();
			state &= (State)(-2);
		}
	}

	public void Pause()
	{
		state |= State.Paused;
	}

	public void Resume()
	{
		state &= (State)(-3);
	}

	private void OnCompleteAnimation()
	{
		previousFrame = -1;
		if (animationCompleteDelegate != null)
		{
			animationCompleteDelegate(this, clipId);
		}
	}

	public void SetFrame(int currFrame)
	{
		SetFrame(currFrame, true);
	}

	public void SetFrame(int currFrame, bool triggerEvent)
	{
		if (currentClip == null && anim != null)
		{
			currentClip = anim.clips[clipId];
		}
		if (triggerEvent && currentClip != null && currentClip.frames.Length > 0 && currFrame >= 0)
		{
			int num = currFrame % currentClip.frames.Length;
			SetFrameInternal(num);
			ProcessEvents(num - 1, num, 1);
		}
	}

	private void SetFrameInternal(int currFrame)
	{
		if (previousFrame != currFrame)
		{
			SwitchCollectionAndSprite(currentClip.frames[currFrame].spriteCollection, currentClip.frames[currFrame].spriteId);
			previousFrame = currFrame;
		}
	}

	private void ProcessEvents(int start, int last, int direction)
	{
		if (animationEventDelegate == null || start == last)
		{
			return;
		}
		int num = last + direction;
		tk2dSpriteAnimationFrame[] frames = currentClip.frames;
		for (int i = start + direction; i != num; i += direction)
		{
			if (frames[i].triggerEvent)
			{
				animationEventDelegate(this, currentClip, frames[i], i);
			}
		}
	}

	private void LateUpdate()
	{
		UpdateAnimation(Time.deltaTime);
	}

	public void UpdateAnimation(float deltaTime)
	{
		State state = this.state | globalState;
		if (state != State.Playing)
		{
			return;
		}
		clipTime += deltaTime * clipFps;
		int num = previousFrame;
		switch (currentClip.wrapMode)
		{
		case tk2dSpriteAnimationClip.WrapMode.Loop:
		case tk2dSpriteAnimationClip.WrapMode.RandomLoop:
		{
			int num5 = (int)clipTime % currentClip.frames.Length;
			SetFrameInternal(num5);
			if (num5 < num)
			{
				ProcessEvents(num, currentClip.frames.Length - 1, 1);
				ProcessEvents(-1, num5, 1);
			}
			else
			{
				ProcessEvents(num, num5, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.LoopSection:
		{
			int num3 = (int)clipTime;
			int num4 = currentClip.loopStart + (num3 - currentClip.loopStart) % (currentClip.frames.Length - currentClip.loopStart);
			if (num3 >= currentClip.loopStart)
			{
				SetFrameInternal(num4);
				num3 = num4;
				if (num < currentClip.loopStart)
				{
					ProcessEvents(num, currentClip.loopStart - 1, 1);
					ProcessEvents(currentClip.loopStart - 1, num3, 1);
				}
				else if (num3 < num)
				{
					ProcessEvents(num, currentClip.frames.Length - 1, 1);
					ProcessEvents(currentClip.loopStart - 1, num3, 1);
				}
				else
				{
					ProcessEvents(num, num3, 1);
				}
			}
			else
			{
				SetFrameInternal(num3);
				ProcessEvents(num, num3, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.PingPong:
		{
			int num6 = (int)clipTime % (currentClip.frames.Length + currentClip.frames.Length - 2);
			int direction = 1;
			if (num6 >= currentClip.frames.Length)
			{
				num6 = 2 * currentClip.frames.Length - 2 - num6;
				direction = -1;
			}
			if (num6 < num)
			{
				direction = -1;
			}
			SetFrameInternal(num6);
			ProcessEvents(num, num6, direction);
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.Once:
		{
			int num2 = (int)clipTime;
			if (num2 >= currentClip.frames.Length)
			{
				SetFrameInternal(currentClip.frames.Length - 1);
				this.state &= (State)(-2);
				ProcessEvents(num, currentClip.frames.Length - 1, 1);
				OnCompleteAnimation();
			}
			else
			{
				SetFrameInternal(num2);
				ProcessEvents(num, num2, 1);
			}
			break;
		}
		case tk2dSpriteAnimationClip.WrapMode.RandomFrame:
			break;
		}
	}
}
