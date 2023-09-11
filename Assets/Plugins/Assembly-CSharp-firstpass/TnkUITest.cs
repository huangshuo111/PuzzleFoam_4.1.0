using TnkAd;
using UnityEngine;

public class TnkUITest : MonoBehaviour
{
	private void Start()
	{
		Plugin.Instance.initInstance();
		Plugin.Instance.setUserName("haha12");
		Plugin.Instance.showVideoCloseButton(false);
	}

	private void Update()
	{
		if (Application.platform == RuntimePlatform.Android && Input.GetKeyUp(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(100f, 100f, 150f, 80f), "Interstitial Ad"))
		{
			Debug.Log("interstitial Ad Visible = " + Plugin.Instance.isInterstitialAdVisible());
			Plugin.Instance.prepareInterstitialAd("notice_start", "testhandler");
		}
		if (GUI.Button(new Rect(100f, 200f, 150f, 80f), "Video Ad"))
		{
			Debug.Log("Video Ad!");
			Plugin.Instance.prepareVideoAdOnce("intro_video", "testhandler");
		}
		if (GUI.Button(new Rect(100f, 300f, 150f, 80f), "Show Offerwall"))
		{
			Debug.Log("Offerwall Ad");
			Plugin.Instance.popupAdList("Free Free Free!", "testhandler");
		}
		if (GUI.Button(new Rect(100f, 400f, 150f, 80f), "Show MoreApps"))
		{
			Debug.Log("MoreApps Ad");
			Plugin.Instance.popupMoreApps("Hot Free Apps!");
		}
		if (GUI.Button(new Rect(100f, 500f, 150f, 80f), "Query point"))
		{
			Debug.Log("Query point");
			Plugin.Instance.withdrawPoints("for test", "testhandler");
		}
		if (GUI.Button(new Rect(100f, 600f, 150f, 80f), "Purchase Item"))
		{
			Debug.Log("Purchase Item");
			Plugin.Instance.purchaseItem(100, "item01", "testhandler");
		}
		if (GUI.Button(new Rect(100f, 700f, 150f, 80f), "ETC Test 1"))
		{
			Debug.Log("ETC Test 1");
		}
		if (GUI.Button(new Rect(100f, 800f, 150f, 80f), "ETC Test 2"))
		{
			Debug.Log("ETC Test 2");
			Plugin.Instance.queryPublishState("testhandler");
		}
	}
}
