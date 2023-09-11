using System.Collections;
using UnityEngine;

public class DialogBannerInformation : DialogBase
{
	private enum eMargin
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		Max = 4
	}

	private const float MAX_X_SCALE = 450f;

	protected WebViewObject webViewObject_;

	private int[] margins_ = new int[4] { 170, 245, 95, 95 };

	private Texture2D texture;

	private Transform image;

	private GameObject ScrollObject_;

	private UIPanel panel_;

	private UICheckbox checkBox_;

	private bool bPlaySE_;

	public bool bOping;

	private bool loadSuccess;

	public override void OnCreate()
	{
		calcMargine(ref margins_);
		checkBox_ = GetComponentsInChildren<UICheckbox>(true)[0];
		checkBox_.gameObject.SetActive(false);
		base.transform.Find("window/Title_Information").gameObject.SetActive(true);
		base.transform.Find("window/title_plate").gameObject.SetActive(true);
		base.transform.Find("window/Checkbox_Label").gameObject.SetActive(false);
		panel_ = base.transform.Find("DragPanel").GetComponent<UIPanel>();
	}

	public IEnumerator show(string url)
	{
		bPlaySE_ = false;
		bOping = true;
		Debug.Log("DialogBannerInformation_dialog : URL = " + url);
		if (loadSuccess && texture != null)
		{
			yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
			base.transform.Find("DragPanel").gameObject.SetActive(true);
			image = base.transform.Find("DragPanel/contents/Image");
			UITexture tex = image.Find("image_00").GetComponent<UITexture>();
			tex.mainTexture = texture;
			tex.MakePixelPerfect();
			if (tex.transform.localScale.x > 450f)
			{
				float rate = 450f / tex.transform.localScale.x;
				tex.transform.localScale = new Vector3(tex.transform.localScale.x * rate, tex.transform.localScale.y * rate, tex.transform.localScale.z);
			}
			image.gameObject.SetActive(true);
			base.transform.Find("DragPanel").GetComponent<UIDraggablePanel>().ResetPosition();
			float clipRange = panel_.clipRange.w;
			if (clipRange >= tex.transform.localScale.y)
			{
				base.transform.Find("DragPanel").GetComponent<UIDraggablePanel>().enabled = false;
			}
			else
			{
				base.transform.Find("DragPanel").GetComponent<UIDraggablePanel>().enabled = true;
			}
			BoxCollider imageCollider = image.GetComponent<BoxCollider>();
			imageCollider.size = tex.transform.localScale;
			yield return null;
		}
		bPlaySE_ = true;
	}

	public IEnumerator loadTexture(string url)
	{
		int failedCount = 0;
		WWW www;
		while (true)
		{
			www = new WWW(url);
			while (!www.isDone && www.error == null)
			{
				yield return null;
			}
			if (www.error == null)
			{
				break;
			}
			www.Dispose();
			failedCount++;
			if (failedCount >= 2)
			{
				yield break;
			}
		}
		texture = www.textureNonReadable;
		loadSuccess = true;
		www.Dispose();
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "Close_Button":
			Constant.SoundUtil.PlayCancelSE();
			image.gameObject.SetActive(false);
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		case "ConfirmButton":
			Constant.SoundUtil.PlayDecideSE();
			image.gameObject.SetActive(false);
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
			break;
		}
	}

	private void calcMargine(ref int[] margins)
	{
		int num = NGUIUtilScalableUIRoot.manualHeight;
		int num2 = NGUIUtilScalableUIRoot.manualWidth;
		float num3 = (float)(Screen.height * NGUIUtilScalableUIRoot.manualWidth) / (float)(Screen.width * NGUIUtilScalableUIRoot.manualHeight);
		if (num3 > 1f)
		{
			num = (int)((float)num * num3);
		}
		if (num3 < 1f)
		{
			num2 = (int)((float)num2 * num3);
		}
		Vector2 zero = Vector2.zero;
		zero.y = (num - NGUIUtilScalableUIRoot.manualHeight) / 2;
		zero.x = (NGUIUtilScalableUIRoot.manualWidth - num2) / 2;
		Vector2 vector = new Vector2(Screen.width, Screen.height);
		Vector2 vector2 = new Vector2(NGUIUtilScalableUIRoot.manualWidth, NGUIUtilScalableUIRoot.manualHeight);
		vector2.y += zero.y * 2f;
		vector2.x += zero.x * 2f;
		Vector2 vector3 = new Vector2(vector.x / vector2.x, vector.y / vector2.y);
		margins[0] = (int)((float)margins_[0] * vector3.y + zero.y * vector3.y);
		margins[1] = (int)((float)margins_[1] * vector3.y + zero.y * vector3.y);
		margins[2] = (int)((float)margins_[2] * vector3.x + zero.x * vector3.x);
		margins[3] = (int)((float)margins_[3] * vector3.x + zero.x * vector3.x);
	}
}
