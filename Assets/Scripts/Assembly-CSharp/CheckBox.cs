using UnityEngine;

public class CheckBox : MonoBehaviour
{
	private GameObject checkmark_;

	private UILabel label_;

	private void Awake()
	{
		checkmark_ = base.transform.Find("Checkmark").gameObject;
		label_ = base.transform.Find("Label").GetComponent<UILabel>();
	}

	public void setLabel(string text)
	{
		if (label_ != null)
		{
			label_.text = text;
		}
	}

	public void setCheck(bool bCheck)
	{
		if (checkmark_ != null)
		{
			checkmark_.gameObject.SetActive(bCheck);
		}
	}
}
