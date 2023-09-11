using System.Collections.Generic;
using SimpleJSON;

public class KakaoParamInviteLinkGameMessage : KakaoParamBase
{
	private string receiverId;

	private string templateId;

	private Dictionary<string, string> metaInfo;

	private string executeUrl;

	public KakaoParamInviteLinkGameMessage(string _receiverId, string _templateId, Dictionary<string, string> _metaInfo, string _executeUrl)
		: base(KakaoAction.SendInviteLinkGameMessage)
	{
		receiverId = _receiverId;
		templateId = _templateId;
		metaInfo = _metaInfo;
		executeUrl = _executeUrl;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		jSONClass[KakaoStringKeys.Params.Leaderboard.receiverId] = receiverId;
		jSONClass[KakaoStringKeys.Params.Leaderboard.templateId] = templateId;
		if (executeUrl != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.executeUrl] = executeUrl;
		}
		if (metaInfo != null)
		{
			foreach (string key in metaInfo.Keys)
			{
				jSONClass[KakaoStringKeys.Params.Leaderboard.metaInfo][key] = metaInfo[key];
			}
		}
		return jSONClass.ToString();
	}
}
