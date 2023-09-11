using UnityEngine;

public class Editor_Chain : MonoBehaviour
{
	public static tk2dAnimatedSprite select;

	private tk2dAnimatedSprite me;

	private void Start()
	{
		me = GetComponentInChildren<tk2dAnimatedSprite>();
	}

	private void Update()
	{
	}

	private void button()
	{
		if (!(select == null) && select.gameObject.activeSelf)
		{
			if (Input.GetMouseButton(0))
			{
				me.Play(select.CurrentClip.name);
			}
			else
			{
				me.Play("bubble_99");
			}
			me.Pause();
		}
	}
}
