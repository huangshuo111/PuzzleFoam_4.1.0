using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Network;
using UnityEngine;

public class RankingItem : PlayerItemBase
{
	[SerializeField]
	protected UILabel RankLabel;

	[SerializeField]
	protected UILabel ScoreLabel;

	[SerializeField]
	protected UISprite RankIcon;

	[SerializeField]
	private float Digit4Scale = 20f;

	[SerializeField]
	protected UILabel SortScoreLabel;

	[SerializeField]
	protected UISprite AvatarIcon_00;

	[SerializeField]
	protected UISprite AvatarIcon_01;

	[SerializeField]
	protected UISprite AvatarIcon_02;

	[SerializeField]
	protected UISprite AvatarIcon_03;

	[SerializeField]
	protected UILabel infoLabel;

	[SerializeField]
	protected GameObject GiftButton_;

	private int score_;

	private int rank_;

	private bool blowcheck;

	private void Awake()
	{
	}

	public virtual void setup(string name, long id, string mid, int score, int rank, int sortScore, int avatarID, int throwChara, int supportChara)
	{
		blowcheck = GameObject.Find("ResourceLoader").GetComponent<ResourceLoader>().isUseLowResource();
		base.setup(name, id, mid);
		score_ = score;
		rank_ = rank;
		ScoreLabel.text = score.ToString("N0");
		if (SortScoreLabel != null)
		{
			SortScoreLabel.text = sortScore.ToString("N0");
		}
		MessageResource instance = MessageResource.Instance;
		string text = rank.ToString();
		Network.Avatar avatar = null;
		Network.Avatar[] array = null;
		avatarID = ((avatarID < 20000) ? 20000 : avatarID);
		float chara_img_diminish_rate = GlobalData.Instance.chara_img_diminish_rate;
		if (AvatarIcon_00 != null && AvatarIcon_01 != null)
		{
			if (rank <= 10)
			{
				string text2 = ((throwChara <= 0) ? string.Empty : ("_" + (throwChara - 1).ToString("00")));
				string text3 = ((supportChara <= 0) ? string.Empty : ("_" + (supportChara - 1).ToString("00")));
				if (throwChara - 1 > 18)
				{
					AvatarIcon_00.atlas = base.transform.Find("avatar/avatar_icon_03").GetComponent<UISprite>().atlas;
				}
				else
				{
					AvatarIcon_00.atlas = base.transform.Find("avatar/avatar_icon_02").GetComponent<UISprite>().atlas;
				}
				if (supportChara - 1 > 18)
				{
					AvatarIcon_01.atlas = base.transform.Find("avatar/avatar_icon_03").GetComponent<UISprite>().atlas;
				}
				else
				{
					AvatarIcon_01.atlas = base.transform.Find("avatar/avatar_icon_02").GetComponent<UISprite>().atlas;
				}
				AvatarIcon_00.spriteName = "avatar_00" + text2 + "_00";
				AvatarIcon_00.MakePixelPerfect();
				AvatarIcon_00.transform.localScale = new Vector3(AvatarIcon_00.transform.localScale.x / chara_img_diminish_rate, AvatarIcon_00.transform.localScale.y / chara_img_diminish_rate, 1f);
				AvatarIcon_01.spriteName = "avatar_01" + text3 + "_00";
				AvatarIcon_01.MakePixelPerfect();
				AvatarIcon_01.transform.localScale = new Vector3(AvatarIcon_01.transform.localScale.x / chara_img_diminish_rate, AvatarIcon_01.transform.localScale.y / chara_img_diminish_rate, 1f);
				AvatarIcon_00.gameObject.SetActive(true);
				AvatarIcon_01.gameObject.SetActive(true);
			}
			else
			{
				AvatarIcon_00.gameObject.SetActive(false);
				AvatarIcon_01.gameObject.SetActive(false);
			}
		}
		if (RankIcon != null)
		{
			switch (rank)
			{
			case 1:
				RankIcon.spriteName = "ranking_icon_00";
				break;
			case 2:
				RankIcon.spriteName = "ranking_icon_01";
				break;
			case 3:
				RankIcon.spriteName = "ranking_icon_02";
				break;
			default:
				text = instance.getMessage(40);
				text = instance.castCtrlCode(text, 1, rank.ToString());
				RankIcon.gameObject.SetActive(false);
				break;
			}
		}
		else
		{
			switch (rank)
			{
			case 1:
				text = instance.getMessage(37);
				break;
			case 2:
				text = instance.getMessage(38);
				break;
			case 3:
				text = instance.getMessage(39);
				break;
			default:
				text = instance.getMessage(40);
				break;
			}
			text = instance.castCtrlCode(text, 1, rank.ToString());
		}
		if (rank.ToString().Length >= 4)
		{
			Vector3 localScale = RankLabel.transform.localScale;
			localScale.x = Digit4Scale;
			RankLabel.transform.localScale = localScale;
		}
		RankLabel.text = text;
	}

	public int getScore()
	{
		return score_;
	}

	public int getRank()
	{
		return rank_;
	}

	public void ShowInfomation()
	{
		if (AvatarIcon_00 != null)
		{
			AvatarIcon_00.gameObject.SetActive(false);
		}
		if (AvatarIcon_01 != null)
		{
			AvatarIcon_01.gameObject.SetActive(false);
		}
		if (RankIcon != null)
		{
			RankIcon.gameObject.SetActive(false);
		}
		if (RankLabel != null)
		{
			RankLabel.gameObject.SetActive(false);
		}
		if (ScoreLabel != null)
		{
			ScoreLabel.gameObject.SetActive(false);
		}
		if (Icon != null)
		{
			Icon.gameObject.SetActive(false);
		}
		if (base.transform.Find("frame") != null)
		{
			base.transform.Find("frame").gameObject.SetActive(false);
		}
		if (ActiveButton != null)
		{
			ActiveButton.SetActive(false);
		}
		if (DeactiveButton != null)
		{
			DeactiveButton.SetActive(false);
		}
		if (base.transform.Find("Labels/name_Label") != null)
		{
			base.transform.Find("Labels/name_Label").gameObject.SetActive(false);
		}
		if (base.transform.Find("Labels/star_icon") != null)
		{
			base.transform.Find("Labels/star_icon").gameObject.SetActive(false);
		}
		if (infoLabel != null)
		{
			infoLabel.gameObject.SetActive(true);
		}
		if (GiftButton_ != null)
		{
			GiftButton_.gameObject.SetActive(false);
		}
		if (BlockButton != null)
		{
			BlockButton.gameObject.SetActive(false);
		}
		if (UnBlockButton != null)
		{
			UnBlockButton.gameObject.SetActive(false);
		}
	}

	public IEnumerator sendHeartMail(DialogManager dialogManager)
	{
		Input.enable = false;
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		List<long> members = new List<long> { Convert.ToInt64(mid_) };
		NetworkMng.Instance.setup(Hash.PlayerStar(members.ToArray()));
		yield return dialogManager.StartCoroutine(NetworkMng.Instance.download(API.PlayerStar, true, false));
		UserData user = DummyPlayFriendData.DummyFriends.SingleOrDefault((UserData x) => x.Mid == mid_);
		WWW www2;
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			www2 = NetworkMng.Instance.getWWW();
			RankingData rankingData = JsonMapper.ToObject<RankingData>(www2.text);
			www2.Dispose();
			RankingStarData stardata = rankingData.starList.SingleOrDefault((RankingStarData x) => x.memberNo == Convert.ToInt64(mid_));
			user.IsHeartRecvFlag = stardata.heartRecvFlg;
		}
		while (!TalkMessage.Instance.isReceived)
		{
			yield return null;
		}
		if (SNSCore.Instance.IsBlockMessage(mid_) || !user.IsHeartRecvFlag)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			StartCoroutine(ShowErrorDialog(dialogManager));
			yield break;
		}
		Hashtable args = new Hashtable { 
		{
			"toMemberNo",
			id_.ToString()
		} };
		NetworkMng.Instance.setup(args);
		yield return dialogManager.StartCoroutine(NetworkMng.Instance.download(OnCreateWWW, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			yield break;
		}
		www2 = NetworkMng.Instance.getWWW();
		if (DailyMission.isTermClear())
		{
			Mission respons_mission2 = JsonMapper.ToObject<Mission>(www2.text);
			GlobalData.Instance.setDailyMissionData(respons_mission2);
			Network.DailyMission dMission = GlobalData.Instance.getDailyMissionData();
			if (dMission == null)
			{
				NetworkMng.Instance.setup(Hash.DailyMissionCreate());
				yield return StartCoroutine(NetworkMng.Instance.download(API.DailyMissionCreate, false, false));
				WWW www_dMission = NetworkMng.Instance.getWWW();
				respons_mission2 = JsonMapper.ToObject<Mission>(www_dMission.text);
				GlobalData.Instance.setDailyMissionData(respons_mission2);
				DailyMission.bMissionCreate = true;
			}
		}
		while (!TalkMessage.Instance.isReceived)
		{
			yield return null;
		}
		string[] presentMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.Present);
		for (int i = 0; i < presentMsg.Length; i++)
		{
			presentMsg[i] = presentMsg[i].Replace("{owner}", DummyPlayerData.Data.UserName);
		}
		List<string> receiverMidList = new List<string>(1) { mid_ };
		DialogSendBase.SendAppLinkMessageCB sendAppLinkMessageCB = new DialogSendBase.SendAppLinkMessageCB();
		SNSCore.Instance.KakaoSendMessagePresent(mid_, sendAppLinkMessageCB.OnMessageSend);
		while (sendAppLinkMessageCB.result_ == -1)
		{
			yield return null;
		}
		if (sendAppLinkMessageCB.result_ != 0)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			Input.enable = true;
			StartCoroutine(ShowErrorDialog(dialogManager, false, (eSNSResultCode)sendAppLinkMessageCB.result_));
			yield break;
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		Input.enable = true;
		PlayerPrefs.SetString(Aes.EncryptString("HeartSend" + id_, Aes.eEncodeType.Percent), Utility.getUnixTime(DateTime.Now).ToString());
		PlayerPrefs.Save();
		setPrevTime(Utility.getUnixTime(DateTime.Now));
		setState(true);
	}

	public IEnumerator SendHeartRecvFlag(DialogManager dialogManager)
	{
		if (!SNSCore.IsAuthorize)
		{
			yield break;
		}
		if (SNSCore.Instance.IsBlockMessage(GlobalData.Instance.LineID.ToString()))
		{
			yield return StartCoroutine(ShowHeartBlockDialog(dialogManager));
			yield break;
		}
		yield return StartCoroutine(NetworkMng.Instance.showIcon(true));
		NetworkMng.Instance.setup(null);
		yield return StartCoroutine(NetworkMng.Instance.download(API.HeartRecvFlag, true, false));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		HeartRecvFlag data = JsonMapper.ToObject<HeartRecvFlag>(www.text);
		yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
		DummyPlayerData.Data.IsHeartRecvFlag = !DummyPlayerData.Data.IsHeartRecvFlag;
		base.IsMessageBlock = !DummyPlayerData.Data.IsHeartRecvFlag;
		setState(false);
	}

	private IEnumerator ShowHeartBlockDialog(DialogManager dialogManager)
	{
		DialogCommon dialog2 = null;
		dialog2 = dialogManager.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		string msg = MessageResource.Instance.getMessage(306);
		dialog2.setup(msg, null, null, true);
		yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog2));
		while (dialog2.isOpen())
		{
			yield return null;
		}
	}

	public void SetGiftButtonEnable(bool enable)
	{
		if (GiftButton_ != null)
		{
			GiftButton_.SetActive(enable);
		}
	}

	private IEnumerator ShowErrorDialog(DialogManager dialogManager, bool bBlock = true, eSNSResultCode resultcode = eSNSResultCode.NONE)
	{
		DialogCommon dialog2 = null;
		dialog2 = dialogManager.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		string msg;
		if (!bBlock)
		{
			switch (resultcode)
			{
			case eSNSResultCode.INVITE_MESSAGE_BLOCKED:
			case eSNSResultCode.MESSAGE_BLOCK_USER:
			case eSNSResultCode.UNSUPPORTED_DEVICE:
				msg = MessageResource.Instance.getMessage(81);
				break;
			default:
				msg = MessageResource.Instance.getMessage(35);
				break;
			}
		}
		else
		{
			msg = MessageResource.Instance.getMessage(81);
		}
		dialog2.setup(msg, null, null, true);
		yield return dialogManager.StartCoroutine(dialogManager.openDialog(dialog2));
		while (dialog2.isOpen())
		{
			yield return null;
		}
	}

	private WWW OnCreateWWW(Hashtable args)
	{
		WWWWrap.setup(WWWWrap.eMethod.Get);
		WWWWrap.addGetParameter("toMemberNo", args["toMemberNo"]);
		return WWWWrap.create("mail/heartsend/");
	}
}
