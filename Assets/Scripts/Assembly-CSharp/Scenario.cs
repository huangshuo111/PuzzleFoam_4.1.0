using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
	public enum ePlace
	{
		Invalid = -1,
		Begin = 1,
		End = 2
	}

	public enum eChara
	{
		Invalid = -1,
		Narration = 1,
		Bubblen = 2,
		Bobblen = 3,
		Chucken = 4,
		Cat = 5,
		Lizard = 6,
		Bear = 7,
		Tapir = 8,
		Frog = 9,
		Owl = 10
	}

	public class Config
	{
		public Stage[] Stages;
	}

	public class Stage
	{
		public int StageNo;

		public int Place;
	}

	public class EventParameter
	{
		public int StageNo;

		public int Place;

		public int FineBG;

		public int BG;

		public CharaInfo[] CharaInfos;

		public Talk[] Talks;
	}

	public class CharaInfo
	{
		public static float INVALID = -9999f;

		public int Chara;

		public Vector2 Pos;

		public float Angle;
	}

	public class Talk
	{
		public int Chara;

		public int MainTextID;

		public Action[] Actions;
	}

	public class Action
	{
		public int Chara;

		public int AnimationClipIdx;
	}

	private class eExAnimationClipIdx
	{
		public const int FadeOut = 5;

		public const int FadeIn = 6;

		public const int Hide = 7;

		public const int ExStart = 5;
	}

	private static Scenario instance_;

	[SerializeField]
	private float TalkWaitTime = 2f;

	private EventParameter event_;

	private Config config_;

	private Config config_challenge_;

	private Config config_collaboration_;

	private Config config_park_;

	private Dictionary<eChara, GameObject> charaDict_ = new Dictionary<eChara, GameObject>();

	private UISprite balloon_;

	private UISprite balloon_00_;

	private UILabel textLabel_;

	private UISprite fineBG_;

	private GameObject scenarioPanel_;

	private GameObject balloonRoot_;

	private GameObject balloonRoot2_;

	private UISprite balloon2_;

	private UISprite balloon2_00_;

	private UILabel textLabel2_;

	private bool isUseBalloon2;

	private MessageResource msgRes_;

	private bool bSkip_;

	private bool isChallenge;

	public bool isCollaboration;

	private bool isScenarioPark_;

	public static Scenario Instance
	{
		get
		{
			return instance_;
		}
	}

	private void Awake()
	{
		if (instance_ == null)
		{
			instance_ = this;
			Object.DontDestroyOnLoad(this);
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void init(GameObject resource)
	{
		msgRes_ = MessageResource.Instance;
		scenarioPanel_ = resource;
		fineBG_ = resource.transform.Find("bg").GetComponent<UISprite>();
		GameObject target = resource.transform.Find("SkipButton").gameObject;
		NGUIUtility.setupButton(target, base.gameObject, true);
		balloonRoot_ = resource.transform.Find("balloon").gameObject;
		balloon_ = balloonRoot_.transform.Find("balloon_bg").GetComponent<UISprite>();
		balloon_00_ = balloonRoot_.transform.Find("balloon_bg_00").GetComponent<UISprite>();
		textLabel_ = balloonRoot_.transform.Find("message_label").GetComponent<UILabel>();
		balloonRoot2_ = resource.transform.Find("balloon2").gameObject;
		balloon2_ = balloonRoot2_.transform.Find("balloon_bg").GetComponent<UISprite>();
		balloon2_00_ = balloonRoot2_.transform.Find("balloon_bg_00").GetComponent<UISprite>();
		textLabel2_ = balloonRoot2_.transform.Find("message_label").GetComponent<UILabel>();
		scenarioPanel_.SetActive(false);
	}

	private void OnButton(GameObject trigger)
	{
		switch (trigger.name)
		{
		case "SkipButton":
			Constant.SoundUtil.PlayDecideSE();
			bSkip_ = true;
			break;
		}
	}

	public void configReset()
	{
		config_ = null;
		config_challenge_ = null;
		config_collaboration_ = null;
		config_park_ = null;
	}

	public void loadConfig()
	{
		TextAsset textAsset = Resources.Load("Parameter/Scenario/config", typeof(TextAsset)) as TextAsset;
		config_ = Xml.DeserializeObject<Config>(textAsset.text) as Config;
	}

	public void loadConfig_Challenge()
	{
		TextAsset textAsset = Resources.Load("Parameter/Scenario/config_challenge", typeof(TextAsset)) as TextAsset;
		config_challenge_ = Xml.DeserializeObject<Config>(textAsset.text) as Config;
	}

	public void loadConfig_Collaboration()
	{
		TextAsset textAsset = Resources.Load("Parameter/Scenario/config_collaboration", typeof(TextAsset)) as TextAsset;
		config_collaboration_ = Xml.DeserializeObject<Config>(textAsset.text) as Config;
	}

	public void loadConfig_Park()
	{
		TextAsset textAsset = Resources.Load("Parameter/Scenario/config_park", typeof(TextAsset)) as TextAsset;
		config_park_ = Xml.DeserializeObject<Config>(textAsset.text) as Config;
	}

	public bool isScenario(int stageNo, ePlace place)
	{
		if (config_ == null)
		{
			loadConfig();
		}
		bool result = false;
		Stage[] stages = config_.Stages;
		foreach (Stage stage in stages)
		{
			if (stage.StageNo == stageNo && stage.Place == (int)place)
			{
				result = true;
				break;
			}
		}
		isChallenge = false;
		isCollaboration = false;
		isScenarioPark_ = false;
		return result;
	}

	public bool isScenario_Challenge(int stageNo, ePlace place)
	{
		if (config_challenge_ == null)
		{
			loadConfig_Challenge();
		}
		bool result = false;
		Stage[] stages = config_challenge_.Stages;
		foreach (Stage stage in stages)
		{
			if (stage.StageNo == stageNo - 1 && stage.Place == (int)place)
			{
				result = true;
				break;
			}
		}
		isChallenge = true;
		isScenarioPark_ = false;
		return result;
	}

	public bool isScenario_Collaboration(int stageNo, ePlace place)
	{
		if (config_collaboration_ == null)
		{
			loadConfig_Collaboration();
		}
		bool result = false;
		Stage[] stages = config_collaboration_.Stages;
		foreach (Stage stage in stages)
		{
			if (stage.StageNo == stageNo - 1 && stage.Place == (int)place)
			{
				result = true;
				break;
			}
		}
		isChallenge = false;
		isCollaboration = true;
		isScenarioPark_ = false;
		return result;
	}

	public bool isScenario_Park(int stageNo, ePlace place)
	{
		if (config_park_ == null)
		{
			loadConfig_Park();
		}
		bool result = false;
		Stage[] stages = config_park_.Stages;
		foreach (Stage stage in stages)
		{
			if (stage.StageNo == stageNo - 1 && stage.Place == (int)place)
			{
				result = true;
				break;
			}
		}
		isScenarioPark_ = true;
		isChallenge = false;
		isCollaboration = false;
		return result;
	}

	public IEnumerator load(int stageNo, ePlace place, GameObject uiRoot, StageDataTable stageTbl)
	{
		if (stageNo < 20001)
		{
			if (!isScenario(stageNo, place))
			{
				yield break;
			}
		}
		else if (stageNo >= 499999)
		{
			if (!isScenario_Park(stageNo, place))
			{
				yield break;
			}
			stageNo--;
		}
		else
		{
			if (!isScenario_Challenge(stageNo, place) && !isScenario_Collaboration(stageNo, place))
			{
				yield break;
			}
			stageNo--;
		}
		isUseBalloon2 = false;
		int num = stageNo;
		string fileName = "Parameter/Scenario/" + getFileName(stageNo, place);
		TextAsset dataText = Resources.Load("Parameter/Scenario/" + getFileName(stageNo, place), typeof(TextAsset)) as TextAsset;
		if (dataText == null)
		{
		}
		event_ = Xml.DeserializeObject<EventParameter>(dataText.text) as EventParameter;
		GameObject ui = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Scenario_Panel")) as GameObject;
		Utility.setParent(ui, uiRoot.transform, true);
		Instance.init(ui);
		yield return StartCoroutine(stageTbl.downloadGameResource(Constant.eDownloadDataNo.BG, 0 + (event_.BG - 1), 3, 4));
		CharaInfo[] charaInfos = event_.CharaInfos;
		foreach (CharaInfo info in charaInfos)
		{
			GameObject chara2 = null;
			int charaNum = info.Chara - 2;
			if (isCollaboration && charaNum > 1)
			{
				charaNum += 97;
			}
			else if (isScenarioPark_ && charaNum > 1)
			{
				charaNum = charaNum + 200 - 2;
			}
			string parseCharaNum = ((charaNum <= 99) ? charaNum.ToString("D2") : charaNum.ToString("D3"));
			chara2 = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Scenario_chara_" + parseCharaNum)) as GameObject;
			charaDict_[(eChara)info.Chara] = chara2;
			Utility.setParent(chara2, scenarioPanel_.transform, false);
			Vector3 pos = chara2.transform.localPosition;
			pos.x = ((info.Pos.x == CharaInfo.INVALID) ? pos.x : info.Pos.x);
			pos.y = ((info.Pos.y == CharaInfo.INVALID) ? pos.y : info.Pos.y);
			chara2.transform.localPosition = pos;
			if (info.Angle != CharaInfo.INVALID)
			{
				Vector3 angle = chara2.transform.localEulerAngles;
				angle.y = info.Angle;
				chara2.transform.localEulerAngles = angle;
			}
			if (info.Chara - 2 == 38)
			{
				isUseBalloon2 = true;
			}
		}
		GameObject bg = Object.Instantiate(ResourceLoader.Instance.loadGameObject("Prefabs/", "Stage_" + event_.BG.ToString("D2"))) as GameObject;
		Utility.setParent(bg, uiRoot.transform, false);
		bg.GetComponent<Animation>().Stop();
		bg.transform.Find("floor").gameObject.SetActive(false);
	}

	public void setup()
	{
		scenarioPanel_.SetActive(true);
		fineBG_.spriteName = "scenario_bg_" + (event_.FineBG - 1).ToString("00");
		fineBG_.gameObject.SetActive(true);
		balloonRoot_.SetActive(false);
		if (balloonRoot2_ != null)
		{
			balloonRoot2_.SetActive(false);
		}
		Talk talk = event_.Talks[0];
		Action[] actions = talk.Actions;
		foreach (Action action in actions)
		{
			playAction(action);
		}
	}

	public void cleanup()
	{
		foreach (eChara key in charaDict_.Keys)
		{
			Object.Destroy(charaDict_[key]);
		}
		fineBG_ = null;
		balloon_ = null;
		balloon_00_ = null;
		textLabel_ = null;
		balloonRoot_ = null;
		balloon2_ = null;
		balloon2_00_ = null;
		textLabel2_ = null;
		balloonRoot2_ = null;
		isUseBalloon2 = false;
		Object.Destroy(scenarioPanel_);
		scenarioPanel_ = null;
		charaDict_.Clear();
		event_ = null;
	}

	public IEnumerator play()
	{
		if (event_ == null)
		{
			yield break;
		}
		bSkip_ = false;
		float startTime = Time.time;
		while (Time.time - startTime < TalkWaitTime && !bSkip_)
		{
			yield return 0;
		}
		if (bSkip_)
		{
			yield break;
		}
		if (!isUseBalloon2)
		{
			balloonRoot_.SetActive(true);
		}
		else if (balloonRoot2_ != null && isUseBalloon2)
		{
			balloonRoot2_.SetActive(true);
		}
		for (int count = 0; event_.Talks.Length > count; count++)
		{
			if (bSkip_)
			{
				break;
			}
			Talk talk = event_.Talks[count];
			textLabel_.text = msgRes_.getMessage(talk.MainTextID);
			if (textLabel2_ != null)
			{
				textLabel2_.text = msgRes_.getMessage(talk.MainTextID);
			}
			setBalloon(talk.Chara);
			scenarioPanel_.SetActive(true);
			if (count > 0)
			{
				Action[] actions = talk.Actions;
				foreach (Action action in actions)
				{
					playAction(action);
				}
			}
			while (true)
			{
				if (bSkip_)
				{
					yield break;
				}
				if (Input.GetMouseButtonDown(0))
				{
					break;
				}
				yield return 0;
			}
			while (true)
			{
				if (bSkip_)
				{
					yield break;
				}
				if (Input.GetMouseButtonUp(0))
				{
					break;
				}
				yield return 0;
			}
			Constant.SoundUtil.PlayDecideSE();
		}
	}

	private string getFileName(int stageNo, ePlace place)
	{
		string text = stageNo + 1 + "_";
		switch (place)
		{
		case ePlace.Begin:
			text += "begin";
			break;
		case ePlace.End:
			text += "end";
			break;
		}
		return text;
	}

	private void setBalloon(int chara)
	{
		balloon_00_.gameObject.SetActive(chara == -1);
		if (balloon2_00_ != null)
		{
			balloon2_00_.gameObject.SetActive(chara == -1);
		}
		string text = "balloon_";
		switch ((eChara)chara)
		{
		case eChara.Bubblen:
			text = (isChallenge ? (text + "02") : (text + "00"));
			break;
		case eChara.Bobblen:
			text += "01";
			break;
		case eChara.Narration:
			text += "03";
			break;
		default:
			text += "02";
			break;
		}
		balloon_.spriteName = text;
		if (balloon2_ != null)
		{
			balloon2_.spriteName = text;
		}
	}

	private IEnumerator charaFadeOut(tk2dAnimatedSprite anime)
	{
		Color c = Color.white;
		while (anime.color.a > 0f)
		{
			c.a -= Time.deltaTime * 2f;
			if (c.a < 0f)
			{
				c.a = 0f;
			}
			anime.color = c;
			yield return null;
		}
		anime.color = Color.clear;
	}

	private IEnumerator charaFadeIn(tk2dAnimatedSprite anime)
	{
		Color c = Color.white;
		c.a = 0f;
		while (c.a < 1f)
		{
			c.a += Time.deltaTime * 2f;
			if (c.a > 1f)
			{
				c.a = 1f;
			}
			anime.color = c;
			yield return null;
		}
	}

	private void playAction(Action action)
	{
		tk2dAnimatedSprite componentInChildren = charaDict_[(eChara)action.Chara].GetComponentInChildren<tk2dAnimatedSprite>();
		componentInChildren.StopAllCoroutines();
		switch (action.AnimationClipIdx)
		{
		case 5:
			componentInChildren.StartCoroutine(charaFadeOut(componentInChildren));
			return;
		case 6:
			componentInChildren.StartCoroutine(charaFadeIn(componentInChildren));
			return;
		case 7:
			componentInChildren.color = Color.clear;
			return;
		}
		componentInChildren.Play(action.AnimationClipIdx - 1);
		if (componentInChildren.color == Color.clear)
		{
			componentInChildren.StartCoroutine(charaFadeIn(componentInChildren));
		}
		else
		{
			componentInChildren.color = Color.white;
		}
	}
}
