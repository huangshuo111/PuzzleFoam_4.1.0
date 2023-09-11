using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : Building
{
	public enum eSpriteSort
	{
		Pillar = 0,
		LeftPlate = 1,
		RightPlate = 2,
		LeftPlateShadow = 3,
		RightPlateShadow = 4
	}

	public enum eTransversePlate
	{
		Left = 0,
		Right = 1,
		Max = 2
	}

	private static readonly string[] PLATE_NAMES = new string[2] { "FrontLeft", "FrontRight" };

	private GameObject[] transversePlateObjects_ = new GameObject[2];

	private SpriteRenderer[] shadows_;

	public override IEnumerator setup(int id)
	{
		yield return StartCoroutine(base.setup(id));
		objectType_ = eType.Fence;
		getTransversePlates();
		setActivePlates(false);
	}

	public override void setupImmediate(int id)
	{
		base.setupImmediate(id);
		objectType_ = eType.Fence;
		getTransversePlates();
		setActivePlates(false);
	}

	private void getTransversePlates()
	{
		for (int i = 0; i < 2; i++)
		{
			transversePlateObjects_[i] = cachedTransform_.Find(PLATE_NAMES[i]).gameObject;
		}
		List<SpriteRenderer> list = new List<SpriteRenderer>();
		Transform[] componentsInChildren = cachedTransform_.GetComponentsInChildren<Transform>(true);
		for (int j = 0; j < componentsInChildren.Length; j++)
		{
			if (componentsInChildren[j].name.Contains("Shadow"))
			{
				list.Add(componentsInChildren[j].GetComponent<SpriteRenderer>());
			}
		}
		shadows_ = list.ToArray();
	}

	public void setActivePlate(eTransversePlate plate, bool active)
	{
		transversePlateObjects_[(int)plate].SetActive(active);
	}

	public void setActivePlate(eTransversePlate[] plates, bool active)
	{
		for (int i = 0; i < plates.Length; i++)
		{
			transversePlateObjects_[(int)plates[i]].SetActive(active);
		}
	}

	public void setActivePlates(bool active)
	{
		for (int i = 0; i < 2; i++)
		{
			transversePlateObjects_[i].SetActive(active);
		}
	}

	protected override void setObjectDirection(eDirection newDirection)
	{
		if (newDirection != direction_)
		{
			ParkObjectManager instance = ParkObjectManager.Instance;
			direction_ = newDirection;
			setColliderOffset();
			colliderOffset_.x *= -1f;
		}
	}

	private void ReversePlate()
	{
		GameObject[] array = Array.FindAll(transversePlateObjects_, (GameObject o) => o.activeSelf);
		if (array != null && array.Length < 2 && array.Length > 0)
		{
			string text = array[0].name;
			if (text == PLATE_NAMES[0])
			{
				transversePlateObjects_[1].SetActive(true);
			}
			if (text == PLATE_NAMES[1])
			{
				transversePlateObjects_[0].SetActive(true);
			}
			array[0].SetActive(false);
		}
	}

	public override void OnDeselect(bool resetColor = true)
	{
		base.OnDeselect(resetColor);
		ParkObjectManager.Instance.ConnectFences();
	}

	public override void OnMoveGrid()
	{
		ParkObjectManager.Instance.ConnectFences();
	}

	protected override void setOrder(int order)
	{
		base.setOrder(order);
		if (!isDragMove_ || shadows_ == null)
		{
			return;
		}
		for (int i = 0; i < shadows_.Length; i++)
		{
			if (shadows_ != null)
			{
				shadows_[i].sortingOrder = 0;
			}
		}
	}
}
