using System.Collections;
using UnityEngine;

public class DialogNologin : DialogBase
{
	private SimpleMessageDraw simpleMessageDraw_;

	private bool isjewelshop_;

	private bool gototitle_;

	private Part_Title.LoginCB loginCB;

	public bool IsJewelShop
	{
		get
		{
			return isjewelshop_;
		}
		set
		{
			isjewelshop_ = value;
		}
	}

	public bool GoToTitle
	{
		get
		{
			return gototitle_;
		}
		set
		{
			gototitle_ = value;
		}
	}

	private void Awake()
	{
		SimpleMessageDraw[] componentsInChildren = GetComponentsInChildren<SimpleMessageDraw>(true);
		if (componentsInChildren.Length > 0)
		{
			simpleMessageDraw_ = componentsInChildren[0];
		}
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (simpleMessageDraw_ != null)
		{
			simpleMessageDraw_.SetMessage((!isjewelshop_) ? 1484 : 304);
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ConfirmButton":
			Input.enable = false;
			Constant.SoundUtil.PlayDecideSE();
			if (gototitle_)
			{
				PlayerPrefs.SetInt("PolicyFlagSkonec", 0);
				while (PlayerPrefs.GetInt("PolicyFlagSkonec") != 0)
				{
					yield return null;
				}
				GlobalData.Instance.LineID = 0L;
				Part_Title.bStartGame = false;
				partManager_.gotoTitle();
			}
			Input.enable = true;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			if (gototitle_)
			{
				PlayerPrefs.SetInt("PolicyFlagSkonec", 0);
				while (PlayerPrefs.GetInt("PolicyFlagSkonec") != 0)
				{
					yield return null;
				}
				GlobalData.Instance.LineID = 0L;
				Part_Title.bStartGame = false;
				partManager_.gotoTitle();
			}
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	public void setup(int strId)
	{
		if (simpleMessageDraw_ != null)
		{
			simpleMessageDraw_.SetMessage(strId);
		}
	}
}
