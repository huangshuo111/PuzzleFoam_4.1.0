using UnityEngine;

public class DummyFriendIcon : MonoBehaviour
{
	[SerializeField]
	private UISprite Icon;

	public void setFriendSprite(DummyFriendInfo.Info info)
	{
		string spriteName = "user_" + info.Type.ToString("00");
		Icon.spriteName = spriteName;
	}
}
