using System.Collections;
using UnityEngine;

public class DialogPolicy : DialogBase
{
	private enum eMargin
	{
		Top = 0,
		Bottom = 1,
		Left = 2,
		Right = 3,
		Max = 4
	}

	protected WebViewObject webViewObject1_;

	protected WebViewObject webViewObject2_;

	private int[] margins_ = new int[4] { 190, 540, 90, 90 };

	private int[] margins2 = new int[4] { 565, 165, 90, 90 };

	private UICheckbox checkBox1_;

	private UICheckbox checkBox2_;

	private bool bPlaySE_;

	private bool mPolicy1Flag;

	private bool mPolicy2Flag;

	private readonly string privacy_policy = "http://14.63.170.51:10080/view/a/policy_service";

	private readonly string terms_of_service = "http://14.63.170.51:10080/view/a/policy_privacy";

	public override void OnCreate()
	{
		mPolicy1Flag = false;
		mPolicy2Flag = false;
		calcMargine(ref margins_);
		calcMargine(ref margins2);
		Transform transform = base.transform.Find("window");
		checkBox1_ = transform.transform.Find("Checkbox1").GetComponent<UICheckbox>();
		checkBox2_ = transform.transform.Find("Checkbox2").GetComponent<UICheckbox>();
	}

	public void preLoad()
	{
		webViewObject1_ = new GameObject("WebViewObject_Policy1").AddComponent<WebViewObject>();
		webViewObject1_.Init();
		webViewObject1_.LoadURL(privacy_policy);
		webViewObject1_.SetMargins(margins_[2], margins_[0], margins_[3], margins_[1]);
		webViewObject1_.SetVisibility(false);
		webViewObject2_ = new GameObject("WebViewObject_Policy2").AddComponent<WebViewObject>();
		webViewObject2_.Init();
		webViewObject2_.LoadURL(terms_of_service);
		webViewObject2_.SetMargins(margins2[2], margins2[0], margins2[3], margins2[1]);
		webViewObject2_.SetVisibility(false);
	}

	public IEnumerator show()
	{
		if (webViewObject1_ == null || webViewObject2_ == null)
		{
			preLoad();
		}
		bPlaySE_ = false;
		Sound.Instance.playSe(Sound.eSe.SE_220_count);
		yield return dialogManager_.StartCoroutine(dialogManager_.openDialog(this));
		bPlaySE_ = true;
		webViewObject1_.SetVisibility(true);
		webViewObject2_.SetVisibility(true);
		mPolicy1Flag = false;
		mPolicy2Flag = false;
		checkBox1_.isChecked = mPolicy1Flag;
		checkBox2_.isChecked = mPolicy2Flag;
		webViewObject1_.EvaluateJS("window.addEventListener('load', function() {\twindow.addEventListener('click', function() {\t\tUnity.call('clicked');\t}, false);}, false);");
		webViewObject2_.EvaluateJS("window.addEventListener('load', function() {\twindow.addEventListener('click', function() {\t\tUnity.call('clicked');\t}, false);}, false);");
		if (Input.enableCount == 0)
		{
			Input.enable = true;
		}
	}

	private void OnCheckBox(bool bActive)
	{
		if (bPlaySE_)
		{
			Constant.SoundUtil.PlayButtonSE();
		}
	}

	protected virtual IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "AgreeButton1":
			Constant.SoundUtil.PlayDecideSE();
			mPolicy1Flag = !mPolicy1Flag;
			checkBox1_.isChecked = mPolicy1Flag;
			break;
		case "AgreeButton2":
			Constant.SoundUtil.PlayDecideSE();
			mPolicy2Flag = !mPolicy2Flag;
			checkBox2_.isChecked = mPolicy2Flag;
			break;
		}
		if (mPolicy1Flag && mPolicy2Flag)
		{
			closeWebView();
			yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(this));
		}
	}

	protected void closeWebView()
	{
		if (webViewObject1_ != null)
		{
			Object.Destroy(webViewObject1_.gameObject);
			webViewObject1_ = null;
		}
		if (webViewObject2_ != null)
		{
			Object.Destroy(webViewObject2_.gameObject);
			webViewObject2_ = null;
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (isOpen())
		{
			if (webViewObject1_ != null)
			{
				webViewObject1_.SetVisibility(!pause);
			}
			if (webViewObject2_ != null)
			{
				webViewObject2_.SetVisibility(!pause);
			}
		}
	}

	public void showWebView(bool bShow)
	{
		webViewObject1_.SetVisibility(bShow);
		webViewObject2_.SetVisibility(bShow);
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
		margins[0] = (int)((float)margins[0] * vector3.y + zero.y * vector3.y);
		margins[1] = (int)((float)margins[1] * vector3.y + zero.y * vector3.y);
		margins[2] = (int)((float)margins[2] * vector3.x + zero.x * vector3.x);
		margins[3] = (int)((float)margins[3] * vector3.x + zero.x * vector3.x);
	}
}
