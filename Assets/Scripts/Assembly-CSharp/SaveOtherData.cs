using UnityEngine;

public class SaveOtherData : SaveDataBase
{
	public enum eFlg
	{
		AllClear = 0,
		TutorialEvent = 1,
		TutorialLineFriend = 2,
		TutorialTreasure = 3,
		AllClearEvent = 4,
		TutorialEventMap = 5,
		TutorialCoinBubble = 6,
		NonUse = 7,
		AllClearChallenge = 8,
		TutorialKeyBubble = 9,
		BossMenu = 10,
		FirstBossOpen = 11,
		RequestBossOpen = 12,
		RequestFirstBossOpen = 13,
		FirstBossOpenKeyGet = 14,
		TutorialCollaboration = 15,
		FirstGachaOpen = 16,
		TutorialStageSkip = 17,
		Morgana = 18,
		TutorialGachaMukCollabo = 19,
		OpenParkAnnounce = 20,
		TutorialMinilenBubbleDrop = 21,
		TutorialMinilenGet = 22,
		TutorialMinilenParkFinished = 23,
		TutorialThanksGet = 24,
		FirstGoPark = 25
	}

	private int flgs_;

	private int stageNo_;

	private int eventStageNo_;

	private int eventStageKeyNo_;

	private int challengeStageNo_;

	private int challengeStageKeyNo_;

	private int collaborationStageNo_;

	private int collaborationStageKeyNo_;

	private SaveInformationData[] infoDatas_ = new SaveInformationData[Constant.InformationSaveMax];

	protected override void OnSetup()
	{
		for (int i = 0; i < infoDatas_.Length; i++)
		{
			infoDatas_[i] = new SaveInformationData(i);
			infoDatas_[i].setup();
		}
		if (!PlayerPrefs.HasKey(SaveKeys.getOtherStageNoKey()))
		{
			saveDefault();
		}
	}

	protected override void OnLoad()
	{
		flgs_ = PlayerPrefs.GetInt(SaveKeys.getOtherFlgDataKey());
		stageNo_ = PlayerPrefs.GetInt(SaveKeys.getOtherStageNoKey());
		eventStageNo_ = PlayerPrefs.GetInt(SaveKeys.getOtherEventStageNoKey());
		eventStageKeyNo_ = PlayerPrefs.GetInt(SaveKeys.getOtherEventKeyStageNoKey());
		challengeStageNo_ = PlayerPrefs.GetInt(SaveKeys.getOtherChallengeStageNoKey());
		challengeStageKeyNo_ = PlayerPrefs.GetInt(SaveKeys.getOtherChallengeKeyStageNoKey());
		collaborationStageNo_ = PlayerPrefs.GetInt(SaveKeys.getOtherCollaborationStageNoKey());
		collaborationStageKeyNo_ = PlayerPrefs.GetInt(SaveKeys.getOtherCollaborationKeyStageNoKey());
		for (int i = 0; i < infoDatas_.Length; i++)
		{
			infoDatas_[i].load();
		}
	}

	protected override void OnReset()
	{
		saveDefault();
	}

	protected override void OnSave()
	{
		PlayerPrefs.SetInt(SaveKeys.getOtherFlgDataKey(), flgs_);
		PlayerPrefs.SetInt(SaveKeys.getOtherStageNoKey(), stageNo_);
		PlayerPrefs.SetInt(SaveKeys.getOtherEventStageNoKey(), eventStageNo_);
		PlayerPrefs.SetInt(SaveKeys.getOtherEventKeyStageNoKey(), eventStageKeyNo_);
		PlayerPrefs.SetInt(SaveKeys.getOtherChallengeStageNoKey(), challengeStageNo_);
		PlayerPrefs.SetInt(SaveKeys.getOtherChallengeKeyStageNoKey(), challengeStageKeyNo_);
		PlayerPrefs.SetInt(SaveKeys.getOtherCollaborationStageNoKey(), collaborationStageNo_);
		PlayerPrefs.SetInt(SaveKeys.getOtherCollaborationKeyStageNoKey(), collaborationStageKeyNo_);
		for (int i = 0; i < infoDatas_.Length; i++)
		{
			infoDatas_[i].save();
		}
	}

	private void saveDefault()
	{
		flgs_ = 0;
		stageNo_ = 0;
		eventStageNo_ = 0;
		eventStageKeyNo_ = 0;
		challengeStageNo_ = 0;
		challengeStageKeyNo_ = 0;
		collaborationStageNo_ = 0;
		collaborationStageKeyNo_ = 0;
		OnSave();
	}

	public void PrefsDefault()
	{
		PlayerPrefs.SetInt(SaveKeys.getOtherFlgDataKey(), 0);
		PlayerPrefs.SetInt(SaveKeys.getOtherStageNoKey(), 0);
		PlayerPrefs.SetInt(SaveKeys.getOtherEventStageNoKey(), 0);
		PlayerPrefs.SetInt(SaveKeys.getOtherEventKeyStageNoKey(), 0);
		PlayerPrefs.SetInt(SaveKeys.getOtherChallengeStageNoKey(), 0);
		PlayerPrefs.SetInt(SaveKeys.getOtherChallengeKeyStageNoKey(), 0);
		PlayerPrefs.SetInt(SaveKeys.getOtherCollaborationStageNoKey(), 0);
		PlayerPrefs.SetInt(SaveKeys.getOtherCollaborationKeyStageNoKey(), 0);
		OnLoad();
	}

	public bool isFlag(eFlg flg)
	{
		return getFlag(flgs_, (int)flg);
	}

	public void setFlag(eFlg flg, bool b)
	{
		setFlag(ref flgs_, (int)flg, b);
	}

	public int getEventStageNo()
	{
		return eventStageNo_;
	}

	public void setEventStageNo(int no)
	{
		eventStageNo_ = no;
	}

	public void resetEventStageNo()
	{
		eventStageNo_ = 0;
	}

	public int getChallengeStageNo()
	{
		return challengeStageNo_;
	}

	public void setChallengeStageNo(int no)
	{
		challengeStageNo_ = no;
	}

	public void resetChallengeStageNo()
	{
		challengeStageNo_ = 0;
	}

	public int getCollaborationStageNo()
	{
		return collaborationStageNo_;
	}

	public void setCollaborationStageNo(int no)
	{
		collaborationStageNo_ = no;
	}

	public void resetCollaborationStageNo()
	{
		collaborationStageNo_ = 0;
	}

	public int getEventStageKeyNo()
	{
		return eventStageKeyNo_;
	}

	public void setEventStageKeyNo(int no)
	{
		eventStageKeyNo_ = no;
	}

	public void resetEventStageKeyNo()
	{
		eventStageKeyNo_ = 0;
	}

	public int getChallengeStageKeyNo()
	{
		return challengeStageKeyNo_;
	}

	public void setChallengeStageKeyNo(int no)
	{
		challengeStageKeyNo_ = no;
	}

	public void resetChallengeStageKeyNo()
	{
		challengeStageKeyNo_ = 0;
	}

	public int getCollaborationStageKeyNo()
	{
		return collaborationStageKeyNo_;
	}

	public void setCollaborationStageKeyNo(int no)
	{
		collaborationStageKeyNo_ = no;
	}

	public void resetCollaborationStageKeyNo()
	{
		collaborationStageKeyNo_ = 0;
	}

	public int getStageNo()
	{
		return stageNo_;
	}

	public void setStageNo(int no)
	{
		stageNo_ = no;
	}

	public void resetStageNo()
	{
		stageNo_ = 0;
	}

	public bool isPlayedStageProd(int stageNo)
	{
		return getStageNo() >= stageNo;
	}

	public bool isPlayedEventStageProd(int stageNo)
	{
		return getEventStageNo() >= stageNo;
	}

	public bool isPlayedChallengeStageProd(int stageNo)
	{
		return getChallengeStageNo() >= stageNo;
	}

	public bool isPlayedCollaborationStageProd(int stageNo)
	{
		return getCollaborationStageNo() >= stageNo;
	}

	public bool isPlayedEventStageKeyProd(int stageNo)
	{
		return getEventStageKeyNo() >= stageNo;
	}

	public bool isPlayedChallengeStageKeyProd(int stageNo)
	{
		return getChallengeStageKeyNo() >= stageNo;
	}

	public bool isPlayedCollaborationStageKeyProd(int stageNo)
	{
		return getCollaborationStageKeyNo() >= stageNo;
	}

	public SaveInformationData getInfoData(int id)
	{
		SaveInformationData saveInformationData = null;
		for (int i = 0; i < infoDatas_.Length; i++)
		{
			if (infoDatas_[i].getID() == id)
			{
				return infoDatas_[i];
			}
			if (saveInformationData == null && infoDatas_[i].isNone())
			{
				saveInformationData = infoDatas_[i];
			}
		}
		return saveInformationData;
	}
}
