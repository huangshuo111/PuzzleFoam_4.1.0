using UnityEngine;

public class FPS : MonoBehaviour
{
	private class FPS_Counter
	{
		private float accum;

		private int frames;

		public float timeleft;

		public void update()
		{
			timeleft -= Time.deltaTime;
			accum += Time.timeScale / Time.deltaTime;
			frames++;
		}

		public void reset(float interval)
		{
			timeleft = interval;
			accum = 0f;
			frames = 0;
		}

		public float getFPS()
		{
			return accum / (float)frames;
		}
	}

	private static FPS instance_;

	public float updateInterval = 0.5f;

	private FPS_Counter update_ = new FPS_Counter();

	private FPS_Counter fixedUpdate_ = new FPS_Counter();

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

	private void Start()
	{
		if (!base.GetComponent<GUIText>())
		{
			Debug.Log("UtilityFramesPerSecond needs a GUIText component!");
			base.enabled = false;
		}
		else
		{
			update_.timeleft = updateInterval;
			fixedUpdate_.timeleft = updateInterval;
		}
	}

	private void drawGUIText()
	{
		float fPS = update_.getFPS();
		float fPS2 = fixedUpdate_.getFPS();
		string text = string.Format("{0:F2} FPS\n{1:F2} FIX FPS\n", fPS, fPS2);
		base.GetComponent<GUIText>().text = text;
		if (fPS < 30f)
		{
			base.GetComponent<GUIText>().material.color = new Color(1f, 0.5f, 0f);
		}
		else if (fPS < 10f)
		{
			base.GetComponent<GUIText>().material.color = Color.red;
		}
		else
		{
			base.GetComponent<GUIText>().material.color = Color.green;
		}
	}

	private void FixedUpdate()
	{
		fixedUpdate_.update();
		if ((double)fixedUpdate_.timeleft <= 0.0)
		{
			drawGUIText();
			fixedUpdate_.reset(updateInterval);
		}
	}

	private void Update()
	{
		update_.update();
		if ((double)update_.timeleft <= 0.0)
		{
			drawGUIText();
			update_.reset(updateInterval);
		}
	}
}
