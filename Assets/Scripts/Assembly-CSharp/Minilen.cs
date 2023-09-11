using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using Network;
using UnityEngine;

public class Minilen : ParkObject
{
	public enum eAnimationState
	{
		Idle = 0,
		Walk = 1,
		Sleep = 2,
		Dance = 3,
		Something = 4,
		Max = 5
	}

	public enum eAction
	{
		Walk = 0,
		Dance = 1,
		Sleep = 2
	}

	public enum eRareType
	{
		Normal = 0,
		Rare = 1,
		Max = 2
	}

	private enum eMoveDirection
	{
		UpperLeft = 0,
		UpperRight = 1,
		LowerLeft = 2,
		LowerRight = 3
	}

	public const int PART_ORDER_MAX = 30;

	public const float DANCE_ANIMATION_LENGTH = 2.25f;

	private const float DANCE_START_PROBABILITY = 0.5f;

	private const float DANCE_START_PROBABILITY_UP = 0.1f;

	private const int SLEEP_EFFECT_ORDER_ADJUST = 30;

	private const int MOVE_WAIT_TIME_MIN = 3;

	private const int MOVE_WAIT_TIME_MAX = 8;

	private const float KEEP_WALKING_PROBABILITY = 70f;

	private static readonly int[] ANIMATOR_STATE_KEYS = new int[5]
	{
		Animator.StringToHash("idle"),
		Animator.StringToHash("walk"),
		Animator.StringToHash("sleep"),
		Animator.StringToHash("dance"),
		Animator.StringToHash("something")
	};

	private bool isSetupFinished_;

	private eRareType rareType_;

	private ParkEffect sleepEffect_;

	private Animator animator_;

	private eAnimationState currentAnimationState_;

	private eAction currentAction_;

	private bool isMove_;

	private float moveWait_;

	private Grid nextGrid_;

	private Grid prevGrid_;

	private Vector3 moveStartPosition_;

	private Vector3 moveEndPosition_;

	private float moveTime_ = 2f;

	private float moveElapsedTime_;

	private float danceStartProbability_ = 0.5f;

	private float danceWait_;

	public eAnimationState animationState
	{
		get
		{
			return currentAnimationState_;
		}
	}

	public bool canAction { get; set; }

	public override IEnumerator setup(int id)
	{
		yield return StartCoroutine(base.setup(id));
		objectID_ = id;
		objectType_ = eType.Minilen;
		collider_ = GetComponentInChildren<ColliderBase>();
		collider_.eventHandler = this;
		base.enableOnClick = true;
		setupEffect();
		setupAnimator();
		int id2 = default(int);
		Network.MinilenData info = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == id2);
		if (info.rank > 0)
		{
			rareType_ = eRareType.Rare;
		}
		canAction = true;
		isSetupFinished_ = true;
	}

	public override void setupImmediate(int id)
	{
		base.setupImmediate(id);
		objectID_ = id;
		objectType_ = eType.Minilen;
		collider_ = GetComponentInChildren<ColliderBase>();
		collider_.eventHandler = this;
		base.enableOnClick = true;
		setupAnimator();
		Network.MinilenData minilenData = Array.Find(Bridge.MinilenData.getMinielenData(), (Network.MinilenData m) => m.index == id);
		if (minilenData.rank > 0)
		{
			rareType_ = eRareType.Rare;
		}
		canAction = true;
		isSetupFinished_ = true;
	}

	private void setupEffect()
	{
		sleepEffect_ = ParkObjectManager.Instance.createEffect(ParkEffect.eEffectType.Sleep);
		Vector3 localScale = cachedTransform_.localScale;
		sleepEffect_.transform.localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, localScale.z);
		sleepEffect_.transform.SetParent(ParkObjectManager.Instance.objectRoot, false);
		sleepEffect_.sortingOrder = base.sortingOrder + 30;
		sleepEffect_.Stop();
	}

	private void setupAnimator()
	{
		animator_ = GetComponentInChildren<Animator>();
		ReturnIdleAnimation();
	}

	private void Update()
	{
		if (isSetupFinished_ && canAction)
		{
			switch (currentAction_)
			{
			case eAction.Walk:
				Walk();
				break;
			case eAction.Dance:
				Dance();
				break;
			case eAction.Sleep:
				break;
			}
		}
	}

	private void Walk()
	{
		if (!isMove_)
		{
			if (moveWait_ <= 0f)
			{
				if (Utility.decideByProbability(danceStartProbability_))
				{
					danceStartProbability_ = 0.5f;
					currentAction_ = eAction.Dance;
					danceWait_ = (float)UnityEngine.Random.Range(2, 7) * 2.25f;
					PlayAnimation(eAnimationState.Dance, true);
				}
				else if (CheckNextGrid())
				{
					isMove_ = true;
					moveElapsedTime_ = 0f;
					PlayAnimation(eAnimationState.Walk, true);
				}
				else
				{
					PlayAnimation(eAnimationState.Idle, true);
				}
			}
			else
			{
				moveWait_ -= Time.deltaTime;
				int num = ParkObjectManager.Instance.findNearMinilenWithAction(this);
				if (num > 0)
				{
					danceStartProbability_ += (float)num * 0.1f;
				}
			}
			return;
		}
		Vector3 vector = Vector3.Lerp(moveStartPosition_, moveEndPosition_, Mathf.Clamp01(moveElapsedTime_ / moveTime_));
		if (moveElapsedTime_ <= moveTime_)
		{
			setPosition(vector);
			moveElapsedTime_ += Time.deltaTime;
			return;
		}
		isMove_ = false;
		horizontalIndex_ = nextGrid_.horizontalIndex;
		verticalIndex_ = nextGrid_.verticalIndex;
		setRecalculatedOrder();
		if (Utility.decideByProbability(70f))
		{
			moveWait_ = 0f;
			return;
		}
		moveWait_ = UnityEngine.Random.Range(3, 8);
		PlayAnimation(eAnimationState.Idle, true);
	}

	private void Dance()
	{
		if (danceWait_ <= 0f)
		{
			currentAction_ = eAction.Walk;
			moveWait_ = 0.5f;
			isMove_ = false;
			PlayAnimation(eAnimationState.Idle);
		}
		else
		{
			danceWait_ -= Time.deltaTime;
		}
	}

	private bool CheckNextGrid()
	{
		bool result = false;
		ParkStructures.IntegerXY baseIndex = base.index;
		List<eMoveDirection> list = new List<eMoveDirection>();
		list.Add(eMoveDirection.UpperLeft);
		list.Add(eMoveDirection.UpperRight);
		list.Add(eMoveDirection.LowerLeft);
		list.Add(eMoveDirection.LowerRight);
		List<eMoveDirection> list2 = list;
		while (list2.Count > 0)
		{
			eMoveDirection eMoveDirection = list2[UnityEngine.Random.Range(0, list2.Count)];
			eDirection eDirection = eDirection.Default;
			ParkStructures.IntegerXY integerXY = new ParkStructures.IntegerXY(-1, -1);
			bool flag = false;
			switch (eMoveDirection)
			{
			case eMoveDirection.UpperLeft:
				eDirection = eDirection.Reverse;
				integerXY = ParkObjectManager.getUpperLeftIndex(baseIndex);
				flag = true;
				break;
			case eMoveDirection.UpperRight:
				eDirection = eDirection.Default;
				integerXY = ParkObjectManager.getUpperRightIndex(baseIndex);
				flag = true;
				break;
			case eMoveDirection.LowerLeft:
				eDirection = eDirection.Reverse;
				integerXY = ParkObjectManager.getLowerLeftIndex(baseIndex);
				flag = true;
				break;
			case eMoveDirection.LowerRight:
				eDirection = eDirection.Default;
				integerXY = ParkObjectManager.getLowerRightIndex(baseIndex);
				flag = true;
				break;
			}
			Grid grid = ParkObjectManager.Instance.getGrid(integerXY.x, integerXY.y);
			if (grid == null)
			{
				list2.Remove(eMoveDirection);
			}
			else if (!grid.isReleased)
			{
				list2.Remove(eMoveDirection);
			}
			else if (!grid.roadExistsOn || grid.minilenExistsOn || grid.buildingExistsOn)
			{
				list2.Remove(eMoveDirection);
			}
			else if (!(prevGrid_ != null) || !(prevGrid_.index == grid.index) || list2.Count <= 1)
			{
				result = true;
				nextGrid_ = grid;
				prevGrid_ = ParkObjectManager.Instance.getGrid(horizontalIndex_, verticalIndex_);
				base.direction = eDirection;
				moveEndPosition_ = nextGrid_.position;
				moveStartPosition_ = prevGrid_.position;
				horizontalIndex_ = nextGrid_.horizontalIndex;
				verticalIndex_ = nextGrid_.verticalIndex;
				if (flag)
				{
					setRecalculatedOrder();
				}
				prevGrid_.DetachObject(eType.Minilen);
				nextGrid_.AttachObject(this);
				break;
			}
		}
		return result;
	}

	public override void OnClick(Vector3 inputPosition)
	{
		if (ParkObjectManager.Instance.mapScroll.isAutoScrolling)
		{
			return;
		}
		switch (rareType_)
		{
		case eRareType.Normal:
			if (!Sound.Instance.isPlayingSe() && Sound.Instance.se_clip.Length > 134)
			{
				Sound.Instance.playSe(Sound.eSe.SE_704_park_mini, false);
			}
			break;
		case eRareType.Rare:
			ParkObjectManager.Instance.StartDialogMinilenProfile(objectID_);
			Constant.SoundUtil.PlayDecideSE();
			break;
		}
	}

	public void PlaySleep()
	{
		if (sleepEffect_ == null)
		{
			setupEffect();
		}
		if (sleepEffect_ != null)
		{
			sleepEffect_.cachedTransform.position = cachedTransform_.position;
			sleepEffect_.horizontalIndex = horizontalIndex_;
			sleepEffect_.verticalIndex = verticalIndex_;
			sleepEffect_.sortingOrder = base.sortingOrder + 30;
			sleepEffect_.Play();
			sleepEffect_.direction = direction_;
		}
	}

	public void RepositionEffect()
	{
		if (sleepEffect_ != null)
		{
			sleepEffect_.cachedTransform.position = cachedTransform_.position;
			sleepEffect_.horizontalIndex = horizontalIndex_;
			sleepEffect_.verticalIndex = verticalIndex_;
			setRecalculatedOrder();
			sleepEffect_.sortingOrder = base.sortingOrder + 30;
			if (cachedTransform_.lossyScale.x < 0f)
			{
				sleepEffect_.direction = eDirection.Reverse;
			}
			else
			{
				sleepEffect_.direction = eDirection.Default;
			}
		}
	}

	public void StopSleep()
	{
		if (sleepEffect_ != null)
		{
			sleepEffect_.Stop();
		}
	}

	public override void OnRemove()
	{
		base.OnRemove();
		if (sleepEffect_ != null)
		{
			sleepEffect_.Stop();
			UnityEngine.Object.Destroy(sleepEffect_.gameObject);
			sleepEffect_ = null;
		}
	}

	public void PlayAnimation(eAnimationState nextAnimation, bool forceChange = false)
	{
		if (forceChange || currentAnimationState_ == nextAnimation)
		{
			if (currentAnimationState_ != 0)
			{
				animator_.SetBool(ANIMATOR_STATE_KEYS[(int)currentAnimationState_], false);
			}
			if (nextAnimation != 0)
			{
				animator_.SetBool(ANIMATOR_STATE_KEYS[(int)nextAnimation], true);
			}
			currentAnimationState_ = nextAnimation;
			OnAnimationChanged(nextAnimation);
		}
	}

	public IEnumerator PlayAnimationWaitTime(eAnimationState nextAnimation, float waitTime, bool forceChange = false)
	{
		float time = 0f;
		while (time < waitTime)
		{
			time += Time.deltaTime;
			yield return null;
		}
		PlayAnimation(nextAnimation, forceChange);
	}

	public void ReturnIdleAnimation()
	{
		for (int i = 1; i < ANIMATOR_STATE_KEYS.Length; i++)
		{
			animator_.SetBool(ANIMATOR_STATE_KEYS[i], false);
		}
	}

	private void OnAnimationChanged(eAnimationState nextAnimation)
	{
		StopSleep();
		if (nextAnimation == eAnimationState.Sleep)
		{
			PlaySleep();
		}
	}

	protected override void setObjectDirection(eDirection newDirection)
	{
		switch (newDirection)
		{
		case eDirection.Default:
			cachedTransform_.localScale = new Vector3(Mathf.Abs(cachedTransform_.localScale.x), cachedTransform_.localScale.y, 1f);
			break;
		case eDirection.Reverse:
			cachedTransform_.localScale = new Vector3(0f - Mathf.Abs(cachedTransform_.localScale.x), cachedTransform_.localScale.y, 1f);
			break;
		}
	}
}
