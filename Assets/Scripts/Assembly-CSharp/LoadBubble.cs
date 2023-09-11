using System.Collections;
using UnityEngine;

public class LoadBubble : MonoBehaviour
{
	public enum eBubble
	{
		Red = 0,
		Green = 1,
		Blue = 2
	}

	[SerializeField]
	private UISprite[] Bubbles;

	[SerializeField]
	private UISpriteAnimation[] BurstBubbles;

	private eBubble getBubbleType(int index)
	{
		string spriteName = Bubbles[index].spriteName;
		if (spriteName.Contains("bubble_00"))
		{
			return eBubble.Red;
		}
		if (spriteName.Contains("bubble_01"))
		{
			return eBubble.Green;
		}
		if (spriteName.Contains("bubble_02"))
		{
			return eBubble.Blue;
		}
		return eBubble.Red;
	}

	public GameObject getBubble(int index)
	{
		return Bubbles[index].gameObject;
	}

	public void burst(int index)
	{
		int bubbleType = (int)getBubbleType(index);
		GameObject gameObject = BurstBubbles[bubbleType].gameObject;
		float z = gameObject.transform.localPosition.z;
		gameObject.transform.localPosition = Bubbles[index].transform.localPosition;
		Vector3 localPosition = gameObject.transform.localPosition;
		gameObject.transform.localPosition = new Vector3(localPosition.x, localPosition.y, z);
		Bubbles[index].spriteName = "star_000";
		Bubbles[index].MakePixelPerfect();
		StartCoroutine(burstCoroutine(BurstBubbles[bubbleType]));
	}

	private IEnumerator burstCoroutine(UISpriteAnimation burstBubble)
	{
		burstBubble.gameObject.SetActive(true);
		burstBubble.Reset();
		while (burstBubble.isPlaying)
		{
			yield return 0;
		}
		burstBubble.gameObject.SetActive(false);
	}
}
