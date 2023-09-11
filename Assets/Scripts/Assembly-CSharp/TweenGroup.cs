using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/Tween/Group")]
public class TweenGroup : MonoBehaviour
{
	[SerializeField]
	private string GroupName = string.Empty;

	[SerializeField]
	private List<TweenGroupData> TargetGroup = new List<TweenGroupData>();

	[SerializeField]
	private bool IsAutoPlay;

	private List<UITweener> targetList_ = new List<UITweener>();

	private List<UITweener> tweenList_ = new List<UITweener>();

	private bool isSetuped_;

	private float endTime_;

	private void Awake()
	{
		foreach (TweenGroupData item in TargetGroup)
		{
			if (item.Target == null)
			{
				continue;
			}
			UITweener[] components = item.Target.GetComponents<UITweener>();
			UITweener[] array = components;
			foreach (UITweener uITweener in array)
			{
				if (item.TargetTweenName == uITweener.TweenName)
				{
					targetList_.Add(uITweener);
					endTime_ = Mathf.Max(endTime_, item.Target.delay + item.Target.duration);
				}
			}
		}
		if (IsAutoPlay)
		{
			Play();
		}
		isSetuped_ = true;
	}

	private void OnEnable()
	{
		if (isSetuped_ && IsAutoPlay && !isPlaying())
		{
			Play();
		}
	}

	public string getGroupName()
	{
		return GroupName;
	}

	public bool isPlaying()
	{
		foreach (UITweener item in targetList_)
		{
			if (item.enabled)
			{
				return true;
			}
		}
		return false;
	}

	public void Play()
	{
		tweenList_.Clear();
		for (int i = 0; i < targetList_.Count; i++)
		{
			float delay = targetList_[i].delay;
			int num = -1;
			for (int j = 0; j < tweenList_.Count; j++)
			{
				if (tweenList_[j].delay < delay)
				{
					num = j;
					break;
				}
			}
			if (num < 0)
			{
				tweenList_.Add(targetList_[i]);
			}
			else
			{
				tweenList_.Insert(num, targetList_[i]);
			}
		}
		foreach (UITweener item in tweenList_)
		{
			item.Reset();
			item.Play(true);
		}
	}

	public float getEndTime()
	{
		return endTime_;
	}

	public void Reset()
	{
		foreach (UITweener item in targetList_)
		{
			item.Reset();
		}
	}

	public void allTargetActive()
	{
		setTargetActive(true);
	}

	public void allTargetDeactive()
	{
		setTargetActive(false);
	}

	private void setTargetActive(bool bActive)
	{
		foreach (UITweener item in targetList_)
		{
			item.gameObject.SetActive(bActive);
		}
	}
}
