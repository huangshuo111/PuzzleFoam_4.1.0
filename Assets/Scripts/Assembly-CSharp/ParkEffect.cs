using System.Collections;
using UnityEngine;

public class ParkEffect : ParkObject
{
	public enum eEffectType
	{
		Smoke = 0,
		Twinkle = 1,
		Sleep = 2,
		Max = 3
	}

	private eEffectType effectType_;

	private Animation animation_;

	public override IEnumerator setup(int id)
	{
		objectType_ = eType.Effect;
		yield return StartCoroutine(base.setup(id));
		effectType_ = (eEffectType)id;
		animation_ = GetComponentInChildren<Animation>();
	}

	public override void setupImmediate(int id)
	{
		objectType_ = eType.Effect;
		base.setupImmediate(id);
		effectType_ = (eEffectType)id;
		animation_ = GetComponentInChildren<Animation>();
	}

	public void Play()
	{
		base.gameObject.SetActive(true);
	}

	public void Stop()
	{
		base.gameObject.SetActive(false);
	}

	public new void setPosition(Vector3 localPosition)
	{
		cachedTransform_.localPosition = localPosition;
	}

	protected override void setObjectDirection(eDirection newDirection)
	{
		if (objectType_ == eType.Effect && effectType_ == eEffectType.Sleep)
		{
			animation_.Stop();
			switch (newDirection)
			{
			case eDirection.Default:
				animation_.Play("eff_sleep_right");
				break;
			case eDirection.Reverse:
				animation_.Play("eff_sleep_left");
				break;
			}
		}
	}
}
