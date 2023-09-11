using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDefend : MonoBehaviour
{
	public enum eType
	{
		UFO_A = 0,
		UFO_B = 1
	}

	public const int POS_Z = 5;

	private Transform myTrans;

	public Part_Stage partStage_;

	public bool isMoving;

	public Bubble currentParentBubble;

	public List<Bubble> nearBubbleList;

	public bool isNearExist;

	public UISpriteAnimationEx spriteAnim;

	public UISprite sprite;

	public Animation anim;

	private Transform eff;

	private Vector3 DefalutEffScale;

	public bool isExplosion;

	public eType myType;

	private static int attackCount;

	private int specialAttackTiming = 5;

	public void Initialize()
	{
		myTrans = base.transform;
		nearBubbleList = new List<Bubble>();
		attackCount = 0;
		anim = base.gameObject.GetComponentInChildren<Animation>();
		anim.Stop();
		eff = base.transform.Find("UFO/eff");
		DefalutEffScale = eff.localScale;
	}

	public void DecideParentBubble(List<Bubble> bubbleList)
	{
		UpdateNearBubbleList(bubbleList);
		if (nearBubbleList.Count > 0)
		{
			System.Random random = new System.Random();
			currentParentBubble = nearBubbleList[random.Next(nearBubbleList.Count)];
			currentParentBubble.onObstacle = this;
		}
	}

	public void UpdateNearBubbleList(List<Bubble> bubbleList)
	{
		UpdateNearBubbleList(bubbleList, false);
	}

	public void UpdateNearBubbleList(List<Bubble> bubbleList, bool movePointSearch)
	{
		isNearExist = false;
		nearBubbleList.Clear();
		Bubble bubble2 = null;
		if (movePointSearch)
		{
			bubble2 = currentParentBubble;
		}
		bubbleList.ForEach(delegate(Bubble bubble)
		{
			if (isNear(bubble) && bubble.onObstacle == null && !bubble.isLineFriend && bubble.type >= Bubble.eType.Red && bubble.type <= Bubble.eType.Black && !bubble.isFrozen && !bubble.isOnChain && !bubble.isLocked && !bubble.inCloud)
			{
				nearBubbleList.Add(bubble);
				isNearExist = true;
			}
			else if (isNear(bubble) && bubble.onObstacle == null && !bubble.isLineFriend && bubble.type >= Bubble.eType.Red && bubble.type <= Bubble.eType.Black && !bubble.isFrozen && (bubble.isOnChain || bubble.isLocked || bubble.inCloud))
			{
				isNearExist = true;
			}
		});
		Debug.Log("nearBubbleList.Count = " + nearBubbleList.Count);
	}

	public IEnumerator Move()
	{
		isMoving = true;
		if (currentParentBubble != null)
		{
			Vector3 tempPos = new Vector3(currentParentBubble.transform.localPosition.x, currentParentBubble.transform.localPosition.y, -5f);
			Sound.Instance.playSe(Sound.eSe.SE_601_obstacle_ufo_move);
			iTween.MoveTo(base.gameObject, iTween.Hash("position", tempPos, "time", 0.5f, "islocal", true, "easetype", iTween.EaseType.easeOutCirc));
			while (base.gameObject != null && base.gameObject.GetComponent<iTween>() != null)
			{
				yield return partStage_.stagePause.sync();
			}
			Sound.Instance.stopSe(Sound.eSe.SE_601_obstacle_ufo_move);
		}
		isMoving = false;
	}

	public IEnumerator MoveRight()
	{
		if (!(currentParentBubble == null))
		{
			isMoving = true;
			Vector3 tempPos = new Vector3(currentParentBubble.transform.localPosition.x, currentParentBubble.transform.localPosition.y, -5f);
			iTween.MoveTo(base.gameObject, iTween.Hash("position", tempPos, "speed", 200f, "islocal", true, "easetype", iTween.EaseType.linear));
			yield return 0;
			while (base.gameObject != null && base.gameObject.GetComponent<iTween>() != null)
			{
				yield return partStage_.stagePause.sync();
			}
			isMoving = false;
		}
	}

	private bool isNear(BubbleBase bubble)
	{
		return isNear(bubble, null);
	}

	private bool isNear(BubbleBase bubble, BubbleBase movePointBubble)
	{
		float num = Bubble.SQR_SIZE * 2f;
		Vector3 vector = ((!(movePointBubble != null)) ? myTrans.position : movePointBubble.myTrans.position);
		if (bubble != null && bubble.gameObject.activeSelf)
		{
			Vector3 vector2 = (vector - bubble.myTrans.position) / partStage_.uiScale;
			vector2.z = 0f;
			float sqrMagnitude = vector2.sqrMagnitude;
			if (sqrMagnitude > 8100f)
			{
				return false;
			}
			if (sqrMagnitude < num && sqrMagnitude > 0f)
			{
				return true;
			}
		}
		return false;
	}

	public void parentBubbleFindSet(List<Bubble> bList)
	{
		foreach (Bubble b in bList)
		{
			if (b.gameObject.activeSelf)
			{
				Vector3 vector = (myTrans.position - b.myTrans.position) / partStage_.uiScale;
				vector.z = 0f;
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude == 0f)
				{
					currentParentBubble = b;
					b.onObstacle = this;
					break;
				}
			}
		}
	}

	public IEnumerator AttackRoutine()
	{
		eType eType = myType;
		if ((eType != 0 && eType != eType.UFO_B) || !(currentParentBubble != null))
		{
			yield break;
		}
		anim.Play();
		Sound.Instance.playSe(Sound.eSe.SE_602_obstacle_ufo_beam);
		float waitTime = 0.75f;
		float elapsedTime = 0f;
		while (elapsedTime < waitTime)
		{
			elapsedTime += Time.deltaTime;
			yield return partStage_.stagePause.sync();
		}
		anim.Stop();
		eff.localScale = DefalutEffScale;
		bool isNearFulcrum = false;
		foreach (Bubble bubble in partStage_.fieldBubbleList)
		{
			if (bubble.type == Bubble.eType.Rock && isNear(bubble))
			{
				isNearFulcrum = true;
			}
		}
		foreach (Bubble fulcrum in partStage_.fulcrumList)
		{
			if (isNear(fulcrum))
			{
				isNearFulcrum = true;
			}
		}
		attackCount++;
		if (attackCount >= specialAttackTiming && !isNearFulcrum)
		{
			attackCount = 0;
			if (myType == eType.UFO_A)
			{
				currentParentBubble.setType(Bubble.eType.BlackHole_A);
			}
			else
			{
				currentParentBubble.setType(Bubble.eType.BlackHole_B);
			}
		}
		else
		{
			currentParentBubble.setType(Bubble.eType.Rainbow);
		}
	}

	public void setLockColor(bool locked)
	{
		if (locked)
		{
			sprite.color = Color.gray;
		}
		else
		{
			sprite.color = Color.white;
		}
	}

	public IEnumerator explosionRoutine()
	{
		if (!(currentParentBubble != null))
		{
			anim.clip = anim.GetClip("UFO_explosion_anm");
			anim.Play();
			Sound.Instance.playSe(Sound.eSe.SE_600_obstacle_ufo_explosion);
			float waitTime = anim.clip.length;
			float elapsedTime = 0f;
			while (elapsedTime < waitTime)
			{
				elapsedTime += Time.deltaTime;
				yield return partStage_.stagePause.sync();
			}
			anim.Stop();
		}
	}

	public void replayUFO()
	{
		anim.clip = anim.GetClip("UFO_anm");
		base.transform.Find("UFO/UFO").gameObject.SetActive(true);
		isExplosion = false;
	}

	public IEnumerator breakRoutine(bool explosion)
	{
		if (!isExplosion)
		{
			isExplosion = explosion;
		}
		foreach (ObstacleDefend od in partStage_.obstacleMoveList)
		{
			if (od == this)
			{
				yield break;
			}
		}
		partStage_.obstacleMoveList.Add(this);
	}
}
