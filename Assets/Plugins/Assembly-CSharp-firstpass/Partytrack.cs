using PartytrackUnityPlugin;
using UnityEngine;

public class Partytrack
{
	public static string UUID = "uuid";

	public static string UDID = "udid";

	public static string ClientID = "client_id";

	private static AndroidJavaClass getTrack()
	{
		return new AndroidJavaClass("it.partytrack.sdk.Track");
	}

	private static AndroidJavaObject getActivity()
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		return androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
	}

	public static void start(int app_id, string app_key)
	{
		getTrack().CallStatic("start", getActivity(), app_id, app_key, getActivity().Call<AndroidJavaObject>("getIntent", new object[0]));
	}

	public static void openDebugInfo()
	{
		getTrack().CallStatic("setDebugMode", true);
	}

	public static void setConfigure(string name, string svalue)
	{
		getTrack().CallStatic("setOptionalparam", name, svalue);
	}

	public static void setCustomEventParameter(string name, string svalue)
	{
		getTrack().CallStatic("setCustomEventParameter", name, svalue);
	}

	public static void sendEvent(int event_id)
	{
		getTrack().CallStatic("event", event_id);
	}

	public static void sendEvent(string event_name)
	{
		getTrack().CallStatic("event", event_name);
	}

	public static void sendEventWithItems(string event_name, Item[] items)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.lang.reflect.Array");
		AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("newInstance", new object[2]
		{
			new AndroidJavaClass("it.partytrack.sdk.Item"),
			items.Length
		});
		AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("java.lang.Integer");
		AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("java.lang.Float");
		for (int i = 0; i < items.Length; i++)
		{
			AndroidJavaObject androidJavaObject2 = null;
			if (items[i].item_num.HasValue)
			{
				androidJavaObject2 = androidJavaClass2.CallStatic<AndroidJavaObject>("valueOf", new object[1] { items[i].item_num.Value });
			}
			AndroidJavaObject androidJavaObject3 = null;
			if (items[i].item_price.HasValue)
			{
				androidJavaObject3 = androidJavaClass3.CallStatic<AndroidJavaObject>("valueOf", new object[1] { (float)items[i].item_price.Value });
			}
			AndroidJavaObject androidJavaObject4 = new AndroidJavaObject("it.partytrack.sdk.Item");
			if (items[i].identifier != null)
			{
				androidJavaObject4.Set("identifier", items[i].identifier);
			}
			if (items[i].item_name != null)
			{
				androidJavaObject4.Set("name", items[i].item_name);
			}
			if (androidJavaObject2 != null)
			{
				androidJavaObject4.Set("num", androidJavaObject2);
			}
			if (androidJavaObject3 != null)
			{
				androidJavaObject4.Set("price", androidJavaObject3);
			}
			if (items[i].item_price_currency != null)
			{
				androidJavaObject4.Set("currency", items[i].item_price_currency);
			}
			if (items[i].achievement != null)
			{
				androidJavaObject4.Set("achievement", items[i].achievement);
			}
			if (items[i].content_type != null)
			{
				androidJavaObject4.Set("content_type", items[i].content_type);
			}
			if (items[i].level_value != null)
			{
				androidJavaObject4.Set("level_value", items[i].level_value);
			}
			if (items[i].max_rating_value != null)
			{
				androidJavaObject4.Set("max_rating_value", items[i].max_rating_value);
			}
			if (items[i].payment_info != null)
			{
				androidJavaObject4.Set("payment_info", items[i].payment_info);
			}
			if (items[i].rating_value != null)
			{
				androidJavaObject4.Set("rating_value", items[i].rating_value);
			}
			if (items[i].registration_method != null)
			{
				androidJavaObject4.Set("registration_method", items[i].registration_method);
			}
			if (items[i].search_string != null)
			{
				androidJavaObject4.Set("search_string", items[i].search_string);
			}
			if (items[i].virtual_currency != null)
			{
				androidJavaObject4.Set("virtual_currency", items[i].virtual_currency);
			}
			if (items[i].virtual_currency_price != null)
			{
				androidJavaObject4.Set("virtual_currency_price", items[i].virtual_currency_price);
			}
			androidJavaClass.CallStatic("set", androidJavaObject, i, androidJavaObject4);
		}
		AndroidJavaClass androidJavaClass4 = new AndroidJavaClass("java.util.Arrays");
		getTrack().CallStatic("items", event_name, androidJavaClass4.CallStatic<AndroidJavaObject>("asList", new object[1] { androidJavaObject }));
	}

	private static string sanitizeItemsToString(Item[] items)
	{
		string[] array = new string[items.Length];
		for (int i = 0; i < items.Length; i++)
		{
			string empty = string.Empty;
			empty = empty + ((items[i].identifier == null) ? string.Empty : sanitize(items[i].identifier)) + ",";
			empty = empty + ((items[i].item_name == null) ? string.Empty : sanitize(items[i].item_name)) + ",";
			empty = empty + ((!items[i].item_num.HasValue) ? string.Empty : sanitize(items[i].item_num.ToString())) + ",";
			empty = empty + ((!items[i].item_price.HasValue) ? string.Empty : sanitize(items[i].item_price.ToString())) + ",";
			empty = empty + ((items[i].item_price_currency == null) ? string.Empty : sanitize(items[i].item_price_currency)) + ",";
			empty = empty + ((items[i].achievement == null) ? string.Empty : sanitize(items[i].achievement)) + ",";
			empty = empty + ((items[i].content_type == null) ? string.Empty : sanitize(items[i].content_type)) + ",";
			empty = empty + ((items[i].level_value == null) ? string.Empty : sanitize(items[i].level_value)) + ",";
			empty = empty + ((items[i].max_rating_value == null) ? string.Empty : sanitize(items[i].max_rating_value)) + ",";
			empty = empty + ((items[i].payment_info == null) ? string.Empty : sanitize(items[i].payment_info)) + ",";
			empty = empty + ((items[i].rating_value == null) ? string.Empty : sanitize(items[i].rating_value)) + ",";
			empty = empty + ((items[i].registration_method == null) ? string.Empty : sanitize(items[i].registration_method)) + ",";
			empty = empty + ((items[i].search_string == null) ? string.Empty : sanitize(items[i].search_string)) + ",";
			empty = empty + ((items[i].virtual_currency == null) ? string.Empty : sanitize(items[i].virtual_currency)) + ",";
			empty += ((items[i].virtual_currency_price == null) ? string.Empty : sanitize(items[i].virtual_currency_price));
			array[i] = empty;
		}
		return string.Join(":", array);
	}

	private static string sanitize(string str)
	{
		return str.Replace("%", "%25").Replace(",", "%2c").Replace(":", "%3a");
	}

	public static void sendPayment(string item_name, int item_num, string item_price_currency, double item_price)
	{
		float num = (float)item_price;
		getTrack().CallStatic("payment", item_name, num, item_price_currency, item_num);
	}

	public static void disableAdvertisementOptimize()
	{
		getTrack().CallStatic("disableAdvertisementOptimize");
	}
}
