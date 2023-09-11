using System;
using UnityEngine;

[Serializable]
public class UserData
{
	public long ID;

	public string Mid = string.Empty;

	public string UserName = string.Empty;

	public int Score;

	public int TotalScore;

	public string URL = string.Empty;

	public Texture2D Texture;

	public int Rank;

	public int StageNo;

	public int StageClearCount;

	public int lastStageClearProgressDay;

	public long lastUpdateTime;

	public bool IsDummy;

	public int treasureboxNum;

	public int allStarSum;

	public int TotalMinilenNum;

	public int minilenId;

	public int avatarId;

	public int throwId;

	public int supportId;

	public bool IsHeartRecvFlag = true;

	public int RankingSortScore;

	public int RankingStageScore;

	private Texture2D _Texture;

	public UserData()
	{
	}

	public UserData(string name, int score, int totalScore, string url, int stageNo, long id)
	{
		setup(name, score, totalScore, url, stageNo, id);
	}

	public void setup(string name, int score, int totalScore, string url, int stageNo, long id)
	{
		UserName = name;
		Score = score;
		URL = url;
		StageNo = stageNo;
		ID = id;
		TotalScore = totalScore;
	}

	public void setMinilenParameter(int minilen_id, int total_minilen_num)
	{
		minilenId = minilen_id;
		TotalMinilenNum = total_minilen_num;
	}

	public void setTexture(Texture2D tex)
	{
		_Texture = tex;
	}

	public Texture2D getTexture()
	{
		return _Texture;
	}

	public void Clone(out UserData output)
	{
		output = new UserData();
		output.ID = ID;
		output.Mid = Mid;
		output.UserName = UserName;
		output.Score = Score;
		output.TotalScore = TotalScore;
		output.URL = URL;
		output.Rank = Rank;
		output.StageNo = StageNo;
		output.StageClearCount = StageClearCount;
		output.lastStageClearProgressDay = lastStageClearProgressDay;
		output.lastUpdateTime = lastUpdateTime;
		output.IsDummy = IsDummy;
		output.treasureboxNum = treasureboxNum;
		output.allStarSum = allStarSum;
		output.TotalMinilenNum = TotalMinilenNum;
		output.minilenId = minilenId;
		output.avatarId = avatarId;
		output.throwId = throwId;
		output.supportId = supportId;
		output.RankingSortScore = RankingSortScore;
		output.RankingStageScore = RankingStageScore;
	}
}
