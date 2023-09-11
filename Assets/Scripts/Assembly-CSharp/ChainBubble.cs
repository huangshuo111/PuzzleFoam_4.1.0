using System.Collections;
using UnityEngine;

public class ChainBubble : BubbleBase
{
	public enum eState
	{
		Field = 0,
		Break = 1,
		Gameover = 2
	}

	private tk2dAnimatedSprite sprite;

	public Part_Stage part;

	public StagePause stagePause;

	public bool mLocked;

	public eState state { get; private set; }

	private void Awake()
	{
		state = eState.Field;
		myTrans = base.transform;
		sprite = myTrans.Find("sprite").GetComponent<tk2dAnimatedSprite>();
		sprite.animationEventDelegate = eventDelegate;
	}

	public void setType(int layerIndex, Bubble.eType newType)
	{
		type = newType;
		string text;
		if (type > Bubble.eType.Blank)
		{
			int num = (int)type;
			text = num.ToString("000");
		}
		else
		{
			int num2 = (int)type;
			text = num2.ToString("00");
		}
		string text2 = text;
		base.name = "chain_" + layerIndex.ToString("00") + "_" + text2;
		sprite.Play("bubble_" + text2);
	}

	public void startBreak()
	{
		if (state != eState.Break)
		{
			state = eState.Break;
			if (myTrans.localPosition.y > 155f)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				StartCoroutine(breakRoutine());
			}
		}
	}

	private IEnumerator breakRoutine()
	{
		myTrans.localPosition += Vector3.back * 0.01f;
		sprite.Play("burst_48");
		while (sprite.IsPlaying(sprite.CurrentClip))
		{
			yield return stagePause.sync();
		}
		Object.Destroy(base.gameObject);
	}

	public void startGameover(int delay)
	{
		state = eState.Gameover;
		StartCoroutine(gameoverRoutine(delay));
	}

	private IEnumerator gameoverRoutine(int delay)
	{
		float waitTime = 0f;
		while (waitTime < 0.01f * (float)delay)
		{
			waitTime += Time.deltaTime;
			yield return stagePause.sync();
		}
		string clipName = "bubble_" + base.name + "_go";
		if (sprite.GetClipByName(clipName) != null)
		{
			sprite.Play(clipName);
		}
	}

	public void resurrection()
	{
		state = eState.Field;
		sprite.Play("bubble_" + base.name);
	}

	public void unlockEffect()
	{
		GameObject gameObject = Object.Instantiate(sprite.gameObject) as GameObject;
		Utility.setParent(gameObject, myTrans.parent.parent, true);
		gameObject.transform.position = sprite.transform.position;
		gameObject.transform.localScale = sprite.transform.localScale;
		DropLock dropLock = gameObject.AddComponent<DropLock>();
		dropLock.part = part;
		dropLock.stagePause = stagePause;
		StartCoroutine(unlockRoutine());
	}

	private IEnumerator unlockRoutine()
	{
		GameObject eff = Object.Instantiate(sprite.gameObject) as GameObject;
		Utility.setParent(eff, sprite.transform.parent, true);
		eff.transform.localPosition += Vector3.back * 0.01f;
		eff.transform.localScale = sprite.transform.localScale;
		tk2dAnimatedSprite spr = eff.GetComponent<tk2dAnimatedSprite>();
		spr.animationEventDelegate = eventDelegate;
		spr.Play("burst_47");
		while (spr.IsPlaying(spr.CurrentClip))
		{
			yield return stagePause.sync();
		}
		Object.Destroy(eff);
	}

	public void attachCollider(Bubble.eType colliderType)
	{
		if (!(GetComponentInChildren<Collider>() != null))
		{
			string prefabName = "ChainColliderHorizon";
			switch (colliderType)
			{
			case Bubble.eType.ChainRightDown:
				prefabName = "ChainColliderRightDown";
				break;
			case Bubble.eType.ChainLeftDown:
				prefabName = "ChainColliderLeftDown";
				break;
			}
			GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", prefabName)) as GameObject;
			gameObject.transform.parent = myTrans;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = Vector3.one;
		}
	}

	private void eventDelegate(tk2dAnimatedSprite sp, tk2dSpriteAnimationClip clip, tk2dSpriteAnimationFrame frame, int frameNum)
	{
		float num = 418f * ((float)frame.eventInt / 100f);
		sp.transform.localScale = new Vector3(num, num, 1f);
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, frame.eventFloat);
	}
}
