using System.Collections;
using System.Collections.Generic;

namespace Network
{
	public class Hash
	{
		public static Hashtable StageContinue(int stageNo, int cntType, bool cntChance)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.StageContinue);
			args["stageNo"] = stageNo;
			args["continueFlg"] = cntType;
			args["continueChance"] = (cntChance ? 1 : 0);
			return args;
		}

		public static Hashtable StageReplay(int stageNo, int cntType, bool cntChance)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.StageReplay);
			args["stageNo"] = stageNo;
			args["continueFlg"] = cntType;
			args["continueChance"] = (cntChance ? 1 : 0);
			return args;
		}

		public static Hashtable BuyJewel(ShopItem buyItem, string signature = "", string data = "")
		{
			Hashtable hashtable = new Hashtable();
			hashtable["productId"] = buyItem.getProductID();
			hashtable["num"] = buyItem.getNum();
			hashtable["signature"] = signature;
			hashtable["data"] = data;
			return hashtable;
		}

		public static Hashtable PresentJewel(ShopItem buyItem, string targetid, string signature = "", string data = "")
		{
			if (buyItem == null)
			{
				Debug.Log("&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&");
			}
			Hashtable hashtable = new Hashtable();
			hashtable["productId"] = buyItem.getProductID();
			hashtable["num"] = buyItem.getNum();
			hashtable["signature"] = signature;
			hashtable["data"] = data;
			hashtable["targetid"] = targetid;
			return hashtable;
		}

		public static Hashtable BuyJewel(JewelShopInfo buyItem, string signature = "", string data = "")
		{
			Hashtable hashtable = new Hashtable();
			hashtable["productId"] = buyItem.productId;
			hashtable["num"] = buyItem.num;
			hashtable["signature"] = signature;
			hashtable["data"] = data;
			return hashtable;
		}

		public static Hashtable BuySaleJewel(JewelShopInfo buyItem)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["productId"] = buyItem.productId;
			hashtable["num"] = buyItem.num;
			return hashtable;
		}

		public static Hashtable BuyCoin(ShopItem buyItem)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.BuyCoin);
			args["num"] = buyItem.getNum();
			args["price"] = buyItem.getPrice().ToString();
			return args;
		}

		public static Hashtable BuyHeart(ShopItem buyItem)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.BuyHeart);
			args["num"] = buyItem.getNum();
			args["price"] = buyItem.getPrice().ToString();
			return args;
		}

		public static Hashtable BuyPackage(PackageShopItem buyItem)
		{
			Hashtable hashtable = new Hashtable();
			hashtable["price"] = buyItem.GetPrice().ToString();
			return hashtable;
		}

		public static Hashtable Recovary(SaveNetworkData netData)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.Recovary);
			args["recoverySaveDate"] = netData.getRecovaryID();
			return args;
		}

		public static Hashtable PlayerStar(long[] members)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.PlayerStar);
			string text = string.Empty;
			foreach (long num in members)
			{
				if (text.Length > 0)
				{
					text += ",";
				}
				text += num;
			}
			args["memberNos"] = text;
			return args;
		}

		public static Hashtable Roulette(int p0)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.Roulette);
			args["rank"] = p0;
			return args;
		}

		public static Hashtable R1(int p0, string p1, int cancel)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.R1);
			args["rankingType"] = 1;
			string value = p0 + ":" + p1;
			MessageResource instance = MessageResource.Instance;
			args[instance.getMessage(200020)] = value;
			args["cancel"] = cancel;
			return args;
		}

		public static Hashtable BuyItem(bool bStage, int stageNo, BoostItem item)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.BuyItem);
			args["stageNo"] = stageNo;
			args["buyType"] = ((!bStage) ? "MAP" : "STAGE");
			args["itemType"] = (int)item.getItemType();
			args["num"] = item.getNum();
			args["priceType"] = (int)item.getPriceType();
			args["price"] = item.getPrice();
			return args;
		}

		public static Hashtable StagePlay(int stageNo, List<BoostItem> itemList)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.StageBegin);
			args["stageNo"] = stageNo;
			int count = itemList.Count;
			args["buyItem"] = count > 0;
			if (count > 0)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = string.Empty;
				string text4 = string.Empty;
				bool flag = true;
				for (int i = 0; i < itemList.Count; i++)
				{
					BoostItem boostItem = itemList[i];
					text += (int)boostItem.getItemType();
					text2 += boostItem.getNum();
					text3 += (int)boostItem.getPriceType();
					text4 += boostItem.getPrice();
					if (i != itemList.Count - 1)
					{
						text += ",";
						text2 += ",";
						text3 += ",";
						text4 += ",";
					}
					if (!boostItem.bSpecial_)
					{
						flag = false;
					}
				}
				args["buyType"] = "START";
				args["itemType"] = text;
				args["num"] = text2;
				args["priceType"] = text3;
				args["price"] = text4;
			}
			return args;
		}

		public static Hashtable S2(int p0, int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10, int p11, int p12, int p13, string p14)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.S2);
			MessageResource instance = MessageResource.Instance;
			args[instance.getMessage(200010)] = p0;
			args[instance.getMessage(200011)] = p1;
			args[instance.getMessage(200012)] = p2;
			args[instance.getMessage(200013)] = p3;
			args[instance.getMessage(200014)] = p4;
			args[instance.getMessage(200015)] = p5;
			args[instance.getMessage(200016)] = p6;
			args[instance.getMessage(200017)] = p7;
			args[instance.getMessage(200030)] = p8;
			args[instance.getMessage(200031)] = p9;
			args[instance.getMessage(200032)] = p10;
			args[instance.getMessage(200033)] = p11;
			args["minilen"] = p12;
			args["minilenBubbleCount"] = p13;
			args["minilenBubbleDrop"] = p14;
			return args;
		}

		public static Hashtable Login(bool bGuestUser)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.Login);
			args["isGuestUser"] = (bGuestUser ? 1 : 0);
			args["IAD_ID"] = GKUnityPluginController.m_AdvertisingId;
			return args;
		}

		public static Hashtable B1(string dateKey, int coin, int status)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.B1);
			args["dateKey"] = dateKey;
			args["coin"] = coin;
			args["status"] = status;
			return args;
		}

		public static Hashtable BonusMultipleNum()
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.BonusMultipleNum);
			return args;
		}

		public static Hashtable B2(string dateKey, int coin)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.B2);
			args["dateKey"] = dateKey;
			args["coin"] = coin;
			return args;
		}

		public static Hashtable DailyMissionCreate()
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.DailyMissionCreate);
			return args;
		}

		public static Hashtable BonusStageStart(string dateKey)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.BonusStageStart);
			args["dateKey"] = dateKey;
			return args;
		}

		public static Hashtable BuyItemBoss(bool bStage, int bossNo, int level, BoostItem item)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.BuyItemBoss);
			args["bossNo"] = bossNo;
			args["level"] = level;
			args["buyType"] = ((!bStage) ? "MAP" : "STAGE");
			args["itemType"] = (int)item.getItemType();
			args["num"] = item.getNum();
			args["priceType"] = (int)item.getPriceType();
			args["price"] = item.getPrice();
			return args;
		}

		public static Hashtable BossStageContinue(int bossNo, int level, int cntType, bool cntChance)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.BossStageContinue);
			args["bossNo"] = bossNo;
			args["level"] = level;
			args["continueFlg"] = cntType;
			args["continueChance"] = (cntChance ? 1 : 0);
			return args;
		}

		public static Hashtable BossStagePlay(int type, int level, List<BoostItem> itemList)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.StageBegin);
			args["bossNo"] = type;
			args["level"] = level;
			Debug.Log("bossNo:" + type);
			Debug.Log("level:" + level);
			int count = itemList.Count;
			args["buyItem"] = count > 0;
			if (count > 0)
			{
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = string.Empty;
				string text4 = string.Empty;
				bool flag = true;
				for (int i = 0; i < itemList.Count; i++)
				{
					BoostItem boostItem = itemList[i];
					text += (int)boostItem.getItemType();
					text2 += boostItem.getNum();
					text3 += (int)boostItem.getPriceType();
					text4 += boostItem.getPrice();
					if (i != itemList.Count - 1)
					{
						text += ",";
						text2 += ",";
						text3 += ",";
						text4 += ",";
					}
					if (!boostItem.bSpecial_)
					{
						flag = false;
					}
				}
				args["buyType"] = "START";
				args["itemType"] = text;
				args["num"] = text2;
				args["priceType"] = text3;
				args["price"] = text4;
			}
			return args;
		}

		public static Hashtable BS2(int p0, int p1, int p2, int p3)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.BS2);
			MessageResource instance = MessageResource.Instance;
			args[instance.getMessage(200040)] = p0;
			args[instance.getMessage(200041)] = p1;
			args[instance.getMessage(200042)] = p2;
			args[instance.getMessage(200043)] = p3;
			return args;
		}

		public static Hashtable AvatarGacha(int categoryType, int priceType, int campaignType)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.GachaStart);
			args["categoryType"] = categoryType;
			args["priceType"] = priceType;
			args["campaignType"] = campaignType;
			return args;
		}

		public static Hashtable AvatarLevelup(int p0, int p1, int avatarID)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.AvatarLevelup);
			args["avatarId"] = avatarID;
			args["priceType"] = p0;
			args["price"] = p1;
			return args;
		}

		public static Hashtable AvatarSetWear(int avatarID)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.AvatarSetWear);
			args["avatarId"] = avatarID;
			return args;
		}

		public static Hashtable MinilenSetWear(int minilenID)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.MinilenSetWear);
			args["minilenId"] = minilenID;
			return args;
		}

		public static Hashtable StageSkip(int stageNo)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.StageSkip);
			args["stageNo"] = stageNo;
			return args;
		}

		public static Hashtable CheckKey(string productId, int num)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.CheckKey);
			args["productId"] = productId;
			args["num"] = num;
			return args;
		}

		public static Hashtable SetPayment(string productId, int num, string key, string signature = "unityeditor", string data = "unityeditor")
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.SetPayment);
			args["productId"] = productId;
			args["num"] = num;
			args["key"] = key;
			args["signature"] = signature;
			args["data"] = data;
			args["isTest"] = "0";
			return args;
		}

		public static Hashtable CancelPayment(string key)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.CancelPayment);
			args["key"] = key;
			return args;
		}

		public static Hashtable GachaList()
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.GachaList);
			return args;
		}

		public static Hashtable GachaTop()
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.GachaTop);
			return args;
		}

		private static void addAPI(ref Hashtable args, eAPI api)
		{
			args["API"] = api;
		}

		public static Hashtable RegisterDeviceToken(string deviceToken)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.RegisterDeviceToken);
			args["devicetoken"] = deviceToken;
			return args;
		}

		public static Hashtable RequestToastPromotionReward(string memberId, string appId, string liststr)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.ToastPromotionReward);
			args["memberNos"] = memberId;
			args["appId"] = appId;
			args["ToastList"] = liststr;
			return args;
		}

		public static Hashtable RequestCollaboBBInfo()
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.CollaboBBInfo);
			return args;
		}

		public static Hashtable RequestMonetizationFreeAdReward()
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.MonetizationFreeAdReward);
			return args;
		}

		public static Hashtable StageContinueByAD(int stageNo, int cntType)
		{
			Hashtable args = new Hashtable();
			addAPI(ref args, eAPI.StageContinue);
			args["stageNo"] = stageNo;
			args["continueFlg"] = cntType;
			return args;
		}
	}
}
