using System.Collections;
using Bridge;
using TapjoyUnity;
using TnkAd;
using UnityEngine;

public class DialogStageSkip : DialogShortageBase
{
	private UILabel payLabel_;

	private UILabel rubyLabel_;

	private int price_ = -1;

	public bool bPaied_;

	private int stageNo_ = -1;

	private Part_Map partMap_;

	private void Awake()
	{
		payLabel_ = base.transform.Find("window/pay_button/Label").GetComponent<UILabel>();
		rubyLabel_ = base.transform.Find("window/Label_ruby").GetComponent<UILabel>();
		createCB();
	}

	public void setPrice(int stageNo, int price)
	{
		stageNo_ = stageNo;
		price_ = price;
		MessageResource instance = MessageResource.Instance;
		string empty = string.Empty;
		empty = payLabel_.text;
		empty = instance.castCtrlCode(empty, 1, price.ToString("N0"));
		payLabel_.text = empty;
		empty = rubyLabel_.text;
		empty = instance.castCtrlCode(empty, 1, price.ToString("N0"));
		rubyLabel_.text = empty;
		setPart();
	}

	private void setPart()
	{
		PartBase execPart = partManager_.execPart;
		if (execPart.name == "Part_Map")
		{
			partMap_ = execPart as Part_Map;
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "pay_button":
			if (price_ > PlayerData.getJewel())
			{
				yield return StartCoroutine(show(eType.Jewel));
				break;
			}
			Plugin.Instance.buyCompleted("USE_JEWEL_SKIPSTAGE");
			Tapjoy.TrackEvent("Game Item", "Lobby Jewel", "Stage No - " + (stageNo_ + 1), "Skip Stage", "Use Jewel", price_, null, 0L, null, 0L);
			Tapjoy.TrackEvent("Money", "Expense Jewel", "Skip Stage", null, price_);
			GlobalGoogleAnalytics.Instance.LogEvent("Item Lobby Jewel", "Skip Stage", "Stage No - " + (stageNo_ + 1), price_);
			GlobalGoogleAnalytics.Instance.LogEvent("Money", "Expense Jewel", "Skip Stage", price_);
			yield return StartCoroutine(partMap_.Clear(stageNo_));
			yield return StartCoroutine(close());
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}
}
