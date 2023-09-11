using System.Collections;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
	public enum eTargetPart
	{
		NormalMap = 0,
		EventMap = 1,
		ChallengeMap = 2,
		CollaborationMap = 3
	}

	[SerializeField]
	private Vector3 Offset = Vector3.zero;

	[SerializeField]
	private UITexture Icon;

	[SerializeField]
	private float MoveTime = 0.75f;

	[SerializeField]
	private iTween.EaseType EaseType = iTween.EaseType.easeInBack;

	private Part_Map _mapPart;

	private Part_EventMap _eventPart;

	private Part_ChallengeMap _challengePart;

	private Part_CollaborationMap _collaboPart;

	public void setTexture(Texture texture)
	{
		if (!Icon.material.name.Contains("Clone"))
		{
			Icon.material = Object.Instantiate(Icon.material) as Material;
		}
		if (texture.width > texture.height)
		{
			float num = (float)texture.height / (float)texture.width;
			Rect uvRect = Icon.uvRect;
			uvRect.width = num;
			uvRect.x = 1f - num * 0.5f;
			Icon.uvRect = uvRect;
		}
		Icon.mainTexture = texture;
	}

	public void setup(Vector3 pos, PartBase part, eTargetPart target)
	{
		Vector3 vector = pos + Offset;
		base.transform.localPosition = new Vector3(vector.x, vector.y, base.transform.localPosition.z);
		switch (target)
		{
		case eTargetPart.NormalMap:
			_mapPart = (Part_Map)part;
			break;
		case eTargetPart.EventMap:
			_eventPart = (Part_EventMap)part;
			break;
		case eTargetPart.ChallengeMap:
			_challengePart = (Part_ChallengeMap)part;
			break;
		case eTargetPart.CollaborationMap:
			_collaboPart = (Part_CollaborationMap)part;
			break;
		}
	}

	public IEnumerator move(Vector3 target)
	{
		Vector3 to = target + Offset;
		iTween.MoveTo(base.gameObject, iTween.Hash("x", to.x, "y", to.y, "islocal", true, "time", MoveTime, "easeType", EaseType));
		while (GetComponent<iTween>() != null)
		{
			yield return 0;
		}
	}

	public IEnumerator loadTexture()
	{
		UserData playerData = DummyPlayerData.Data;
		if (string.IsNullOrEmpty(playerData.URL))
		{
			yield break;
		}
		int failedCount = 0;
		do
		{
			WWW www = new WWW(playerData.URL);
			yield return www;
			if (www.error == null)
			{
				DummyPlayerData.Data.Texture = www.textureNonReadable;
				www.Dispose();
				if (DummyPlayerData.Data.Texture != null)
				{
					setTexture(DummyPlayerData.Data.Texture);
				}
				break;
			}
			www.Dispose();
			failedCount++;
		}
		while (failedCount < 3);
	}

	private void TapEvent()
	{
		UserData data = DummyPlayerData.Data;
		if (_mapPart != null)
		{
			StartCoroutine(_mapPart.showDialog_playerData(data));
		}
		else if (_eventPart != null)
		{
			StartCoroutine(_eventPart.showDialog_playerData(data));
		}
		else if (_challengePart != null)
		{
			StartCoroutine(_challengePart.showDialog_playerData(data));
		}
		else if (_collaboPart != null)
		{
			StartCoroutine(_collaboPart.showDialog_playerData(data));
		}
	}
}
