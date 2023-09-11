using System;
using System.Collections.Generic;
using SimpleJSON;

public class KakaoInvitationStates
{
	public class State
	{
		public string registeredUserId { get; private set; }

		public string profileImageUrl { get; private set; }

		public string nickName { get; private set; }

		public string senderRewardState { get; private set; }

		public string receiverRewardState { get; private set; }

		public string senderReward { get; private set; }

		public DateTime createdAt { get; private set; }

		public State(JSONNode state)
		{
			registeredUserId = state["user_id"];
			profileImageUrl = state["profile_image_url"];
			nickName = state["nickname"];
			senderRewardState = state["sender_reward_state"];
			receiverRewardState = state["receiver_reward_state"];
			senderReward = state["sender_reward"];
			string s = state["created_at"].Value.ToString();
			DateTime result;
			DateTime.TryParse(s, out result);
			createdAt = result;
		}
	}

	public List<State> states = new List<State>();

	private static KakaoInvitationStates _instance;

	public int registedTotalCount { get; private set; }

	public static KakaoInvitationStates Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoInvitationStates();
			}
			return _instance;
		}
	}

	public void printForDebugging()
	{
		foreach (State state in states)
		{
			if (state != null)
			{
				Debug.Log(string.Format("Name:{0} / RegisteredUserId:{1} / senderRewardState:{2} / receiverRewardState:{3}", state.nickName, state.registeredUserId, state.senderRewardState, state.receiverRewardState));
			}
		}
		KakaoNativeExtension.Instance.ShowAlertMessage(string.Format("Register User: {0}", registedTotalCount.ToString()));
	}

	public void clear()
	{
		states.Clear();
		registedTotalCount = 0;
	}

	public void setInvitationStatesFromJSON(JSONNode root)
	{
		clear();
		registedTotalCount = root["total_count"].AsInt;
		JSONArray asArray = root["invitation_states"].AsArray;
		if (asArray == null)
		{
			Debug.Log("Invitation states is empty!");
			return;
		}
		int count = asArray.Count;
		for (int i = 0; i < count; i++)
		{
			JSONNode jSONNode = asArray[i];
			if (!(jSONNode == null))
			{
				states.Add(new State(jSONNode));
			}
		}
	}
}
