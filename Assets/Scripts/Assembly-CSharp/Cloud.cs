using System.Collections;
using UnityEngine;

public class Cloud : GimmickBase
{
	public enum CloudArea
	{
		LeftUp = 0,
		CenterUp = 1,
		RightUp = 2,
		LeftDown = 3,
		CenterDown = 4,
		RightDown = 5,
		Vacuum = 6,
		Max = 7
	}

	public enum CloudAreaState
	{
		None = 0,
		LeftUp = 1,
		CenterUp = 2,
		RightUp = 3,
		LeftDown = 4,
		CenterDown = 5,
		RightDown = 6,
		Vacuum = 7,
		Max = 8
	}

	public const int MAX_SET_CLOUD = 2;

	private int areaState_;

	public Vector3[] cloudPos = new Vector3[7];

	private float diffY = 112f;

	private float diffX = 90f;

	public float cloudMinX;

	public float cloudMaxX;

	public float cloudMinY;

	public float cloudMaxY;

	public int moveDiff = -1;

	public int hitDiff;

	public bool isCloudMove;

	private Vector3 cloudColliderSize = new Vector3(200f, 235f, 1f);

	private Vector3 cloudColliderPosition = new Vector3(0f, -5f, 0f);

	private Animation anime;

	private int vacuumBufferArea = -1;

	private UISprite[] sprites = new UISprite[14];

	public static Vector3 CloudLocalOffset = new Vector3(-270f, 455f, 0f);

	private Color[] spriteColor = new Color[14];

	private GameObject boxCollider;

	private GameObject boxCollider2;

	public int priority = -1;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void cloudInit(int area)
	{
		cloudPos[0] = new Vector3(-180f, 280f, 9.5f);
		cloudPos[1] = new Vector3(0f, 280f, 9.5f);
		cloudPos[2] = new Vector3(180f, 280f, 9.5f);
		cloudPos[3] = new Vector3(-180f, 70f, 9.5f);
		cloudPos[4] = new Vector3(0f, 70f, 9.5f);
		cloudPos[5] = new Vector3(180f, 70f, 9.5f);
		cloudPos[6] = new Vector3(-750f, -750f, 9.5f);
		areaState_ = area;
		base.transform.localPosition = cloudPos[area - 1];
		anime = base.transform.Find("cloud_eff").GetComponent<Animation>();
		cloudRangeSetup(area);
		sprites = base.transform.GetComponentsInChildren<UISprite>();
		for (int i = 0; i < sprites.Length; i++)
		{
			spriteColor[i] = sprites[i].color;
		}
	}

	private void cloudRangeSetup(int area)
	{
		cloudMinX = cloudPos[area - 1].x - diffX;
		cloudMaxX = cloudPos[area - 1].x + diffX;
		cloudMinY = cloudPos[area - 1].y - diffY;
		cloudMaxY = cloudPos[area - 1].y + diffY;
		if (this.boxCollider != null)
		{
			Object.DestroyImmediate(this.boxCollider);
		}
		this.boxCollider = new GameObject("CloudCollider");
		this.boxCollider.transform.parent = base.transform;
		this.boxCollider.transform.localPosition = cloudColliderPosition;
		this.boxCollider.transform.localScale = Vector3.one;
		this.boxCollider.layer = LayerMask.NameToLayer("StageCollider");
		BoxCollider boxCollider = this.boxCollider.AddComponent<BoxCollider>();
		boxCollider.size = cloudColliderSize;
	}

	public Vector3 getCloudPosition()
	{
		return cloudPos[areaState_ - 1];
	}

	public void setAreaState(CloudAreaState area)
	{
		areaState_ = (int)area;
		base.transform.localPosition = cloudPos[areaState_ - 1];
		cloudRangeSetup((int)area);
	}

	public CloudAreaState getAreaState()
	{
		return (CloudAreaState)areaState_;
	}

	public IEnumerator cloudMoveRoutine(StagePause pause, bool bBound)
	{
		if (GetComponent<iTween>() != null)
		{
			Object.Destroy(GetComponent<iTween>());
		}
		if (moveDiff == 0)
		{
			areaState_ -= 3;
		}
		else if (moveDiff == 1)
		{
			areaState_ += 3;
		}
		else if (moveDiff == 2)
		{
			areaState_++;
		}
		else if (moveDiff == 3)
		{
			areaState_--;
		}
		float moveToX = cloudPos[areaState_ - 1].x;
		float moveToY = cloudPos[areaState_ - 1].y;
		if (bBound)
		{
			Sound.Instance.playSe(Sound.eSe.SE_527_cloud_bound);
			Vector3 pos = base.transform.localPosition;
			float x = pos.x - (moveToX - pos.x) * 0.1f;
			float y = pos.y - (moveToY - pos.y) * 0.1f;
			iTween.MoveTo(base.gameObject, iTween.Hash("x", x, "y", y, "easetype", iTween.EaseType.easeOutQuad, "time", 0.2f, "islocal", true));
			while ((bool)GetComponent<iTween>())
			{
				yield return pause.sync();
			}
		}
		Sound.Instance.playSe(Sound.eSe.SE_526_cloud_move);
		iTween.MoveTo(base.gameObject, iTween.Hash("x", moveToX, "y", moveToY, "easetype", iTween.EaseType.easeInOutQuad, "time", 1f, "islocal", true));
		cloudRangeSetup(areaState_);
		moveDiff = -1;
		while ((bool)GetComponent<iTween>())
		{
			yield return pause.sync();
		}
		priority = -1;
		hitDiff = 0;
	}

	public IEnumerator hitAnimationPlay()
	{
		anime.clip = anime.GetComponent<Animation>()["Cloud_eff_anm_00"].clip;
		anime.Play();
		yield return null;
	}

	public IEnumerator clearAnimationPlay()
	{
		anime.clip = anime.GetComponent<Animation>()["Cloud_eff_anm_01"].clip;
		anime.Play();
		yield return null;
	}

	public IEnumerator vacuumAnimationPlay_IN(StagePause pause, Vector3 moveToPos)
	{
		float moveToX = moveToPos.x - 168f;
		float moveToY = moveToPos.y - 430f;
		float alpha = 1f;
		vacuumBufferArea = areaState_;
		areaState_ = 7;
		yield return pause.sync();
		iTween.MoveTo(base.gameObject, iTween.Hash("x", moveToX, "y", moveToY, "easetype", iTween.EaseType.easeInOutQuad, "time", 2f, "islocal", true));
		iTween.ScaleTo(base.gameObject, iTween.Hash("x", 0.1f, "y", 0.1f, "easetype", iTween.EaseType.easeInOutQuad, "time", 2f, "islocal", true));
		cloudRangeSetup(areaState_);
		while ((bool)GetComponent<iTween>())
		{
			if (alpha > 0f)
			{
				alpha -= 0.5f * Time.deltaTime;
			}
			for (int i = 1; i <= 7; i++)
			{
				spriteColor[i].a = alpha;
				sprites[i].color = spriteColor[i];
			}
			yield return pause.sync();
		}
		base.transform.localPosition = cloudPos[areaState_ - 1];
		base.transform.localScale = Vector3.one;
		priority = -1;
		hitDiff = 0;
		yield return null;
	}

	public IEnumerator vacuumAnimationPlay_OUT(StagePause pause, Vector3 StartPos)
	{
		base.transform.localPosition = StartPos;
		Vector3 startScale = new Vector3(0.1f, 0.1f, 1f);
		base.transform.localScale = startScale;
		float alpha = 0f;
		for (int j = 1; j <= 7; j++)
		{
			spriteColor[j].a = alpha;
			sprites[j].color = spriteColor[j];
		}
		areaState_ = vacuumBufferArea;
		float moveToX = cloudPos[areaState_ - 1].x;
		float moveToY = cloudPos[areaState_ - 1].y;
		iTween.MoveTo(base.gameObject, iTween.Hash("x", moveToX, "y", moveToY, "easetype", iTween.EaseType.easeInOutQuad, "time", 2f, "islocal", true));
		iTween.ScaleTo(base.gameObject, iTween.Hash("x", 1f, "y", 1f, "easetype", iTween.EaseType.easeInOutQuad, "time", 2f, "islocal", true));
		cloudRangeSetup(areaState_);
		while ((bool)GetComponent<iTween>())
		{
			if (alpha < 1f)
			{
				alpha += 0.5f * Time.deltaTime;
			}
			for (int i = 1; i <= 7; i++)
			{
				spriteColor[i].a = alpha;
				sprites[i].color = spriteColor[i];
			}
			yield return pause.sync();
		}
		base.transform.localPosition = cloudPos[areaState_ - 1];
		priority = -1;
		hitDiff = 0;
		yield return null;
	}

	public bool tweenAnimationPlaying()
	{
		return GetComponent<iTween>();
	}

	public bool animationIsPlaying()
	{
		return anime.isPlaying;
	}

	public void resurrection()
	{
		for (int i = 1; i <= 7; i++)
		{
			spriteColor[i].a = 1f;
			sprites[i].color = spriteColor[i];
		}
		areaState_ = vacuumBufferArea;
		base.transform.localPosition = cloudPos[areaState_ - 1];
	}

	public CloudAreaState getVacumeBufferArea()
	{
		return (CloudAreaState)vacuumBufferArea;
	}
}
