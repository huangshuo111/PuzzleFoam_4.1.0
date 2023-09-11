using UnityEngine;

public class StageBoostItem : BoostItemBase
{
	private enum eState
	{
		None = 0,
		Enable = 1,
		Disable = 2
	}

	[SerializeField]
	private GameObject PlusIcon;

	private bool bBuy_;

	public int initNum_;

	private UIButton[] buttons_;

	private eState state_;

	private bool bFixed_;

	private UISprite canUseEff_;

	public bool isSetItemFirst;

	public bool bUse;

	protected override void OnAwake()
	{
		base.OnAwake();
		Type = eType.Stage;
		buttons_ = GetComponentsInChildren<UIButton>(true);
		GameObject gameObject = base.transform.Find("item_use").gameObject;
		gameObject.SetActive(true);
		canUseEff_ = gameObject.GetComponent<UISprite>();
		canUseEff_.enabled = false;
	}

	public void setup(Constant.Item.eType item, int initNum, int num, bool bBuy)
	{
		bBuy_ = bBuy;
		initNum_ = initNum;
		base.setup(item, num);
		if ((item == Constant.Item.eType.TimeStop || item == Constant.Item.eType.Vacuum) && bBuy)
		{
			isSetItemFirst = true;
		}
		if (bBuy)
		{
			buy(num);
		}
		else
		{
			reset();
		}
	}

	public bool isBuy()
	{
		return bBuy_;
	}

	public void setStateFixed(bool bFixed)
	{
		bFixed_ = bFixed;
	}

	public void disable()
	{
		UIButton[] array = buttons_;
		foreach (UIButton button in array)
		{
			NGUIUtility.disable(button, false);
		}
		state_ = eState.Disable;
		canUseEff_.enabled = false;
	}

	public void enable()
	{
		if (!bFixed_)
		{
			UIButton[] array = buttons_;
			foreach (UIButton button in array)
			{
				NGUIUtility.enable(button, false);
			}
			state_ = eState.Enable;
			if (bBuy_)
			{
				canUseEff_.enabled = true;
			}
		}
	}

	public bool isEnable()
	{
		return state_ == eState.Enable;
	}

	public void buy()
	{
		buy(initNum_);
	}

	public void buy(int num)
	{
		bBuy_ = true;
		base.setup(item_, num);
		PlusIcon.SetActive(false);
		if (Constant.Item.IsAutoUse(item_))
		{
			if (item_ != Constant.Item.eType.TimeStop && item_ != Constant.Item.eType.Vacuum)
			{
				setStateFixed(true);
				disable();
			}
			else if (!isSetItemFirst)
			{
				setStateFixed(true);
				disable();
			}
			else
			{
				canUseEff_.enabled = true;
			}
		}
		else
		{
			canUseEff_.enabled = true;
		}
	}

	public void reset()
	{
		bBuy_ = false;
		bUse = false;
		base.setup(item_, initNum_);
		NumLabel.gameObject.SetActive(false);
		PlusIcon.SetActive(true);
		if (state_ == eState.Disable)
		{
			disable();
		}
		canUseEff_.enabled = false;
		if (initNum_ == 0)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void use()
	{
		num_--;
		updateNumLabel();
	}

	public void use_setItemFirst()
	{
		bUse = true;
		setStateFixed(true);
		disable();
	}

	public void back()
	{
		if (!bBuy_)
		{
			bBuy_ = true;
			num_ = 0;
			NumLabel.gameObject.SetActive(true);
			PlusIcon.SetActive(false);
			if (state_ == eState.Disable)
			{
				disable();
			}
			else
			{
				enable();
			}
		}
		else
		{
			num_++;
		}
		updateNumLabel();
	}
}
