using Network;
using UnityEngine;

public class MailData : MonoBehaviour
{
	public string UserName = string.Empty;

	public long ID;

	public string Mid = string.Empty;

	public string Date = string.Empty;

	public Mail Mail;

	public void setup()
	{
		UserName = Random.Range(0, 10000).ToString();
		Date = Random.Range(2013, 2015) + "/" + Random.Range(1, 13) + "/" + Random.Range(1, 32) + "/" + Random.Range(0, 13) + ":" + Random.Range(0, 61);
	}
}
