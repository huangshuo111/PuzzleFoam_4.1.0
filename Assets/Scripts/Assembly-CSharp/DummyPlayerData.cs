public class DummyPlayerData
{
	public static UserData Data = new UserData(string.Empty, 0, 0, string.Empty, 0, 100L);

	public static int result_ = -1;

	public DummyPlayerData()
	{
		if (Data != null)
		{
			Data.setMinilenParameter(3000, 200);
		}
	}

	public void OnMyProfileLoad()
	{
		result_ = 0;
		Data.ID = SNSCore.local_UserData_.id;
		Data.Mid = SNSCore.local_UserData_.id.ToString();
		Data.UserName = SNSCore.local_UserData_.name;
		Data.URL = SNSCore.local_UserData_.ImageUrl;
	}
}
