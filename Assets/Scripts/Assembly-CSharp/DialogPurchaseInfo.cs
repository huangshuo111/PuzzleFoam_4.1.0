using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class DialogPurchaseInfo : DialogBase
{
	public IEnumerator show()
	{
		Input.enable = false;
		NetworkMng.Instance.setup(null);
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			Input.enable = true;
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		PurchaseInfo purchaseInfo = JsonMapper.ToObject<PurchaseInfo>(www.text);
		MessageResource msgRes = MessageResource.Instance;
		UILabel[] labels = base.transform.Find("window/texts").GetComponentsInChildren<UILabel>(true);
		UILabel[] array = labels;
		foreach (UILabel label in array)
		{
			switch (label.name)
			{
			case "00_jewelLabel_charge":
				Object.Destroy(label.GetComponent<SimpleMessageDraw>());
				label.text = msgRes.castCtrlCode(msgRes.getMessage(2492), 1, purchaseInfo.buyJewel.ToString());
				break;
			case "00_jewelLabel_free":
				Object.Destroy(label.GetComponent<SimpleMessageDraw>());
				label.text = msgRes.castCtrlCode(msgRes.getMessage(2493), 1, purchaseInfo.bonusJewel.ToString());
				break;
			case "01_coinLabel_charge":
				Object.Destroy(label.GetComponent<SimpleMessageDraw>());
				label.text = msgRes.castCtrlCode(msgRes.getMessage(2492), 1, purchaseInfo.paidCoin.ToString());
				break;
			case "01_coinLabel_free":
				Object.Destroy(label.GetComponent<SimpleMessageDraw>());
				label.text = msgRes.castCtrlCode(msgRes.getMessage(2493), 1, purchaseInfo.freeCoin.ToString());
				break;
			case "02_heartLabel_charge":
				Object.Destroy(label.GetComponent<SimpleMessageDraw>());
				label.text = msgRes.castCtrlCode(msgRes.getMessage(2492), 1, purchaseInfo.paidHeart.ToString());
				break;
			case "02_heartLabel_free":
				Object.Destroy(label.GetComponent<SimpleMessageDraw>());
				label.text = msgRes.castCtrlCode(msgRes.getMessage(2493), 1, purchaseInfo.freeHeart.ToString());
				break;
			}
		}
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		return WWWWrap.create("player/purchaseinfo/");
	}
}
