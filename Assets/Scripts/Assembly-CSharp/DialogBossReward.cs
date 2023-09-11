using System.Collections;
using UnityEngine;

public class DialogBossReward : DialogBase
{
	public enum eConnectStatus
	{
		None = 0,
		Loading = 1,
		Finish = 2
	}

	private StageDataTable stageData_;

	private GameObject jewel;

	private GameObject coin;

	private GameObject heart;

	private GameObject item;

	private int closeCount;

	private WWW www_;

	public override void OnCreate()
	{
		init();
		NGUIUtilScalableUIRoot.OffsetUI(base.transform.Find("Bottom"), false);
	}

	public override void OnStartClose()
	{
		if (partManager_.currentPart == PartManager.ePart.Map && www_ != null)
		{
			www_ = null;
		}
	}

	public override void OnClose()
	{
		if (partManager_.currentPart == PartManager.ePart.Map)
		{
			closeCount++;
			if (closeCount > 15)
			{
				closeCount = 0;
				Resources.UnloadUnusedAssets();
			}
		}
	}

	public void init()
	{
		jewel = base.transform.Find("Bottom/Tween/items/jewel_root").gameObject;
		coin = base.transform.Find("Bottom/Tween/items/coin_root").gameObject;
		heart = base.transform.Find("Bottom/Tween/items/heart_root").gameObject;
		item = base.transform.Find("Bottom/Tween/items/item_root").gameObject;
		jewel.SetActive(false);
		coin.SetActive(false);
		heart.SetActive(false);
		item.SetActive(false);
	}

	public IEnumerator show(int stageNo)
	{
		yield return dialogManager_.StartCoroutine(UpdateBossRewardInfo(stageNo));
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Input.enable = true;
	}

	private IEnumerator UpdateBossRewardInfo(int info)
	{
		stageData_ = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<StageDataTable>();
		stageData_.loadBossData();
		if (stageData_ != null)
		{
			jewel.transform.Find("Price_Label").GetComponent<UILabel>().text = "!!!";
		}
		yield return null;
	}

	private void OnDestroy()
	{
	}

	public void forceQuitCoroutine()
	{
	}

	private WWW createWWW(int stageNo)
	{
		WWWWrap.setup(WWWWrap.eMethod.Post);
		WWWWrap.addGetParameter("stageNo", stageNo);
		string text = string.Empty;
		UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
		foreach (UserData userData in dummyFriends)
		{
			if (text.Length > 0)
			{
				text += ",";
			}
			text += userData.ID;
		}
		WWWWrap.addPostParameter("memberNos", text);
		return WWWWrap.create("player/stagedata/");
	}
}
