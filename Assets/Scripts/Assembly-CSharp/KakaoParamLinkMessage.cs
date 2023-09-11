using System.Collections.Generic;
using SimpleJSON;

public class KakaoParamLinkMessage : KakaoParamBase
{
	private string templateId;

	private string receiverId;

	private string imagePath;

	private string executeUrl;

	private Dictionary<string, string> metaInfo;

	public KakaoParamLinkMessage(string _templateId, string _receiverId, string _imagePath, string _executeUrl, Dictionary<string, string> _metaInfo)
		: base(KakaoAction.SendLinkMessage)
	{
		templateId = _templateId;
		receiverId = _receiverId;
		imagePath = _imagePath;
		executeUrl = _executeUrl;
		metaInfo = _metaInfo;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		jSONClass[KakaoStringKeys.Params.templateId] = templateId;
		jSONClass[KakaoStringKeys.Params.receiverId] = receiverId;
		if (imagePath != null)
		{
			jSONClass[KakaoStringKeys.Params.imagePath] = imagePath;
		}
		if (executeUrl != null)
		{
			jSONClass[KakaoStringKeys.Params.executeUrl] = executeUrl;
		}
		if (metaInfo != null)
		{
			foreach (string key in metaInfo.Keys)
			{
				jSONClass[KakaoStringKeys.Params.metaInfo][key] = metaInfo[key];
			}
		}
		return jSONClass.ToString();
	}
}
