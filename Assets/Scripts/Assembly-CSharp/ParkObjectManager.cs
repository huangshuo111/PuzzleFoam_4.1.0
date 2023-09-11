using System;
using System.Collections;
using System.Collections.Generic;
using Bridge;
using LitJson;
using Network;
using UnityEngine;

public class ParkObjectManager : MonoBehaviour
{
	public enum ePlantCheckResult
	{
		Succeeded = 0,
		GridNotYetReleased = 1,
		GridShortage = 2,
		ExistsObject = 3
	}

	public enum eParkViewMode
	{
		Player = 0,
		Friend = 1
	}

	public class PostBuildingList
	{
		public BuildingData[] buildings;
	}

	private const string CAMERA_PREFAB_PATH = "Prefabs/Common/ParkPart/ParkMapCamera";

	private const string PREFABS_REFERENCE_PACK_PATH = "Prefabs/Common/ParkPart/ParkObjectReferencePack";

	private const string ATLAS_REFERENCE_PACK_PATH = "Prefabs/Common/ParkPart/ParkObject_Atlases";

	private const string BACKGROUND_PREFAB_NAME = "P_BG_001";

	public const int PARK_CAMERA_Z = 0;

	public const int PARK_OBJECT_Z = 10;

	private static ParkObjectManager instance_;

	private DialogManager dialogManager_;

	private ColliderManager colliderManager_;

	private ParkMap parkMap_;

	private Camera mapCamera_;

	private MapScroll mapScroll_;

	private ParkMapInfo.MapInfo mapInfo_;

	private ParkBuildingDataTable buildingDataTable_;

	private bool createGameObject_;

	private bool isSetupFinished_;

	private GameObject parkObjectReferencePack_;

	private GameObject parkObjectAtlases_;

	private List<UIAtlas> roadAtlasPool_ = new List<UIAtlas>();

	private List<UIAtlas> buildingAtlasPool_ = new List<UIAtlas>();

	private List<UIAtlas> minilenAtlasPool_ = new List<UIAtlas>();

	private List<UIAtlas> otherAtlasPool_ = new List<UIAtlas>();

	private Dictionary<string, Sprite> roadSpritePool_ = new Dictionary<string, Sprite>();

	private Dictionary<string, Sprite> buildingSpritePool_ = new Dictionary<string, Sprite>();

	private Dictionary<string, Sprite> minilenSpritePool_ = new Dictionary<string, Sprite>();

	private Transform mapRoot_;

	private Transform gridRoot_;

	private Transform roadRoot_;

	private Transform objectRoot_;

	private GameObject background_;

	private List<Grid> gridList_ = new List<Grid>();

	private List<Road> roadList_ = new List<Road>();

	private List<Building> buildingList_ = new List<Building>();

	private List<Fence> fenceList_ = new List<Fence>();

	private List<Minilen> minilenList_ = new List<Minilen>();

	private LayoutGrid layoutGrid_;

	private BuildingEffects buildingEffects_;

	private List<ReleaseArea> releaseAreas_ = new List<ReleaseArea>();

	private Building selectObject_;

	public static ParkObjectManager Instance
	{
		get
		{
			if (instance_ == null)
			{
				instance_ = UnityEngine.Object.FindObjectOfType<ParkObjectManager>();
				if (instance_ == null)
				{
					GameObject gameObject = new GameObject("ParkObjectManager");
					instance_ = gameObject.AddComponent<ParkObjectManager>();
				}
			}
			return instance_;
		}
	}

	public static bool haveInstance
	{
		get
		{
			return instance_ != null;
		}
	}

	public eParkViewMode viewMode { get; private set; }

	public Transform mapRoot
	{
		get
		{
			return mapRoot_;
		}
	}

	public Transform roadRoot
	{
		get
		{
			return roadRoot_;
		}
	}

	public Transform objectRoot
	{
		get
		{
			return objectRoot_;
		}
	}

	public int gridCount
	{
		get
		{
			return gridList_.Count;
		}
	}

	public int roadCount
	{
		get
		{
			return roadList_.Count;
		}
	}

	public int buildingCount
	{
		get
		{
			return buildingList_.Count;
		}
	}

	public int fenceCount
	{
		get
		{
			return fenceList_.Count;
		}
	}

	public List<Building> buildingList
	{
		get
		{
			return buildingList_;
		}
	}

	public LayoutGrid layoutGrid
	{
		get
		{
			return layoutGrid_;
		}
	}

	public BuildingEffects buildingEffects
	{
		get
		{
			return buildingEffects_;
		}
	}

	public MapScroll mapScroll
	{
		get
		{
			return mapScroll_;
		}
	}

	public Camera mapCamera
	{
		get
		{
			return mapCamera_;
		}
	}

	public ParkStructures.Size gridSize { get; private set; }

	public ParkStructures.Size backgroundSize { get; private set; }

	public ParkStructures.Size backgroundOffset { get; private set; }

	public ParkStructures.Size mapGridCount
	{
		get
		{
			return new ParkStructures.Size(mapInfo_.MapWidth, mapInfo_.MapHeight);
		}
	}

	public ParkStructures.Size mapSize
	{
		get
		{
			return new ParkStructures.Size(mapInfo_.GridWidth, mapInfo_.GridHeight);
		}
	}

	public bool enableDetaildLayout { get; set; }

	public Building selectedObject
	{
		get
		{
			return selectObject_;
		}
		set
		{
			selectObject_ = value;
		}
	}

	public void setParent(Transform parent)
	{
		base.transform.SetParent(parent, false);
	}

	public Road getRoad(int index)
	{
		return roadList_[index];
	}

	public Building getBuilding(int index)
	{
		return buildingList_[index];
	}

	public Fence getFence(int index)
	{
		return fenceList_[index];
	}

	public IEnumerator setup(Transform parent, DialogManager dialogManager)
	{
		dialogManager_ = dialogManager;
		colliderManager_ = ColliderManager.Instance;
		GameObject root = new GameObject("MapRoot");
		mapRoot_ = root.transform;
		mapRoot_.SetParent(parent, false);
		GameObject gridRoot = new GameObject("GridRoot");
		gridRoot_ = gridRoot.transform;
		gridRoot_.SetParent(mapRoot_, false);
		GameObject objectRoot = new GameObject("ObjectRoot");
		objectRoot_ = objectRoot.transform;
		objectRoot_.SetParent(mapRoot_, false);
		GameObject roadRoot = new GameObject("RoadRoot");
		roadRoot_ = roadRoot.transform;
		roadRoot_.SetParent(mapRoot_, false);
		yield return null;
		parkObjectReferencePack_ = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Common/ParkPart/ParkObjectReferencePack")) as GameObject;
		parkObjectReferencePack_.transform.SetParent(base.transform, false);
		parkObjectAtlases_ = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Common/ParkPart/ParkObject_Atlases")) as GameObject;
		parkObjectAtlases_.transform.SetParent(base.transform, false);
		yield return StartCoroutine(LoadAtlas());
		GameObject dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable);
		mapInfo_ = dataTable.GetComponent<ParkMapDataTable>().getInfo(0);
		gridSize = new ParkStructures.Size(mapInfo_.GridWidth, mapInfo_.GridHeight);
		backgroundSize = new ParkStructures.Size(mapInfo_.BackgroundWidth, mapInfo_.BackgroundHeight);
		backgroundOffset = new ParkStructures.Size(mapInfo_.BackgroundOffsetX, mapInfo_.BackgroundOffsetY);
		layoutGrid_ = LayoutGrid.createObject(objectRoot_);
		yield return StartCoroutine(layoutGrid_.setup(gridSize));
		buildingEffects_ = BuildingEffects.createObject(objectRoot_);
		yield return StartCoroutine(buildingEffects_.setup(gridSize));
		buildingDataTable_ = dataTable.GetComponent<ParkBuildingDataTable>();
		GameObject cameraObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Prefabs/Common/ParkPart/ParkMapCamera")) as GameObject;
		cameraObject.transform.SetParent(base.transform.parent);
		mapCamera_ = cameraObject.GetComponent<Camera>();
		colliderManager_.setCheckCamera(mapCamera_);
		mapScroll_ = mapCamera_.gameObject.AddComponent<MapScroll>();
		yield return StartCoroutine(mapScroll_.setup(backgroundSize));
		isSetupFinished_ = true;
	}

	private void Update()
	{
		if (isSetupFinished_)
		{
			if (viewMode == eParkViewMode.Friend)
			{
				colliderManager_.dontIntersect = true;
			}
			if (dialogManager_.getActiveDialogNum() > 0)
			{
				mapScroll_.enableScroll = false;
			}
			else if (colliderManager_.draggingObject == null)
			{
				mapScroll_.enableScroll = true;
			}
		}
	}

	public void Clear()
	{
		roadAtlasPool_.Clear();
		buildingSpritePool_.Clear();
		minilenAtlasPool_.Clear();
		otherAtlasPool_.Clear();
		roadSpritePool_.Clear();
		buildingSpritePool_.Clear();
		minilenSpritePool_.Clear();
		gridList_.Clear();
		roadList_.Clear();
		buildingList_.Clear();
		fenceList_.Clear();
		minilenList_.Clear();
	}

	public void setRootPosition(Vector2 position)
	{
		mapRoot_.localPosition = new Vector3(position.x, position.y, 10f);
	}

	public GameObject findPrefab(string name)
	{
		if (parkObjectReferencePack_ == null)
		{
			return null;
		}
		Transform[] componentsInChildren = parkObjectReferencePack_.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name == name)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	public IEnumerator LoadAtlas()
	{
		UIAtlas[] atlases = parkObjectAtlases_.GetComponentsInChildren<UIAtlas>(true);
		for (int i = 0; i < atlases.Length; i++)
		{
			string atlasName = atlases[i].name;
			if (atlasName.Contains("Building"))
			{
				buildingAtlasPool_.Add(atlases[i]);
			}
			else if (atlasName.Contains("Road") || atlasName.Contains("Garden"))
			{
				roadAtlasPool_.Add(atlases[i]);
			}
			else if (atlasName.Contains("Mini"))
			{
				minilenAtlasPool_.Add(atlases[i]);
			}
			else
			{
				otherAtlasPool_.Add(atlases[i]);
			}
		}
		yield break;
	}

	public UIAtlas findAtlasWithName(string atlasName)
	{
		UIAtlas uIAtlas = null;
		uIAtlas = otherAtlasPool_.Find((UIAtlas atlas) => atlas.name.Contains(atlasName));
		if (uIAtlas != null)
		{
			return uIAtlas;
		}
		uIAtlas = buildingAtlasPool_.Find((UIAtlas atlas) => atlas.name.Contains(atlasName));
		if (uIAtlas != null)
		{
			return uIAtlas;
		}
		uIAtlas = minilenAtlasPool_.Find((UIAtlas atlas) => atlas.name.Contains(atlasName));
		if (uIAtlas != null)
		{
			return uIAtlas;
		}
		uIAtlas = roadAtlasPool_.Find((UIAtlas atlas) => atlas.name.Contains(atlasName));
		if (uIAtlas != null)
		{
			return uIAtlas;
		}
		return uIAtlas;
	}

	public UIAtlas findBuildingAtlas(string spriteName, ref UIAtlas.Sprite atlasSprite)
	{
		int count = buildingAtlasPool_.Count;
		for (int i = 0; i < count; i++)
		{
			UIAtlas.Sprite sprite = buildingAtlasPool_[i].GetSprite(spriteName);
			if (sprite != null)
			{
				if (atlasSprite != null)
				{
					atlasSprite = sprite;
				}
				return buildingAtlasPool_[i];
			}
		}
		return null;
	}

	public UIAtlas findRoadAtlas(string spriteName, ref UIAtlas.Sprite atlasSprite)
	{
		int count = roadAtlasPool_.Count;
		for (int i = 0; i < count; i++)
		{
			UIAtlas.Sprite sprite = roadAtlasPool_[i].GetSprite(spriteName);
			if (sprite != null)
			{
				if (atlasSprite != null)
				{
					atlasSprite = sprite;
				}
				return roadAtlasPool_[i];
			}
		}
		return null;
	}

	public UIAtlas findOtherAtlas(string spriteName, ref UIAtlas.Sprite atlasSprite)
	{
		int count = roadAtlasPool_.Count;
		for (int i = 0; i < count; i++)
		{
			UIAtlas.Sprite sprite = otherAtlasPool_[i].GetSprite(spriteName);
			if (sprite != null)
			{
				if (atlasSprite != null)
				{
					atlasSprite = sprite;
				}
				return otherAtlasPool_[i];
			}
		}
		return null;
	}

	public void ConvertSpritesWithSpriteDetails(UIAtlas atlas, GameObject singleSpriteObject)
	{
		SpriteDetails[] componentsInChildren = singleSpriteObject.GetComponentsInChildren<SpriteDetails>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			SpriteDetails spriteDetails = componentsInChildren[i];
			if (!string.IsNullOrEmpty(spriteDetails.spriteName))
			{
				Sprite value = null;
				if (minilenSpritePool_.TryGetValue(componentsInChildren[i].spriteName, out value))
				{
					spriteDetails.spriteRenderer.sprite = value;
					continue;
				}
				value = ParkSprite.createSprite(atlas.spriteMaterial.mainTexture as Texture2D, spriteDetails.rect, spriteDetails.pivot, spriteDetails.border, spriteDetails.pixelsPerUnits);
				minilenSpritePool_.Add(componentsInChildren[i].spriteName, value);
				spriteDetails.spriteRenderer.sprite = value;
				spriteDetails.spriteRenderer.sprite.name = spriteDetails.spriteName + "_inatlas";
			}
		}
	}

	public IEnumerator createBackground(Vector2 startPosition)
	{
		if (!(background_ != null))
		{
			background_ = UnityEngine.Object.Instantiate(findPrefab("P_BG_001")) as GameObject;
			background_.SetActive(true);
			background_.transform.SetParent(mapRoot, false);
			background_.transform.localPosition = new Vector3(startPosition.x, startPosition.y, 10f);
			UIAtlas atlas = findAtlasWithName("P_BG_001".Replace("P_", "A_"));
			ConvertSpritesWithSpriteDetails(atlas, background_);
		}
		yield break;
	}

	private IEnumerator RemakeMap()
	{
		colliderManager_.Clear();
		UnityEngine.Object.Destroy(gridRoot_.gameObject);
		gridList_.Clear();
		GameObject gridRoot = new GameObject("GridRoot");
		gridRoot_ = gridRoot.transform;
		gridRoot_.SetParent(mapRoot_, false);
		yield return null;
		UnityEngine.Object.Destroy(roadRoot_.gameObject);
		roadList_.Clear();
		GameObject roadRoot = new GameObject("RoadRoot");
		roadRoot_ = roadRoot.transform;
		roadRoot_.SetParent(mapRoot_, false);
		yield return null;
		buildingList_.ForEach(delegate(Building obj)
		{
			obj.OnRemove();
			UnityEngine.Object.Destroy(obj.gameObject);
		});
		fenceList_.ForEach(delegate(Fence obj)
		{
			obj.OnRemove();
			UnityEngine.Object.Destroy(obj.gameObject);
		});
		minilenList_.ForEach(delegate(Minilen obj)
		{
			obj.OnRemove();
			UnityEngine.Object.Destroy(obj.gameObject);
		});
		yield return null;
		buildingList_.Clear();
		fenceList_.Clear();
		minilenList_.Clear();
		UnityEngine.Object.Destroy(parkMap_);
		parkMap_ = null;
		parkMap_ = mapRoot_.gameObject.AddComponent<ParkMap>();
	}

	public IEnumerator createParkMapForPlayer(int roadID, int areaReleasedCount, SaveParkData.PlacedData[] buildings)
	{
		Input.enable = false;
		FadeMng fade = UnityEngine.Object.FindObjectOfType<FadeMng>();
		GameObject loading = null;
		if (parkMap_ != null)
		{
			loading = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Loading);
			loading.SetActive(true);
			fade.setActive(FadeMng.eType.AllMask, true);
			yield return StartCoroutine(fade.startFadeOut(FadeMng.eType.AllMask));
			yield return StartCoroutine(RemakeMap());
		}
		else
		{
			parkMap_ = base.gameObject.AddComponent<ParkMap>();
		}
		GlobalRoot root = GlobalRoot.Instance;
		root.getObject(GlobalObjectParam.eObject.MapMainUI2).SetActive(true);
		root.getObject(GlobalObjectParam.eObject.MapMainUI).SetActive(true);
		root.getObject(GlobalObjectParam.eObject.FriendParkUI).SetActive(false);
		if (viewMode == eParkViewMode.Friend)
		{
			Part_Park partPark = UnityEngine.Object.FindObjectOfType<Part_Park>();
			if (partPark != null)
			{
				yield return StartCoroutine(partPark.updatePlayerInfo());
			}
		}
		viewMode = eParkViewMode.Player;
		yield return StartCoroutine(parkMap_.setup(this, gridSize, backgroundSize, roadID, areaReleasedCount, buildings));
		parkMap_.userID = GlobalData.Instance.LineID;
		parkMap_.isPlayerMap = true;
		mapScroll_.ResetZoom();
		mapCamera_.transform.localPosition = getReleasedRoadsCenter(gridSize);
		if (loading != null)
		{
			loading.SetActive(false);
			yield return StartCoroutine(fade.startFadeIn(FadeMng.eType.AllMask));
		}
		Input.enable = true;
	}

	public IEnumerator createParkMapForFriend(DialogParkNiceHistoryList.NiceHistoryListData userData)
	{
		Input.enable = false;
		FadeMng fade = UnityEngine.Object.FindObjectOfType<FadeMng>();
		GameObject loading = null;
		if (parkMap_ != null)
		{
			loading = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.Loading);
			loading.SetActive(true);
			fade.setActive(FadeMng.eType.AllMask, true);
			yield return StartCoroutine(fade.startFadeOut(FadeMng.eType.AllMask));
			PostBuildingList postData = null;
			SaveData.Instance.getParkData().getNetworkPostParameter(out postData);
			yield return StartCoroutine(SendMapUpdate(postData));
			yield return StartCoroutine(RemakeMap());
		}
		else
		{
			parkMap_ = base.gameObject.AddComponent<ParkMap>();
		}
		SaveParkData.MapSaveData savePlacedData = null;
		bool takableNice = false;
		bool isDummy = false;
		if (userData.user.IsDummy)
		{
			if (savePlacedData == null)
			{
				savePlacedData = new SaveParkData.MapSaveData();
			}
			SaveParkData.DummyParkData dummyData = SaveData.Instance.getParkData().dummyParkData;
			savePlacedData.releasedAreaCount = dummyData.ReleaseAreaCount;
			savePlacedData.roadID = Array.Find(dummyData.PlacedDatas, (SaveParkData.PlacedData p) => p.ID >= 50000).ID;
			savePlacedData.buildings = (SaveParkData.PlacedData[])dummyData.PlacedDatas.Clone();
			isDummy = true;
			yield return StartCoroutine(getFriendParkData(userData.user.ID, delegate(SaveParkData.MapSaveData ret)
			{
				savePlacedData = ret;
			}, delegate(bool tn)
			{
				takableNice = tn;
			}));
			if (savePlacedData.roadID == 0)
			{
				savePlacedData = SaveParkData.getInitialData();
			}
		}
		else
		{
			yield return StartCoroutine(getFriendParkData(userData.user.ID, delegate(SaveParkData.MapSaveData ret)
			{
				savePlacedData = ret;
			}, delegate(bool tn)
			{
				takableNice = tn;
			}));
			if (savePlacedData.roadID == 0)
			{
				savePlacedData = SaveParkData.getInitialData();
			}
		}
		viewMode = eParkViewMode.Friend;
		GlobalRoot root = GlobalRoot.Instance;
		root.getObject(GlobalObjectParam.eObject.MapMainUI2).SetActive(false);
		root.getObject(GlobalObjectParam.eObject.MapMainUI).SetActive(false);
		GameObject friendParkUI = root.getObject(GlobalObjectParam.eObject.FriendParkUI);
		MenuParkFriendMap friendMap = friendParkUI.GetComponent<MenuParkFriendMap>();
		friendMap.setup(userData, takableNice);
		friendMap.setActive(true);
		yield return StartCoroutine(parkMap_.setup(this, gridSize, backgroundSize, savePlacedData.roadID, savePlacedData.releasedAreaCount, savePlacedData.buildings, isDummy));
		parkMap_.userID = userData.user.ID;
		parkMap_.isPlayerMap = false;
		mapScroll_.ResetZoom();
		mapCamera_.transform.localPosition = getReleasedRoadsCenter(gridSize);
		layoutGrid_.ForceUnvisible();
		if (loading != null)
		{
			loading.SetActive(false);
			yield return StartCoroutine(fade.startFadeIn(FadeMng.eType.AllMask));
		}
		Input.enable = true;
	}

	public Grid createGrid(Vector3 position)
	{
		if (createGameObject_)
		{
			GameObject gameObject = new GameObject("Grid " + gridList_.Count);
			gameObject.transform.SetParent(gridRoot_, false);
			Grid grid = gameObject.AddComponent<Grid>();
			grid.setupImmediate(position, gridSize);
			gridList_.Add(grid);
			return grid;
		}
		Grid grid2 = gridRoot_.gameObject.AddComponent<Grid>();
		grid2.setupImmediate(position, gridSize);
		gridList_.Add(grid2);
		return grid2;
	}

	public Obstacle createObstacle(int id, Transform parent)
	{
		ParkBuildingInfo.BuildingInfo info = buildingDataTable_.getInfo(id);
		GameObject original = Resources.Load<GameObject>("Prefabs/Common/ParkPart/" + info.PrefabName);
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		Obstacle obstacle = gameObject.AddComponent<Obstacle>();
		obstacle.cachedTransform.SetParent(parent, false);
		obstacle.setupImmediate(id);
		string spriteName = info.SpriteName;
		Sprite sprite = null;
		if (buildingSpritePool_.ContainsKey(spriteName))
		{
			sprite = buildingSpritePool_[spriteName];
		}
		else
		{
			UIAtlas.Sprite atlasSprite = new UIAtlas.Sprite();
			UIAtlas atlas = findBuildingAtlas(spriteName, ref atlasSprite);
			sprite = ParkSprite.ConvertNGUISpriteToUnitySprite(atlas, atlasSprite, new Vector2(0.5f, 0f), Vector4.zero);
			sprite.name = spriteName + "_inatlas";
			buildingSpritePool_.Add(spriteName, sprite);
		}
		obstacle.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
		obstacle.sortingOrder = obstacle.calculateOrder();
		return obstacle;
	}

	public Road createRoad(int id, int spriteID, bool isReverse)
	{
		ParkBuildingInfo.BuildingInfo info = buildingDataTable_.getInfo(id);
		string text = info.PrefabName.Remove(0, 2).ToLower() + "_" + spriteID.ToString("00");
		Sprite sprite = null;
		if (roadSpritePool_.ContainsKey(text))
		{
			sprite = roadSpritePool_[text];
		}
		else
		{
			UIAtlas.Sprite atlasSprite = new UIAtlas.Sprite();
			UIAtlas uIAtlas = findRoadAtlas(text, ref atlasSprite);
			Rect outer = atlasSprite.outer;
			outer.y = (float)uIAtlas.spriteMaterial.mainTexture.height - atlasSprite.outer.max.y;
			outer.height = gridSize.height;
			sprite = ParkSprite.ConvertNGUISpriteToUnitySprite(uIAtlas, outer, new Vector2(0.5f, 0f), Vector4.zero);
			sprite.name = text + "_inatlas";
			roadSpritePool_.Add(text, sprite);
		}
		GameObject gameObject = new GameObject("Road");
		gameObject.gameObject.layer = LayerMask.NameToLayer("Park");
		gameObject.transform.SetParent(roadRoot_);
		if (isReverse)
		{
			gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
		}
		else
		{
			gameObject.transform.localScale = Vector3.one;
		}
		SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = sprite;
		Road road = gameObject.AddComponent<Road>();
		road.setupImmediate(id);
		road.spriteID = spriteID;
		road.isReverce = isReverse;
		road.sortingOrder = road.calculateOrder();
		roadList_.Add(road);
		return road;
	}

	public Building createBuilding(int id)
	{
		ParkBuildingInfo.BuildingInfo info = buildingDataTable_.getInfo(id);
		GameObject gameObject = Resources.Load<GameObject>("Prefabs/Common/ParkPart/" + info.PrefabName);
		if (!gameObject)
		{
			Debug.LogError("Cannot Create Building " + info.PrefabName + ", because no Prefab found in Prefabs/Common/ParkPart/ !!!!");
			return null;
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
		Building building = gameObject2.AddComponent<Building>();
		building.cachedTransform.SetParent(objectRoot_, false);
		building.setupImmediate(id);
		if (building.useMultipleSprites)
		{
			Sprite[] array = new Sprite[building.spriteCount];
			for (int i = 0; i < building.spriteCount; i++)
			{
				string text = info.SpriteName + "_" + i.ToString("00");
				if (buildingSpritePool_.ContainsKey(text))
				{
					array[i] = buildingSpritePool_[text];
					continue;
				}
				UIAtlas.Sprite atlasSprite = new UIAtlas.Sprite();
				UIAtlas atlas = findBuildingAtlas(text, ref atlasSprite);
				array[i] = ParkSprite.ConvertNGUISpriteToUnitySprite(atlas, atlasSprite, new Vector2(0.5f, 0f), Vector4.zero);
				array[i].name = info.SpriteName + "_inatlas";
				buildingSpritePool_.Add(text, array[i]);
			}
			building.setMultipleSprites(array);
		}
		else
		{
			string spriteName = info.SpriteName;
			Sprite sprite = null;
			if (buildingSpritePool_.ContainsKey(spriteName))
			{
				sprite = buildingSpritePool_[spriteName];
			}
			else
			{
				UIAtlas.Sprite atlasSprite2 = new UIAtlas.Sprite();
				UIAtlas atlas2 = findBuildingAtlas(spriteName, ref atlasSprite2);
				sprite = ParkSprite.ConvertNGUISpriteToUnitySprite(atlas2, atlasSprite2, new Vector2(0.5f, 0f), Vector4.zero);
				sprite.name = spriteName + "_inatlas";
				buildingSpritePool_.Add(spriteName, sprite);
			}
			gameObject2.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
		}
		building.sortingOrder = building.calculateOrder();
		buildingList_.Add(building);
		return building;
	}

	public Fence createFence(int id)
	{
		ParkBuildingInfo.BuildingInfo info = buildingDataTable_.getInfo(id);
		Vector2 pivot = new Vector2(0.5f, 0f);
		GameObject original = Resources.Load<GameObject>("Prefabs/Common/ParkPart/" + info.PrefabName);
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		SpriteRenderer[] componentsInChildren = gameObject.GetComponentsInChildren<SpriteRenderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			string text = info.SpriteName + "_";
			string text2 = componentsInChildren[i].gameObject.name;
			if (text2.Contains("Pillar"))
			{
				text += 0.ToString("00");
			}
			else
			{
				string text3 = componentsInChildren[i].transform.parent.name;
				if (text3.Contains("Left"))
				{
					text = ((!text2.Contains("Shadow")) ? (text + 1.ToString("00")) : (text + 3.ToString("00")));
				}
				else if (text3.Contains("Right"))
				{
					text = ((!text2.Contains("Shadow")) ? (text + 2.ToString("00")) : (text + 4.ToString("00")));
				}
			}
			Sprite sprite = null;
			if (buildingSpritePool_.ContainsKey(text))
			{
				sprite = buildingSpritePool_[text];
			}
			else
			{
				UIAtlas.Sprite atlasSprite = new UIAtlas.Sprite();
				UIAtlas atlas = findBuildingAtlas(text, ref atlasSprite);
				sprite = ParkSprite.ConvertNGUISpriteToUnitySprite(atlas, atlasSprite, pivot, Vector4.zero);
				sprite.name = text + "_inatlas";
				buildingSpritePool_.Add(text, sprite);
			}
			componentsInChildren[i].sprite = sprite;
		}
		Fence fence = gameObject.AddComponent<Fence>();
		fence.cachedTransform.SetParent(objectRoot_, false);
		fence.setupImmediate(id);
		fence.sortingOrder = fence.calculateOrder();
		fenceList_.Add(fence);
		return fence;
	}

	public TransitGate createTransitGate(int id)
	{
		ParkBuildingInfo.BuildingInfo info = buildingDataTable_.getInfo(id);
		Vector2 pivot = new Vector2(0.5f, 0f);
		GameObject original = Resources.Load<GameObject>("Prefabs/Common/ParkPart/" + info.PrefabName);
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		TransitGate transitGate = gameObject.AddComponent<TransitGate>();
		transitGate.cachedTransform.SetParent(objectRoot_, false);
		transitGate.setupImmediate(id);
		string spriteName = info.SpriteName;
		Sprite sprite = null;
		if (buildingSpritePool_.ContainsKey(spriteName))
		{
			sprite = buildingSpritePool_[spriteName];
		}
		else
		{
			UIAtlas.Sprite atlasSprite = new UIAtlas.Sprite();
			UIAtlas atlas = findBuildingAtlas(spriteName, ref atlasSprite);
			sprite = ParkSprite.ConvertNGUISpriteToUnitySprite(atlas, atlasSprite, pivot, Vector4.zero);
			sprite.name = spriteName + "_inatlas";
			buildingSpritePool_.Add(spriteName, sprite);
		}
		gameObject.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
		buildingList_.Add(transitGate);
		return transitGate;
	}

	public Minilen createMinilen(int id)
	{
		string minilenIDString = (id % 30000).ToString("000");
		GameObject gameObject = UnityEngine.Object.Instantiate(findPrefab("P_Mini_" + minilenIDString)) as GameObject;
		gameObject.SetActive(true);
		gameObject.transform.SetParent(objectRoot_, false);
		UIAtlas uIAtlas = minilenAtlasPool_.Find((UIAtlas a) => a.name.Contains("A_Mini_" + minilenIDString));
		if (uIAtlas != null)
		{
			ConvertSpritesWithSpriteDetails(uIAtlas, gameObject);
		}
		Minilen minilen = gameObject.AddComponent<Minilen>();
		minilen.setupImmediate(id);
		minilenList_.Add(minilen);
		return minilen;
	}

	public ParkEffect createEffect(ParkEffect.eEffectType type)
	{
		ParkEffect parkEffect = null;
		GameObject gameObject = null;
		switch (type)
		{
		case ParkEffect.eEffectType.Smoke:
			gameObject = UnityEngine.Object.Instantiate(findPrefab("P_eff_smork_anim")) as GameObject;
			break;
		case ParkEffect.eEffectType.Twinkle:
			gameObject = UnityEngine.Object.Instantiate(findPrefab("P_eff_twinkle_anim")) as GameObject;
			break;
		case ParkEffect.eEffectType.Sleep:
			gameObject = UnityEngine.Object.Instantiate(findPrefab("P_eff_sleep_anim")) as GameObject;
			break;
		}
		if (gameObject != null)
		{
			gameObject.SetActive(true);
			gameObject.transform.SetParent(objectRoot_, false);
			parkEffect = gameObject.AddComponent<ParkEffect>();
			parkEffect.setupImmediate((int)type);
			parkEffect.sortingOrder = -1;
			return parkEffect;
		}
		return null;
	}

	public IEnumerator createReleaseArea()
	{
		if (releaseAreas_.Count > 0)
		{
			for (int j = 0; j < releaseAreas_.Count; j++)
			{
				UnityEngine.Object.Destroy(releaseAreas_[j].gameObject);
			}
			releaseAreas_.Clear();
		}
		ParkAreaReleaseDataTable dataTable = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ParkAreaReleaseDataTable>();
		ParkAreaReleaseInfo.AreaReleaseInfo[] releaseAreaDatas = dataTable.getAllInfo();
		int count = releaseAreaDatas.Length;
		for (int i = 0; i < count; i++)
		{
			ReleaseArea area = ReleaseArea.createObject(objectRoot_);
			yield return StartCoroutine(area.setup(releaseAreaDatas[i]));
			area.setVisible(true);
			releaseAreas_.Add(area);
		}
	}

	public void setVisibleReleaseArea(int releaseCount)
	{
		for (int i = 0; i < releaseAreas_.Count; i++)
		{
			if (i < releaseCount)
			{
				releaseAreas_[i].ForceUnvisible();
			}
		}
	}

	public void Remove(Road road)
	{
		int num = roadList_.FindIndex((Road g) => g.isSame(road));
		if (num > -1)
		{
			roadList_.RemoveAt(num);
		}
	}

	public void Remove(Building building, bool removeRelationalGrids = false)
	{
		if (removeRelationalGrids)
		{
			RemoveRelationGrid(building);
		}
		if (building.objectType == ParkObject.eType.Building)
		{
			int num = buildingList_.FindIndex((Building b) => b.isSame(building));
			if (num > -1)
			{
				buildingList_.RemoveAt(num);
			}
		}
		else if (building.objectType == ParkObject.eType.Fence)
		{
			int num2 = fenceList_.FindIndex((Fence f) => f.isSame(building));
			if (num2 > -1)
			{
				fenceList_.RemoveAt(num2);
			}
		}
		ConnectFences();
	}

	public void Remove(Minilen minilen)
	{
		int num = minilenList_.FindIndex((Minilen m) => m.isSame(minilen));
		if (num > -1)
		{
			minilenList_.RemoveAt(num);
		}
	}

	public Grid getGrid(int horizontalIndex, int verticalIndex)
	{
		return gridList_.Find((Grid o) => o.horizontalIndex == horizontalIndex && o.verticalIndex == verticalIndex);
	}

	public Grid getScreenCenterGrid()
	{
		Vector3 position = new Vector3(Screen.width / 2, Screen.height / 2);
		int num = gridCount;
		Vector3 vector = mapCamera_.ScreenToWorldPoint(position);
		for (int i = 0; i < num; i++)
		{
			if (gridList_[i].colliderObject.Contains(vector))
			{
				return gridList_[i];
			}
		}
		return null;
	}

	public void AddRelationGrid(ParkObject parkObject, List<ParkStructures.IntegerXY> relationalGrids = null)
	{
		if (relationalGrids == null)
		{
			List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
			getRelationalIndices(parkObject, ref indices);
			if (indices.Count != parkObject.gridSize.width * parkObject.gridSize.height)
			{
				return;
			}
			for (int i = 0; i < indices.Count; i++)
			{
				Grid grid = getGrid(indices[i].x, indices[i].y);
				if (!(grid != null))
				{
					continue;
				}
				switch (parkObject.objectType)
				{
				case ParkObject.eType.Fence:
				case ParkObject.eType.Building:
				{
					Building building = parkObject as Building;
					if (!grid.buildingExistsOn || grid.isSameObject(building))
					{
						grid.AttachObject(building);
					}
					break;
				}
				case ParkObject.eType.Road:
					if (!grid.roadExistsOn)
					{
						grid.AttachObject(parkObject as Road);
					}
					break;
				}
			}
		}
		else
		{
			if (relationalGrids.Count != parkObject.gridSize.width * parkObject.gridSize.height)
			{
				return;
			}
			for (int j = 0; j < relationalGrids.Count; j++)
			{
				Grid grid2 = getGrid(relationalGrids[j].x, relationalGrids[j].y);
				if (grid2 != null && !grid2.buildingExistsOn && !grid2.roadExistsOn)
				{
					switch (parkObject.objectType)
					{
					case ParkObject.eType.Fence:
					case ParkObject.eType.Building:
						grid2.AttachObject(parkObject as Building);
						break;
					case ParkObject.eType.Road:
						grid2.AttachObject(parkObject as Road);
						break;
					}
				}
			}
		}
	}

	public void RemoveRelationGrid(Building building)
	{
		List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
		getRelationalIndices(building, ref indices);
		for (int i = 0; i < indices.Count; i++)
		{
			Grid grid = getGrid(indices[i].x, indices[i].y);
			if (grid != null && grid.buildingExistsOn)
			{
				grid.DetachObject(ParkObject.eType.Building);
			}
		}
	}

	public void RemoveRelationGrid(ParkStructures.IntegerXY baseIndex, ParkStructures.Size size, ParkObject.eDirection direction)
	{
		List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
		getRelationalIndices(baseIndex, size, direction, ref indices);
		for (int i = 0; i < indices.Count; i++)
		{
			Grid grid = getGrid(indices[i].x, indices[i].y);
			if (grid != null && grid.buildingExistsOn)
			{
				grid.DetachObject(ParkObject.eType.Building);
			}
		}
	}

	public bool existsFront(ParkStructures.IntegerXY baseIndex)
	{
		ParkStructures.IntegerXY baseIndex2 = baseIndex;
		baseIndex2.y++;
		Grid grid = getGrid(baseIndex2.x, baseIndex2.y);
		if (grid != null)
		{
			return true;
		}
		ParkStructures.IntegerXY upperLeftIndex = getUpperLeftIndex(baseIndex2);
		ParkStructures.IntegerXY upperRightIndex = getUpperRightIndex(baseIndex2);
		grid = getGrid(upperLeftIndex.x, upperLeftIndex.y);
		if (grid != null)
		{
			return true;
		}
		grid = getGrid(upperRightIndex.x, upperRightIndex.y);
		if (grid != null)
		{
			return true;
		}
		return false;
	}

	private static void getUpperLeftIndex(ParkStructures.IntegerXY baseIndex, List<ParkStructures.IntegerXY> indices, int depth)
	{
		if (depth > 0)
		{
			ParkStructures.IntegerXY upperLeftIndex = getUpperLeftIndex(baseIndex);
			indices.Add(upperLeftIndex);
			if (depth > 1)
			{
				getUpperLeftIndex(upperLeftIndex, indices, depth - 1);
			}
		}
	}

	public static ParkStructures.IntegerXY getUpperLeftIndex(ParkStructures.IntegerXY baseIndex)
	{
		if (baseIndex.x % 2 == 0)
		{
			return new ParkStructures.IntegerXY(baseIndex.x - 1, baseIndex.y - 1);
		}
		return new ParkStructures.IntegerXY(baseIndex.x - 1, baseIndex.y);
	}

	public static ParkStructures.IntegerXY getLowerLeftIndex(ParkStructures.IntegerXY baseIndex)
	{
		if (baseIndex.x % 2 == 0)
		{
			return new ParkStructures.IntegerXY(baseIndex.x - 1, baseIndex.y);
		}
		return new ParkStructures.IntegerXY(baseIndex.x - 1, baseIndex.y + 1);
	}

	private static void getUpperRightIndex(ParkStructures.IntegerXY baseIndex, List<ParkStructures.IntegerXY> indices, int depth)
	{
		if (depth > 0)
		{
			ParkStructures.IntegerXY upperRightIndex = getUpperRightIndex(baseIndex);
			indices.Add(upperRightIndex);
			if (depth > 1)
			{
				getUpperRightIndex(upperRightIndex, indices, depth - 1);
			}
		}
	}

	public static ParkStructures.IntegerXY getUpperRightIndex(ParkStructures.IntegerXY baseIndex)
	{
		if (baseIndex.x % 2 == 0)
		{
			return new ParkStructures.IntegerXY(baseIndex.x + 1, baseIndex.y - 1);
		}
		return new ParkStructures.IntegerXY(baseIndex.x + 1, baseIndex.y);
	}

	public static ParkStructures.IntegerXY getLowerRightIndex(ParkStructures.IntegerXY baseIndex)
	{
		if (baseIndex.x % 2 == 0)
		{
			return new ParkStructures.IntegerXY(baseIndex.x + 1, baseIndex.y);
		}
		return new ParkStructures.IntegerXY(baseIndex.x + 1, baseIndex.y + 1);
	}

	public static void getRelationalIndices(ParkObject parkObject, ref List<ParkStructures.IntegerXY> indices)
	{
		getRelationalIndices(parkObject.index, parkObject.gridSize, parkObject.direction, ref indices);
	}

	public static void getRelationalIndices(ParkStructures.IntegerXY baseIndex, ParkStructures.Size size, ParkObject.eDirection direction, ref List<ParkStructures.IntegerXY> indices, bool ignoreOutOfRange = true)
	{
		ParkStructures.IntegerXY integerXY = baseIndex;
		indices.Add(integerXY);
		if (size.width == 1 && size.height == 1)
		{
			return;
		}
		if (direction == ParkObject.eDirection.Reverse)
		{
			for (int j = 0; j < size.height; j++)
			{
				if (j > 0)
				{
					integerXY = getUpperLeftIndex(integerXY);
					indices.Add(integerXY);
				}
				getUpperRightIndex(integerXY, indices, size.width - 1);
			}
		}
		else
		{
			for (int k = 0; k < size.height; k++)
			{
				if (k > 0)
				{
					integerXY = getUpperRightIndex(integerXY);
					indices.Add(integerXY);
				}
				getUpperLeftIndex(integerXY, indices, size.width - 1);
			}
		}
		if (ignoreOutOfRange)
		{
			indices.RemoveAll((ParkStructures.IntegerXY i) => i.x < 0 || i.y < 0);
		}
	}

	public void createBuildingOnScreenCenter(int ID, int uniqueID)
	{
		Building building = null;
		ParkBuildingInfo.BuildingInfo info = buildingDataTable_.getInfo(ID);
		switch ((ParkBuildingDataTable.eBuildingType)info.Type)
		{
		case ParkBuildingDataTable.eBuildingType.Fence:
			building = createFence(ID);
			break;
		case ParkBuildingDataTable.eBuildingType.Building:
			building = createBuilding(ID);
			break;
		}
		if ((bool)building)
		{
			Grid screenCenterGrid = getScreenCenterGrid();
			building.horizontalIndex = screenCenterGrid.horizontalIndex;
			building.verticalIndex = screenCenterGrid.verticalIndex;
			building.uniqueID = uniqueID;
			building.priority = 0;
			building.setPosition(screenCenterGrid.position);
			colliderManager_.SortBySortingOrder();
			StartObjectTracking(building, true);
			Grid candidateGrid = null;
			Vector3 position = new Vector3(Screen.width / 2, Screen.height / 2);
			Vector3 position2 = mapCamera_.ScreenToWorldPoint(position);
			if (canPlantBuilding(position2, building, ref candidateGrid) != 0)
			{
				layoutGrid.setColor(false);
				building.canReplace = false;
			}
			else
			{
				layoutGrid.setColor(true);
				building.canReplace = true;
			}
			ConnectFences();
		}
	}

	public ePlantCheckResult canPlantBuilding(Vector3 position, Building building, ref Grid candidateGrid)
	{
		ePlantCheckResult result = ePlantCheckResult.Succeeded;
		int count = gridList_.Count;
		for (int i = 0; i < count; i++)
		{
			Grid grid = gridList_[i];
			if (!grid.colliderObject.Contains(position))
			{
				continue;
			}
			candidateGrid = grid;
			int num = building.gridSize.width * building.gridSize.height;
			if (num == 1)
			{
				if (!candidateGrid.isReleased)
				{
					result = ePlantCheckResult.GridNotYetReleased;
				}
				else if (CheckGridObjects(candidateGrid, building))
				{
					result = ePlantCheckResult.ExistsObject;
				}
				break;
			}
			List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
			getRelationalIndices(grid.index, building.gridSize, building.direction, ref indices);
			if (indices.Count < num)
			{
				result = ePlantCheckResult.GridShortage;
				break;
			}
			for (int j = 0; j < indices.Count; j++)
			{
				Grid grid2 = getGrid(indices[j].x, indices[j].y);
				if (grid2 == null)
				{
					result = ePlantCheckResult.GridShortage;
					break;
				}
				if (!grid2.isReleased)
				{
					result = ePlantCheckResult.GridNotYetReleased;
					break;
				}
				if (CheckGridObjects(grid2, building))
				{
					result = ePlantCheckResult.ExistsObject;
					break;
				}
			}
			break;
		}
		return result;
	}

	public ePlantCheckResult canPlantBuilding(Building building)
	{
		List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
		getRelationalIndices(building, ref indices);
		if (indices.Count < building.gridSize.width * building.gridSize.height)
		{
			return ePlantCheckResult.GridShortage;
		}
		for (int i = 0; i < indices.Count; i++)
		{
			Grid grid = getGrid(indices[i].x, indices[i].y);
			if (grid == null)
			{
				return ePlantCheckResult.GridShortage;
			}
			if (!grid.isReleased)
			{
				return ePlantCheckResult.GridNotYetReleased;
			}
			if (CheckGridObjects(grid, building))
			{
				return ePlantCheckResult.ExistsObject;
			}
		}
		return ePlantCheckResult.Succeeded;
	}

	public static bool CheckGridObjects(Grid grid, Building building)
	{
		if (grid.roadExistsOn)
		{
			return true;
		}
		if (grid.buildingExistsOn && !grid.isSameObject(building))
		{
			return true;
		}
		return false;
	}

	public Vector3 getReleasedRoadsCenter(ParkStructures.Size gridSize)
	{
		int count = roadList_.Count;
		float num = 0f;
		float num2 = 9999f;
		for (int i = 0; i < count; i++)
		{
			if (roadList_[i].gameObject.activeSelf)
			{
				float x = roadList_[i].position.x;
				if (num < x)
				{
					num = x;
				}
				if (num2 > x)
				{
					num2 = x;
				}
			}
		}
		return new Vector3((float)(-backgroundSize.width / 2) + (num - num2) / 2f + num2, 0f);
	}

	public int findNearMinilenWithAction(Minilen minilen)
	{
		List<Minilen> list = minilenList_.FindAll((Minilen m) => m.animationState == minilen.animationState);
		if (list.Count == 0)
		{
			return 0;
		}
		int num = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (!(list[i].index == minilen.index))
			{
				List<ParkStructures.IntegerXY> indices = new List<ParkStructures.IntegerXY>();
				getRelationalIndices(new ParkStructures.IntegerXY(minilen.horizontalIndex, minilen.verticalIndex + 1), new ParkStructures.Size(3, 3), ParkObject.eDirection.Default, ref indices);
				if (indices.Contains(list[i].index))
				{
					num++;
				}
			}
		}
		return num;
	}

	public int getRareBuildingCount()
	{
		int num = 0;
		Network.MinilenData[] minielenData = Bridge.MinilenData.getMinielenData();
		int i;
		for (i = 0; i < buildingList_.Count; i++)
		{
			Network.MinilenData minilenData = Array.Find(minielenData, (Network.MinilenData m) => m.releaseBuildingID == buildingList_[i].objectID);
			if (minilenData != null)
			{
				num++;
			}
		}
		return num;
	}

	public void ScrollMapCameraByObjectDragging()
	{
		mapScroll_.ScrollByObjectDragging();
	}

	public IEnumerator ReleaseLockedArea(int areaID)
	{
		if (parkMap_ != null && parkMap_.isPlayerMap)
		{
			int areaID2 = default(int);
			ReleaseArea releaseArea = releaseAreas_.Find((ReleaseArea area) => area.areaID == areaID2);
			if (releaseArea != null)
			{
				yield return StartCoroutine(releaseArea.ReleaseLockedArea());
			}
			SaveData.Instance.getParkData().AddReleasedAreaCount();
			SaveData.Instance.getParkData().save();
		}
	}

	private bool ConnectFence(Fence fence)
	{
		bool result = false;
		fence.setActivePlates(false);
		ParkStructures.IntegerXY lowerLeftIndex = getLowerLeftIndex(fence.index);
		Fence fence2 = fenceList_.Find((Fence f) => f.index == lowerLeftIndex);
		bool flag = false;
		if (fence2 != null && fence2.objectID == fence.objectID)
		{
			flag = true;
			result = true;
		}
		fence.setActivePlate(Fence.eTransversePlate.Left, flag);
		ParkStructures.IntegerXY lowerRightIndex = getLowerRightIndex(fence.index);
		bool flag2 = false;
		Fence fence3 = fenceList_.Find((Fence f) => f.index == lowerRightIndex);
		if (fence3 != null && fence3.objectID == fence.objectID)
		{
			flag2 = true;
			result = true;
		}
		fence.setActivePlate(Fence.eTransversePlate.Right, flag2);
		return result;
	}

	public void ConnectFences()
	{
		fenceList_.Sort((Fence a, Fence b) => (int)(0f - (a.position.y - b.position.y)));
		int count = fenceList_.Count;
		for (int i = 0; i < count; i++)
		{
			ConnectFence(fenceList_[i]);
		}
	}

	private IEnumerator getFriendParkData(long friendID, Action<SaveParkData.MapSaveData> ret, Action<bool> takableNice)
	{
		NetworkMng netManager = NetworkMng.Instance;
		netManager.setup(new Hashtable { 
		{
			"memberNo",
			friendID.ToString()
		} });
		yield return netManager.StartCoroutine(netManager.download(API.ParkVisitFriendPark, true, true));
		if (netManager.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(netManager.showIcon(false));
			yield break;
		}
		WWW www = netManager.getWWW();
		FriendParkData friendData = JsonMapper.ToObject<FriendParkData>(www.text);
		FriendParkDataMember memberData = friendData.memberData;
		takableNice(memberData.takableNice);
		int roadID = 0;
		List<SaveParkData.PlacedData> placedDatas = new List<SaveParkData.PlacedData>();
		for (int i = 0; i < memberData.buildings.Length; i++)
		{
			if (memberData.buildings[i].x >= 0)
			{
				if (memberData.buildings[i].id >= 50000)
				{
					roadID = memberData.buildings[i].id;
					continue;
				}
				placedDatas.Add(new SaveParkData.PlacedData
				{
					uniqueID = memberData.buildings[i].uid,
					ID = memberData.buildings[i].id,
					x = memberData.buildings[i].x,
					y = memberData.buildings[i].y,
					direction = memberData.buildings[i].d
				});
			}
		}
		ret(new SaveParkData.MapSaveData
		{
			roadID = roadID,
			releasedAreaCount = memberData.mapReleaseNum,
			buildings = placedDatas.ToArray()
		});
	}

	public IEnumerator SendMapUpdateForRemove(Building removeObject)
	{
		PostBuildingList post = null;
		SaveData.Instance.getParkData().getNetworkPostParameter(out post);
		Building removeObject2 = default(Building);
		Array.ForEach(post.buildings, delegate(BuildingData b)
		{
			if (b.uid == removeObject2.uniqueID && b.id == removeObject2.objectID)
			{
				b.d = -1;
				b.x = -1;
				b.y = -1;
			}
		});
		yield return StartCoroutine(SendMapUpdate(post));
	}

	public IEnumerator SendMapUpdate()
	{
		PostBuildingList post = null;
		SaveData.Instance.getParkData().getNetworkPostParameter(out post);
		yield return StartCoroutine(SendMapUpdate(post));
	}

	private IEnumerator SendMapUpdate(PostBuildingList postData)
	{
		string text = JsonMapper.ToJson(postData);
		Hashtable args = new Hashtable { { "buildings", text } };
		NetworkMng.Instance.setup(args);
		yield return NetworkMng.Instance.StartCoroutine(NetworkMng.Instance.download(API.ParkUpdateMap, true, true));
		if (NetworkMng.Instance.getStatus() != NetworkMng.eStatus.Success)
		{
			yield return StartCoroutine(NetworkMng.Instance.showIcon(false));
			yield break;
		}
		WWW www = NetworkMng.Instance.getWWW();
		ParkMapUpdateData mapData = JsonMapper.ToObject<ParkMapUpdateData>(www.text);
		GameData gameData = GlobalData.Instance.getGameData();
		gameData.setParkData(null, null, null, mapData.buildings, mapData.mapReleaseNum);
		BuildingData[] roadArray = Array.FindAll(mapData.buildings, (BuildingData b) => b.id >= 50000);
		SaveParkData parkData = SaveData.Instance.getParkData();
		int currentRoadID = 0;
		if (roadArray == null || roadArray.Length == 0)
		{
			currentRoadID = parkData.roadID;
		}
		else
		{
			for (int i = 0; i < roadArray.Length; i++)
			{
				if (roadArray[i].x >= 0)
				{
					currentRoadID = roadArray[i].id;
					break;
				}
			}
		}
		parkData.roadID = currentRoadID;
		parkData.areaReleasedCount = mapData.mapReleaseNum;
		parkData.UpdatePlacedData(mapData.buildings);
	}

	public void StartObjectTracking(Building building, bool isFirst = false)
	{
		DialogParkObjectChoices dialogParkObjectChoices = dialogManager_.getDialog(DialogManager.eDialog.ParkObjectChoices) as DialogParkObjectChoices;
		dialogParkObjectChoices.StartTracking(building, isFirst);
	}

	public void StartWatchingLongPress(Building building)
	{
		DialogParkObjectChoices dialogParkObjectChoices = dialogManager_.getDialog(DialogManager.eDialog.ParkObjectChoices) as DialogParkObjectChoices;
		dialogParkObjectChoices.StartWatchingLongPress(building);
	}

	public void EndWatchingLongPress(bool savePrevFillAmount = true)
	{
		DialogParkObjectChoices dialogParkObjectChoices = dialogManager_.getDialog(DialogManager.eDialog.ParkObjectChoices) as DialogParkObjectChoices;
		dialogParkObjectChoices.EndWatchingLongPress(savePrevFillAmount);
	}

	public void StartDialogMinilenProfile(int minilenID)
	{
		GlobalData.Instance.isParkControlAfterOpeningDialog = true;
		DialogParkMinilenProfile dialogParkMinilenProfile = dialogManager_.getDialog(DialogManager.eDialog.ParkMinilenProfile) as DialogParkMinilenProfile;
		dialogParkMinilenProfile.setup(minilenID);
		StartCoroutine(dialogParkMinilenProfile.open());
	}

	public void StartDialogFriendList()
	{
		DialogParkFriendList dialogParkFriendList = dialogManager_.getDialog(DialogManager.eDialog.ParkFriendList) as DialogParkFriendList;
		if (!dialogParkFriendList.isOpen())
		{
			StartCoroutine(dialogParkFriendList.show());
		}
	}

	public IEnumerator ChangeRoadSprite(int id)
	{
		int count = roadCount;
		for (int i = 0; i < count; i++)
		{
			if (id != roadList_[i].objectID)
			{
				ParkBuildingInfo.BuildingInfo info = buildingDataTable_.getInfo(id);
				string spriteName = info.PrefabName.Remove(0, 2).ToLower() + "_" + roadList_[i].spriteID.ToString("00");
				Sprite roadSprite = null;
				if (roadSpritePool_.ContainsKey(spriteName))
				{
					roadSprite = roadSpritePool_[spriteName];
				}
				else
				{
					UIAtlas.Sprite atlasSprite = new UIAtlas.Sprite();
					UIAtlas atlas = findRoadAtlas(spriteName, ref atlasSprite);
					Rect rect = atlasSprite.outer;
					rect.y = (float)atlas.spriteMaterial.mainTexture.height - atlasSprite.outer.max.y;
					rect.height = gridSize.height;
					roadSprite = ParkSprite.ConvertNGUISpriteToUnitySprite(atlas, rect, new Vector2(0.5f, 0f), Vector4.zero);
					roadSprite.name = spriteName;
					roadSpritePool_.Add(spriteName, roadSprite);
				}
				roadList_[i].ChangeSprite(id, roadSprite);
			}
		}
		yield break;
	}
}
