using System.Collections;
using Bridge;
using Network;
using UnityEngine;

public class Area : MonoBehaviour
{
	public enum eMap
	{
		Normal = 0,
		Event = 1,
		Challenge = 2,
		Collaboration = 3
	}

	[SerializeField]
	private int AreaNo;

	[SerializeField]
	private GameObject Mask;

	[SerializeField]
	private GameObject GateRoot;

	[SerializeField]
	private GameObject Gate;

	[SerializeField]
	private GameObject GateOpen;

	[SerializeField]
	private GameObject AreaOpen;

	private GameObject openEffect_;

	private GameObject star_;

	private int termStarsNum;

	public bool bUnlocked_;

	public MapCamera mapCamera_;

	[SerializeField]
	public GameObject root_2;

	public void init(GameObject openEffect, GameObject part)
	{
		openEffect_ = openEffect;
		if (GateRoot != null)
		{
			GameObject @object = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
			StageDataTable component = @object.GetComponent<StageDataTable>();
			int num = 0;
			for (int i = 0; i < component.getStageData().Infos.Length; i++)
			{
				if (component.getInfo(i).Area == AreaNo)
				{
					num = i;
					break;
				}
			}
			int[] unlockedStages = GlobalData.Instance.unlockedStages;
			int[] array = unlockedStages;
			foreach (int num2 in array)
			{
				if (num == num2 - 1)
				{
					bUnlocked_ = true;
				}
			}
			termStarsNum = component.getInfo(num).Common.EntryStars;
			star_ = GateRoot.transform.Find("star").gameObject;
			if ((bUnlocked_ || termStarsNum <= 0) && star_ != null)
			{
				star_.SetActive(false);
				if (GateOpen != null)
				{
					GateOpen.SetActive(false);
				}
				if (AreaOpen != null)
				{
					AreaOpen.SetActive(false);
				}
			}
		}
		if (Gate != null)
		{
			UIButtonMessage component2 = Gate.GetComponent<UIButtonMessage>();
			if (component2 != null)
			{
				component2.target = part;
				component2.trigger = UIButtonMessage.Trigger.OnClick;
				component2.functionName = "OnButton";
			}
		}
	}

	public void setEffect(GameObject effect)
	{
		openEffect_ = effect;
	}

	public void open()
	{
		if (AreaNo != 0)
		{
			if (Gate != null)
			{
				Gate.SetActive(false);
			}
			if (Mask != null)
			{
				Mask.SetActive(false);
			}
			if (GateOpen != null)
			{
				GateOpen.SetActive(false);
			}
			if (AreaOpen != null)
			{
				AreaOpen.SetActive(false);
			}
			if (star_ != null)
			{
				star_.SetActive(false);
			}
		}
	}

	public int getAreaNo()
	{
		return AreaNo;
	}

	public void setAreaNo(int no)
	{
		AreaNo = no;
	}

	public IEnumerator openDirect(eMap map)
	{
		if (AreaNo == 0)
		{
			yield break;
		}
		if (map == eMap.Event || map == eMap.Challenge)
		{
			if (map == eMap.Event && root_2 != null)
			{
				root_2.SetActive(false);
			}
			if (mapCamera_ != null)
			{
				Transform gateTrans = GateRoot.transform;
				Vector3 gatePos = gateTrans.localPosition + gateTrans.parent.localPosition + gateTrans.parent.parent.localPosition;
				yield return StartCoroutine(mapCamera_.moveProd(gatePos));
			}
			base.gameObject.SetActive(true);
			Animation anim = base.gameObject.GetComponent<Animation>();
			anim.Play();
			while (anim.isPlaying)
			{
				yield return 0;
			}
			yield return new WaitForSeconds(0.5f);
			TweenAlpha tw = Gate.GetComponent<TweenAlpha>();
			tw.enabled = true;
			while (tw.enabled)
			{
				yield return 0;
			}
			yield break;
		}
		if (mapCamera_ != null)
		{
			Transform gateTrans2 = GateRoot.transform;
			Vector3 gatePos2 = gateTrans2.localPosition + gateTrans2.parent.localPosition + gateTrans2.parent.parent.localPosition;
			yield return StartCoroutine(mapCamera_.moveProd(gatePos2));
		}
		Sound.Instance.playSe(Sound.eSe.SE_245_door);
		Utility.setParent(openEffect_, GateRoot.transform, false);
		openEffect_.SetActive(true);
		openEffect_.GetComponent<Animation>().Play();
		yield return new WaitForSeconds(0.5f);
		Gate.SetActive(false);
		AreaOpen.SetActive(false);
		GateOpen.SetActive(false);
		star_.SetActive(false);
		if (Mask != null)
		{
			TweenAlpha alpha = Mask.GetComponent<TweenAlpha>();
			alpha.Play(true);
			while (alpha.enabled)
			{
				yield return 0;
			}
			Mask.SetActive(false);
		}
		while (openEffect_.GetComponent<Animation>().isPlaying)
		{
			yield return 0;
		}
		openEffect_.SetActive(false);
	}

	public IEnumerator openDirect(eMap map, bool bKeyOpen)
	{
		if (AreaNo == 0 || (map != eMap.Event && map != eMap.Challenge && map != eMap.Collaboration))
		{
			yield break;
		}
		switch (map)
		{
		case eMap.Event:
			Gate.SetActive(!bKeyOpen);
			if (root_2 != null)
			{
				root_2.SetActive(false);
			}
			break;
		default:
			Gate.SetActive(false);
			break;
		case eMap.Collaboration:
			break;
		}
		if (mapCamera_ != null)
		{
			if (map == eMap.Collaboration)
			{
				yield return StartCoroutine(mapCamera_.moveProd(base.transform.localPosition));
			}
			else
			{
				Transform gateTrans = GateRoot.transform;
				Vector3 gatePos = gateTrans.localPosition + gateTrans.parent.localPosition + gateTrans.parent.parent.localPosition;
				yield return StartCoroutine(mapCamera_.moveProd(gatePos));
			}
		}
		base.gameObject.SetActive(true);
		Animation anim = base.gameObject.GetComponent<Animation>();
		anim.Play();
		while (anim.isPlaying)
		{
			yield return 0;
		}
	}

	public IEnumerator openKeyDirect()
	{
		Utility.setParent(openEffect_, GateRoot.transform, false);
		openEffect_.SetActive(true);
		openEffect_.GetComponent<Animation>().Play();
		TweenAlpha tw = Gate.GetComponent<TweenAlpha>();
		tw.Reset();
		tw.Play(true);
		while (tw.enabled)
		{
			yield return 0;
		}
	}

	public IEnumerator showRoot2()
	{
		if (!(root_2 == null))
		{
			root_2.SetActive(true);
			TweenAlpha tw = root_2.GetComponentInChildren<TweenAlpha>();
			tw.Reset();
			tw.Play(true);
			while (tw.enabled)
			{
				yield return 0;
			}
		}
	}

	public void updateStarsNumLabel(bool enable)
	{
		if (star_ == null || !star_.activeSelf)
		{
			return;
		}
		UILabel component = star_.transform.Find("Label").GetComponent<UILabel>();
		int areaStar = Bridge.StageData.getAreaStar(AreaNo - 1);
		if (bUnlocked_ || (areaStar >= termStarsNum && !enable))
		{
			star_.SetActive(false);
			if (GateOpen != null)
			{
				GateOpen.SetActive(false);
			}
			if (AreaOpen != null)
			{
				AreaOpen.SetActive(false);
			}
		}
		else
		{
			int num = Mathf.Min(areaStar, termStarsNum);
			string message = MessageResource.Instance.getMessage(19);
			message = MessageResource.Instance.castCtrlCode(message, 1, num.ToString());
			message = MessageResource.Instance.castCtrlCode(message, 2, termStarsNum.ToString());
			component.text = message;
		}
	}

	public bool isCompleateTerm()
	{
		return bUnlocked_ || termStarsNum <= Bridge.StageData.getAreaStar(AreaNo - 1);
	}

	public void updateSale()
	{
		if (base.transform.Find("icon_sale") == null)
		{
			return;
		}
		bool flag = false;
		GameObject gameObject = base.transform.Find("icon_sale").gameObject;
		GameData gameData = GlobalData.Instance.getGameData();
		if (gameData.saleStageItemArea != null)
		{
			int[] saleStageItemArea = gameData.saleStageItemArea;
			foreach (int num in saleStageItemArea)
			{
				if (num < 10000 && num == AreaNo)
				{
					gameObject.SetActive(true);
					gameObject.GetComponent<UISprite>().spriteName = "UI_shop_sale2_map";
					flag = true;
					break;
				}
			}
		}
		else
		{
			gameObject.SetActive(false);
		}
		if (gameData.saleArea != null)
		{
			int[] saleArea = gameData.saleArea;
			foreach (int num2 in saleArea)
			{
				if (num2 == AreaNo)
				{
					gameObject.SetActive(true);
					gameObject.GetComponent<UISprite>().spriteName = "UI_shop_sale_map";
					return;
				}
			}
		}
		else if (!flag)
		{
			gameObject.SetActive(false);
			return;
		}
		if (!flag)
		{
			gameObject.SetActive(false);
		}
	}

	public void setGateButtonEnable(bool enable)
	{
		if (Gate != null)
		{
			BoxCollider component = Gate.GetComponent<BoxCollider>();
			if (component != null)
			{
				component.enabled = enable;
			}
		}
		if (GateOpen != null)
		{
			GateOpen.SetActive(enable);
		}
		if (AreaOpen != null)
		{
			AreaOpen.SetActive(enable);
		}
	}

	public void setGateEnable(bool enable)
	{
		if (Gate != null)
		{
			Gate.SetActive(enable);
		}
	}
}
