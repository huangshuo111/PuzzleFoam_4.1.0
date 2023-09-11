using System.Collections;
using UnityEngine;

public class FadeMng : MonoBehaviour
{
	public enum eType
	{
		AllMask = 0,
		Cutout = 1,
		Twirl = 2,
		Scenario = 3,
		MapChange = 4,
		Max = 5
	}

	[SerializeField]
	private FadeBase[] Fades;

	private void Awake()
	{
	}

	public IEnumerator startFade(eType type, float from, float to, float duration)
	{
		yield return StartCoroutine(getFade(type).startFade(from, to, duration));
	}

	public IEnumerator startFadeIn(eType type)
	{
		yield return StartCoroutine(getFade(type).startFade(1f, 0f, getFade(type).FadeTime));
	}

	public IEnumerator startFadeOut(eType type)
	{
		yield return StartCoroutine(getFade(type).startFade(0f, 1f, getFade(type).FadeTime));
	}

	public bool isFade(eType type)
	{
		return getFade(type).isFade();
	}

	public bool isFade()
	{
		for (int i = 0; i < Fades.Length; i++)
		{
			if (Fades[i].isFade())
			{
				return true;
			}
		}
		return false;
	}

	public void setActive(eType type, bool bActive)
	{
		getFade(type).gameObject.SetActive(bActive);
	}

	private FadeBase getFade(eType type)
	{
		return Fades[(int)type];
	}
}
