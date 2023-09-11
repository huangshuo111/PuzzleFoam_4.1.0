using System.Collections;
using UnityEngine;

public class Twirl : FadeBase
{
	private TwirlEffect te;

	protected override IEnumerator updateFade()
	{
		if (te == null)
		{
			te = Camera.main.GetComponent<TwirlEffect>();
		}
		te.enabled = true;
		fadeObject_.SetActive(true);
		bFade_ = true;
		Color color = Color.white;
		while (Time.time - startTime_ < fadeDuration_)
		{
			color.a = Mathf.Lerp(fromAlpha_, toAlpha_, (Time.time - startTime_) / fadeDuration_);
			fadeMaterial_.color = color;
			setTwirlAngle(color.a);
			yield return null;
		}
		color.a = toAlpha_;
		fadeMaterial_.color = color;
		setTwirlAngle(color.a);
		if (color.a <= 0f)
		{
			fadeObject_.SetActive(false);
		}
		bFade_ = false;
		te.enabled = false;
	}

	private void setTwirlAngle(float alpha)
	{
		if (toAlpha_ > 0.5f)
		{
			te.angle = alpha * -180f;
		}
		else
		{
			te.angle = alpha * 180f;
		}
	}
}
