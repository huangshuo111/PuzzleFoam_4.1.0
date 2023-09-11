using UnityEngine;

public class EventHandler : MonoBehaviour
{
	[SerializeField]
	protected int priority_ = -32768;

	public int priority
	{
		get
		{
			return priority_;
		}
		set
		{
			priority_ = value;
		}
	}

	public bool enableOnPress { get; set; }

	public bool enableOnRelease { get; set; }

	public bool enableOnLongPress { get; set; }

	public bool enableOnClick { get; set; }

	public bool enableOnDragStart { get; set; }

	public bool enableOnDrag { get; set; }

	public bool enableOnDragEnd { get; set; }

	private void Awake()
	{
		enableOnPress = false;
		enableOnRelease = false;
		enableOnLongPress = false;
		enableOnClick = false;
		enableOnDragStart = false;
		enableOnDrag = false;
		enableOnDragEnd = false;
	}

	public virtual void OnPress(Vector3 inputPosition)
	{
	}

	public virtual void OnRelease(Vector3 inputPosition)
	{
	}

	public virtual void OnLongPress(Vector3 inputPosition)
	{
	}

	public virtual void OnClick(Vector3 inputPosition)
	{
	}

	public virtual void OnDragStart(Vector3 inputPosition)
	{
	}

	public virtual void OnDrag(Vector3 inputPosition, Vector3 delta)
	{
	}

	public virtual void OnDragEnd(Vector3 inputPosition)
	{
	}
}
