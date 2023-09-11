using System.Collections;
using Bridge;
using UnityEngine;

public class DialogPauseBoss : DialogBase
{
	private enum eState
	{
		Exit = 0,
		Retry = 1
	}

	private const int BossTargetBaseMsgNo = 8100;

	public StagePause_Boss stagePause;

	public int stageNo;

	private GameObject option;

	private eState state_;

	private UILabel targetLabel_;

	private EventStageInfo eventTbl_;

	public override void OnCreate()
	{
		targetLabel_ = base.transform.Find("Target_Label").GetComponent<UILabel>();
		option = base.transform.Find("Option_00").gameObject;
		Object.DestroyImmediate(base.transform.Find("Option_01").gameObject);
	}

	public void init(BossStageInfo.BossData data)
	{
		targetLabel_.text = MessageResource.Instance.getMessage(8100 + data.BossType);
	}

	public void setup()
	{
		option.SetActive(true);
		base.transform.Find("Retry_Button").GetComponent<UIButton>().setEnable(false);
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
		case "Tutorial_Button":
		{
			Constant.SoundUtil.PlayButtonSE();
			DialogHowToPlayIndex dialog = dialogManager_.getDialog(DialogManager.eDialog.HowToPlayIndex) as DialogHowToPlayIndex;
			dialog.isChallenge = true;
			dialog.setup();
			StartCoroutine(dialogManager_.openDialog(dialog));
			break;
		}
		}
		yield break;
	}

	private IEnumerator OnDecide()
	{
		Hashtable args = new Hashtable();
		PartManager.ePart next_part2 = PartManager.ePart.Map;
		next_part2 = PartManager.ePart.Map;
		args.Add("StageNo", PlayerData.getCurrentStage());
		args.Add("IsForceSendInactive", true);
		partManager_.requestTransition(next_part2, args, FadeMng.eType.AllMask, true);
		yield break;
	}

	private IEnumerator openCommonDialog(eState state)
	{
		state_ = state;
		DialogCommon dialog2 = null;
		if (state_ == eState.Exit)
		{
			dialog2 = dialogManager_.getDialog(DialogManager.eDialog.ExitConfirm) as DialogCommon;
			string msg3 = string.Empty;
			PartBase part = partManager_.execPart;
			if (part is Part_BonusStage)
			{
				msg3 = MessageResource.Instance.getMessage(4524);
			}
			msg3 = ((!(part is Part_BossStage)) ? MessageResource.Instance.getMessage(1404) : MessageResource.Instance.getMessage(8006));
			dialog2.setup(msg3, OnDecide, null, true);
		}
		else
		{
			dialog2 = dialogManager_.getDialog(DialogManager.eDialog.RetryConfirm) as DialogCommon;
			dialog2.setup(OnDecide, null, true);
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
