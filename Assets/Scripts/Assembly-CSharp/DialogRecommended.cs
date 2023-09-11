using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DialogRecommended : DialogBase
{
	public enum eBtn
	{
		Close = 0,
		Decide = 1,
		Max = 2
	}

	public enum eText
	{
		Retry = 0,
		Confirm = 1
	}

	public enum eResult
	{
		Unfinished = 0,
		Decide = 1,
		Cancel = 2
	}

	public delegate IEnumerator OnDecideButton();

	public delegate IEnumerator OnCancelButton();

	private UILabel itemnameLabel_;

	private UILabel detailLabel_;

	private UILabel itemnumLabel_;

	private UILabel priceLabel_;

	private UILabel freeNumLabel_;

	private GameObject coinIcon_;

	private GameObject jewelIcon_;

	private GameObject freeIcon_;

	private GameObject ticketIcon_;

	private UISprite itemIconTex_;

	private GameObject free;

	private UISysFontLabel sysLabel_;

	private UIButton[] buttons_ = new UIButton[2];

	private OnDecideButton decideCB_;

	private OnCancelButton cancelCB_;

	private bool bAutoClose_;

	public eResult result_ { get; private set; }

	public override void OnCreate()
	{
		Transform transform = base.transform.Find("window/item/Name_Label");
		if (transform != null)
		{
			itemnameLabel_ = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/item/Num_Label");
		if (transform != null)
		{
			itemnumLabel_ = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/item/Detail_Label");
		if (transform != null)
		{
			detailLabel_ = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/item000/Item_Button/Price_Label");
		if (transform != null)
		{
			priceLabel_ = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/item/Free_Num_Label");
		if (transform != null)
		{
			freeNumLabel_ = transform.GetComponent<UILabel>();
		}
		transform = base.transform.Find("window/item000/Item_Button/Free_Label");
		if (transform != null)
		{
			free = transform.gameObject;
		}
		transform = base.transform.Find("window/Close_Button");
		if (transform != null)
		{
			buttons_[0] = transform.GetComponent<UIButton>();
		}
		transform = base.transform.Find("window/item000/Item_Button");
		if (transform != null)
		{
			buttons_[1] = transform.GetComponent<UIButton>();
		}
		transform = base.transform.Find("window/item000/Item_Button/icon_coin");
		if (transform != null)
		{
			coinIcon_ = transform.gameObject;
		}
		transform = base.transform.Find("window/item000/Item_Button/icon_jewel");
		if (transform != null)
		{
			jewelIcon_ = transform.gameObject;
		}
		transform = base.transform.Find("window/item/Background");
		if (transform != null)
		{
			itemIconTex_ = transform.GetComponent<UISprite>();
		}
		transform = base.transform.Find("window/item/icon_free");
		if (transform != null)
		{
			freeIcon_ = transform.gameObject;
		}
		transform = base.transform.Find("window/item000/Item_Button/icon_freeticket");
		if (transform != null)
		{
			ticketIcon_ = transform.gameObject;
		}
		result_ = eResult.Unfinished;
	}

	public void setItem(StageInfo.Item item_info, int sale_par)
	{
		MessageResource instance = MessageResource.Instance;
		if (itemnameLabel_ != null)
		{
			itemnameLabel_.text = instance.getMessage(item_info.Type - 1 + 1000);
		}
		if (detailLabel_ != null)
		{
			string text = instance.getMessage(item_info.Type - 1 + 1200);
			if (instance.isCtrlCode(text, 1))
			{
				text = instance.castCtrlCode(text, 1, item_info.Num.ToString());
			}
			detailLabel_.text = text;
			if (Utility.getStringLine(text) >= 4)
			{
				detailLabel_.transform.localScale = new Vector3(24f, 24f, 1f);
			}
			else
			{
				detailLabel_.transform.localScale = new Vector3(30f, 30f, 1f);
			}
		}
		Dictionary<Constant.Item.eType, int> dictionary = new Dictionary<Constant.Item.eType, int>();
		dictionary.Clear();
		GameData gameData = GlobalData.Instance.getGameData();
		UserItemList[] userItemList = gameData.userItemList;
		foreach (UserItemList userItemList2 in userItemList)
		{
			dictionary.Add((Constant.Item.eType)userItemList2.itemType, userItemList2.count);
		}
		if (itemnumLabel_ != null)
		{
			updateNumLabel(item_info);
		}
		if (priceLabel_ != null)
		{
			if (item_info.PriceType == 2)
			{
				coinIcon_.SetActive(false);
			}
			else
			{
				jewelIcon_.SetActive(false);
			}
			int num = item_info.Price * sale_par / 100;
			string text2 = ((num <= 1000) ? string.Empty : (num / 1000).ToString());
			string text3 = ((num <= 1000) ? num.ToString() : (text2 + "," + (num % 1000).ToString("000")));
			priceLabel_.text = text3;
		}
		if (dictionary.ContainsKey((Constant.Item.eType)item_info.Type) && dictionary[(Constant.Item.eType)item_info.Type] > 0)
		{
			priceLabel_.gameObject.SetActive(false);
			jewelIcon_.SetActive(false);
			coinIcon_.SetActive(false);
			freeIcon_.SetActive(true);
			ticketIcon_.SetActive(true);
			freeNumLabel_.gameObject.SetActive(true);
			free.SetActive(true);
			freeNumLabel_.text = dictionary[(Constant.Item.eType)item_info.Type].ToString();
			updateNumLabel(item_info);
		}
		if (itemIconTex_ != null)
		{
			itemIconTex_.spriteName = "item_" + item_info.Type.ToString("000") + "_00";
			itemIconTex_.MakePixelPerfect();
		}
	}

	public void setDecideCB(OnDecideButton cb)
	{
		decideCB_ = cb;
	}

	public void setCancelCB(OnCancelButton cb)
	{
		cancelCB_ = cb;
	}

	public void setAutoCloseFlg(bool bFlg)
	{
		bAutoClose_ = bFlg;
	}

	public void setButtonText(eText textType)
	{
	}

	public void setButtonActive(eBtn btn, bool bActive)
	{
		if (!(buttons_[(int)btn] == null))
		{
			buttons_[(int)btn].gameObject.SetActive(bActive);
		}
	}

	public override void OnClose()
	{
		for (int i = 0; i < 2; i++)
		{
			setButtonActive((eBtn)i, true);
		}
		setButtonText(eText.Confirm);
	}

	public void setup(int msgID, OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose)
	{
		setup(MessageResource.Instance.getMessage(msgID), decideCB, cancelCB, bAutoClose);
	}

	public void setup(string msg, OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose)
	{
		setup(decideCB, cancelCB, bAutoClose);
	}

	public void setup(OnDecideButton decideCB, OnCancelButton cancelCB, bool bAutoClose)
	{
		setButtonText(eText.Confirm);
		setAutoCloseFlg(bAutoClose);
		setDecideCB(decideCB);
		setCancelCB(cancelCB);
		result_ = eResult.Unfinished;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			result_ = eResult.Cancel;
			if (cancelCB_ != null)
			{
				yield return dialogManager_.StartCoroutine(cancelCB_());
			}
			if (bAutoClose_)
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		case "Item_Button":
			Constant.SoundUtil.PlayDecideSE();
			result_ = eResult.Decide;
			if (decideCB_ != null)
			{
				yield return dialogManager_.StartCoroutine(decideCB_());
			}
			if (bAutoClose_)
			{
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		}
	}

	public void updateNumLabel(StageInfo.Item item_info)
	{
		if (itemnumLabel_ == null)
		{
			return;
		}
		if (item_info.Type == 6)
		{
			itemnumLabel_.gameObject.SetActive(false);
			return;
		}
		MessageResource instance = MessageResource.Instance;
		int num = item_info.Num;
		if (num != -1)
		{
			string empty = string.Empty;
			if (item_info.Type == 5)
			{
				empty = instance.getMessage(66);
				empty = instance.castCtrlCode(empty, 1, num.ToString());
			}
			else if (item_info.Type == 11)
			{
				empty = instance.getMessage(49);
				empty = instance.castCtrlCode(empty, 1, num.ToString());
			}
			else
			{
				empty = instance.getMessage(45);
				empty = instance.castCtrlCode(empty, 1, num.ToString());
			}
			itemnumLabel_.gameObject.SetActive(true);
			itemnumLabel_.text = empty;
		}
		else
		{
			itemnumLabel_.gameObject.SetActive(false);
		}
	}
}
