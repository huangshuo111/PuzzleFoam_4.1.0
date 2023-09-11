using System.Collections;
using UnityEngine;

public class Fade : FadeBase
{
	protected override IEnumerator updateFade()
	{
		fadeObject_.SetActive(true);
		bFade_ = true;
		Color color = fadeMaterial_.color;
		while (Time.time - startTime_ < fadeDuration_)
		{
			color.a = Mathf.Lerp(fromAlpha_, toAlpha_, (Time.time - startTime_) / fadeDuration_);
			fadeMaterial_.color = color;
			yield return null;
		}
		color.a = toAlpha_;
		fadeMaterial_.color = color;
		if (color.a <= 0f)
		{
			fadeObject_.SetActive(false);
		}
		bFade_ = false;
	}
}
