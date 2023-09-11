using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class DialogBossSelect : DialogSetupBase
{
	public class ContentsData
	{
		public int bossType;

		public int level;
	}

	private const int messageNo = 8200;

	private const int steps = 20;

	private const float content_z = -1f;

	private const float width = 224f;

	private GameObject contentsRoot;

	private GameObject[] contents;

	private GameObject toBeContinued;

	private BossListData bossDatas_;

	private BossStageInfo bossData_;

	private StageDataTable dataTable_;

	private List<BossStageInfo.Info> activeBossList_ = new List<BossStageInfo.Info>();

	public override void OnCreate()
	{
		base.OnCreate();
		dataTable_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		contentsRoot = base.transform.Find("DragPanel/contents").gameObject;
	}

	public IEnumerator show(DialogManager dialogManager)
	{
		Input.enable = false;
		dialogManager_ = dialogManager;
		UIDraggablePanel dragPanel = base.transform.Find("DragPanel").GetComponent<UIDraggablePanel>();
		UIGrid grid = base.transform.Find("DragPanel/contents").GetComponent<UIGrid>();
		dragPanel.scale = new Vector3(1f, 0f, 0f);
		BossDataTable bossDataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
		if (bossDataTable != null)
		{
			yield return dialogManager_.StartCoroutine(bossDataTable.download(true, true));
		}
		bossDatas_ = bossDataTable.getBossData();
		activeBossList_.Clear();
		bossData_ = dataTable_.getBossData();
		BossStageInfo.Info[] infos = bossData_.Infos;
		foreach (BossStageInfo.Info data in infos)
		{
			if (data.BossInfo.Active != -1)
			{
				activeBossList_.Add(data);
			}
		}
		if (activeBossList_.Count <= 0)
		{
			Input.enable = true;
			yield break;
		}
		activeBossList_.Sort((BossStageInfo.Info x, BossStageInfo.Info y) => x.BossInfo.BossType - y.BossInfo.BossType);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(getDialogType()));
		int contentsCount = 0;
		for (int k = 0; k < activeBossList_.Count; k++)
		{
			for (int l = 0; l < activeBossList_[k].BossInfo.LevelInfos.Length; l++)
			{
				contentsCount++;
			}
		}
		contents = new GameObject[contentsCount];
		GameObject contentBase = ResourceLoader.Instance.loadGameObject("Prefabs/", "Boss_item");
		int count = 0;
		for (int j = 0; j < activeBossList_.Count; j++)
		{
			for (int m = 0; m < activeBossList_[j].BossInfo.LevelInfos.Length; m++)
			{
				bool isBattle2 = false;
				BossListData.BossLevelData bd = bossDatas_.bossList[j].bossLevelList[m];
				bool bClear = true;
				if (m > 0)
				{
					bClear = bossDatas_.bossList[j].bossLevelList[m - 1].clearCount > 0;
				}
				else if (j > 0)
				{
					bClear = bossDatas_.bossList[j - 1].bossLevelList[0].clearCount > 0;
				}
				isBattle2 = bd.status == 2;
				if (bClear)
				{
					contents[count] = Object.Instantiate(contentBase) as GameObject;
					contents[count].transform.parent = contentsRoot.transform;
					contents[count].transform.localScale = Vector3.one;
					contents[count].transform.localPosition = new Vector3((float)count * 224f, 0f, -1f);
					BossItem item = contents[count].GetComponent<BossItem>();
					item.setup(bd.maxHp, bd.hp, bd.level, isBattle2, activeBossList_[j].BossInfo);
					item.setupButton(base.gameObject);
					count++;
				}
			}
		}
		int rest = contentsCount - count;
		Debug.Log("rest = " + rest);
		if (rest > 0)
		{
			for (int i = 0; i < rest; i++)
			{
				contents[count] = Object.Instantiate(contentBase) as GameObject;
				contents[count].transform.parent = contentsRoot.transform;
				contents[count].transform.localScale = Vector3.one;
				contents[count].transform.localPosition = new Vector3((float)count * 224f, 0f, -1f);
				contents[count].GetComponent<BossItem>().setLockState(true);
				count++;
			}
		}
		else if (AllClearBossStage(bossDatas_))
		{
			toBeContinued = Object.Instantiate(contentBase) as GameObject;
			toBeContinued.transform.parent = contentsRoot.transform;
			toBeContinued.transform.localScale = Vector3.one;
			toBeContinued.transform.localPosition = new Vector3((float)count * 224f, 0f, -1f);
			toBeContinued.GetComponent<BossItem>().setLockState(true);
			toBeContinued.transform.Find("lock/bg_root/bg03").gameObject.SetActive(false);
			toBeContinued.transform.Find("lock/bg_root/bg04").gameObject.SetActive(true);
		}
		dragPanel.ResetPosition();
		grid.Reposition();
		yield return 0;
		Input.enable = true;
	}

	private int getRewardNum(BossStageInfo.LevelInfo levelInfo, int type)
	{
		if (type == 0)
		{
			return levelInfo.RewardNums[3];
		}
		for (int i = 0; i < levelInfo.Rewards.Length; i++)
		{
			if (levelInfo.Rewards[i] == type)
			{
				return levelInfo.RewardNums[i];
			}
		}
		return 0;
	}

	public IEnumerator ContentsDestroy()
	{
		GameObject[] array = contents;
		foreach (GameObject content in array)
		{
			Object.Destroy(content);
		}
		if (toBeContinued != null)
		{
			Object.Destroy(toBeContinued);
		}
		yield return 0;
	}

	public bool AllClearBossStage(BossListData data)
	{
		for (int i = 0; i < data.bossList.Length; i++)
		{
			for (int j = 0; j < bossDatas_.bossList[i].bossLevelList.Length; j++)
			{
				if (bossDatas_.bossList[i].bossLevelList[j].clearCount <= 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.Contains("BossButton"))
		{
			Constant.SoundUtil.PlayButtonSE();
			if (!isKeyShortage())
			{
				BossItem.ContentsData data = trigger.transform.parent.parent.GetComponent<BossItem>().getContentData();
				yield return StartCoroutine(ContentsDestroy());
				yield return dialogManager_.StartCoroutine(dialogManager_.openBossSetDialog(this, data.bossType, data.level));
			}
			else
			{
				dialogManager_.StartCoroutine(playBossTutorial());
			}
		}
		else
		{
			switch (trigger.name)
			{
			case "Close_Button":
				Constant.SoundUtil.PlayCancelSE();
				yield return StartCoroutine(ContentsDestroy());
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
				break;
			}
		}
	}

	public bool isKeyShortage()
	{
		KeyBubbleData keyBubbleData = GlobalData.Instance.getKeyBubbleData();
		return keyBubbleData.keyBubbleCount < keyBubbleData.keyBubbleMax;
	}

	private IEnumerator playBossTutorial()
	{
		GameObject uiRoot = dialogManager_.getCurrentUiRoot();
		TutorialManager.Instance.load(-13, uiRoot);
		int tutorialStageNo = -13;
		TutorialManager.Instance.SetWindowDepth(-65f);
		yield return StartCoroutine(TutorialManager.Instance.play(tutorialStageNo, TutorialDataTable.ePlace.Setup, uiRoot, null, null));
		TutorialManager.Instance.SetWindowDepth(-45f);
		TutorialManager.Instance.unload(-13);
	}
}
