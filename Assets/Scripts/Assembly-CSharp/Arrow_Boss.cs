using System;
using UnityEngine;

public class Arrow_Boss : MonoBehaviour
{
	private const int LIMIT_ANGLE = 80;

	private const int INVALID_ANGLE = 90;

	private const int ANIM_DIV = 5;

	private const int ANIM_CHANGE = 32;

	public bool isValidTouch;

	public bool isButtonTouch;

	public Vector3 fireVector = Vector3.up;

	public Vector3 preFireVector = Vector3.up;

	public Part_BossStage bossPart;

	public Guide_Boss guide;

	private float prevAngle = 1000f;

	private int prevAnimIndex = -1;

	public tk2dAnimatedSprite bubblen;

	public string[] CHARA_SPRITE_ANIMATION_HEADER;

	public int charaIndex;

	public string[] charaAnimNames;

	public string[] pinchCharaAnimNames;

	private void Start()
	{
		fireVector = Vector3.up;
	}

	private void Update()
	{
		if (bossPart == null || bossPart.stagePause.pause || bossPart.state != Part_BossStage.eState.Wait)
		{
			return;
		}
		if (Input.GetMouseButton(0))
		{
			if (isButtonTouch)
			{
				isValidTouch = false;
				return;
			}
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				int layerMask = 1 << LayerMask.NameToLayer("Default");
				if (Physics.Raycast(ray, float.PositiveInfinity, layerMask))
				{
					isButtonTouch = true;
					isValidTouch = false;
					return;
				}
			}
			Vector3 vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 vector2 = vector - base.transform.position;
			float num = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f - 90f;
			if (num < -180f)
			{
				num += 360f;
			}
			if (prevAngle > 360f)
			{
				prevAngle = num;
			}
			isValidTouch = true;
			if (guide.isShootButton)
			{
				if (num > 80f || num < -80f)
				{
					isValidTouch = false;
					return;
				}
				fireVector = vector2;
				preFireVector = fireVector;
			}
			else
			{
				if (num > 90f || num < -90f)
				{
					guide.setActive(false);
					isButtonTouch = true;
					isValidTouch = false;
					return;
				}
				if (num > 80f)
				{
					num = 80f;
				}
				else if (num < -80f)
				{
					num = -80f;
				}
				fireVector = Quaternion.Euler(0f, 0f, num) * Vector3.up;
				preFireVector = fireVector;
			}
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			localEulerAngles.z = num;
			base.transform.localEulerAngles = localEulerAngles;
			for (int i = 1; i <= 5; i++)
			{
				if (Mathf.RoundToInt(num) > -80 + 32 * i)
				{
					continue;
				}
				if (prevAnimIndex != i)
				{
					prevAnimIndex = i;
					if (bossPart.sweatEff.activeSelf)
					{
						bossPart.waitPinchAnimName = pinchCharaAnimNames[charaIndex] + (5 - i).ToString("00") + "_0";
						bubblen.Play(bossPart.waitPinchAnimName);
					}
					else
					{
						bossPart.waitAnimName = charaAnimNames[charaIndex] + (5 - i).ToString("00") + "_0";
						bubblen.Play(bossPart.waitAnimName);
					}
				}
				break;
			}
			if (Mathf.Abs(prevAngle - num) > 0.1f && !Sound.Instance.isPlayingSe(Sound.eSe.SE_204_kassya))
			{
				Sound.Instance.playSe(Sound.eSe.SE_204_kassya);
			}
			prevAngle = num;
			guide.lineUpdate();
			return;
		}
		isButtonTouch = false;
		if (Input.GetMouseButtonUp(0))
		{
			if (isValidTouch)
			{
				isValidTouch = false;
				bossPart.fire(fireVector);
			}
			if (!guide.isShootButton)
			{
				guide.setActive(false);
			}
		}
	}

	public void updateWaitAnimationImmediate()
	{
		if (bossPart == null)
		{
			return;
		}
		for (int i = 1; i <= 5; i++)
		{
			if (Mathf.RoundToInt(prevAngle) > -80 + 32 * i)
			{
				continue;
			}
			prevAnimIndex = i;
			if (bossPart != null)
			{
				if (bossPart.sweatEff.activeSelf)
				{
					bossPart.waitPinchAnimName = pinchCharaAnimNames[charaIndex] + (5 - i).ToString("00") + "_0";
					bubblen.Play(bossPart.waitPinchAnimName);
				}
				else
				{
					bossPart.waitAnimName = charaAnimNames[charaIndex] + (5 - i).ToString("00") + "_0";
					bubblen.Play(bossPart.waitAnimName);
				}
			}
			break;
		}
	}

	public void shootAnim()
	{
		prevAnimIndex = -1;
		for (int i = 1; i <= 5; i++)
		{
			float num = prevAngle;
			if (num > 360f)
			{
				num = 0f;
			}
			if (Mathf.RoundToInt(num) <= -80 + 32 * i)
			{
				if (bossPart != null && bossPart.sweatEff.activeSelf)
				{
					bubblen.Play(pinchCharaAnimNames[charaIndex] + (5 - i).ToString("00") + "_1");
				}
				else
				{
					bubblen.Play(charaAnimNames[charaIndex] + (5 - i).ToString("00") + "_1");
				}
				break;
			}
		}
	}

	public Vector3 getRandomFireVector()
	{
		System.Random random = new System.Random();
		float num = (float)random.Next(800) / 10f;
		if (random.Next(100) % 2 == 1)
		{
			num = 0f - num;
		}
		return fireVector = Quaternion.Euler(0f, 0f, num) * Vector3.up;
	}

	public void revertPreFireVector()
	{
		fireVector = preFireVector;
	}
}
