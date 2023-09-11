using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class DialogHowToPlayIndex : DialogScrollListBase
{
	private List<int> indexList_ = new List<int>();

	public bool isChallenge;

	public override void OnCreate()
	{
		base.OnCreate();
		createLine(-40f);
		GameObject gameObject = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "HowToPlay_item")) as GameObject;
		Utility.setParent(gameObject, base.transform, false);
		gameObject.SetActive(false);
		base.init(gameObject);
	}

	public virtual void setup()
	{
		indexList_.Clear();
		TutorialManager instance = TutorialManager.Instance;
		int num = Bridge.PlayerData.getCurrentStage() + 1;
		bool flag = false;
		if (partManager_.currentPart == PartManager.ePart.Park)
		{
			flag = true;
		}
		else if (partManager_.currentPart == PartManager.ePart.Stage)
		{
			Part_Stage part_Stage = Object.FindObjectOfType<Part_Stage>();
			if ((bool)part_Stage && part_Stage.isParkStage)
			{
				flag = true;
				isChallenge = true;
			}
		}
		int[] hoToPlayPageClearStages = instance.HoToPlayPageClearStages;
		foreach (int num2 in hoToPlayPageClearStages)
		{
			if (!isChallenge)
			{
				if (num >= num2)
				{
					addHowToPlay(indexList_.Count);
				}
			}
			else
			{
				addHowToPlay(indexList_.Count);
			}
		}
		SaveOtherData otherData = SaveData.Instance.getGameData().getOtherData();
		if (!isChallenge)
		{
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialTreasure))
			{
				addHowToPlay(400);
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialLineFriend))
			{
				addHowToPlay(401);
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialEvent))
			{
				addHowToPlay(402);
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialEventMap))
			{
				addHowToPlay(403);
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialCoinBubble))
			{
				addHowToPlay(404);
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialKeyBubble))
			{
				addHowToPlay(405);
			}
			if (otherData.isFlag(SaveOtherData.eFlg.BossMenu))
			{
				addHowToPlay(406);
			}
			BossDataTable component = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<BossDataTable>();
			BossListData bossData = component.getBossData();
			bool flag2 = false;
			for (int j = 0; j < bossData.bossList.Length; j++)
			{
				if (component.isPlayed(j, 1))
				{
					addHowToPlay(600 + j);
				}
			}
			if (component.isPlayed(0, 2) || component.isPlayed(1, 1))
			{
				addHowToPlay(408);
			}
			bool flag3 = ResourceLoader.Instance.isJapanResource();
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialGachaMukCollabo))
			{
				addHowToPlay(416);
			}
			if (otherData.isFlag(SaveOtherData.eFlg.TutorialStageSkip))
			{
				addHowToPlay(418);
			}
		}
		if (flag)
		{
			addHowToPlay(650);
			addHowToPlay(651);
			addHowToPlay(653);
			if (Bridge.StageData.getPlayCount(500003) > 0)
			{
				addHowToPlay(652);
			}
		}
		addLine();
		for (int k = 0; k < itemList_.Count; k++)
		{
			addItem(itemList_[k], k);
			itemList_[k].name += "_HowToPlay_item";
		}
		repositionItem();
	}

	private void addHowToPlay(int index)
	{
		GameObject gameObject = createItem(null);
		itemList_.Add(gameObject);
		indexList_.Add(index);
		gameObject.transform.Find("name_Label").GetComponent<UILabel>().text = MessageResource.Instance.getMessage(3000 + index);
		if (PlayerPrefs.HasKey("ShowHowToPlay" + index.ToString("000")))
		{
			gameObject.transform.Find("new").gameObject.SetActive(false);
		}
	}

	private void addSpecialHowToPlay(int index)
	{
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		if (trigger.name.EndsWith("_HowToPlay_item"))
		{
			Constant.SoundUtil.PlayDecideSE();
			int index = int.Parse(trigger.name.Replace("_HowToPlay_item", string.Empty)) - 1;
			PlayerPrefs.SetInt("ShowHowToPlay" + indexList_[index].ToString("000"), 0);
			PlayerPrefs.Save();
			trigger.transform.Find("new").gameObject.SetActive(false);
			DialogHowToPlay dialog = dialogManager_.getDialog(DialogManager.eDialog.HowToPlay) as DialogHowToPlay;
			dialog.setup(index, this, isChallenge);
			StartCoroutine(dialogManager_.openDialog(dialog));
		}
		else
		{
			switch (trigger.name)
			{
			case "Close_Button":
				Constant.SoundUtil.PlayCancelSE();
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
				break;
			}
		}
	}

	public override void OnClose()
	{
		clear();
	}

	public void disableNew(int index)
	{
		PlayerPrefs.SetInt("ShowHowToPlay" + indexList_[index].ToString("000"), 0);
		PlayerPrefs.Save();
		itemList_[index].transform.Find("new").gameObject.SetActive(false);
	}
}
