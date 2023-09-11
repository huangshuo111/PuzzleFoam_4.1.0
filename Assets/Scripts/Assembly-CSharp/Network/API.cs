using System.Collections;
using LitJson;
using UnityEngine;

namespace Network
{
	public class API
	{
		private static void addGetParameters(Hashtable args, string key)
		{
			WWWWrap.addGetParameter(key, args[key]);
		}

		private static void addPostParameters(Hashtable args, string key)
		{
			WWWWrap.addPostParameter(key, args[key]);
		}

		public static WWW Inactive(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			if (DummyPlayFriendData.FriendNum > 0)
			{
				string text = string.Empty;
				UserData[] dummyFriends = DummyPlayFriendData.DummyFriends;
				foreach (UserData userData in dummyFriends)
				{
					if (text.Length > 0)
					{
						text += ",";
					}
					text += userData.ID;
				}
				WWWWrap.addPostParameter("memberNos", text);
			}
			if (SaveData.IsInstance())
			{
				ParkObjectManager.PostBuildingList parameter = null;
				SaveData.Instance.getParkData().getNetworkPostParameter(out parameter);
				WWWWrap.addPostParameter("buildings", JsonMapper.ToJson(parameter));
			}
			return WWWWrap.create("inactive/");
		}

		public static WWW UpdatePlayerData(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			return WWWWrap.create("player/mydata/");
		}

		public static WWW StageContinue(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "stageNo");
			addGetParameters(args, "continueFlg");
			addGetParameters(args, "continueChance");
			return WWWWrap.create("stage/continuestage/");
		}

		public static WWW StageContinueByAd(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "stageNo");
			addGetParameters(args, "continueFlg");
			return WWWWrap.create("stage/continuestagebyad/");
		}

		public static WWW StageReplay(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "stageNo");
			addGetParameters(args, "continueFlg");
			addGetParameters(args, "continueChance");
			return WWWWrap.create("stage/replay/");
		}

		public static WWW BuyJewel(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			addPostParameters(args, "productId");
			addPostParameters(args, "num");
			addPostParameters(args, "signature");
			addPostParameters(args, "data");
			return WWWWrap.create("jewelShop/purchase/");
		}

		public static WWW PresentJewel(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			addPostParameters(args, "productId");
			addPostParameters(args, "num");
			addPostParameters(args, "signature");
			addPostParameters(args, "data");
			addPostParameters(args, "targetid");
			return WWWWrap.create("jewelShop/present/");
		}

		public static WWW BuyCoin(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "num");
			addGetParameters(args, "price");
			return WWWWrap.create("charge/coin/");
		}

		public static WWW BuyHeart(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "num");
			addGetParameters(args, "price");
			return WWWWrap.create("charge/heart/");
		}

		public static WWW BuySet(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "productId");
			addGetParameters(args, "num");
			return WWWWrap.create("charge/setpayment/");
		}

		public static WWW Recovary(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "recoverySaveDate");
			return WWWWrap.create("player/recovery/");
		}

		public static WWW PlayerStar(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			addPostParameters(args, "memberNos");
			return WWWWrap.create("player/star/");
		}

		public static WWW R1(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			MessageResource instance = MessageResource.Instance;
			addGetParameters(args, "rankingType");
			addGetParameters(args, instance.getMessage(200020));
			addGetParameters(args, "cancel");
			return WWWWrap.create(instance.getMessage(210001));
		}

		public static WWW Roulette(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "rank");
			return WWWWrap.create("ranking/info/");
		}

		public static WWW BuyItem(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "stageNo");
			addGetParameters(args, "buyType");
			addGetParameters(args, "itemType");
			addGetParameters(args, "num");
			addGetParameters(args, "priceType");
			addGetParameters(args, "price");
			return WWWWrap.create("stage/buyitem/");
		}

		public static WWW StagePlay(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			addGetParameters(args, "stageNo");
			if ((bool)args["buyItem"])
			{
				addPostParameters(args, "buyType");
				addPostParameters(args, "itemType");
				addPostParameters(args, "num");
				addPostParameters(args, "priceType");
				addPostParameters(args, "price");
			}
			if (SaveData.IsInstance())
			{
				ParkObjectManager.PostBuildingList parameter = null;
				SaveData.Instance.getParkData().getNetworkPostParameter(out parameter);
				WWWWrap.addPostParameter("buildings", JsonMapper.ToJson(parameter));
			}
			return WWWWrap.create("stage/start/");
		}

		public static WWW S2(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			MessageResource instance = MessageResource.Instance;
			addGetParameters(args, instance.getMessage(200010));
			addGetParameters(args, instance.getMessage(200011));
			addGetParameters(args, instance.getMessage(200012));
			addGetParameters(args, instance.getMessage(200013));
			addGetParameters(args, instance.getMessage(200014));
			addGetParameters(args, instance.getMessage(200015));
			addGetParameters(args, instance.getMessage(200016));
			addGetParameters(args, instance.getMessage(200017));
			addPostParameters(args, instance.getMessage(200030));
			addPostParameters(args, instance.getMessage(200031));
			addPostParameters(args, instance.getMessage(200032));
			addPostParameters(args, instance.getMessage(200033));
			addPostParameters(args, "minilen");
			addPostParameters(args, "minilenBubbleCount");
			addPostParameters(args, "minilenBubbleDrop");
			return WWWWrap.create(instance.getMessage(210000));
		}

		public static WWW Login(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "isGuestUser");
			addGetParameters(args, "IAD_ID");
			return WWWWrap.create("login/", true, false);
		}

		public static WWW ServerStatusCheck(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			return WWWWrap.create("login/status/1/4/5", true, false);
		}

		public static WWW HeartRecvFlag(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			int num = ((!DummyPlayerData.Data.IsHeartRecvFlag) ? 1 : 0);
			WWWWrap.addPostParameter("heartRecvFlg", num);
			return WWWWrap.create("player/heart/recvflg/");
		}

		public static WWW RequestMarketReview(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			return WWWWrap.create("/player/market/review");
		}

		public static WWW Unregister(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			return WWWWrap.create("player/resign/");
		}

		public static WWW DailyMissionCreate(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			return WWWWrap.create("dailyMission/create/");
		}

		public static WWW BonusStageStart(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "dateKey");
			return WWWWrap.create("dailyMission/bonusstart/");
		}

		public static WWW B1(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "dateKey");
			addGetParameters(args, "coin");
			addGetParameters(args, "status");
			return WWWWrap.create("dailyMission/bonusend/");
		}

		public static WWW B2(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "dateKey");
			addGetParameters(args, "coin");
			return WWWWrap.create("dailyMission/bonusbuy/");
		}

		public static WWW BuyItemBoss(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "bossNo");
			addGetParameters(args, "level");
			addGetParameters(args, "buyType");
			addGetParameters(args, "itemType");
			addGetParameters(args, "num");
			addGetParameters(args, "priceType");
			addGetParameters(args, "price");
			return WWWWrap.create("boss/buyitem/");
		}

		public static WWW BossStageContinue(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "bossNo");
			addGetParameters(args, "level");
			addGetParameters(args, "continueFlg");
			addGetParameters(args, "continueChance");
			return WWWWrap.create("boss/continueboss/");
		}

		public static WWW BossStagePlay(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			addGetParameters(args, "bossNo");
			addGetParameters(args, "level");
			if ((bool)args["buyItem"])
			{
				addPostParameters(args, "buyType");
				addPostParameters(args, "itemType");
				addPostParameters(args, "num");
				addPostParameters(args, "priceType");
				addPostParameters(args, "price");
			}
			return WWWWrap.create("boss/start/");
		}

		public static WWW BS2(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			MessageResource instance = MessageResource.Instance;
			addGetParameters(args, instance.getMessage(200040));
			addGetParameters(args, instance.getMessage(200041));
			addGetParameters(args, instance.getMessage(200042));
			addGetParameters(args, instance.getMessage(200043));
			return WWWWrap.create(instance.getMessage(210002));
		}

		public static WWW AvatarGacha(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "categoryType");
			addGetParameters(args, "priceType");
			addGetParameters(args, "campaignType");
			return WWWWrap.create("avatar/gacha/category/");
		}

		public static WWW AvatarLevelup(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "avatarId");
			addGetParameters(args, "priceType");
			addGetParameters(args, "price");
			return WWWWrap.create("avatar/levelup/");
		}

		public static WWW AvatarSetwear(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "avatarId");
			return WWWWrap.create("avatar/setwear/");
		}

		public static WWW StageSkip(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "stageNo");
			return WWWWrap.create("stage/skip/");
		}

		public static WWW CheckKey(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "productId");
			addGetParameters(args, "num");
			return WWWWrap.create("charge/generate/checkkey/");
		}

		public static WWW SetPayment(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			addPostParameters(args, "productId");
			addPostParameters(args, "num");
			addPostParameters(args, "key");
			addPostParameters(args, "signature");
			addPostParameters(args, "data");
			addPostParameters(args, "isTest");
			return WWWWrap.create("charge/setpayment/");
		}

		public static WWW CancelPayment(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "key");
			return WWWWrap.create("charge/cancelpayment/");
		}

		public static WWW GachaList(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			return WWWWrap.create("avatar/gachalist/");
		}

		public static WWW GachaTop(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			return WWWWrap.create("avatar/gachatop/");
		}

		public static WWW ShopAll(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			return WWWWrap.create("charge/shop/all/");
		}

		public static WWW RegisterDeviceToken(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addGetParameters(args, "devicetoken");
			return WWWWrap.create("player/deviceToken/");
		}

		public static WWW RequestToastPromotionReward(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			addPostParameters(args, "memberNos");
			addPostParameters(args, "appId");
			addPostParameters(args, "ToastList");
			return WWWWrap.create("player/Toast/");
		}

		public static WWW RequestCollaboBBInfo(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			return WWWWrap.create("player/collaboBBInfo/");
		}

		public static WWW RequestMonetizationFreeAdReward(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			return WWWWrap.create("player/monetization/freeAd/reward");
		}

		public static WWW ParkMinilenThanksClear(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			int num = (int)args["minilnThanksData_id"];
			addGetParameters(args, "minilnThanksData_id");
			if (SaveData.IsInstance())
			{
				ParkObjectManager.PostBuildingList parameter = null;
				SaveData.Instance.getParkData().getNetworkPostParameter(out parameter);
				WWWWrap.addPostParameter("buildings", JsonMapper.ToJson(parameter));
			}
			return WWWWrap.create("park/minilenthanks/clear/");
		}

		public static WWW ParkSendNice(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			WWWWrap.addPostParameter("memberNo", args["memberNo"]);
			return WWWWrap.create("player/nice/");
		}

		public static WWW ParkGetNiceList(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			return WWWWrap.create("player/nice/list/");
		}

		public static WWW ParkVisitFriendPark(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			WWWWrap.addPostParameter("memberNo", args["memberNo"]);
			return WWWWrap.create("player/park/");
		}

		public static WWW ParkUpdateMap(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Get);
			addPostParameters(args, "buildings");
			return WWWWrap.create("park/mapupdate/");
		}

		public static WWW ParkGetBuildingDailyReward(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			return WWWWrap.create("park/dailyreward/");
		}

		public static WWW MinilenSetwear(Hashtable args)
		{
			WWWWrap.setup(WWWWrap.eMethod.Post);
			addPostParameters(args, "minilenId");
			return WWWWrap.create("park/minilen/setwear/");
		}
	}
}
