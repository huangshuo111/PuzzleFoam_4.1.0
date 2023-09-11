using System.Collections;
using UnityEngine;

public class ConfettiEff : MonoBehaviour
{
	private DialogManager dialogManager_;

	private void Start()
	{
		if (dialogManager_ != null)
		{
			StartCoroutine(checkAppQuitDialogRoutine((DialogAppQuit)dialogManager_.getDialog(DialogManager.eDialog.AppQuit)));
		}
	}

	private void Update()
	{
	}

	public void checkAppQuitDialog(DialogManager dialogManager)
	{
		dialogManager_ = dialogManager;
	}

	private IEnumerator checkAppQuitDialogRoutine(DialogAppQuit dialog)
	{
		if (dialog == null)
		{
			yield break;
		}
		Transform trans = base.transform;
		GameObject[] particles = new GameObject[trans.childCount];
		for (int i = 0; i < trans.childCount; i++)
		{
			particles[i] = trans.GetChild(i).gameObject;
		}
		while (true)
		{
			GameObject[] array = particles;
			foreach (GameObject particle in array)
			{
				particle.SetActive(!dialog.isOpen());
			}
			yield return null;
		}
	}
}
