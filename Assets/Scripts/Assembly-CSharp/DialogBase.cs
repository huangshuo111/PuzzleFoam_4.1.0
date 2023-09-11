using System.Collections;
using UnityEngine;

public class DialogBase : MonoBehaviour
{
	private TweenGroup openTween_;

	private TweenGroup closeTween_;

	protected DialogManager dialogManager_;

	protected FadeMng fadeManager_;

	protected PartManager partManager_;

	protected DialogManager.eDialog dialogType_;

	private bool bOpen_;

	private GameObject confetti_eff;

	public virtual void OnCreate()
	{
	}

	public virtual void OnOpen()
	{
	}

	public virtual void OnClose()
	{
	}

	public virtual void OnStartClose()
	{
	}

	public void init(DialogManager dialogMng, PartManager partMng, FadeMng fadeMng, DialogManager.eDialog dialog)
	{
		TweenGroup[] components = GetComponents<TweenGroup>();
		TweenGroup[] array = components;
		foreach (TweenGroup tweenGroup in array)
		{
			switch (tweenGroup.getGroupName())
			{
			case "In":
				openTween_ = tweenGroup;
				break;
			case "Out":
				closeTween_ = tweenGroup;
				break;
			}
		}
		dialogType_ = dialog;
		dialogManager_ = dialogMng;
		partManager_ = partMng;
		fadeManager_ = fadeMng;
	}

	public bool isOpen()
	{
		return bOpen_;
	}

	public virtual IEnumerator open()
	{
		Input.enable = false;
		bOpen_ = true;
		base.gameObject.SetActive(true);
		yield return dialogManager_.StartCoroutine(playAnimation(openTween_));
		Input.enable = true;
		OnOpen();
	}

	public virtual IEnumerator close()
	{
		Input.enable = false;
		stopConfettiEff();
		OnStartClose();
		yield return dialogManager_.StartCoroutine(playAnimation(closeTween_));
		base.gameObject.SetActive(false);
		bOpen_ = false;
		Input.enable = true;
		OnClose();
	}

	public bool isPlayingAnimation()
	{
		if (openTween_ == null)
		{
			return false;
		}
		if (closeTween_ == null)
		{
			return false;
		}
		if (openTween_.isPlaying() || closeTween_.isPlaying())
		{
			return true;
		}
		return false;
	}

	public bool isPlayingOpenAnime()
	{
		if (openTween_ == null)
		{
			return false;
		}
		return openTween_.isPlaying();
	}

	public bool isPlayingCloseAnime()
	{
		if (closeTween_ == null)
		{
			return false;
		}
		return closeTween_.isPlaying();
	}

	private IEnumerator playAnimation(TweenGroup tween)
	{
		if (!(tween == null))
		{
			tween.Play();
			while (tween.isPlaying())
			{
				yield return 0;
			}
		}
	}

	public DialogManager.eDialog getDialogType()
	{
		return dialogType_;
	}

	protected void startConfettiEff()
	{
		if (confetti_eff == null)
		{
			confetti_eff = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "confetti_eff")) as GameObject;
			Vector3 localPosition = confetti_eff.transform.localPosition;
			Utility.setParent(confetti_eff, base.transform, false);
			confetti_eff.transform.localPosition = localPosition;
			ConfettiEff confettiEff = confetti_eff.AddComponent<ConfettiEff>();
			confettiEff.checkAppQuitDialog(dialogManager_);
		}
	}

	protected void stopConfettiEff()
	{
		if (confetti_eff != null)
		{
			Object.Destroy(confetti_eff);
			confetti_eff = null;
		}
	}
}
