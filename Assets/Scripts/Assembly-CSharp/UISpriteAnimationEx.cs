using UnityEngine;

[AddComponentMenu("NGUI/UI/Sprite Animation Ex")]
[RequireComponent(typeof(UISprite))]
public class UISpriteAnimationEx : MonoBehaviour
{
	private int mClipIndex;

	public int clipIndex;

	public SpriteAnimationClip[] clips;

	private UISprite mSprite;

	private float mDelta;

	private int mIndex;

	private int mCount;

	private bool mActive = true;

	public bool isPlaying
	{
		get
		{
			return mActive;
		}
	}

	private void Start()
	{
		mSprite = GetComponent<UISprite>();
		SetClip(mClipIndex);
	}

	private void Update()
	{
		if (clips == null)
		{
			return;
		}
		if (mSprite == null)
		{
			mSprite = GetComponent<UISprite>();
			if (mSprite == null)
			{
				return;
			}
		}
		if (clipIndex != mClipIndex)
		{
			clipIndex = Mathf.Clamp(clipIndex, 0, clips.Length - 1);
			SetClip(clipIndex);
		}
		if (!mActive || clips.Length <= 0 || !Application.isPlaying)
		{
			return;
		}
		SpriteAnimationClip spriteAnimationClip = clips[mClipIndex];
		mDelta += Time.deltaTime;
		float num = 1f / spriteAnimationClip.fps;
		if (!(num < mDelta))
		{
			return;
		}
		mDelta = ((!(num > 0f)) ? 0f : (mDelta - num));
		if (++mCount >= spriteAnimationClip.frames[mIndex].frame)
		{
			mCount = 0;
			if (++mIndex >= spriteAnimationClip.frames.Length)
			{
				mIndex = spriteAnimationClip.loopStart;
				mActive = spriteAnimationClip.wrapMode != SpriteAnimationClip.WrapMode.Once;
			}
			if (mActive)
			{
				mSprite.spriteName = spriteAnimationClip.frames[mIndex].spriteName;
				mSprite.MakePixelPerfect();
			}
		}
	}

	public void SetClip(int index)
	{
		if (!(mSprite == null) && clips != null && index < clips.Length)
		{
			mClipIndex = index;
			mDelta = 0f;
			mCount = 0;
			mIndex = 0;
			if (mIndex < clips[mClipIndex].frames.Length)
			{
				mActive = true;
				mSprite.spriteName = clips[mClipIndex].frames[mIndex].spriteName;
				mSprite.MakePixelPerfect();
			}
		}
	}

	public void SetClip(string name)
	{
		if (clips == null)
		{
			return;
		}
		for (int i = 0; i < clips.Length; i++)
		{
			if (clips[i].clipName == name)
			{
				clipIndex = i;
				SetClip(clipIndex);
				break;
			}
		}
	}
}
