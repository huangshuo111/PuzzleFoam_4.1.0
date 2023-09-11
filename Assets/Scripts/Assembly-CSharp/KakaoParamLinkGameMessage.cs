using System.Collections.Generic;
using System.Text;
using SimpleJSON;

public class KakaoParamLinkGameMessage : KakaoParamBase
{
	private string receiverId;

	private string templateId;

	private string gameMessage;

	private int heart;

	private byte[] data;

	private string imagePath;

	private Dictionary<string, string> metaInfo;

	private string executeUrl;

	public KakaoParamLinkGameMessage(string _receiverId, string _templateId, string _gameMessage, int _heart, byte[] _data, string _imagePath, Dictionary<string, string> _metaInfo, string _executeUrl)
		: base(KakaoAction.SendLinkGameMessage)
	{
		receiverId = _receiverId;
		templateId = _templateId;
		gameMessage = _gameMessage;
		heart = _heart;
		data = _data;
		imagePath = _imagePath;
		metaInfo = _metaInfo;
		executeUrl = _executeUrl;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		jSONClass[KakaoStringKeys.Params.Leaderboard.receiverId] = receiverId;
		jSONClass[KakaoStringKeys.Params.Leaderboard.templateId] = templateId;
		jSONClass[KakaoStringKeys.Params.Leaderboard.heart] = heart.ToString();
		if (gameMessage != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.gameMessage] = gameMessage;
		}
		if (executeUrl != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.executeUrl] = executeUrl;
		}
		if (data != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.data] = Encoding.UTF8.GetString(data);
		}
		if (imagePath != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.imagePath] = imagePath;
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
