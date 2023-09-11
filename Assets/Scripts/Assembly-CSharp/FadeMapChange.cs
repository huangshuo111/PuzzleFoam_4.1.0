using System.Collections;
using UnityEngine;

public class FadeMapChange : FadeBase
{
	protected override IEnumerator updateFade()
	{
		fadeObject_.SetActive(true);
		bFade_ = true;
		Color color = fadeMaterial_.color;
		color.a = toAlpha_;
		fadeMaterial_.color = color;
		if (color.a <= 0f)
		{
			fadeObject_.SetActive(false);
		}
		bFade_ = false;
		yield break;
	}
}
