using System.Collections;
using System.Collections.Generic;
using LitJson;
using Network;
using UnityEngine;

public class NetworkUtility
{
	public class ResponceHeaderKeys
	{
		public static string ApplicationID = "X-TAITO-LINEAPPLICATION-ID";

		public static string ResourceVersion = "X-TAITO-LINEAPPLICATION-STAGERESOURCE-VERSION";

		public static string EventID = "X-TAITO-LINEAPPLICATION-EVENT-ID";

		public static string GameResourceVersion = "X-TAITO-LINEAPPLICATION-GAMERESOURCE-VERSION";

		public static string SessionID = "X-TAITO-LINEAPPLICATION-SESSIONID";

		public static string Encrypt = "X-Taito-LINEApplication-enc";

		public static string MemberNo = "X-Taito-LINEApplication-MyMemberNo";

		public static string Region = "X-Taito-LINEApplication-Region";

		public static string DeviceInfo = "X-Taito-LINEApplication-DeviceInfo";

		public static string AppVersion = "X-Taito-LINEApplication-ClientVersion";
	}

	public static ResponceHeaderData createResponceHeaderData(WWW www)
	{
		if (www == null)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			return null;
		}
		if (www.responseHeaders == null)
		{
			return null;
		}
		if (www.responseHeaders.Count <= 0)
		{
			return null;
		}
		return createResponceHeaderData(www.responseHeaders);
	}

	public static ResponceHeaderData createResponceHeaderData(Dictionary<string, string> responseHeaders)
	{
		ResponceHeaderData responceHeaderData = new ResponceHeaderData();
		if (responseHeaders.ContainsKey(ResponceHeaderKeys.ApplicationID))
		{
			responceHeaderData.ApplicationID = int.Parse(responseHeaders[ResponceHeaderKeys.ApplicationID]);
		}
		if (responseHeaders.ContainsKey(ResponceHeaderKeys.ResourceVersion))
		{
			responceHeaderData.ResourceVersion = responseHeaders[ResponceHeaderKeys.ResourceVersion];
		}
		if (responseHeaders.ContainsKey(ResponceHeaderKeys.EventID))
		{
			responceHeaderData.EventID = int.Parse(responseHeaders[ResponceHeaderKeys.EventID]);
		}
		if (responseHeaders.ContainsKey(ResponceHeaderKeys.GameResourceVersion))
		{
			responceHeaderData.GameResourceVersion = responseHeaders[ResponceHeaderKeys.GameResourceVersion];
		}
		if (responseHeaders.ContainsKey(ResponceHeaderKeys.SessionID))
		{
			responceHeaderData.SessionID = responseHeaders[ResponceHeaderKeys.SessionID];
		}
		return responceHeaderData;
	}

	public static bool isError(WWW www)
	{
		return getResultCode(www) != eResultCode.Success;
	}

	public static eResultCode getResultCode(WWW www)
	{
		if (www == null)
		{
			return eResultCode.ErrorUnknown;
		}
		if (!string.IsNullOrEmpty(www.error))
		{
			return eResultCode.ErrorUnknown;
		}
		if (string.IsNullOrEmpty(www.text))
		{
			return eResultCode.ErrorUnknown;
		}
		if (!string.IsNullOrEmpty(www.text) && www.text.Contains("resultCode"))
		{
			JsonData jsonData = JsonMapper.ToObject(www.text);
			int num = (int)jsonData["resultCode"];
			if (num != 0)
			{
				return (eResultCode)num;
			}
		}
		return eResultCode.Success;
	}

	public static void SortScore(ref List<UserData> userList)
	{
		userList.Sort(delegate(UserData a, UserData b)
		{
			if (b.Score > a.Score)
			{
				return 1;
			}
			if (b.Score < a.Score)
			{
				return -1;
			}
			if (a.ID > b.ID)
			{
				return 1;
			}
			return (a.ID < b.ID) ? (-1) : 0;
		});
	}

	public static void SortRankingStageScore(ref List<UserData> userList)
	{
		userList.Sort(delegate(UserData a, UserData b)
		{
			if (b.RankingStageScore > a.RankingStageScore)
			{
				return 1;
			}
			if (b.RankingStageScore < a.RankingStageScore)
			{
				return -1;
			}
			if (a.allStarSum > b.allStarSum)
			{
				return 1;
			}
			return (a.allStarSum < b.allStarSum) ? (-1) : 0;
		});
	}

	public static void SortName(ref List<GameObject> userList)
	{
		userList.Sort(delegate(GameObject a, GameObject b)
		{
			UserDataObject component = a.GetComponent<UserDataObject>();
			UserDataObject component2 = b.GetComponent<UserDataObject>();
			return (component != null && component2 != null) ? string.Compare(component.getData().UserName, component2.getData().UserName) : 0;
		});
		userList.Reverse();
	}

	public static void SortScore(ref List<GameObject> userList)
	{
		userList.Sort(delegate(GameObject a, GameObject b)
		{
			UserDataObject component = a.GetComponent<UserDataObject>();
			UserDataObject component2 = b.GetComponent<UserDataObject>();
			return (component != null && component2 != null) ? (component.getData().Score - component2.getData().Score) : 0;
		});
		userList.Reverse();
	}

	public static void SortTotalScore(ref List<GameObject> userList)
	{
		userList.Sort(delegate(GameObject a, GameObject b)
		{
			UserDataObject component = a.GetComponent<UserDataObject>();
			UserDataObject component2 = b.GetComponent<UserDataObject>();
			if (component != null && component2 != null)
			{
				if (component.getData().TotalScore != component2.getData().TotalScore)
				{
					return component.getData().TotalScore - component2.getData().TotalScore;
				}
				if (component.getData().RankingSortScore != component2.getData().RankingSortScore)
				{
					return component.getData().RankingSortScore - component2.getData().RankingSortScore;
				}
				return (int)(component2.getData().ID - component.getData().ID);
			}
			return 0;
		});
		userList.Reverse();
	}

	public static void SortRankingSortScore(ref List<GameObject> userList)
	{
		userList.Sort(delegate(GameObject a, GameObject b)
		{
			UserDataObject component = a.GetComponent<UserDataObject>();
			UserDataObject component2 = b.GetComponent<UserDataObject>();
			if (component != null && component2 != null)
			{
				if (component.getData().RankingStageScore != component2.getData().RankingStageScore)
				{
					return component.getData().RankingStageScore - component2.getData().RankingStageScore;
				}
				if (component.getData().allStarSum != component2.getData().allStarSum)
				{
					return component.getData().allStarSum - component2.getData().allStarSum;
				}
				return (int)(component2.getData().ID - component.getData().ID);
			}
			return 0;
		});
		userList.Reverse();
	}

	public static IEnumerator downloadPlayerData(bool bCancel, bool bShowIcon)
	{
		NetworkMng.Instance.setup(null);
		yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(API.UpdatePlayerData, bCancel, bShowIcon));
		WWW www = NetworkMng.Instance.getWWW();
		PlayerData playerData = JsonMapper.ToObject<PlayerData>(www.text);
		GlobalData.Instance.setPlayerData(playerData);
		GlobalData.Instance.setInviteRewardData(JsonMapper.ToObject<InviteRewardData>(www.text).inviteBasicReward);
		GameData gamedata = GlobalData.Instance.getGameData();
		gamedata.setEventData(playerData);
		gamedata.setDailyMissionData(playerData);
		gamedata.setParkData(playerData.parkStageList, playerData.minilenList, playerData.minilenThanksList, playerData.buildings, playerData.mapReleaseNum);
		SaveData.Instance.getParkData().UpdatePlacedData(playerData.buildings);
		if (DailyMission.isTermClear())
		{
			Mission respons_mission2 = JsonMapper.ToObject<Mission>(www.text);
			GlobalData.Instance.setDailyMissionData(respons_mission2);
			Network.DailyMission dMission = GlobalData.Instance.getDailyMissionData();
			if (dMission == null)
			{
				NetworkMng.Instance.setup(Hash.DailyMissionCreate());
				yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(API.DailyMissionCreate, false, false));
				WWW www_dMission = NetworkMng.Instance.getWWW();
				respons_mission2 = JsonMapper.ToObject<Mission>(www_dMission.text);
				GlobalData.Instance.setDailyMissionData(respons_mission2);
				DailyMission.bMissionCreate = true;
			}
		}
	}
}
