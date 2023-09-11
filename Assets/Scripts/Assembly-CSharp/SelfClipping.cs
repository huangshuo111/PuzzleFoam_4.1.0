using UnityEngine;

public class SelfClipping : MonoBehaviour
{
	private UIWidget[] widgets;

	private Renderer[] renderers;

	private Collider[] colliders;

	public Rect range = new Rect(-1f, -1f, 2f, 2f);

	private void Start()
	{
		widgets = base.transform.GetComponentsInChildren<UIWidget>();
		renderers = base.transform.GetComponentsInChildren<Renderer>();
		colliders = base.transform.GetComponentsInChildren<Collider>();
	}

	private void LateUpdate()
	{
		if (widgets == null || widgets.Length < 0)
		{
			return;
		}
		if (widgets[0].enabled)
		{
			if (range.Contains(base.transform.position))
			{
				return;
			}
			UIWidget[] array = widgets;
			foreach (UIWidget uIWidget in array)
			{
				if (uIWidget != null)
				{
					uIWidget.enabled = false;
				}
			}
			Renderer[] array2 = renderers;
			foreach (Renderer renderer in array2)
			{
				renderer.enabled = false;
			}
			Collider[] array3 = colliders;
			foreach (Collider collider in array3)
			{
				collider.enabled = false;
			}
		}
		else
		{
			if (!range.Contains(base.transform.position))
			{
				return;
			}
			UIWidget[] array4 = widgets;
			foreach (UIWidget uIWidget2 in array4)
			{
				if (uIWidget2 != null)
				{
					uIWidget2.enabled = true;
				}
			}
			Renderer[] array5 = renderers;
			foreach (Renderer renderer2 in array5)
			{
				renderer2.enabled = true;
			}
			Utility.updateUIWidget(base.gameObject);
			Collider[] array6 = colliders;
			foreach (Collider collider2 in array6)
			{
				collider2.enabled = true;
			}
		}
	}
}
