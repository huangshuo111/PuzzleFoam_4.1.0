using System.Collections.Generic;
using System.Text;
using SimpleJSON;

public class KakaoGameMessages
{
	public class GameMessage
	{
		public string messageId { get; private set; }

		public string senderId { get; private set; }

		public string senderNickName { get; private set; }

		public string senderProfileImageUrl { get; private set; }

		public int heart { get; private set; }

		public byte[] data { get; private set; }

		public string message { get; private set; }

		public double sentAt { get; private set; }

		public int messageCount { get; private set; }

		public GameMessage(JSONNode gameMessage)
		{
			messageId = gameMessage[KakaoStringKeys.Parsers.Leaderboard.messageId].Value.ToString();
			senderId = gameMessage[KakaoStringKeys.Parsers.Leaderboard.senderId].Value.ToString();
			senderNickName = gameMessage[KakaoStringKeys.Parsers.Leaderboard.senderNickName].Value.ToString();
			senderProfileImageUrl = gameMessage[KakaoStringKeys.Parsers.Leaderboard.senderProfileImageUrl].Value.ToString();
			heart = gameMessage[KakaoStringKeys.Parsers.Leaderboard.heart].AsInt;
			message = gameMessage[KakaoStringKeys.Parsers.Leaderboard.message].Value.ToString();
			sentAt = gameMessage[KakaoStringKeys.Parsers.Leaderboard.sentAt].AsDouble;
			messageCount = gameMessage[KakaoStringKeys.Parsers.Leaderboard.messageCount].AsInt;
			string text = gameMessage[KakaoStringKeys.Parsers.Leaderboard.data].Value.ToString();
			if (text != null && text.Length > 0)
			{
				data = Encoding.UTF8.GetBytes(text);
			}
		}
	}

	public Dictionary<string, GameMessage> gameMessages = new Dictionary<string, GameMessage>();

	private static KakaoGameMessages _instance;

	public static KakaoGameMessages Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new KakaoGameMessages();
			}
			return _instance;
		}
	}

	public void clear()
	{
		gameMessages.Clear();
	}

	public void setGameMessagesFromJSON(JSONNode root)
	{
		clear();
		JSONArray asArray = root[KakaoStringKeys.Parsers.Leaderboard.messages].AsArray;
		foreach (JSONNode item in asArray)
		{
			if (!(item == null))
			{
				GameMessage gameMessage = new GameMessage(item);
				gameMessages.Add(gameMessage.messageId, gameMessage);
			}
		}
	}

	public void updateGameMessagesFromJSON(JSONNode root)
	{
		string text = root[KakaoStringKeys.Parsers.Leaderboard.messageId].Value.ToString();
		if (text != null && gameMessages.ContainsKey(text))
		{
			gameMessages.Remove(text);
		}
	}
}
