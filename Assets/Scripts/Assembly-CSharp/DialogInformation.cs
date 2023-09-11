using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogInformation : DialogBase
{
	private enum eMargin
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		Max = 4
	}

	public enum eShop
	{
		None = 0,
		Juwel = 1,
		Package = 2,
		Coin = 3,
		Heart = 4,
		Max = 5
	}

	public eShop eshop_;

	protected WebViewObject webViewObject_;

	private int[] margins_ = new int[4] { 170, 245, 95, 95 };

	private UICheckbox checkBox_;

	private bool bPlaySE_;

	private int id_;

	private Dictionary<int, WebViewObject> webViewDic = new Dictionary<int, WebViewObject>();

	public bool bOping;

	public override void OnCreate()
	{
		calcMargine(ref margins_);
		checkBox_ = GetComponentsInChildren<UICheckbox>(true)[0];
		checkBox_.functionName = "OnCheckBox";
		checkBox_.eventReceiver = base.gameObject;
	}

	public void preLoad(string url, int id, int shopId)
	{
		WebViewObject webViewObject = new GameObject("WebViewObject" + id).AddComponent<WebViewObject>();
		webViewObject.Init();
		eshop_ = (eShop)shopId;
		webViewObject.LoadURL(url);
		webViewObject.SetMargins(margins_[2], margins_[0], margins_[3], margins_[1]);
		webViewObject.SetVisibility(false);
		webViewDic.Add(id, webViewObject);
	}

	private IEnumerator SetupShopDialog(string msg)
	{
		Vector3 pos = base.transform.localPosition;
		base.transform.localPosition = new Vector3(pos.x, pos.y, -45f);
		webViewObject_.SetVisibility(false);
		DialogAllShop dialog = dialogManager_.getDialog(DialogManager.eDialog.AllShop) as DialogAllShop;
		yield return StartCoroutine(dialog.show((DialogAllShop.ePanelType)eshop_));
		while (dialog.isOpen())
		{
			yield return 0;
		}
		base.transform.localPosition = pos;
		webViewObject_.SetVisibility(true);
	}

	public IEnumerator show(string url, int id, int shopId)
	{
		eshop_ = (eShop)shopId;
		bool bButton = eshop_ >= eShop.Juwel && eshop_ <= eShop.Heart;
		base.transform.Find("window/ConfirmButton").gameObject.SetActive(!bButton);
		base.transform.Find("window/ConfirmButton_2").gameObject.SetActive(bButton);
		base.transform.Find("window/ShopButton").gameObject.SetActive(bButton);
		bPlaySE_ = false;
		checkBox_.isChecked = false;
		bOping = true;
		id_ = id;
		Sound.Instance.playSe(Sound.eSe.SE_220_count);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		bPlaySE_ = true;
		webViewObject_ = webViewDic[id];
		webViewObject_.SetVisibility(true);
		webViewObject_.EvaluateJS("window.addEventListener('load', function() {\twindow.addEventListener('click', function() {\t\tUnity.call('clicked');\t}, false);}, false);");
		bOping = false;
	}

	private void OnCheckBox(bool bActive)
	{
		if (bPlaySE_)
		{
			Constant.SoundUtil.PlayButtonSE();
		}
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ShopButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(SetupShopDialog("Shop"));
			break;
		case "Close_Button":
			saveInformationDate();
			Constant.SoundUtil.PlayCancelSE();
			closeWebView();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton":
		case "ConfirmButton_2":
			saveInformationDate();
			Constant.SoundUtil.PlayDecideSE();
			closeWebView();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private void saveInformationDate()
	{
		SaveInformationData infoData = SaveData.Instance.getGameData().getOtherData().getInfoData(id_);
		if (checkBox_.isChecked)
		{
			infoData.setID(id_);
			infoData.setDate(DateTime.Now);
		}
		else
		{
			infoData.resetID();
			infoData.resetDate();
		}
		infoData.save();
	}

	protected void closeWebView()
	{
		if (webViewObject_ != null)
		{
			UnityEngine.Object.Destroy(webViewObject_.gameObject);
			webViewObject_ = null;
			webViewDic.Remove(id_);
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause && !(webViewObject_ == null))
		{
			webViewObject_.SetVisibility(!pause);
		}
	}

	public void ForcingShow()
	{
		if (webViewObject_ != null && !dialogManager_.getDialog(DialogManager.eDialog.AllShop).isOpen())
		{
			webViewObject_.SetVisibility(true);
		}
	}

	public void showWebView(bool bShow)
	{
		if (webViewObject_ != null)
		{
			if (bShow && dialogManager_.getActiveDialogNum() > 1 && isOpen())
			{
				bShow = false;
			}
			webViewObject_.SetVisibility(bShow);
		}
	}

	private void calcMargine(ref int[] margins)
	{
		int num = NGUIUtilScalableUIRoot.manualHeight;
		int num2 = NGUIUtilScalableUIRoot.manualWidth;
		float num3 = (float)(Screen.height * NGUIUtilScalableUIRoot.manualWidth) / (float)(Screen.width * NGUIUtilScalableUIRoot.manualHeight);
		if (num3 > 1f)
		{
			num = (int)((float)num * num3);
		}
		if (num3 < 1f)
		{
			num2 = (int)((float)num2 * num3);
		}
		Vector2 zero = Vector2.zero;
		zero.y = (num - NGUIUtilScalableUIRoot.manualHeight) / 2;
		zero.x = (NGUIUtilScalableUIRoot.manualWidth - num2) / 2;
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		Vector2 vector2 = new Vector2(NGUIUtilScalableUIRoot.manualWidth, NGUIUtilScalableUIRoot.manualHeight);
		vector2.y += zero.y * 2f;
		vector2.x += zero.x * 2f;
		Vector2 vector3 = new Vector2(vector.x / vector2.x, vector.y / vector2.y);
		margins[0] = (int)((float)margins_[0] * vector3.y + zero.y * vector3.y);
		margins[1] = (int)((float)margins_[1] * vector3.y + zero.y * vector3.y);
		margins[2] = (int)((float)margins_[2] * vector3.x + zero.x * vector3.x);
		margins[3] = (int)((float)margins_[3] * vector3.x + zero.x * vector3.x);
	}
}
