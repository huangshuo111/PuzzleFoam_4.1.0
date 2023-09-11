using UnityEngine;

[ExecuteInEditMode]
public class NegaEffect : ImageEffectBase
{
	public Vector2 center = new Vector2(0.5f, 0.5f);

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		ImageEffects.RenderDistortion(base.material, source, destination, 50f, center, Vector2.zero);
	}
}
