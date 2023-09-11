using UnityEngine;

public class RankingDummyItem : RankingItem
{
	[SerializeField]
	protected DummyFriendIcon FriendIcon;

	public void setFriendSprite(DummyFriendInfo.Info info)
	{
		FriendIcon.setFriendSprite(info);
	}
}
