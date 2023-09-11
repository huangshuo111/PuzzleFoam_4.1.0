using System.Collections;
using UnityEngine;

public class DialogDailyMissionClear : DialogBase
{
	public enum eTargetPart
	{
		NormalMap = 0,
		EventMap = 1,
		ChallengeMap = 2,
		CollaborationMap = 3,
		ParkMap = 4
	}

	private UILabel label;

	private Part_Map _mapPart;

	private Part_EventMap _eventPart;

	private Part_ChallengeMap _challengePart;

	private Part_CollaborationMap _collaboPart;

	private Part_Park _parkPart;

	public bool toBonus;

	private bool bButtonEnable_ = true;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public override void OnCreate()
	{
		Transform transform = base.transform.Find("window/Label01");
		if (transform != null)
		{
			label = transform.GetComponent<UILabel>();
		}
	}

	public void setup(PartBase part, eTargetPart target)
	{
		switch (target)
		{
		case eTargetPart.NormalMap:
			_mapPart = (Part_Map)part;
			break;
		case eTargetPart.EventMap:
			_eventPart = (Part_EventMap)part;
			break;
		case eTargetPart.ChallengeMap:
			_challengePart = (Part_ChallengeMap)part;
			break;
		case eTargetPart.CollaborationMap:
			_collaboPart = (Part_CollaborationMap)part;
			break;
		case eTargetPart.ParkMap:
			_parkPart = (Part_Park)part;
			break;
		}
		label.text = string.Empty + MessageResource.Instance.getMessage(4512);
	}

	private IEnumerator OnButton(GameObject trig)
	{
		if (!bButtonEnable_)
		{
			yield break;
		}
		switch (trig.name)
		{
		case "Close_Button":
		{
			Constant.SoundUtil.PlayCancelSE();
			bButtonEnable_ = false;
			DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
			dialog.setup(4513, null, null, true);
			dialog.sysLabelEnable(false);
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(dialog));
			while (dialog.isOpen())
			{
				yield return 0;
			}
			dialog.sysLabelEnable(true);
			if (dialog.result_ == DialogCommon.eResult.Cancel)
			{
				bButtonEnable_ = true;
				break;
			}
			if (_mapPart != null)
			{
				yield return StartCoroutine(_mapPart.setBonusGamePlayed(1));
			}
			else if (_eventPart != null)
			{
				yield return StartCoroutine(_eventPart.setBonusGamePlayed(1));
			}
			else if (_challengePart != null)
			{
				yield return StartCoroutine(_challengePart.setBonusGamePlayed(1));
			}
			else if (_collaboPart != null)
			{
				yield return StartCoroutine(_collaboPart.setBonusGamePlayed(1));
			}
			else if (_parkPart != null)
			{
				yield return StartCoroutine(_parkPart.setBonusGamePlayed(1));
			}
			bButtonEnable_ = true;
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
		case "Play_Button":
		{
			Constant.SoundUtil.PlayDecideSE();
			Hashtable args = new Hashtable { { "isReturnPark", false } };
			bButtonEnable_ = false;
			if (_mapPart != null)
			{
				yield return StartCoroutine(_mapPart.setBonusGamePlayed(0));
			}
			else if (_eventPart != null)
			{
				yield return StartCoroutine(_eventPart.setBonusGamePlayed(0));
			}
			else if (_challengePart != null)
			{
				yield return StartCoroutine(_challengePart.setBonusGamePlayed(0));
			}
			else if (_collaboPart != null)
			{
				yield return StartCoroutine(_collaboPart.setBonusGamePlayed(0));
			}
			else if (_parkPart != null)
			{
				yield return StartCoroutine(_parkPart.setBonusGamePlayed(0));
				args["isReturnPark"] = true;
			}
			Sound.Instance.playSe(Sound.eSe.SE_326_heart_break);
			toBonus = true;
			bButtonEnable_ = true;
			partManager_.requestTransition(PartManager.ePart.BonusStage, args, FadeMng.eType.Cutout, true);
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
		}
	}
}
