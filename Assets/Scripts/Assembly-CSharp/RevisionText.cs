using UnityEngine;

[ExecuteInEditMode]
public class RevisionText : MonoBehaviour
{
	private void Start()
	{
		updateText();
	}

	private void updateText()
	{
		TextAsset textAsset = Resources.Load("DebugMenu/revision", typeof(TextAsset)) as TextAsset;
		if (textAsset != null)
		{
			GUIText component = GetComponent<GUIText>();
			component.text = textAsset.text.Replace("\r", string.Empty).Replace("\n", string.Empty) + " comm:";
			component.text += "ON";
		}
	}
}
