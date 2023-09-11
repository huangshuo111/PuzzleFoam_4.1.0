using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleNavi : MonoBehaviour
{
	private Part_Stage partStage_;

	private Part_BonusStage partBonusStage_;

	private Part_RankingStage partRankingStage_;

	private Dictionary<Bubble.eType, List<Bubble>> bubbleNaviDic_ = new Dictionary<Bubble.eType, List<Bubble>>();

	private List<Bubble> bubbleNavi_;

	private System.Random rand = new System.Random();

	private void Start()
	{
		partStage_ = base.gameObject.GetComponent<Part_Stage>();
		partBonusStage_ = base.gameObject.GetComponent<Part_BonusStage>();
		partRankingStage_ = base.gameObject.GetComponent<Part_RankingStage>();
	}

	private void Update()
	{
	}

	private void settingNavi(List<Bubble> searchedBubbleList, Bubble.eType type)
	{
		bubbleNavi_ = null;
		if (bubbleNaviDic_.ContainsKey(type))
		{
			bubbleNavi_ = bubbleNaviDic_[type];
			return;
		}
		List<List<Bubble>> list = null;
		List<List<Bubble>> list2 = null;
		int[] checkedFlags = null;
		foreach (Bubble searchedBubble in searchedBubbleList)
		{
			if ((partStage_ != null && type != partStage_.convertColorBubble(searchedBubble.type)) || (partBonusStage_ != null && type != partBonusStage_.convertColorBubble(searchedBubble.type)) || (partRankingStage_ != null && type != partRankingStage_.convertColorBubble(searchedBubble.type)))
			{
				continue;
			}
			bool flag = false;
			if (list != null)
			{
				foreach (List<Bubble> item in list)
				{
					if (item.Contains(searchedBubble))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
			if (list2 != null)
			{
				foreach (List<Bubble> item2 in list2)
				{
					if (item2.Contains(searchedBubble))
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					continue;
				}
			}
			List<Bubble> list3 = new List<Bubble>();
			if (checkedFlags == null)
			{
				if (partStage_ != null)
				{
					checkedFlags = new int[partStage_.fieldBubbleList.Count];
				}
				else if (partBonusStage_ != null)
				{
					checkedFlags = new int[partBonusStage_.fieldBubbleList.Count];
				}
				else if (partRankingStage_ != null)
				{
					checkedFlags = new int[partRankingStage_.fieldBubbleList.Count];
				}
			}
			else
			{
				for (int i = 0; i < checkedFlags.Length; i++)
				{
					checkedFlags[i] = 0;
				}
			}
			checkSamColor(searchedBubble, ref checkedFlags, ref list3);
			if (list3.Count < 2)
			{
				continue;
			}
			bool flag2 = false;
			foreach (Bubble item3 in list3)
			{
				if (item3.type >= Bubble.eType.MinusRed && item3.type <= Bubble.eType.MinusBlack)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2)
			{
				if (list == null)
				{
					list = new List<List<Bubble>>();
				}
				list.Add(list3);
			}
			else
			{
				if (list2 == null)
				{
					list2 = new List<List<Bubble>>();
				}
				list2.Add(list3);
			}
		}
		if (list != null && list.Count > 0)
		{
			bubbleNavi_ = list[rand.Next(list.Count)];
		}
		else if (list2 != null && list2.Count > 0)
		{
			bubbleNavi_ = list2[rand.Next(list2.Count)];
		}
		if (bubbleNavi_ != null)
		{
			bubbleNaviDic_.Add(type, bubbleNavi_);
		}
	}

	private void settingNavi(List<Bubble_Boss> searchedBubbleList, Bubble.eType type)
	{
	}

	public void startNavi(List<Bubble> searchedBubbleList, Bubble.eType type)
	{
		if (bubbleNavi_ != null)
		{
			foreach (Bubble item in bubbleNavi_)
			{
				TweenAlpha component = item.transform.Find("AS_spr_bubble/bubble_erase").GetComponent<TweenAlpha>();
				component.Reset();
				component.enabled = false;
			}
		}
		StopAllCoroutines();
		settingNavi(searchedBubbleList, type);
		StartCoroutine(naviRoutine());
		if (partStage_ != null)
		{
			partStage_.guide.setMetal(type == Bubble.eType.Metal);
			partStage_.guide.setCeiling(type == Bubble.eType.Water || type == Bubble.eType.Shine);
		}
		else if (partRankingStage_ != null)
		{
			partRankingStage_.guide.setMetal(type == Bubble.eType.Metal);
			partRankingStage_.guide.setCeiling(type == Bubble.eType.Water || type == Bubble.eType.Shine);
		}
	}

	public void startNavi(List<Bubble_Boss> searchedBubbleList, Bubble.eType type)
	{
		if (bubbleNavi_ != null)
		{
			foreach (Bubble item in bubbleNavi_)
			{
				TweenAlpha component = item.transform.Find("AS_spr_bubble/bubble_erase").GetComponent<TweenAlpha>();
				component.Reset();
				component.enabled = false;
			}
		}
		StopAllCoroutines();
		settingNavi(searchedBubbleList, type);
		StartCoroutine(naviRoutine());
		if (partStage_ != null)
		{
			partStage_.guide.setMetal(type == Bubble.eType.Metal);
			partStage_.guide.setCeiling(type == Bubble.eType.Water || type == Bubble.eType.Shine);
		}
		else if (partRankingStage_ != null)
		{
			partRankingStage_.guide.setMetal(type == Bubble.eType.Metal);
			partRankingStage_.guide.setCeiling(type == Bubble.eType.Water || type == Bubble.eType.Shine);
		}
	}

	private IEnumerator naviRoutine()
	{
		float elapsedTime3 = 0f;
		float waitTime = 5f;
		while (bubbleNavi_ != null)
		{
			elapsedTime3 = 0f;
			while (elapsedTime3 < waitTime)
			{
				if ((partStage_ != null && partStage_.state != Part_Stage.eState.Wait) || (partBonusStage_ != null && partBonusStage_.state != Part_BonusStage.eState.Wait) || (partRankingStage_ != null && partRankingStage_.state != Part_RankingStage.eState.Wait))
				{
					yield return null;
					continue;
				}
				elapsedTime3 += Time.deltaTime;
				if (partStage_ != null)
				{
					yield return partStage_.stagePause.sync();
				}
				else if (partBonusStage_ != null)
				{
					yield return partBonusStage_.stagePause.sync();
				}
				else if (partRankingStage_ != null)
				{
					yield return partRankingStage_.stagePause.sync();
				}
			}
			foreach (Bubble b in bubbleNavi_)
			{
				TweenAlpha ta = b.transform.Find("AS_spr_bubble/bubble_erase").GetComponent<TweenAlpha>();
				waitTime = ta.duration;
				ta.gameObject.SetActive(true);
				ta.Reset();
				ta.Play(true);
			}
			elapsedTime3 = 0f;
			while (elapsedTime3 < waitTime)
			{
				if ((partStage_ != null && partStage_.state != Part_Stage.eState.Wait) || (partBonusStage_ != null && partBonusStage_.state != Part_BonusStage.eState.Wait) || (partRankingStage_ != null && partRankingStage_.state != Part_RankingStage.eState.Wait))
				{
					yield return null;
					continue;
				}
				elapsedTime3 += Time.deltaTime;
				if (partStage_ != null)
				{
					yield return partStage_.stagePause.sync();
				}
				else if (partBonusStage_ != null)
				{
					yield return partBonusStage_.stagePause.sync();
				}
				else if (partRankingStage_ != null)
				{
					yield return partRankingStage_.stagePause.sync();
				}
			}
			waitTime = 7.5f;
		}
	}

	public void stopNavi()
	{
		StopAllCoroutines();
		if (bubbleNavi_ != null)
		{
			foreach (Bubble item in bubbleNavi_)
			{
				if (!(item == null))
				{
					TweenAlpha component = item.transform.Find("AS_spr_bubble/bubble_erase").GetComponent<TweenAlpha>();
					component.Reset();
					component.enabled = false;
				}
			}
			bubbleNavi_.Clear();
			bubbleNavi_ = null;
		}
		bubbleNaviDic_.Clear();
	}

	private void checkSamColor(Bubble me, ref int[] checkedFlags, ref List<Bubble> list)
	{
		if (!me.isColorBubble())
		{
			return;
		}
		List<Bubble> list2 = new List<Bubble>(6);
		List<int> list3 = new List<int>(6);
		Bubble bubble = null;
		int num = 0;
		if (partStage_ != null)
		{
			num = partStage_.fieldBubbleList.Count;
		}
		else if (partBonusStage_ != null)
		{
			num = partBonusStage_.fieldBubbleList.Count;
		}
		else if (partRankingStage_ != null)
		{
			num = partRankingStage_.fieldBubbleList.Count;
		}
		for (int i = 0; i < num; i++)
		{
			if (partStage_ != null)
			{
				bubble = partStage_.fieldBubbleList[i];
			}
			else if (partBonusStage_ != null)
			{
				bubble = partBonusStage_.fieldBubbleList[i];
			}
			else if (partRankingStage_ != null)
			{
				bubble = partRankingStage_.fieldBubbleList[i];
			}
			if (me.GetInstanceID() != bubble.GetInstanceID() && checkedFlags[i] == 0 && bubble.state == Bubble.eState.Field && !bubble.isLocked && !bubble.isFrozen && me.isNearBubble(bubble))
			{
				list3.Add(i);
				list2.Add(bubble);
			}
		}
		List<Bubble> list4 = new List<Bubble>();
		Bubble.eType eType = Bubble.eType.Invalid;
		if (partStage_ != null)
		{
			eType = partStage_.convertColorBubble(me.type);
		}
		else if (partBonusStage_ != null)
		{
			eType = partBonusStage_.convertColorBubble(me.type);
		}
		else if (partRankingStage_ != null)
		{
			eType = partRankingStage_.convertColorBubble(me.type);
		}
		for (int j = 0; j < list2.Count; j++)
		{
			Bubble.eType type = list2[j].type;
			if ((partStage_ != null && eType == partStage_.convertColorBubble(type)) || (partBonusStage_ != null && eType == partBonusStage_.convertColorBubble(type)) || (partRankingStage_ != null && eType == partRankingStage_.convertColorBubble(type)))
			{
				checkedFlags[list3[j]] = 1;
				list4.Add(list2[j]);
				list.Add(list2[j]);
			}
			else
			{
				checkedFlags[list3[j]] = -1;
			}
		}
		for (int k = 0; k < list4.Count; k++)
		{
			checkSamColor(list4[k], ref checkedFlags, ref list);
		}
	}
}
