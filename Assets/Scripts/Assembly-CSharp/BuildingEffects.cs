using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingEffects : MonoBehaviour
{
	public delegate IEnumerator OnFinishedAction();

	private const int EFFECT_MAX = 25;

	public const float SMOKE_EFFECT_LENGTH = 0.95f;

	public const float TWINKLE_EFFECT_LENGTH = 2.95f;

	private int currentEffectCount_ = 25;

	private List<ParkEffect> smokeEffects_ = new List<ParkEffect>();

	private List<ParkEffect> twinkleEffects_ = new List<ParkEffect>();

	private Minilen leftMinilen_;

	private Minilen rightMinilen_;

	public bool isPlaying { get; private set; }

	public static BuildingEffects createObject(Transform parent)
	{
		GameObject gameObject = new GameObject("BuildingEffects");
		gameObject.transform.SetParent(parent, false);
		return gameObject.AddComponent<BuildingEffects>();
	}

	public IEnumerator setup(ParkStructures.Size gridSize)
	{
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		for (int i = 0; i < 25; i++)
		{
			AddEffect(objectManager);
		}
		leftMinilen_ = objectManager.createMinilen(30000);
		rightMinilen_ = objectManager.createMinilen(30000);
		Action<Minilen> lambdaInitMinilen = delegate(Minilen m)
		{
			objectManager.Remove(m);
			m.canAction = false;
			m.gameObject.SetActive(false);
			m.cachedTransform.Find("Root/shadow").gameObject.SetActive(false);
		};
		lambdaInitMinilen(leftMinilen_);
		lambdaInitMinilen(rightMinilen_);
		ForceUnvisible();
		yield break;
	}

	private void AddEffect(ParkObjectManager objectManager)
	{
		ParkEffect parkEffect = objectManager.createEffect(ParkEffect.eEffectType.Smoke);
		parkEffect.transform.SetParent(base.transform, false);
		smokeEffects_.Add(parkEffect);
		ParkEffect parkEffect2 = objectManager.createEffect(ParkEffect.eEffectType.Twinkle);
		parkEffect2.transform.SetParent(base.transform, false);
		twinkleEffects_.Add(parkEffect2);
	}

	public void ForceUnvisible()
	{
		for (int i = 0; i < currentEffectCount_; i++)
		{
			smokeEffects_[i].gameObject.SetActive(false);
			twinkleEffects_[i].gameObject.SetActive(false);
		}
		leftMinilen_.gameObject.SetActive(false);
		rightMinilen_.gameObject.SetActive(false);
	}

	private void PlaySmoke(int playEffectCount)
	{
		for (int i = 0; i < playEffectCount; i++)
		{
			smokeEffects_[i].Play();
		}
	}

	private void StopSmoke(int playEffectCount)
	{
		for (int i = 0; i < playEffectCount; i++)
		{
			smokeEffects_[i].Stop();
		}
	}

	private void PlayTwinkle(int playEffectCount)
	{
		for (int i = 0; i < playEffectCount; i++)
		{
			twinkleEffects_[i].Play();
		}
	}

	private void StopTwinkle(int playEffectCount)
	{
		for (int i = 0; i < playEffectCount; i++)
		{
			twinkleEffects_[i].Stop();
		}
	}

	private int RepositionEffect(ParkStructures.IntegerXY baseIndex, List<ParkStructures.IntegerXY> indices, out ParkEffect centerEffect)
	{
		ParkObjectManager instance = ParkObjectManager.Instance;
		centerEffect = smokeEffects_[0];
		int count = indices.Count;
		if (count > currentEffectCount_)
		{
			int num = count - currentEffectCount_;
			for (int i = 0; i < num; i++)
			{
				AddEffect(instance);
			}
			currentEffectCount_ = count;
		}
		for (int j = 0; j < count; j++)
		{
			Vector3 vector = new Vector3(0f, UnityEngine.Random.Range(-10f, 10f), 0f);
			float num2 = UnityEngine.Random.Range(0.8f, 1.4f);
			Grid grid = instance.getGrid(indices[j].x, indices[j].y);
			smokeEffects_[j].horizontalIndex = baseIndex.x;
			smokeEffects_[j].verticalIndex = baseIndex.y;
			smokeEffects_[j].setRecalculatedOrder();
			smokeEffects_[j].horizontalIndex = grid.horizontalIndex;
			smokeEffects_[j].verticalIndex = grid.verticalIndex;
			smokeEffects_[j].setPosition(grid.position + vector);
			if (Utility.decideByProbability(50f))
			{
				smokeEffects_[j].cachedTransform.localScale = new Vector3(0f - num2, num2, 1f);
			}
			else
			{
				smokeEffects_[j].cachedTransform.localScale = new Vector3(num2, num2, 1f);
			}
			twinkleEffects_[j].horizontalIndex = baseIndex.x;
			twinkleEffects_[j].verticalIndex = baseIndex.y;
			twinkleEffects_[j].setRecalculatedOrder();
			twinkleEffects_[j].horizontalIndex = grid.horizontalIndex;
			twinkleEffects_[j].verticalIndex = grid.verticalIndex;
			twinkleEffects_[j].setPosition(grid.position + vector);
			if (Utility.decideByProbability(50f))
			{
				twinkleEffects_[j].cachedTransform.localScale = new Vector3(0f - num2, num2, 1f);
			}
			else
			{
				twinkleEffects_[j].cachedTransform.localScale = new Vector3(num2, num2, 1f);
			}
		}
		int num3 = 0;
		int num4 = 100;
		for (int k = 0; k < indices.Count; k++)
		{
			if (indices[k].x < num4)
			{
				num4 = indices[k].x;
			}
			if (indices[k].x > num3)
			{
				num3 = indices[k].x;
			}
		}
		ParkStructures.IntegerXY centerIndex = new ParkStructures.IntegerXY((num4 + num3) / 2, 0);
		centerEffect = smokeEffects_.Find((ParkEffect smoke) => smoke.index.x == centerIndex.x);
		return count;
	}

	public IEnumerator PlaySequentialEffect(ParkStructures.IntegerXY baseIndex, List<ParkStructures.IntegerXY> indices, ParkStructures.Size size, bool moveCenter = false, OnFinishedAction onFinishedSlideIn = null, OnFinishedAction onFinishedSmoke = null, OnFinishedAction onFinishedTwinkle = null)
	{
		isPlaying = true;
		for (int i = 0; i < Input.enableCount; i++)
		{
			Input.enable = false;
		}
		ParkEffect centerEffect = null;
		int effectCount = RepositionEffect(baseIndex, indices, out centerEffect);
		ForceUnvisible();
		if (moveCenter)
		{
			yield return StartCoroutine(MoveEffectsCenter(centerEffect));
		}
		ParkObjectManager objectManager = ParkObjectManager.Instance;
		Transform maproot = objectManager.mapRoot;
		Camera mapCamera = objectManager.mapCamera;
		int gridWidth = objectManager.gridSize.width;
		Vector3 screenLeft = maproot.InverseTransformPoint(mapCamera.ScreenToWorldPoint(new Vector3(0f, 0f)));
		screenLeft.x -= gridWidth;
		Vector3 screenRight = maproot.InverseTransformPoint(mapCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0f)));
		screenRight.x += gridWidth;
		ParkStructures.IntegerXY minilenIndex = baseIndex;
		if (moveCenter)
		{
			minilenIndex.x = centerEffect.horizontalIndex;
		}
		yield return StartCoroutine(SlideInMinilen(minilenIndex, size, screenLeft, screenRight));
		yield return new WaitForSeconds(0.5f);
		if (onFinishedSlideIn != null)
		{
			StartCoroutine(onFinishedSlideIn());
		}
		PlaySmoke(effectCount);
		yield return new WaitForSeconds(0.95f);
		StopSmoke(effectCount);
		if (onFinishedSmoke != null)
		{
			StartCoroutine(onFinishedSmoke());
		}
		PlayTwinkle(effectCount);
		yield return StartCoroutine(DanceMinilen());
		StopTwinkle(effectCount);
		if (onFinishedTwinkle != null)
		{
			StartCoroutine(onFinishedTwinkle());
		}
		yield return StartCoroutine(SlideOutMinilen(minilenIndex, size, screenLeft, screenRight));
		Input.enable = true;
		isPlaying = false;
	}

	private IEnumerator MoveEffectsCenter(ParkEffect centerEffect)
	{
		MapScroll mapScroll = ParkObjectManager.Instance.mapScroll;
		mapScroll.StartTracking(centerEffect);
		while (mapScroll.isTracking)
		{
			yield return null;
		}
		mapScroll.EndTracking();
	}

	private IEnumerator SlideInMinilen(ParkStructures.IntegerXY baseIndex, ParkStructures.Size size, Vector3 screenLeft, Vector3 screenRight)
	{
		float currentZoom = ParkObjectManager.Instance.mapScroll.currentZoom;
		ParkStructures.Size gridSize = ParkObjectManager.Instance.gridSize;
		leftMinilen_.gameObject.SetActive(true);
		rightMinilen_.gameObject.SetActive(true);
		leftMinilen_.direction = ParkObject.eDirection.Default;
		rightMinilen_.direction = ParkObject.eDirection.Reverse;
		leftMinilen_.PlayAnimation(Minilen.eAnimationState.Walk, true);
		rightMinilen_.PlayAnimation(Minilen.eAnimationState.Walk, true);
		ParkStructures.IntegerXY baseIndex2 = default(ParkStructures.IntegerXY);
		Action<Minilen> lambdaSetOrder = delegate(Minilen m)
		{
			m.horizontalIndex = baseIndex2.x;
			m.verticalIndex = baseIndex2.y;
			m.setRecalculatedOrder();
			m.sortingOrder += 25;
		};
		lambdaSetOrder(leftMinilen_);
		lambdaSetOrder(rightMinilen_);
		Grid grid = ParkObjectManager.Instance.getGrid(baseIndex.x, baseIndex.y);
		Vector3 leftTarget = grid.position - new Vector3(Mathf.Max(size.width / 2, gridSize.width), 0f);
		Vector3 rightTarget = grid.position + new Vector3(Mathf.Max(size.width / 2, gridSize.width), 0f);
		float baseY = grid.position.y;
		leftMinilen_.setPosition(screenLeft);
		rightMinilen_.setPosition(screenRight);
		float time = 0f;
		float moveTime = 1f;
		while (time < moveTime)
		{
			float currentPercentage = time / moveTime;
			float currentY = Mathf.Abs(Mathf.Sin(currentPercentage * 360f * ((float)Math.PI / 180f))) * (100f / currentZoom);
			float leftX = Mathf.Lerp(screenLeft.x, leftTarget.x, currentPercentage);
			Vector3 current2 = new Vector3(leftX, baseY + currentY);
			leftMinilen_.setPosition(current2);
			float rightX = Mathf.Lerp(screenRight.x, rightTarget.x, currentPercentage);
			current2 = new Vector3(rightX, baseY + currentY);
			rightMinilen_.setPosition(current2);
			time += Time.deltaTime;
			yield return null;
		}
		leftMinilen_.PlayAnimation(Minilen.eAnimationState.Something, true);
		rightMinilen_.PlayAnimation(Minilen.eAnimationState.Something, true);
	}

	private IEnumerator DanceMinilen()
	{
		leftMinilen_.PlayAnimation(Minilen.eAnimationState.Dance, true);
		rightMinilen_.PlayAnimation(Minilen.eAnimationState.Dance, true);
		yield return new WaitForSeconds(2.95f);
	}

	private IEnumerator SlideOutMinilen(ParkStructures.IntegerXY baseIndex, ParkStructures.Size size, Vector3 screenLeft, Vector3 screenRight)
	{
		float currentZoom = ParkObjectManager.Instance.mapScroll.currentZoom;
		ParkStructures.Size gridSize = ParkObjectManager.Instance.gridSize;
		Vector3 leftStartPosition = leftMinilen_.position;
		Vector3 rightStartPosition = rightMinilen_.position;
		leftMinilen_.direction = ParkObject.eDirection.Reverse;
		rightMinilen_.direction = ParkObject.eDirection.Default;
		leftMinilen_.PlayAnimation(Minilen.eAnimationState.Walk, true);
		rightMinilen_.PlayAnimation(Minilen.eAnimationState.Walk, true);
		Grid grid = ParkObjectManager.Instance.getGrid(baseIndex.x, baseIndex.y);
		float baseY = grid.position.y;
		float time = 0f;
		float moveTime = 1f;
		while (time < moveTime)
		{
			float currentPercentage = time / moveTime;
			float currentY = Mathf.Abs(Mathf.Sin(currentPercentage * 360f * ((float)Math.PI / 180f))) * (100f / currentZoom);
			float leftX = Mathf.Lerp(leftStartPosition.x, screenLeft.x, currentPercentage);
			Vector3 current2 = new Vector3(leftX, baseY + currentY);
			leftMinilen_.setPosition(current2);
			float rightX = Mathf.Lerp(rightStartPosition.x, screenRight.x, currentPercentage);
			current2 = new Vector3(rightX, baseY + currentY);
			rightMinilen_.setPosition(current2);
			time += Time.deltaTime;
			yield return null;
		}
		leftMinilen_.gameObject.SetActive(false);
		rightMinilen_.gameObject.SetActive(false);
	}

	public IEnumerator PlaySmokeEffects(ParkStructures.IntegerXY baseIndex, ParkStructures.Size size, ParkObject.eDirection direction)
	{
		List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
		ParkObjectManager.getRelationalIndices(baseIndex, size, direction, ref indices);
		ParkEffect centerEffect = null;
		int effectCount = RepositionEffect(baseIndex, indices, out centerEffect);
		PlaySmoke(effectCount);
		yield return new WaitForSeconds(0.95f);
		StopSmoke(effectCount);
	}
}
