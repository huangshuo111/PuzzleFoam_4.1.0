using System.Collections;
using LitJson;
using Network;
using UnityEngine;

public class DialogAward : DialogBase
{
	private enum eLabel
	{
		Award = 0,
		Rank = 1,
		Max = 2
	}

	private enum eRank
	{
		First = 0,
		Second = 1,
		Third = 2,
		Other = 3,
		Max = 4
	}

	private const float Digit4Scale = 20f;

	private UILabel[] labels_ = new UILabel[2];

	private UISprite rankIcon_;

	private GameObject[] chars_ = new GameObject[4];

	private Constant.Reward reward_ = new Constant.Reward();

	private int rank_;

	private void Awake()
	{
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>();
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			switch (transform.name)
			{
			case "Label00":
				labels_[0] = transform.GetComponent<UILabel>();
				break;
			case "rank_Label":
				labels_[1] = transform.GetComponent<UILabel>();
				break;
			case "chara_1":
				chars_[0] = transform.gameObject;
				break;
			case "chara_2":
				chars_[1] = transform.gameObject;
				break;
			case "chara_3":
				chars_[2] = transform.gameObject;
				break;
			case "chara_4":
				chars_[3] = transform.gameObject;
				break;
			case "rank":
				rankIcon_ = transform.GetComponent<UISprite>();
				break;
			}
		}
	}

	public IEnumerator show(int rank)
	{
		Input.enable = false;
		int index = Mathf.Clamp(rank - 1, 0, 3);
		GameObject dataTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		rank_ = rank;
		dataTbl.GetComponent<RankingDataTable>().getReward(index, ref reward_);
		setText(rank, reward_);
		showChara(rank);
		setSprite(rank);
		bool bLimitOver = Constant.Reward.addReward(reward_);
		MainMenu menu = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		menu.update();
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
		if (bLimitOver)
		{
			DialogLimitOver limitOverDialog = dialogManager_.getDialog(DialogManager.eDialog.LimitOver) as DialogLimitOver;
			yield return StartCoroutine(limitOverDialog.show(Constant.eMoney.Coin));
		}
		Input.enable = true;
	}

	private void setText(int rank, Constant.Reward reward)
	{
	}

	private void showChara(int rank)
	{
	}

	private void setSprite(int rank)
	{
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			yield return StartCoroutine(sendRankingAward(rank_));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			yield return StartCoroutine(sendRankingAward(rank_));
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private IEnumerator sendRankingAward(int p0)
	{
		SaveNetworkData netData = SaveData.Instance.getGameData().getNetworkData();
		NetworkMng.Instance.setup(Hash.R1(p0, netData.getRankingUniqueID(), 0));
		yield return StartCoroutine(NetworkMng.Instance.download(API.R1, true));
		if (NetworkMng.Instance.getResultCode() == eResultCode.InvalidRankingReward)
		{
			netData.save();
			yield break;
		}
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			netData.resetRankingDate();
			netData.resetRankingUniqueID();
			yield break;
		}
		netData.save();
		WWW www2 = NetworkMng.Instance.getWWW();
		CommonData commonData = JsonMapper.ToObject<CommonData>(www2.text);
		GlobalData.Instance.getGameData().setCommonData(commonData, false);
		www2.Dispose();
		www2 = null;
		MainMenu menu = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.MapMainUI).GetComponent<MainMenu>();
		menu.update();
	}
}
