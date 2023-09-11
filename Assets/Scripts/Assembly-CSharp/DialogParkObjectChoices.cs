using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogParkObjectChoices : DialogBase
{
	public enum eUpdateType
	{
		WatchingLongPress = 0,
		Tracking = 1,
		None = 2
	}

	public class ChoiceButton
	{
		private UIButton button_;

		private UIButtonScale[] scales_;

		private UIButtonMessage[] messages_;

		private UIButtonSound[] sounds_;

		private bool isEnable_ = true;

		public void setup(UIButton button)
		{
			button_ = button;
			scales_ = button.GetComponents<UIButtonScale>();
			messages_ = button.GetComponents<UIButtonMessage>();
			sounds_ = button.GetComponents<UIButtonSound>();
			isEnable_ = button.enabled;
		}

		public void setEnable(bool enable)
		{
			if (isEnable_ == enable)
			{
				return;
			}
			button_.setEnable(enable);
			button_.enabled = enable;
			button_.GetComponent<Collider>().enabled = true;
			UIButtonScale[] array = scales_;
			foreach (UIButtonScale uIButtonScale in array)
			{
				uIButtonScale.enabled = enable;
			}
			UIButtonMessage[] array2 = messages_;
			foreach (UIButtonMessage uIButtonMessage in array2)
			{
				uIButtonMessage.enabled = enable;
			}
			UIButtonSound[] array3 = sounds_;
			foreach (UIButtonSound uIButtonSound in array3)
			{
				uIButtonSound.enabled = enable;
			}
			TweenColor[] componentsInChildren = button_.GetComponentsInChildren<TweenColor>(false);
			TweenColor[] array4 = componentsInChildren;
			foreach (TweenColor tweenColor in array4)
			{
				if (enable)
				{
					tweenColor.color = button_.defaultColor;
				}
				else
				{
					tweenColor.color = button_.disabledColor;
				}
			}
			isEnable_ = enable;
		}
	}

	private const float ARROW_DECREASE_COEFFICIENT = 2f;

	private eUpdateType updateType_ = eUpdateType.None;

	private GameObject gaugeRoot_;

	private UIFilledSprite gaugeSprite_;

	private float gaugePrevFillAmount_;

	private Vector3 gaugeOffset_;

	private Building gaugeWatchingObject_;

	private GameObject alreadyRoot_;

	private ChoiceButton alreadyOKButton_ = new ChoiceButton();

	private ChoiceButton alreadyInvertButton_ = new ChoiceButton();

	private GameObject firstRoot_;

	private ChoiceButton firstOKButton_ = new ChoiceButton();

	private ChoiceButton firstInvertButton_ = new ChoiceButton();

	private bool isAlready_;

	private Building trackingObject_;

	private Vector3 trackingOffset_;

	private ParkStructures.IntegerXY initialGridIndex_;

	private ParkObject.eDirection initialDirection_;

	public bool canOtherClickDecision { get; set; }

	public static Vector3 ConvertPositionFromUnity2DToNGUI(Camera mapCamera, Vector3 worldPosition)
	{
		Vector3 position = mapCamera.WorldToViewportPoint(worldPosition);
		Vector3 result = UICamera.currentCamera.ViewportToWorldPoint(position);
		result.z = 0f;
		return result;
	}

	public IEnumerator setup()
	{
		gaugeRoot_ = base.transform.Find("Gauge").gameObject;
		gaugeSprite_ = base.transform.Find("Gauge/gauge").GetComponent<UIFilledSprite>();
		gaugeSprite_.fillAmount = 0f;
		gaugeOffset_ = gaugeRoot_.transform.localPosition;
		gaugeRoot_.transform.localPosition = Vector3.zero;
		alreadyRoot_ = base.transform.Find("Already").gameObject;
		alreadyOKButton_.setup(alreadyRoot_.transform.Find("ok_button").GetComponent<UIButton>());
		alreadyInvertButton_.setup(alreadyRoot_.transform.Find("invert_button").GetComponent<UIButton>());
		trackingOffset_ = alreadyRoot_.transform.localPosition;
		alreadyRoot_.transform.localPosition = Vector3.zero;
		firstRoot_ = base.transform.Find("First").gameObject;
		firstOKButton_.setup(firstRoot_.transform.Find("ok_button").GetComponent<UIButton>());
		firstInvertButton_.setup(firstRoot_.transform.Find("invert_button").GetComponent<UIButton>());
		firstRoot_.transform.localPosition = Vector3.zero;
		gaugeRoot_.SetActive(false);
		alreadyRoot_.SetActive(false);
		firstRoot_.SetActive(false);
		canOtherClickDecision = true;
		yield break;
	}

	private void LateUpdate()
	{
		switch (updateType_)
		{
		case eUpdateType.WatchingLongPress:
			WatchLongPress();
			break;
		case eUpdateType.Tracking:
			Track();
			break;
		case eUpdateType.None:
			PostProcess();
			break;
		}
	}

	public void StartWatchingLongPress(Building building)
	{
		if (updateType_ != eUpdateType.Tracking)
		{
			if (gaugeWatchingObject_ == building)
			{
				gaugeSprite_.fillAmount = gaugePrevFillAmount_;
			}
			else
			{
				gaugePrevFillAmount_ = 0f;
				gaugeSprite_.fillAmount = 0f;
			}
			gaugeWatchingObject_ = building;
			updateType_ = eUpdateType.WatchingLongPress;
			gaugeRoot_.SetActive(true);
			firstRoot_.SetActive(false);
			alreadyRoot_.SetActive(false);
		}
	}

	private void repositionForWatchLongPress()
	{
		ParkObjectManager instance = ParkObjectManager.Instance;
		Vector3 localPosition = gaugeWatchingObject_.cachedTransform.localPosition;
		float currentZoom = instance.mapScroll.currentZoom;
		Vector3 position = new Vector3(gaugeWatchingObject_.objectCenter.x, gaugeWatchingObject_.objectCenter.y, 10f);
		position.x -= gaugeWatchingObject_.spriteSize.width;
		position.y += gaugeSprite_.transform.localScale.y / 2f;
		gaugeWatchingObject_.setPosition(position);
		Vector3 position2 = ConvertPositionFromUnity2DToNGUI(instance.mapCamera, gaugeWatchingObject_.cachedTransform.position);
		base.transform.position = position2;
		gaugeWatchingObject_.setPosition(localPosition);
		gaugeRoot_.transform.localScale = new Vector3(currentZoom / 3f, currentZoom / 3f, 1f);
	}

	public void WatchLongPress()
	{
		if (gaugeWatchingObject_ == null)
		{
			return;
		}
		repositionForWatchLongPress();
		ColliderManager instance = ColliderManager.Instance;
		gaugeSprite_.fillAmount = Mathf.Clamp01(instance.pressTime / instance.longPressTime) + gaugePrevFillAmount_;
		if (instance.pressTime > 0.1f)
		{
			if (!gaugeRoot_.activeSelf)
			{
				gaugeRoot_.SetActive(true);
			}
		}
		else if (gaugePrevFillAmount_ <= 0f && gaugeRoot_.activeSelf)
		{
			gaugeRoot_.SetActive(false);
		}
		if (gaugeSprite_.fillAmount >= 1f)
		{
			StartTracking(gaugeWatchingObject_, false);
			gaugeWatchingObject_ = null;
			gaugePrevFillAmount_ = 0f;
			gaugeSprite_.fillAmount = 0f;
		}
	}

	public void EndWatchingLongPress(bool savePrevFillAmount = true)
	{
		if (updateType_ != eUpdateType.Tracking && !(gaugeWatchingObject_ == null))
		{
			if (savePrevFillAmount)
			{
				gaugePrevFillAmount_ = gaugeSprite_.fillAmount;
			}
			else
			{
				gaugePrevFillAmount_ = 0f;
				gaugeSprite_.fillAmount = 0f;
				gaugeRoot_.SetActive(false);
			}
			updateType_ = eUpdateType.None;
		}
	}

	public void StartTracking(Building building, bool isFirst = true)
	{
		if (!(building == null) && !(trackingObject_ != null))
		{
			if (Sound.Instance.se_clip.Length > 131)
			{
				Sound.Instance.playSe(Sound.eSe.SE_702_park_move, false);
			}
			trackingObject_ = building;
			updateType_ = eUpdateType.Tracking;
			initialGridIndex_ = building.index;
			initialDirection_ = building.direction;
			gaugeRoot_.SetActive(false);
			trackingObject_.OnSelect();
			if (isFirst)
			{
				alreadyRoot_.SetActive(false);
				firstRoot_.SetActive(true);
				isAlready_ = false;
				firstInvertButton_.setEnable(trackingObject_.objectType != ParkObject.eType.Fence);
			}
			else
			{
				alreadyRoot_.SetActive(true);
				firstRoot_.SetActive(false);
				isAlready_ = true;
				alreadyInvertButton_.setEnable(trackingObject_.objectType != ParkObject.eType.Fence);
			}
		}
	}

	private void Track()
	{
		ParkObjectManager instance = ParkObjectManager.Instance;
		Vector3 localPosition = trackingObject_.cachedTransform.localPosition;
		float currentZoom = instance.mapScroll.currentZoom;
		Vector3 position = new Vector3(trackingObject_.objectCenter.x, trackingObject_.objectCenter.y, 10f);
		position += trackingOffset_;
		trackingObject_.setPosition(position);
		Vector3 position2 = ConvertPositionFromUnity2DToNGUI(instance.mapCamera, trackingObject_.cachedTransform.position);
		base.transform.position = position2;
		trackingObject_.setPosition(localPosition);
		if (isAlready_)
		{
			alreadyOKButton_.setEnable(trackingObject_.canReplace);
			alreadyRoot_.transform.localScale = new Vector3(currentZoom / 2f, currentZoom / 2f, 1f);
		}
		else
		{
			firstOKButton_.setEnable(trackingObject_.canReplace);
			firstRoot_.transform.localScale = new Vector3(currentZoom / 2f, currentZoom / 2f, 1f);
		}
		if (!canOtherClickDecision)
		{
			return;
		}
		ColliderManager instance2 = ColliderManager.Instance;
		EventHandler lastClickObject = instance2.lastClickObject;
		if (!Input.GetMouseButtonUp(0) || !trackingObject_.canReplace)
		{
			return;
		}
		if (lastClickObject == null)
		{
			if (!instance.mapScroll.isScrolling && !instance.mapScroll.isPinch)
			{
				EventHandler draggingObject = instance2.draggingObject;
				if ((!(draggingObject != null) || !draggingObject.Equals(trackingObject_)) && !instance2.isDownNGUIObject)
				{
					StartCoroutine(OnOK());
				}
			}
		}
		else if (!lastClickObject.Equals(trackingObject_))
		{
			StartCoroutine(OnOK());
		}
	}

	public void EndTracking(bool forceReturn = false)
	{
		if (trackingObject_ != null)
		{
			if (forceReturn)
			{
				returnOriginalGrid();
			}
			if (isAlready_)
			{
				trackingObject_.OnDeselect();
			}
			else
			{
				trackingObject_.OnDeselect(false);
			}
		}
		trackingObject_ = null;
		updateType_ = eUpdateType.None;
		gaugeRoot_.SetActive(false);
		firstRoot_.SetActive(false);
		alreadyRoot_.SetActive(false);
	}

	private void PostProcess()
	{
		if (gaugeWatchingObject_ != null && gaugeRoot_.activeSelf)
		{
			repositionForWatchLongPress();
			if (gaugePrevFillAmount_ > 0f)
			{
				gaugePrevFillAmount_ = Mathf.Clamp01(gaugeSprite_.fillAmount - Time.deltaTime * 2f);
				gaugeSprite_.fillAmount = gaugePrevFillAmount_;
			}
			else if (gaugeRoot_.activeSelf)
			{
				gaugeRoot_.SetActive(false);
			}
		}
	}

	private IEnumerator OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "remove_button":
			StartCoroutine(OnRemove());
			break;
		case "invert_button":
			OnInvert();
			break;
		case "ok_button":
			yield return StartCoroutine(OnOK());
			break;
		case "cancel_button":
			OnCancel();
			break;
		}
	}

	private IEnumerator OnRemove()
	{
		Constant.SoundUtil.PlayButtonSE();
		DialogParkRemoveConfirmation dialog = dialogManager_.getDialog(DialogManager.eDialog.ParkRemoveConfirm) as DialogParkRemoveConfirmation;
		dialog.setSelectedObject(trackingObject_);
		yield return StartCoroutine(dialog.open());
		returnOriginalGrid();
		trackingObject_.EndDragMoving();
		EndTracking();
		ParkObjectManager.Instance.mapScroll.EndAutoScroll();
	}

	private void OnInvert()
	{
		Constant.SoundUtil.PlayButtonSE();
		trackingObject_.InvertDirection();
	}

	private IEnumerator OnOK()
	{
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		Building temp = trackingObject_;
		if (objectManager.viewMode == ParkObjectManager.eParkViewMode.Player)
		{
			SaveData.Instance.getParkData().UpdateBuildingData(trackingObject_, initialGridIndex_);
			SaveData.Instance.getParkData().save();
		}
		trackingObject_.EndDragMoving();
		objectManager.mapScroll.EndAutoScroll();
		EndTracking();
		trackingObject_ = null;
		if (isAlready_)
		{
			objectManager.RemoveRelationGrid(initialGridIndex_, temp.gridSize, initialDirection_);
			if (Sound.Instance.se_clip.Length > 132)
			{
				Sound.Instance.playSe(Sound.eSe.SE_702_park_locate, false);
			}
		}
		else
		{
			yield return StartCoroutine(objectManager.SendMapUpdate());
			gaugeRoot_.SetActive(false);
			firstRoot_.SetActive(false);
			alreadyRoot_.SetActive(false);
			List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
			ParkObjectManager.getRelationalIndices(temp, ref indices);
			ParkBuildingDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkBuildingDataTable>();
			bool useEffect = true;
			if (dataTable != null)
			{
				ParkBuildingInfo.BuildingInfo info = dataTable.getInfo(temp.objectID);
				if (info != null)
				{
					useEffect = info.UseBuildingEffect;
				}
			}
			if (useEffect)
			{
				StartCoroutine(WaitSlideIn(temp));
				yield return objectManager.StartCoroutine(objectManager.buildingEffects.PlaySequentialEffect(temp.index, indices, temp.spriteSize, false, OnFinishSlideIn, OnFinishedSmoke));
			}
			else
			{
				temp.StopAllCoroutines();
				temp.setAlpha(1f);
			}
		}
		objectManager.AddRelationGrid(temp);
	}

	public IEnumerator OnFinishSlideIn()
	{
		if (Sound.Instance.se_clip.Length > 130)
		{
			Sound.Instance.playSe(Sound.eSe.SE_701_park_build, false);
		}
		yield break;
	}

	public IEnumerator WaitSlideIn(Building target)
	{
		yield return new WaitForSeconds(2f);
		target.StopAllCoroutines();
		StartCoroutine(target.fadeInAlpha(0f));
	}

	public IEnumerator OnFinishedSmoke()
	{
		yield return new WaitForSeconds(0.3f);
		Sound.Instance.playSe(Sound.eSe.SE_113_Clap);
	}

	private void OnCancel()
	{
		Constant.SoundUtil.PlayCancelSE();
		MapScroll mapScroll = ParkObjectManager.Instance.mapScroll;
		if (isAlready_)
		{
			returnOriginalGrid();
			trackingObject_.EndDragMoving();
			EndTracking();
			mapScroll.EndAutoScroll();
			return;
		}
		trackingObject_.EndDragMoving();
		ParkObjectManager.Instance.Remove(trackingObject_);
		trackingObject_.OnRemove();
		Object.Destroy(trackingObject_.gameObject);
		trackingObject_ = null;
		updateType_ = eUpdateType.None;
		gaugeRoot_.SetActive(false);
		firstRoot_.SetActive(false);
		alreadyRoot_.SetActive(false);
		mapScroll.EndAutoScroll();
		mapScroll.enableScroll = true;
	}

	private void returnOriginalGrid()
	{
		trackingObject_.horizontalIndex = initialGridIndex_.x;
		trackingObject_.verticalIndex = initialGridIndex_.y;
		trackingObject_.setRecalculatedOrder();
		if (trackingObject_.direction != initialDirection_)
		{
			trackingObject_.direction = initialDirection_;
		}
	}
}
