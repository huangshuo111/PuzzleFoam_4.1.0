using System;
using System.Collections.Generic;
using UnityEngine;

public class StageBoostItemParent : MonoBehaviour
{
	private StageBoostItem[] items_;

	private void Awake()
	{
		items_ = GetComponentsInChildren<StageBoostItem>(true);
	}

	public void setup(StageInfo.CommonInfo stageInfo, Dictionary<Constant.Item.eType, int> buyItemList)
	{
		if (items_ != null)
		{
			for (int i = 0; i < items_.Length; i++)
			{
				StageBoostItem stageBoostItem = items_[i];
				if (i >= stageInfo.StageItemNum)
				{
					stageBoostItem.gameObject.SetActive(false);
					continue;
				}
				stageBoostItem.gameObject.SetActive(true);
				Constant.Item.eType type = (Constant.Item.eType)stageInfo.StageItems[i].Type;
				if (isBuy(type, buyItemList))
				{
					stageBoostItem.setup(type, stageInfo.StageItems[i].Num, buyItemList[type], true);
				}
				else
				{
					stageBoostItem.setup(type, stageInfo.StageItems[i].Num, 0, false);
				}
				if (type == Constant.Item.eType.Replay)
				{
					stageBoostItem.setStateFixed(true);
					stageBoostItem.disable();
				}
			}
		}
		foreach (Constant.Item.eType key in buyItemList.Keys)
		{
			if (Constant.Item.IsAutoUse(key))
			{
				continue;
			}
			bool flag = false;
			for (int j = 0; j < items_.Length; j++)
			{
				StageBoostItem stageBoostItem2 = items_[j];
				if (stageBoostItem2.gameObject.activeSelf && stageBoostItem2.getItemType() == key)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			for (int k = 0; k < items_.Length; k++)
			{
				StageBoostItem stageBoostItem3 = items_[k];
				if (!stageBoostItem3.gameObject.activeSelf)
				{
					stageBoostItem3.gameObject.SetActive(true);
					stageBoostItem3.setup(key, 0, buyItemList[key], true);
					break;
				}
			}
		}
	}

	public void AddLightningG()
	{
		int num = 1;
		StageBoostItem stageBoostItem = Array.Find(items_, (StageBoostItem item) => item.getItemType() == Constant.Item.eType.LightningG);
		if (stageBoostItem == null)
		{
			for (int i = 0; i < items_.Length; i++)
			{
				if (items_[i].getItemType() == Constant.Item.eType.Invalid)
				{
					stageBoostItem = items_[i];
					break;
				}
			}
		}
		if (stageBoostItem == null)
		{
			Debug.Log("アイコンが不足しているためアイテムを追加できません。");
			return;
		}
		if (stageBoostItem.isBuy())
		{
			num = stageBoostItem.getNum() + 1;
		}
		stageBoostItem.gameObject.SetActive(true);
		stageBoostItem.setup(Constant.Item.eType.LightningG, num);
		stageBoostItem.enable();
		stageBoostItem.buy(num);
	}

	private bool isBuy(Constant.Item.eType itemType, Dictionary<Constant.Item.eType, int> buyItemList)
	{
		foreach (Constant.Item.eType key in buyItemList.Keys)
		{
			if (key == itemType)
			{
				return true;
			}
		}
		return false;
	}

	public StageBoostItem getItem(Constant.Item.eType itemType)
	{
		StageBoostItem[] array = items_;
		foreach (StageBoostItem stageBoostItem in array)
		{
			if (stageBoostItem.getItemType() == itemType)
			{
				return stageBoostItem;
			}
		}
		return null;
	}

	public void disable()
	{
		setEnable(false);
	}

	public void enable()
	{
		setEnable(true);
	}

	private void setEnable(bool bEnable)
	{
		StageBoostItem[] array = items_;
		foreach (StageBoostItem stageBoostItem in array)
		{
			if (bEnable)
			{
				stageBoostItem.enable();
			}
			else
			{
				stageBoostItem.disable();
			}
		}
	}

	public bool isEnable()
	{
		StageBoostItem[] array = items_;
		foreach (StageBoostItem stageBoostItem in array)
		{
			if (stageBoostItem.isEnable())
			{
				return true;
			}
		}
		return false;
	}
}
