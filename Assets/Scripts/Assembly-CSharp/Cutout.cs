using System.Collections;
using UnityEngine;

public class Cutout : FadeBase
{
	protected override IEnumerator updateFade()
	{
		fadeObject_.SetActive(true);
		bFade_ = true;
		float cutoff3 = fadeMaterial_.GetFloat("_Cutoff");
		while (Time.time - startTime_ < fadeDuration_)
		{
			cutoff3 = Mathf.Lerp(fromAlpha_, toAlpha_, (Time.time - startTime_) / fadeDuration_);
			fadeMaterial_.SetFloat("_Cutoff", 1f - cutoff3);
			yield return null;
		}
		cutoff3 = toAlpha_;
		fadeMaterial_.SetFloat("_Cutoff", 1f - cutoff3);
		if (cutoff3 <= 0f)
		{
			fadeObject_.SetActive(false);
		}
		bFade_ = false;
	}
}
