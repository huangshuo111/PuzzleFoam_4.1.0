using System.Collections;
using UnityEngine;

public class ScenarioFade : FadeBase
{
	private Animation anime_;

	protected override void Awake()
	{
		anime_ = base.GetComponent<Animation>();
		Transform[] componentsInChildren = GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == "fade")
			{
				fadeObject_ = transform.gameObject;
				break;
			}
		}
	}

	protected override IEnumerator updateFade()
	{
		fadeObject_.SetActive(true);
		bFade_ = true;
		bool bFadeIn = ((fromAlpha_ < toAlpha_) ? true : false);
		anime_.Play((!bFadeIn) ? "Scenario_fadeout_anm" : "Scenario_fadein_anm");
		while (anime_.isPlaying)
		{
			yield return 0;
		}
		bFade_ = false;
	}
}
