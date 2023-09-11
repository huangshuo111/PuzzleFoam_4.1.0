using System.Collections;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class DialogPause : DialogBase
{
	private enum eState
	{
		Exit = 0,
		Retry = 1
	}

	public StagePause stagePause;

	public int stageNo;

	private GameObject[] options = new GameObject[2];

	private eState state_;

	private UILabel targetLabel_;

	private UILabel back_to_map_label_;

	private EventStageInfo eventTbl_;

	public override void OnCreate()
	{
		targetLabel_ = base.transform.Find("Target_Label").GetComponent<UILabel>();
		Transform transform = base.transform.Find("Menu_Button/Label");
		if ((bool)transform)
		{
			back_to_map_label_ = transform.GetComponent<UILabel>();
		}
		options[0] = base.transform.Find("Option_00").gameObject;
		options[1] = base.transform.Find("Option_01").gameObject;
	}

	public void init(StageInfo.CommonInfo info)
	{
		targetLabel_.text = Constant.MessageUtil.getTargetMsg(info, MessageResource.Instance, Constant.MessageUtil.eTargetType.Setup);
	}

	public void setup()
	{
		if (Constant.ParkStage.isParkStage(stageNo))
		{
			options[0].SetActive(false);
			options[1].SetActive(true);
			if ((bool)back_to_map_label_)
			{
				back_to_map_label_.text = MessageResource.Instance.getMessage(9145);
			}
			if (stageNo == 500001 && Bridge.StageData.getClearCount_Park(stageNo) <= 0)
			{
				base.transform.Find("Menu_Button").GetComponent<UIButton>().setEnable(false);
				base.transform.Find("Retry_Button").GetComponent<UIButton>().setEnable(false);
			}
		}
		else if (stageNo >= 20001)
		{
			options[0].SetActive(false);
			options[1].SetActive(true);
		}
		else
		{
			options[0].SetActive(true);
			options[1].SetActive(false);
		}
		PartBase execPart = partManager_.execPart;
		if (execPart is Part_BonusStage)
		{
			base.transform.Find("Retry_Button").GetComponent<UIButton>().setEnable(false);
			targetLabel_.text = MessageResource.Instance.getMessage(4525);
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Option_Button":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogOption optionDialog = dialogManager_.getDialog(DialogManager.eDialog.Option) as DialogOption;
			optionDialog.setup();
			dialogManager_.StartCoroutine(dialogManager_.openDialog(optionDialog));
			break;
		}
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			stagePause.pause = false;
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "Continue_Button":
			Constant.SoundUtil.PlayButtonSE();
			stagePause.pause = false;
			dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "Menu_Button":
			Constant.SoundUtil.PlayButtonSE();
			dialogManager_.StartCoroutine(openCommonDialog(eState.Exit));
			break;
		case "Retry_Button":
			Constant.SoundUtil.PlayButtonSE();
			dialogManager_.StartCoroutine(openCommonDialog(eState.Retry));
			break;
		case "Tutorial_Button":
			Constant.SoundUtil.PlayButtonSE();
			StartCoroutine(OpenHowtoPlayIndex());
			break;
		}
		yield break;
	}

	private IEnumerator OpenHowtoPlayIndex()
	{
		Input.enable = false;
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		StageDataTable stageTbl = dataTable.GetComponent<StageDataTable>();
		Constant.eDownloadDataNo downloadData = Constant.eDownloadDataNo.HowtoPlay_JP;
		int file_num = Constant.outsideFiles_howtoplay_JP.Length;
		if (!ResourceLoader.Instance.isJapanResource())
		{
			downloadData = Constant.eDownloadDataNo.HowtoPlay_EN;
			file_num = Constant.outsideFiles_howtoplay_EN.Length;
		}
		GlobalData.Instance.isResourceDownloading = true;
		ResourceURLData resourceURLData2 = JsonMapper.ToObject<ResourceURLData>(stageTbl.getResourceURLData().text);
		for (int i = 0; i < file_num; i++)
		{
			bool isLast = ((i == file_num - 1) ? true : false);
			if (!stageTbl.isNewestGameResourceData(downloadData, i, resourceURLData2))
			{
				yield return StartCoroutine(stageTbl.downloadGameResource(downloadData, i, isLast, i, file_num));
			}
		}
		resourceURLData2 = null;
		GlobalData.Instance.isResourceDownloading = false;
		DialogHowToPlayIndex dialog = dialogManager_.getDialog(DialogManager.eDialog.HowToPlayIndex) as DialogHowToPlayIndex;
		dialog.isChallenge = false;
		if (stageNo >= 40001)
		{
			dialog.isChallenge = true;
		}
		dialog.setup();
		StartCoroutine(dialogManager_.openDialog(dialog));
		Input.enable = true;
	}

	private IEnumerator OnDecide()
	{
		Hashtable args = new Hashtable { { "StageNo", stageNo } };
		if (state_ == eState.Retry)
		{
			args.Add("IsRetry", true);
		}
		args.Add("IsExit", true);
		bool bParkStage = Constant.ParkStage.isParkStage(stageNo);
		bool bEventStage = isEventStage(stageNo);
		bool bEventFinish = false;
		if (!bParkStage && bEventStage)
		{
			GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			StageDataTable stageTbl = dataTable.GetComponent<StageDataTable>();
			eventTbl_ = stageTbl.getEventData();
			if (eventTbl_ == null)
			{
				bEventFinish = true;
			}
			else
			{
				yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(OnCreateEventSendListWWW, false, true));
				if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success && NetworkMng.Instance.getResultCode() == eResultCode.EventIsNotHolding)
				{
					bEventFinish = true;
				}
			}
		}
		PartManager.ePart next_part = PartManager.ePart.Map;
		if (bParkStage)
		{
			next_part = PartManager.ePart.Park;
		}
		else if (bEventStage && !bEventFinish)
		{
			if (stageNo / Constant.Event.BaseEventStageNo == 1)
			{
				next_part = PartManager.ePart.EventMap;
			}
			else if (stageNo / Constant.Event.BaseEventStageNo == 2)
			{
				next_part = PartManager.ePart.ChallengeMap;
			}
			else if (stageNo / Constant.Event.BaseEventStageNo == 11)
			{
				next_part = PartManager.ePart.CollaborationMap;
			}
		}
		if (bEventFinish)
		{
			args.Clear();
			args.Add("StageNo", Bridge.PlayerData.getCurrentStage());
			args.Add("IsForceSendInactive", true);
		}
		partManager_.requestTransition(next_part, args, FadeMng.eType.AllMask, true);
		if (Sound.Instance.isPlayingSe(Sound.eSe.SE_601_obstacle_ufo_move))
		{
			Sound.Instance.stopSe(Sound.eSe.SE_601_obstacle_ufo_move);
		}
	}

	private IEnumerator openCommonDialog(eState state)
	{
		state_ = state;
		DialogCommon dialog2 = null;
		if (state_ == eState.Exit)
		{
			dialog2 = dialogManager_.getDialog(DialogManager.eDialog.ExitConfirm) as DialogCommon;
			string msg4 = string.Empty;
			PartBase part = partManager_.execPart;
			msg4 = (Constant.ParkStage.isParkStage(stageNo) ? MessageResource.Instance.getMessage(9160) : ((part is Part_BonusStage) ? MessageResource.Instance.getMessage(4524) : ((stageNo <= 110000 || Bridge.StageData.isClear(stageNo)) ? MessageResource.Instance.getMessage(1404) : MessageResource.Instance.getMessage(1404))));
			dialog2.setup(msg4, OnDecide, null, true);
		}
		else
		{
			dialog2 = dialogManager_.getDialog(DialogManager.eDialog.RetryConfirm) as DialogCommon;
			string msg2 = string.Empty;
			msg2 = ((stageNo <= 110000 || Bridge.StageData.isClear(stageNo)) ? MessageResource.Instance.getMessage(1442) : MessageResource.Instance.getMessage(1442));
			dialog2.setup(msg2, OnDecide, null, true);
		}
		yield return StartCoroutine(dialogManager_.openDialog(dialog2));
	}

	private bool isEventStage(int stageNo)
	{
		return stageNo >= 10000;
	}

	private WWW OnCreateEventSendListWWW(Hashtable hash)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		string param = eventTbl_.EventNo.ToString();
		WWWWrap.addGetParameter("eventNo", param);
		return WWWWrap.create("message/eventsendlist/");
	}
}
