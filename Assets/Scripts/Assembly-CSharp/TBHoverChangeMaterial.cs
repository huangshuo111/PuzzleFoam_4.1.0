using UnityEngine;

public class TBHoverChangeMaterial : MonoBehaviour
{
	public Material hoverMaterial;

	private Material normalMaterial;

	private void Start()
	{
		normalMaterial = base.GetComponent<Renderer>().sharedMaterial;
	}

	private void OnFingerHover(FingerHoverEvent e)
	{
		if (e.Phase == FingerHoverPhase.Enter)
		{
			base.GetComponent<Renderer>().sharedMaterial = hoverMaterial;
		}
		else
		{
			base.GetComponent<Renderer>().sharedMaterial = normalMaterial;
		}
	}
}
