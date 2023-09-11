using System.Collections;
using UnityEngine;

[AddComponentMenu("Original Scripts/Message/SimpleMessage")]
public class SimpleMessageDraw : MonoBehaviour
{
	public int MessageID;

	private UILabel label_;

	private bool bSetup_;

	private void Awake()
	{
		label_ = GetComponent<UILabel>();
		if (label_ == null)
		{
		}
		if (MessageResource.Instance != null)
		{
			label_.text = MessageResource.Instance.getMessage(MessageID);
			bSetup_ = true;
		}
	}

	private IEnumerator Start()
	{
		if (label_ == null)
		{
			yield break;
		}
		while (!bSetup_)
		{
			if (MessageResource.Instance != null)
			{
				label_.text = MessageResource.Instance.getMessage(MessageID);
				bSetup_ = true;
			}
			yield return 0;
		}
	}

	public void SetMessage(int id)
	{
		MessageID = id;
		if (MessageResource.Instance != null)
		{
			label_.text = MessageResource.Instance.getMessage(MessageID);
		}
	}

	public void SetMessage(int id, string[] param)
	{
		MessageID = id;
		if (MessageResource.Instance != null)
		{
			string text = MessageResource.Instance.getMessage(MessageID);
			for (int i = 0; i < param.Length; i++)
			{
				text = MessageResource.Instance.castCtrlCode(text, i + 1, param[i]);
			}
			label_.text = text;
		}
	}
}
