using UnityEngine;

public class StageUseBoostItemParent : MonoBehaviour
{
	private BoostItemBase[] items_;

	public void setup()
	{
		items_ = GetComponentsInChildren<BoostItemBase>(true);
		BoostItemBase[] array = items_;
		foreach (BoostItemBase boostItemBase in array)
		{
			boostItemBase.gameObject.SetActive(false);
		}
	}

	public void setActive(Constant.Item.eType itemType, int num)
	{
		BoostItemBase deactiveSprite = getDeactiveSprite();
		deactiveSprite.setup(itemType, num);
		deactiveSprite.gameObject.SetActive(true);
	}

	public void setDeactive(Constant.Item.eType itemType, int num)
	{
		string spriteName = getSpriteName(itemType, num);
		BoostItemBase[] array = items_;
		foreach (BoostItemBase boostItemBase in array)
		{
			if (boostItemBase.gameObject.activeSelf && boostItemBase.getSpriteName(itemType, num) == spriteName)
			{
				boostItemBase.gameObject.SetActive(false);
				break;
			}
		}
	}

	private string getSpriteName(Constant.Item.eType itemType, int num)
	{
		if (itemType == Constant.Item.eType.ScoreUp)
		{
			int num2 = (int)itemType;
			return "st_item_" + num2.ToString("D3") + num.ToString("D2");
		}
		int num3 = (int)itemType;
		return "st_item_" + num3.ToString("D3") + "_00";
	}

	private BoostItemBase getDeactiveSprite()
	{
		BoostItemBase[] array = items_;
		foreach (BoostItemBase boostItemBase in array)
		{
			if (!boostItemBase.gameObject.activeSelf)
			{
				return boostItemBase;
			}
		}
		return null;
	}

	public BoostItemBase getItem(Constant.Item.eType itemType)
	{
		BoostItemBase[] array = items_;
		foreach (BoostItemBase boostItemBase in array)
		{
			if (boostItemBase.getItemType() == itemType && boostItemBase.gameObject.activeSelf)
			{
				return boostItemBase;
			}
		}
		return null;
	}
}
