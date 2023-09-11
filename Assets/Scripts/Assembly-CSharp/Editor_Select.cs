using UnityEngine;

public class Editor_Select : MonoBehaviour
{
	private tk2dAnimatedSprite select;

	private UILabel label_;

	private void Start()
	{
		select = GetComponent<tk2dAnimatedSprite>();
		select.Play();
		select.Pause();
		if (base.gameObject.transform.Find("label") != null)
		{
			label_ = base.gameObject.transform.Find("label").GetComponent<UILabel>();
			label_.text = string.Empty;
		}
	}

	private void Update()
	{
	}

	private void button(GameObject obj)
	{
		select.Play(obj.name);
		select.Pause();
		if (obj.name.Contains("_78") || obj.name.Contains("_88"))
		{
			label_.text = ((!obj.name.Contains("_78")) ? "B" : "A");
			label_.color = Color.white;
			label_.transform.localScale = new Vector3(0.13f, 0.13f, 1f);
		}
		else
		{
			label_.text = string.Empty;
		}
	}
}
