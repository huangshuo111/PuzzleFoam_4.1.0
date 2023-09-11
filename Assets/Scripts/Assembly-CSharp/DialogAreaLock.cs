using System.Collections;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class DialogAreaLock : DialogBase
{
	private UILabel mPriceLabel;

	private UILabel mUnlockStarLabel;

	private int mAreaNo;

	private int mFirstStageNo;

	private int mUnlockPrice;

	protected MainMenu mainMenu_;

	private Hashtable args_;

	private bool bForceQuit_;

	private WWW www_;

	public override void OnCreate()
	{
		Transform transform = base.gameObject.transform.Find("window");
		mUnlockStarLabel = transform.transform.Find("Label").GetComponent<UILabel>();
		mPriceLabel = transform.transform.Find("Buypass_Button").Find("Price").GetComponentInChildren<UILabel>();
		mainMenu_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
	}

	public IEnumerator show()
	{
		bForceQuit_ = false;
		Sound.Instance.playSe(Sound.eSe.SE_220_count);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
	}

	private void OnDestroy()
	{
		bForceQuit_ = true;
	}

	public void setup(Area area, ref Hashtable args)
	{
		args_ = args;
		mAreaNo = area.getAreaNo();
		int areaStar = Bridge.StageData.getAreaStar(mAreaNo - 1);
		GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable component = @object.GetComponent<StageDataTable>();
		for (int i = 0; i < component.getStageData().Infos.Length; i++)
		{
			if (component.getInfo(i).Area == mAreaNo)
			{
				mFirstStageNo = i;
				break;
			}
		}
		int entryStars = component.getInfo(mFirstStageNo).Common.EntryStars;
		string message = MessageResource.Instance.getMessage(1490);
		message = MessageResource.Instance.castCtrlCode(message, 1, areaStar.ToString());
		message = MessageResource.Instance.castCtrlCode(message, 2, entryStars.ToString());
		mUnlockStarLabel.text = message;
		mUnlockPrice = component.getInfo(mFirstStageNo).Common.UnlockPrice;
		message = MessageResource.Instance.getMessage(500006);
		message = MessageResource.Instance.castCtrlCode(message, 1, mUnlockPrice.ToString());
		mPriceLabel.text = message;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Buypass_Button":
			Constant.SoundUtil.PlayDecideSE();
			if (GlobalData.Instance.getGameData().buyJewel + GlobalData.Instance.getGameData().bonusJewel < mUnlockPrice)
			{
				DialogJewelShop dialog = dialogManager_.getDialog(DialogManager.eDialog.JewelShop) as DialogJewelShop;
				yield return dialogManager_.StartCoroutine(dialog.show());
			}
			else
			{
				yield return dialogManager_.StartCoroutine(recvUnlock(mFirstStageNo + 1));
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			}
			break;
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			bForceQuit_ = true;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private IEnumerator recvUnlock(int stageNo)
	{
		if (!bForceQuit_)
		{
			www_ = createWWW(stageNo);
			while (www_ != null && !www_.isDone)
			{
				if (bForceQuit_)
				{
					www_.Dispose();
					www_ = null;
					yield break;
				}
				yield return 0;
			}
			if (www_ == null)
			{
				yield break;
			}
			if (NetworkUtility.getResultCode(www_) == eResultCode.Success)
			{
				CommonData commonData = JsonMapper.ToObject<CommonData>(www_.text);
				GlobalData.Instance.getGameData().setCommonData(commonData, false);
				GameData gameData = GlobalData.Instance.getGameData();
				Network.StageData stageData = GlobalData.Instance.getStageData(gameData.progressStageNo);
				stageData.stageStatus = commonData.lastStageStatus;
				gameData.allStageScoreSum = commonData.allStageScoreSum;
				gameData.allStarSum = commonData.allStarSum;
				gameData.allPlayCount = commonData.allPlayCount;
				gameData.allClearCount = commonData.allClearCount;
				gameData.bonusJewel = commonData.bonusJewel;
				gameData.buyJewel = commonData.buyJewel;
				gameData.level = commonData.level;
				gameData.exp = commonData.exp;
				gameData.coin = commonData.coin;
				gameData.heart = commonData.heart;
				gameData.treasureboxNum = commonData.treasureboxNum;
				gameData.progressStageNo = commonData.progressStageNo;
				gameData.heartRecoverySsRemaining = commonData.heartRecoverySsRemaining;
				gameData.progressStageOpen = commonData.progressStageOpen;
				args_.Add("StageNo", mFirstStageNo - 1);
				args_.Add("IsProgressOpen", true);
				args_.Add("IsClear", true);
				args_.Add("IsUnlockArea", true);
				mainMenu_.update();
			}
		}
		www_.Dispose();
		www_ = null;
	}

	private WWW createWWW(int stageNo)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		WWWWrap.addGetParameter("stageNo", stageNo);
		return WWWWrap.create("stage/lock/unlock/");
	}
}
