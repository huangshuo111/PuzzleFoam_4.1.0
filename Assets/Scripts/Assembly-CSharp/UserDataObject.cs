using UnityEngine;

public class UserDataObject : MonoBehaviour
{
	private UserData data_;

	public UserData getData()
	{
		return data_;
	}

	public void setData(UserData data)
	{
		data_ = data;
	}
}
