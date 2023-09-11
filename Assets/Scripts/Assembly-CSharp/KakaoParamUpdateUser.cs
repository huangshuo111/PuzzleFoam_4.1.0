using System.Text;
using SimpleJSON;

public class KakaoParamUpdateUser : KakaoParamBase
{
	private int additionalHeart;

	private byte[] publicData;

	private byte[] privateData;

	public KakaoParamUpdateUser(int _additionalHeart, byte[] _publicData, byte[] _privateData)
		: base(KakaoAction.UpdateUser)
	{
		additionalHeart = _additionalHeart;
		publicData = _publicData;
		privateData = _privateData;
	}

	public override string getParamString()
	{
		JSONClass jSONClass = makeDefaultParam();
		jSONClass[KakaoStringKeys.Params.Leaderboard.additionalHeart] = additionalHeart.ToString();
		jSONClass[KakaoStringKeys.Params.Leaderboard.currentHeart] = KakaoGameUserInfo.Instance.heart.ToString();
		if (publicData != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.publicData] = Encoding.UTF8.GetString(publicData);
		}
		if (privateData != null)
		{
			jSONClass[KakaoStringKeys.Params.Leaderboard.privateData] = Encoding.UTF8.GetString(privateData);
		}
		return jSONClass.ToString();
	}
}
