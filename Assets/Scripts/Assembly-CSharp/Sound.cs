using System.Collections;
using UnityEngine;

public class Sound : MonoBehaviour
{
	public enum eBgm
	{
		BGM_000_Title = 0,
		BGM_001_Stage1 = 1,
		BGM_002_Stage2 = 2,
		BGM_003_Stage3 = 3,
		BGM_004_Boss = 4,
		BGM_005_Ending = 5,
		BGM_006_Event = 6,
		BGM_007_Clear = 7,
		BGM_008_Over = 8,
		BGM_010_Map = 9,
		BGM_011_ParkMap = 10,
		BGM_012_Parkstage1 = 11,
		BGM_013_Parkstage2 = 12,
		BGM_500_roulette_normal_win = 13,
		BGM_501_roulette_special_win = 14,
		BGM_502_roulette_lose = 15,
		BGM_music_challenge = 16,
		BGM_music_bonus = 17,
		Max = 18
	}

	public enum eSe
	{
		SE_102_Ready = 0,
		SE_103_Go = 1,
		SE_104_Great = 2,
		SE_105_Bravo = 3,
		SE_106_Fantastic = 4,
		SE_107_Wow = 5,
		SE_108_Yay = 6,
		SE_109_OhNo = 7,
		SE_113_Clap = 8,
		SE_201_kettei = 9,
		SE_202_cancel = 10,
		SE_203_cursor2 = 11,
		SE_204_kassya = 12,
		SE_205_tedama = 13,
		SE_206_gugugu = 14,
		SE_207_koukan = 15,
		SE_211_bound1 = 16,
		SE_212_bound2 = 17,
		SE_213_bound3 = 18,
		SE_214_bound4 = 19,
		SE_215_bound5 = 20,
		SE_216_sessyoku = 21,
		SE_217_tenjou = 22,
		SE_218_hakai = 23,
		SE_219_daihakai = 24,
		SE_220_count = 25,
		SE_222_kakoumae = 26,
		SE_225_fuusen = 27,
		SE_227_tuika = 28,
		SE_228_bossdmg = 29,
		SE_234_x2 = 30,
		SE_235_target = 31,
		SE_236_slow = 32,
		SE_239_tassei = 33,
		SE_240_hanko = 34,
		SE_241_score = 35,
		SE_242_kirakira = 36,
		SE_245_door = 37,
		SE_246_bossnodmg = 38,
		SE_247_bosspiyo = 39,
		SE_301_chakkun01 = 40,
		SE_301_chakkun02 = 41,
		SE_301_chakkun03 = 42,
		SE_321_buyitem = 43,
		SE_326_heart_break = 44,
		SE_330_thunderbubble = 45,
		SE_331_searchbubble01 = 46,
		SE_331_searchbubble02 = 47,
		SE_332_starbubble = 48,
		SE_333_timebubble = 49,
		SE_334_coinbubble = 50,
		SE_335_plusbubble = 51,
		SE_336_minusbubble = 52,
		SE_338_skulbubble = 53,
		SE_339_hyperbubble = 54,
		SE_340_bombbubble = 55,
		SE_341_shakebubble = 56,
		SE_357_readygo = 57,
		SE_360_resultstar = 58,
		SE_400_metalbubble_shot = 59,
		SE_401_metalbubble_bound = 60,
		SE_402_metalbubble_break = 61,
		SE_403_key_broken = 62,
		SE_404_chain_broken = 63,
		SE_405_metalbubble_broken = 64,
		SE_503_bee = 65,
		SE_504_bubble_freeze = 66,
		SE_505_frozen_bubble_broken = 67,
		SE_506_turn_roulette = 68,
		SE_507_bubble_freeze_big = 69,
		SE_508_tsuta_nobiru = 70,
		SE_511_fire_explosion = 71,
		SE_512_snake_voice = 72,
		SE_513_snake_move_loop = 73,
		SE_514_snake_egg_break = 74,
		SE_515_water_bubble_hit = 75,
		SE_516_water_bubble_break = 76,
		SE_517_water_bubble_waterfall = 77,
		SE_518_water_bubble_allow = 78,
		SE_519_shine_bubble_hit = 79,
		SE_520_gas_bubble_countdown = 80,
		SE_521_gas_bubble_smoke01 = 81,
		SE_522_gas_bubble_smoke02 = 82,
		SE_523_time_start = 83,
		SE_524_tokei_hari = 84,
		SE_525_tokeishutsugen = 85,
		SE_526_cloud_move = 86,
		SE_527_cloud_bound = 87,
		SE_528_vacuum_light = 88,
		SE_529_vacuum = 89,
		SE_530_vacuum_move = 90,
		SE_531_back_cloud = 91,
		SE_532_cameleon_bubble = 92,
		SE_533_unknown_bubble = 93,
		SE_534_blackhole = 94,
		SE_535_bubblefall = 95,
		SE_536_boss_shutsugen = 96,
		SE_537_bubble_flow = 97,
		SE_538_fukurou_attack = 98,
		SE_539_egg_broken = 99,
		SE_540_fukurou_damage = 100,
		SE_541_fukurou_fly = 101,
		SE_542_kani_arm_broken = 102,
		SE_543_kani_damage = 103,
		SE_544_kani_jyakin = 104,
		SE_545_mahoutsukai_mahou = 105,
		SE_546_mahoutsukai_shutsugen = 106,
		SE_547_mahoutsukai_side = 107,
		SE_548_skelton_damage = 108,
		SE_549_skelton_glowl = 109,
		SE_550_skelton_attack = 110,
		SE_551_skelton_sakebi = 111,
		SE_230_bossbakushi = 112,
		SE_560_gacha_lever = 113,
		SE_561_gacha_vibrate = 114,
		SE_562_gacha_through_chackn = 115,
		SE_563_gacha_flash_eye = 116,
		SE_564_gacha_mouth_open = 117,
		SE_565_gacha_bubble_out = 118,
		SE_566_gacha_fanfare_s = 119,
		SE_567_gacha_fanfare_a = 120,
		SE_568_gacha_fanfare_b = 121,
		SE_569_gacha_avatarLevelup_fanfare = 122,
		SE_600_obstacle_ufo_explosion = 123,
		SE_601_obstacle_ufo_move = 124,
		SE_602_obstacle_ufo_beam = 125,
		SE_603_invader_explosion = 126,
		SE_604_cannon = 127,
		SE_605_gear = 128,
		SE_606_gacha_fanfare_ss = 129,
		SE_701_park_build = 130,
		SE_702_park_move = 131,
		SE_702_park_locate = 132,
		SE_703_park_extend = 133,
		SE_704_park_mini = 134,
		SE_012_park_discover = 135,
		SE_013_park_gift = 136,
		Max = 137
	}

	private const int BGM_SOURCE_NUM = 2;

	private const int SE_SOURCE_NUM = 10;

	[SerializeField]
	private float SEVolume = 1f;

	[SerializeField]
	private float BGMVolume = 0.8f;

	public AudioClip[] bgm_clip;

	private AudioSource[] bgm_audio = new AudioSource[2];

	private int bgm_audio_index;

	public AudioClip[] se_clip;

	private AudioSource[] se_audio = new AudioSource[10];

	private float[] se_start_time = new float[10];

	private float seVolume_;

	private float bgmVolume_;

	private eBgm currentBgm_ = eBgm.Max;

	public bool BGMFading;

	private static Sound instance;

	public eBgm currentBgm
	{
		get
		{
			return currentBgm_;
		}
	}

	public static Sound Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			seVolume_ = SEVolume;
			bgmVolume_ = BGMVolume;
			for (int i = 0; i < bgm_audio.Length; i++)
			{
				bgm_audio[i] = base.gameObject.AddComponent<AudioSource>();
			}
			for (int j = 0; j < se_audio.Length; j++)
			{
				se_audio[j] = base.gameObject.AddComponent<AudioSource>();
				se_audio[j].playOnAwake = false;
			}
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (bgm_audio[bgm_audio_index].clip != null && !bgm_audio[bgm_audio_index].isPlaying && !bgm_audio[bgm_audio_index].loop)
		{
			bgm_audio[bgm_audio_index].clip = null;
		}
	}

	private void OnApplicationQuit()
	{
	}

	private AudioSource getUnusedAudioSource()
	{
		AudioSource audioSource = null;
		if (audioSource == null)
		{
			for (int i = 0; i < se_audio.Length; i++)
			{
				if (!se_audio[i].isPlaying)
				{
					audioSource = se_audio[i];
					se_start_time[i] = Time.time;
					break;
				}
			}
		}
		if (audioSource == null)
		{
			float num = Time.time;
			int num2 = 0;
			for (int j = 0; j < se_audio.Length; j++)
			{
				if (se_start_time[j] < num)
				{
					num = se_start_time[j];
					num2 = j;
				}
			}
			audioSource = se_audio[num2];
			se_start_time[num2] = Time.time;
		}
		return audioSource;
	}

	public void setSeMasterVolume(float volume)
	{
		seVolume_ = volume;
	}

	public float getDefaultSeVolume()
	{
		return SEVolume;
	}

	public void playSe(eSe num)
	{
		playSe(num, false);
	}

	public void playLoopSe(eSe num)
	{
		playSe(num, true);
	}

	public void playSe(eSe num, bool bLoop)
	{
		AudioSource unusedAudioSource = getUnusedAudioSource();
		unusedAudioSource.Stop();
		if (num >= eSe.SE_102_Ready && (int)num < se_clip.Length)
		{
			unusedAudioSource.clip = se_clip[(int)num];
			unusedAudioSource.loop = bLoop;
			unusedAudioSource.volume = seVolume_;
			unusedAudioSource.Play();
		}
	}

	public void playSeVolumeControl(eSe num, bool bLoop, float volume)
	{
		AudioSource unusedAudioSource = getUnusedAudioSource();
		unusedAudioSource.Stop();
		unusedAudioSource.clip = se_clip[(int)num];
		unusedAudioSource.loop = bLoop;
		if (seVolume_ > 0f)
		{
			unusedAudioSource.volume = volume;
		}
		else
		{
			unusedAudioSource.volume = 0f;
		}
		unusedAudioSource.Play();
	}

	public void stopSe(eSe se)
	{
		if (se < eSe.SE_102_Ready || (int)se >= se_clip.Length)
		{
			return;
		}
		for (int i = 0; i < se_audio.Length; i++)
		{
			if (se_audio[i].isPlaying && se_audio[i].clip == se_clip[(int)se])
			{
				se_audio[i].Stop();
				se_audio[i].clip = null;
			}
		}
	}

	public void stopSe()
	{
		for (int i = 0; i < se_audio.Length; i++)
		{
			if (se_audio[i].isPlaying)
			{
				se_audio[i].Stop();
				se_audio[i].clip = null;
			}
		}
	}

	public void setBgmMasterVolume(float volume)
	{
		bgmVolume_ = volume;
	}

	public float getDefaultBgmVolume()
	{
		return BGMVolume;
	}

	public float getBgmVolume()
	{
		return bgm_audio[bgm_audio_index].volume;
	}

	public void setBgmVolume(float volume)
	{
		bgm_audio[bgm_audio_index].volume = volume;
	}

	public float playBgm(eBgm num, bool b_loop)
	{
		currentBgm_ = num;
		stopBgm();
		bgm_audio[bgm_audio_index].clip = bgm_clip[(int)num];
		bgm_audio[bgm_audio_index].volume = bgmVolume_;
		bgm_audio[bgm_audio_index].loop = b_loop;
		bgm_audio[bgm_audio_index].time = 0f;
		bgm_audio[bgm_audio_index].Play();
		return bgm_audio[bgm_audio_index].clip.length;
	}

	public void pauseBgm(bool b_pause)
	{
		if (b_pause)
		{
			if (bgm_audio[bgm_audio_index].isPlaying)
			{
				bgm_audio[bgm_audio_index].Pause();
			}
		}
		else if (!bgm_audio[bgm_audio_index].isPlaying)
		{
			bgm_audio[bgm_audio_index].Play();
		}
	}

	public void stopBgm()
	{
		stopBgm(true);
	}

	public void stopBgm(bool bFade)
	{
		if (isPlayingBgm())
		{
			if (bFade)
			{
				StartCoroutine(stopBgmCoroutine(bgm_audio[bgm_audio_index]));
				bgm_audio_index = (bgm_audio_index + 1) % bgm_audio.Length;
			}
			else
			{
				bgm_audio[bgm_audio_index].Stop();
				bgm_audio[bgm_audio_index].clip = null;
			}
		}
	}

	private IEnumerator stopBgmCoroutine(AudioSource source)
	{
		BGMFading = true;
		iTween.AudioTo(base.gameObject, iTween.Hash("audiosource", source, "volume", 0, "time", 0.5f));
		yield return new WaitForSeconds(0.5f);
		source.clip = null;
		source.Stop();
		BGMFading = false;
	}

	public IEnumerator reVolumeBgmCoroutine(float newVolume)
	{
		setBgmMasterVolume(newVolume);
		if (bgm_audio[bgm_audio_index].isPlaying)
		{
			iTween.AudioTo(base.gameObject, iTween.Hash("audiosource", bgm_audio[bgm_audio_index], "volume", newVolume, "time", 0.5f));
		}
		yield break;
	}

	public float timeBgm()
	{
		return bgm_audio[bgm_audio_index].time;
	}

	public void setBGMTime(float time)
	{
		bgm_audio[bgm_audio_index].time = time;
	}

	public bool isPlayingSe()
	{
		for (int i = 0; i < se_audio.Length; i++)
		{
			if (se_audio[i].isPlaying)
			{
				return true;
			}
		}
		return false;
	}

	public bool isPlayingSe(eSe num)
	{
		for (int i = 0; i < se_audio.Length; i++)
		{
			if (se_audio[i].isPlaying && se_audio[i].clip == se_clip[(int)num])
			{
				return true;
			}
		}
		return false;
	}

	public bool isPlayingBgm()
	{
		if (bgm_audio[bgm_audio_index].clip != null)
		{
			return true;
		}
		return false;
	}

	public bool isBgmLoop()
	{
		return bgm_audio[bgm_audio_index].loop;
	}

	public void stopBgmFading()
	{
		iTween component = base.gameObject.GetComponent<iTween>();
		if (component != null)
		{
			Debug.Log("stop bgm fade.");
			Object.DestroyImmediate(component);
		}
	}
}
