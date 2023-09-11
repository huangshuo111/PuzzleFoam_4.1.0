using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Network;
using UnityEngine;

public class DialogSendHighScore : DialogSendBase
{
	private string userName1_;

	private string userName2_;

	private string score_;

	private string sendMid_;

	private string stage_;

	private UserData ReceiveUser_;

	public void setup(string mid, UserData user1, UserData user2, int score, int stageNo)
	{
		sendMid_ = mid;
		stage_ = (stageNo + 1).ToString();
		ReceiveUser_ = user2;
		MessageResource instance = MessageResource.Instance;
		string message = instance.getMessage(33);
		label_.text = user1.UserName;
		userName1_ = Constant.UserName.ReplaceOverStr(label_);
		label_.text = user2.UserName;
		userName2_ = Constant.UserName.ReplaceOverStr(label_);
		score_ = score.ToString("N0");
		scoretext_ = score_;
		message = instance.castCtrlCode(message, 1, userName1_);
		message = instance.castCtrlCode(message, 2, score_);
		message = instance.castCtrlCode(message, 3, userName2_);
		setMessage(message);
		ActionReward.setupButton(base.transform.Find("window/SendButtun"), ActionReward.eType.HiscoreChange);
	}

	protected override IEnumerator send()
	{
		List<long> members = new List<long> { Convert.ToInt64(sendMid_) };
		NetworkMng.Instance.setup(Hash.PlayerStar(members.ToArray()));
		yield return dialogManager_.StartCoroutine(NetworkMng.Instance.download(API.PlayerStar, true, false));
		if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
		{
			WWW www = NetworkMng.Instance.getWWW();
			RankingData rankingData = JsonMapper.ToObject<RankingData>(www.text);
			www.Dispose();
			RankingStarData stardata = rankingData.starList.SingleOrDefault((RankingStarData x) => x.memberNo == Convert.ToInt64(sendMid_));
			ReceiveUser_.IsHeartRecvFlag = stardata.heartRecvFlg;
		}
		bool bError2 = false;
		if (!SNSCore.Instance.IsBlockMessage(sendMid_) && ReceiveUser_.IsHeartRecvFlag)
		{
			Input.enable = false;
			while (!TalkMessage.Instance.isReceived)
			{
				yield return null;
			}
			int TmpStageNo = int.Parse(stage_);
			string[] highscoreMsg;
			if (TmpStageNo >= 10000)
			{
				if (TmpStageNo >= 500000)
				{
					stage_ = (TmpStageNo % 500000 - 1).ToString();
					highscoreMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.ParkExpeditionRankingChange);
				}
				else if (TmpStageNo >= 40000)
				{
					if (TmpStageNo >= 110000 && TmpStageNo <= 110020)
					{
						stage_ = (TmpStageNo % 10000 - 1).ToString();
						highscoreMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.ChallengeRankingChange);
					}
					else
					{
						stage_ = (TmpStageNo % 10000 - 1).ToString();
						highscoreMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.CollaboRankingChange);
					}
				}
				else
				{
					stage_ = (TmpStageNo % 10000 - 1).ToString();
					highscoreMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.EventRankingChange);
				}
			}
			else
			{
				highscoreMsg = TalkMessage.Instance.getMessage(TalkMessage.eType.RankingChange);
			}
			for (int i = 0; i < highscoreMsg.Length; i++)
			{
				highscoreMsg[i] = highscoreMsg[i].Replace("{owner}", userName1_).Replace("{opponent}", userName2_).Replace("{score}", score_)
					.Replace("{stage}", stage_);
			}
			yield return dialogManager_.StartCoroutine(sendRankChangeMessage(sendMid_, stage_));
			bError2 = !isSuccessSendMessage();
			Input.enable = true;
			Debug.Log("bError : " + bError2);
			if (bError2)
			{
				string msg2;
				switch (base.GetMessageResult)
				{
				case eSNSResultCode.INVITE_MESSAGE_BLOCKED:
				case eSNSResultCode.MESSAGE_BLOCK_USER:
				case eSNSResultCode.UNSUPPORTED_DEVICE:
					msg2 = MessageResource.Instance.getMessage(81);
					break;
				default:
					msg2 = MessageResource.Instance.getMessage(35);
					break;
				}
				yield return dialogManager_.StartCoroutine(openCommonDialog(msg2));
			}
			else
			{
				yield return dialogManager_.StartCoroutine(sendSuccess());
			}
		}
		else
		{
			string msg = MessageResource.Instance.getMessage(81);
			yield return dialogManager_.StartCoroutine(openCommonDialog(msg));
		}
	}

	private IEnumerator sendSuccess()
	{
		yield return dialogManager_.StartCoroutine(ActionReward.addActionReward(dialogManager_));
		callCB();
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
	}

	protected IEnumerator sendHighScoreMessage(string mid, string[] msg)
	{
		yield return StartCoroutine(registerTimeline(7000044L, msg[0], eTimeLineType.HighScore));
		if (!isSuccessRegisterTimeline())
		{
			string text = MessageResource.Instance.getMessage(35);
			yield return dialogManager_.StartCoroutine(openErrorDialog(text));
		}
	}
}
