using UnityEngine;

public class BoostItemBase : MonoBehaviour
{
	protected enum eType
	{
		Stage = 0,
		Setup = 1,
		Use = 2
	}

	[SerializeField]
	protected UISprite Icon;

	[SerializeField]
	protected UILabel NumLabel;

	[SerializeField]
	protected GameObject Plate;

	[SerializeField]
	protected eType Type = eType.Setup;

	public Constant.Item.eType item_ = Constant.Item.eType.Invalid;

	protected int num_;

	protected MessageResource msgRes_;

	protected GameObject Noitem_Button_;

	protected virtual void OnAwake()
	{
	}

	private void Awake()
	{
		msgRes_ = MessageResource.Instance;
		OnAwake();
	}

	public virtual void setup(Constant.Item.eType type, int num)
	{
		item_ = type;
		num_ = num;
		if (NumLabel != null)
		{
			NumLabel.gameObject.SetActive(true);
		}
		setSprite(type, num);
		updateNumLabel();
		Icon.MakePixelPerfect();
		if (Noitem_Button_ == null && base.transform.Find("Noitem_Button") != null)
		{
			Noitem_Button_ = base.transform.Find("Noitem_Button").gameObject;
		}
		if (Noitem_Button_ != null)
		{
			Noitem_Button_.SetActive(false);
		}
	}

	public Constant.Item.eType getItemType()
	{
		return item_;
	}

	public int getNum()
	{
		return num_;
	}

	public void setNum(int num)
	{
		setNum(num, true);
	}

	public void setNum(int num, bool bLabelUpdate)
	{
		num_ = num;
		if (bLabelUpdate)
		{
			updateNumLabel();
		}
	}

	public void noneItem()
	{
		Icon.spriteName = getSpriteRootName() + "_999_00";
		Icon.MakePixelPerfect();
		if (NumLabel != null)
		{
			NumLabel.gameObject.SetActive(false);
		}
		if (Plate != null)
		{
			Plate.SetActive(false);
		}
		if (Noitem_Button_ != null)
		{
			Noitem_Button_.SetActive(true);
		}
	}

	public void setSprite(Constant.Item.eType type, int num)
	{
		Icon.spriteName = getSpriteName(type, num);
		if (Plate != null)
		{
			Plate.SetActive(true);
		}
	}

	public void updateNumLabel()
	{
		if (NumLabel == null)
		{
			return;
		}
		if (item_ == Constant.Item.eType.ScoreUp)
		{
			NumLabel.gameObject.SetActive(false);
			return;
		}
		if (msgRes_ == null)
		{
			msgRes_ = MessageResource.Instance;
		}
		if (num_ != -1)
		{
			string empty = string.Empty;
			if (item_ == Constant.Item.eType.BubblePlus)
			{
				empty = msgRes_.getMessage(66);
				empty = msgRes_.castCtrlCode(empty, 1, num_.ToString());
			}
			else if (item_ == Constant.Item.eType.TimePlus)
			{
				empty = msgRes_.getMessage(49);
				empty = msgRes_.castCtrlCode(empty, 1, num_.ToString());
			}
			else
			{
				empty = msgRes_.getMessage(45);
				empty = msgRes_.castCtrlCode(empty, 1, num_.ToString());
			}
			NumLabel.gameObject.SetActive(true);
			NumLabel.text = empty;
		}
		else
		{
			NumLabel.gameObject.SetActive(false);
		}
	}

	public string getSpriteName(Constant.Item.eType type, int num)
	{
		if (type == Constant.Item.eType.LightningG)
		{
			return "sb_item_101_00";
		}
		if (num != -1 && item_ == Constant.Item.eType.ScoreUp)
		{
			string[] obj = new string[5]
			{
				getSpriteRootName(),
				"_",
				null,
				null,
				null
			};
			int num2 = (int)type;
			obj[2] = num2.ToString("D3");
			obj[3] = "_";
			obj[4] = (num - 2).ToString("D2");
			return string.Concat(obj);
		}
		string spriteRootName = getSpriteRootName();
		int num3 = (int)type;
		return spriteRootName + "_" + num3.ToString("D3") + "_00";
	}

	private string getSpriteRootName()
	{
		string result = string.Empty;
		switch (Type)
		{
		case eType.Setup:
			result = "item";
			break;
		case eType.Stage:
			result = "sb_item";
			break;
		case eType.Use:
			result = "st_item";
			break;
		}
		return result;
	}
}
