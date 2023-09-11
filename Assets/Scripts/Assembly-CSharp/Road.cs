using System.Collections;
using UnityEngine;

public class Road : ParkObject
{
	private bool isSetupFinished_;

	public int spriteID { get; set; }

	public bool isReverce { get; set; }

	public override IEnumerator setup(int id)
	{
		yield return StartCoroutine(base.setup(id));
		objectID_ = id;
		objectType_ = eType.Road;
		isSetupFinished_ = true;
	}

	public override void setupImmediate(int id)
	{
		base.setupImmediate(id);
		objectID_ = id;
		objectType_ = eType.Road;
		isSetupFinished_ = true;
	}

	public void ChangeSprite(int newID, Sprite newSprite)
	{
		objectID_ = newID;
		for (int i = 0; i < spriteRenderers_.Length; i++)
		{
			spriteRenderers_[i].sprite = newSprite;
		}
	}
}
