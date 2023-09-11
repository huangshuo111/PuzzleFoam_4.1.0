using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AppleGameCenter
{
	private static AppleGameCenter m_Instance;

	public static AppleGameCenter Instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new AppleGameCenter();
			}
			return m_Instance;
		}
	}

	public void Authenticate()
	{
		Social.localUser.Authenticate(HandleAuthenticated);
	}

	private void HandleAuthenticated(bool success)
	{
		Debug.Log("*** HandleAuthenticated : success = " + success);
		if (success)
		{
			Social.LoadAchievements(HandleAchievementsLoaded);
		}
	}

	private void HandleFriendsLoaded(bool success)
	{
		Debug.Log("*** HandleFriendsLoaded : success = " + success);
		IUserProfile[] friends = Social.localUser.friends;
		foreach (IUserProfile userProfile in friends)
		{
			Debug.Log("*\tfriend = " + userProfile.ToString());
		}
	}

	private void HandleAchievementsLoaded(IAchievement[] achievements)
	{
		Debug.Log("*** HandleAchievementsLoaded");
		foreach (IAchievement achievement in achievements)
		{
			Debug.Log("*\tachievement = " + achievement.ToString());
		}
	}

	private void HandleAchievementDescriptionsLoaded(IAchievementDescription[] achievementDescriptions)
	{
		Debug.Log("*** HandleAchievementDescriptionsLoaded");
		foreach (IAchievementDescription achievementDescription in achievementDescriptions)
		{
			Debug.Log("*\tachievementDescription = " + achievementDescription.ToString());
		}
	}

	private void HandleProgressReported(bool success)
	{
		Debug.Log("*** HandleProgressReported : success = " + success);
	}

	public void ShowAchievements()
	{
		if (Social.localUser.authenticated)
		{
			Social.ShowAchievementsUI();
		}
	}

	public void ReportScore(string leaderboardId, long score)
	{
		if (Social.localUser.authenticated)
		{
			Social.ReportScore(score, leaderboardId, HandleScoreReported);
		}
	}

	private void HandleScoreReported(bool success)
	{
		Debug.Log("*** HandleScoreReported : success = " + success);
	}

	public void ShowLeaderboard()
	{
		if (Social.localUser.authenticated)
		{
			Social.ShowLeaderboardUI();
		}
	}
}
