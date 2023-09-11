using System.Collections;
using Bridge;
using UnityEngine;

public class DebugMenuNetStage : DebugMenuBase
{
	private enum eItem
	{
		Clear = 0,
		EventClear = 1,
		ChallengeClear = 2,
		CollaborationClear = 3,
		Max = 4
	}

	private const int DEFAULT_STAGE_NO = 149;

	private int stageNo_;

	private int eventStageNo_ = 10001;

	private StageDataTable stageTbl_;

	private EventStageInfo eventTbl_;

	private int challengeStageNo_ = 20001;

	private int collaborationStageNo_ = 110001;

	private IEnumerator Start()
	{
		yield return StartCoroutine(Setup(4, "NetStage"));
		if (PlayerData.isInstance())
		{
			stageNo_ = PlayerData.getCurrentStage();
			GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			stageTbl_ = dataTable.GetComponent<StageDataTable>();
			eventTbl_ = stageTbl_.getEventData();
			eventStageNo_ = getCurrentEventStage();
			challengeStageNo_ = getCurrentChallengeStage();
			collaborationStageNo_ = getCurrentCollaborationStage();
		}
	}

	public override void OnDraw()
	{
		DrawItem(0, "Clear : " + stageNo_);
		DrawItem(1, "EventClear : " + eventStageNo_);
		DrawItem(2, "ChallengeClear : " + challengeStageNo_);
		DrawItem(3, "CollaboClear : " + collaborationStageNo_);
	}

	public override void OnExecute()
	{
		if (GlobalData.Instance.getNormalStageNum() <= 0)
		{
			stageNo_ = (int)Vary(0, stageNo_, 1, 0, 149);
		}
		else
		{
			stageNo_ = (int)Vary(0, stageNo_, 1, 0, GlobalData.Instance.getNormalStageNum() - 1);
		}
		if (IsPressCenterButton(0))
		{
			NetworkMng.Instance.setup(null);
			StartCoroutine(NetworkMng.Instance.download(StageClear, true, false));
		}
		eventStageNo_ = (int)Vary(1, eventStageNo_, 1, 10001, 10000 + getEventStageNum() - 1);
		if (IsPressCenterButton(1))
		{
			NetworkMng.Instance.setup(null);
			StartCoroutine(NetworkMng.Instance.download(EventStageClear, true, false));
		}
		challengeStageNo_ = (int)Vary(2, challengeStageNo_, 1, 20001, 20000 + getChallengeStageNum() - 1);
		if (IsPressCenterButton(2))
		{
			NetworkMng.Instance.setup(null);
			StartCoroutine(NetworkMng.Instance.download(ChallengeStageClear, true, false));
		}
		collaborationStageNo_ = (int)Vary(3, collaborationStageNo_, 1, 110001, 110000 + getCollaborationStageNum() - 1);
		if (IsPressCenterButton(3))
		{
			NetworkMng.Instance.setup(null);
			StartCoroutine(NetworkMng.Instance.download(CollaborationStageClear, true, false));
		}
	}

	private WWW StageClear(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string text = string.Empty;
		for (int i = 0; i < stageNo_; i++)
		{
			text += i + 1;
			if (i != stageNo_ - 1)
			{
				text += ",";
			}
		}
		WWWWrap.addPostParameter("stageNos", text);
		return WWWWrap.create("debug/stageclear/");
	}

	private WWW EventStageClear(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string text = string.Empty;
		for (int i = 10001; i <= eventStageNo_; i++)
		{
			text += i;
			if (i != eventStageNo_)
			{
				text += ",";
			}
		}
		WWWWrap.addPostParameter("stageNos", text);
		return WWWWrap.create("debug/stageclear/");
	}

	private int getCurrentEventStage()
	{
		if (eventTbl_ == null)
		{
			return -1;
		}
		if (eventTbl_ == null || eventTbl_.EventNo != 1)
		{
			return 10001;
		}
		int num = eventTbl_.EventNo * 10000 + 1;
		bool flag = true;
		for (int i = 0; i < getEventStageNum(); i++)
		{
			EventStageInfo.Info info = eventTbl_.Infos[i];
			if (EventMenu.isPrevLevelClear(info))
			{
				num = info.Common.StageNo;
				continue;
			}
			flag = false;
			break;
		}
		if (Bridge.StageData.isClear(num))
		{
			num++;
		}
		Debug.Log("currentstage:" + num);
		return num;
	}

	private WWW ChallengeStageClear(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string text = string.Empty;
		for (int i = 20001; i <= challengeStageNo_; i++)
		{
			text += i;
			if (i != challengeStageNo_)
			{
				text += ",";
			}
		}
		WWWWrap.addPostParameter("stageNos", text);
		return WWWWrap.create("debug/stageclear/");
	}

	private WWW CollaborationStageClear(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		string text = string.Empty;
		for (int i = 40001; i < collaborationStageNo_; i++)
		{
			text += i;
			if (i != collaborationStageNo_)
			{
				text += ",";
			}
		}
		WWWWrap.addPostParameter("stageNos", text);
		return WWWWrap.create("debug/stageclear/");
	}

	private int getCurrentChallengeStage()
	{
		if (eventTbl_ == null)
		{
			return -1;
		}
		if (eventTbl_ == null || eventTbl_.EventNo != 2)
		{
			return 20001;
		}
		int num = 20001;
		for (int i = 0; i < getEventStageNum(); i++)
		{
			EventStageInfo.Info info = eventTbl_.Infos[i];
			if (EventMenu.isPrevLevelClear(info))
			{
				num = info.Common.StageNo;
				continue;
			}
			break;
		}
		if (Bridge.StageData.isClear(num))
		{
			num++;
		}
		Debug.Log("currentstage:" + num);
		return num;
	}

	private int getCurrentCollaborationStage()
	{
		if (eventTbl_ == null)
		{
			return -1;
		}
		int num = 110001;
		for (int i = 0; i < getEventStageNum(); i++)
		{
			EventStageInfo.Info info = eventTbl_.Infos[i];
			if (EventMenu.isPrevLevelClear(info))
			{
				num = info.Common.StageNo;
				continue;
			}
			break;
		}
		if (Bridge.StageData.isClear(num))
		{
			num++;
		}
		return num;
	}

	private int getEventStageNum()
	{
		if (GlobalData.Instance.getGameData() == null)
		{
			return 10;
		}
		return GlobalData.Instance.getGameData().eventMaxStageNo % 10000;
	}

	private int getChallengeStageNum()
	{
		if (GlobalData.Instance.getGameData() == null)
		{
			return 10;
		}
		return GlobalData.Instance.getGameData().eventMaxStageNo % 10000;
	}

	private int getCollaborationStageNum()
	{
		if (GlobalData.Instance.getGameData() == null)
		{
			return 10;
		}
		return GlobalData.Instance.getGameData().eventMaxStageNo % 10000;
	}
}
