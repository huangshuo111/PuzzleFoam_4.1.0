using UnityEngine;

public class DebugNetworkStatus : MonoBehaviour
{
	private static DebugNetworkStatus instance_;

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		if (!base.GetComponent<GUIText>())
		{
			Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
			base.enabled = false;
		}
	}

	private void drawGUIText()
	{
		string empty = string.Empty;
		empty = empty + "status : " + NetworkMng.Instance.getStatus();
		empty += "\n";
		empty = empty + "result : " + NetworkMng.Instance.getResultCode();
		empty += "\n";
		empty += "error  : ";
		if (NetworkMng.Instance.getWWW() != null)
		{
			empty += NetworkMng.Instance.getWWW().error;
		}
		base.GetComponent<GUIText>().text = empty;
		NetworkMng.eStatus status = NetworkMng.Instance.getStatus();
		if (status == NetworkMng.eStatus.Error)
		{
			base.GetComponent<GUIText>().material.color = Color.red;
		}
		else
		{
			base.GetComponent<GUIText>().material.color = Color.green;
		}
	}

	private void Update()
	{
		if (NetworkMng.IsInstance())
		{
			drawGUIText();
		}
	}
}
