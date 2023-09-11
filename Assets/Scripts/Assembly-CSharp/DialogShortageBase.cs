using System.Collections;

public abstract class DialogShortageBase : DialogBase
{
	public enum eType
	{
		Coin = 0,
		Jewel = 1,
		Heart = 2,
		Max = 3
	}

	private eType type_ = eType.Jewel;

	private static int[] MsgIDs = new int[3] { 46, 9, 51 };

	private DialogCommon.OnDecideButton decideCB_;

	private DialogCommon.OnCancelButton cancelCB_;

	public void createCB()
	{
		decideCB_ = OnDecide;
		cancelCB_ = OnCancel;
	}

	public virtual IEnumerator show(eType type)
	{
		yield return StartCoroutine(show(type, decideCB_, cancelCB_));
	}

	public virtual IEnumerator show(eType type, DialogCommon.OnDecideButton decideCB, DialogCommon.OnCancelButton cancelCB)
	{
		type_ = type;
		DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		dialog.setup(MsgIDs[(int)type], decideCB, cancelCB, false);
		yield return StartCoroutine(dialogManager_.openDialog(dialog));
	}

	protected virtual IEnumerator OnCancel()
	{
		DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(dialog));
	}

	protected IEnumerator transition()
	{
		DialogCommon dialog = dialogManager_.getDialog(DialogManager.eDialog.Common) as DialogCommon;
		ShopDataTable shopTbl = GlobalRoot.Instance.getObject(GlobalObjectParam.eObject.DataTable).GetComponent<ShopDataTable>();
		switch (type_)
		{
		case eType.Jewel:
			yield return StartCoroutine(shopTbl.download(Constant.eShop.Jewel, dialogManager_));
			if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
			{
				DialogJewelShop shopDialog = dialogManager_.getDialog(DialogManager.eDialog.JewelShop) as DialogJewelShop;
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(dialog));
				yield return dialogManager_.StartCoroutine(shopDialog.show());
			}
			break;
		case eType.Coin:
			yield return StartCoroutine(shopTbl.download(Constant.eShop.Coin, dialogManager_));
			if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
			{
				DialogCoinShop shopDialog2 = dialogManager_.getDialog(DialogManager.eDialog.CoinShop) as DialogCoinShop;
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(dialog));
				yield return dialogManager_.StartCoroutine(shopDialog2.show());
			}
			break;
		case eType.Heart:
			yield return StartCoroutine(shopTbl.download(Constant.eShop.Heart, dialogManager_));
			if (NetworkMng.Instance.getStatus() == NetworkMng.eStatus.Success)
			{
				DialogHeartShop shopDialog3 = dialogManager_.getDialog(DialogManager.eDialog.HeartShop) as DialogHeartShop;
				yield return dialogManager_.StartCoroutine(dialogManager_.closeDialog(dialog));
				yield return dialogManager_.StartCoroutine(shopDialog3.show());
			}
			break;
		}
	}

	protected virtual IEnumerator OnDecide()
	{
		yield return dialogManager_.StartCoroutine(transition());
	}
}
