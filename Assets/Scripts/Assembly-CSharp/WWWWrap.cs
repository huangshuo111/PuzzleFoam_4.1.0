using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWWrap
{
	public enum eMethod
	{
		Get = 0,
		Post = 1
	}

	private static string url_ = string.Empty;

	private static Hashtable requestHeader_ = null;

	private static bool bEncrypt_ = false;

	private static Dictionary<string, object> getParamDict_ = new Dictionary<string, object>();

	private static Dictionary<string, object> postParamDict_ = new Dictionary<string, object>();

	private static bool bJapan_ = false;

	public static void init(string url, Hashtable requestHeader, bool bEncrypt)
	{
		url_ = url;
		bEncrypt_ = bEncrypt;
		if (requestHeader.Contains(NetworkUtility.ResponceHeaderKeys.Region))
		{
			bJapan_ = (string)requestHeader[NetworkUtility.ResponceHeaderKeys.Region] == "JA";
		}
		if (bEncrypt)
		{
			Hashtable hashtable = new Hashtable();
			foreach (string key3 in requestHeader.Keys)
			{
				string value = requestHeader[key3].ToString();
				hashtable[key3] = encrypt(value);
			}
			requestHeader_ = new Hashtable();
			foreach (string key4 in hashtable.Keys)
			{
				requestHeader_[key4] = hashtable[key4];
			}
			requestHeader_[NetworkUtility.ResponceHeaderKeys.Encrypt] = "ON";
		}
		else
		{
			requestHeader_ = requestHeader;
		}
	}

	public static void ChangeMemberNo(string memberno)
	{
		if (bEncrypt_)
		{
			requestHeader_[NetworkUtility.ResponceHeaderKeys.MemberNo] = encrypt(memberno);
		}
		else
		{
			requestHeader_[NetworkUtility.ResponceHeaderKeys.MemberNo] = memberno;
		}
		DummyPlayerData.Data.ID = (GlobalData.Instance.LineID = Convert.ToInt64(memberno));
	}

	public static bool isJapan()
	{
		return bJapan_;
	}

	public static void setup(eMethod method)
	{
		getParamDict_.Clear();
		postParamDict_.Clear();
		switch (method)
		{
		case eMethod.Get:
			addGetParameter("Method", "Get");
			break;
		case eMethod.Post:
			addGetParameter("Method", "Post");
			break;
		}
	}

	public static void addGetParameter(string key, object param)
	{
		getParamDict_[key] = param;
	}

	public static void addPostParameter(string key, object param)
	{
		postParamDict_[key] = param;
	}

	public static Dictionary<string, object> getGetParameter()
	{
		return getParamDict_;
	}

	public static WWW create(string uri)
	{
		return create(uri, true, true);
	}

	public static WWW create(string uri, bool bSessionID)
	{
		return create(uri, true, bSessionID);
	}

	public static WWW create(string uri, bool bRequestHeader, bool bSessionID)
	{
		WWWForm form = new WWWForm();
		Hashtable headers = form.headers;
		uri = addField(ref form, uri, getParamDict_);
		addField(ref form, string.Empty, postParamDict_);
		if (bRequestHeader)
		{
			foreach (object key in requestHeader_.Keys)
			{
				headers[key] = requestHeader_[key].ToString();
			}
			if (bSessionID)
			{
				SaveNetworkData networkData = SaveData.Instance.getGameData().getNetworkData();
				if (networkData.getSessionID() != null && networkData.getSessionID().Length > 0)
				{
					headers[NetworkUtility.ResponceHeaderKeys.SessionID] = getEncryptValue(networkData.getSessionID());
				}
			}
		}
		return new WWW(url_ + uri, form.data, headers);
	}

	private static string addField(ref WWWForm form, string uri, Dictionary<string, object> paramDict)
	{
		string text = uri;
		foreach (string key in paramDict.Keys)
		{
			object obj = paramDict[key];
			if (obj.GetType() == typeof(int))
			{
				int num = (int)obj;
				form.AddField(key, getEncryptValue(num.ToString()));
			}
			else
			{
				string value = obj.ToString();
				form.AddField(key, getEncryptValue(value));
			}
			if (uri.Length > 0 && key != "Method")
			{
				string text2 = obj.ToString();
				if (bEncrypt_)
				{
					text2 = encrypt(text2);
				}
				text = text + text2 + "/";
			}
		}
		return text;
	}

	private static string getEncryptValue(string value)
	{
		if (bEncrypt_)
		{
			return encrypt(value);
		}
		return value;
	}

	private static string encrypt(object value)
	{
		return Aes.EncryptString(value.ToString(), Aes.eEncodeType.URLSafe);
	}
}
