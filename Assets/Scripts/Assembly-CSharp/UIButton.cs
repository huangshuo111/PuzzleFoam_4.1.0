using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button")]
public class UIButton : UIButtonColor
{
	public Color disabledColor = Color.grey;

	public bool isEnabled
	{
		get
		{
			Collider collider = base.GetComponent<Collider>();
			return (bool)collider && collider.enabled;
		}
		set
		{
			Collider collider = base.GetComponent<Collider>();
			if ((bool)collider && collider.enabled != value)
			{
				collider.enabled = value;
				UpdateColor(value, false);
			}
		}
	}

	public void setEnable(bool bEnable)
	{
		setEnable(bEnable, false);
	}

	public void setEnable(bool bEnable, bool bOnlySelf)
	{
		if (!mStarted)
		{
			Init();
			mStarted = true;
		}
		if (!bEnable)
		{
			base.GetComponent<Collider>().enabled = false;
			OnEnable();
		}
		else
		{
			base.GetComponent<Collider>().enabled = true;
			OnHover(true);
		}
		if (bOnlySelf)
		{
			return;
		}
		UIButton[] componentsInChildren = GetComponentsInChildren<UIButton>(true);
		UIButton[] array = componentsInChildren;
		foreach (UIButton uIButton in array)
		{
			if (!(uIButton == this))
			{
				uIButton.setEnable(bEnable, true);
			}
		}
	}

	protected override void OnEnable()
	{
		if (isEnabled)
		{
			base.OnEnable();
		}
		else
		{
			UpdateColor(false, true);
		}
	}

	protected override void OnHover(bool isOver)
	{
		if (isEnabled)
		{
			base.OnHover(isOver);
		}
	}

	protected override void OnPress(bool isPressed)
	{
		if (isEnabled)
		{
			base.OnPress(isPressed);
		}
	}

	public void UpdateColor(bool shouldBeEnabled, bool immediate)
	{
		if ((bool)tweenTarget)
		{
			Color color = ((!shouldBeEnabled) ? disabledColor : base.defaultColor);
			TweenColor tweenColor = TweenColor.Begin(tweenTarget, 0.15f, color);
			if (immediate)
			{
				tweenColor.color = color;
				tweenColor.enabled = false;
			}
		}
	}
}
