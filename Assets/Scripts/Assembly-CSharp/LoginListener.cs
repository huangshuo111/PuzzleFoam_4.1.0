public class LoginListener
{
	public enum eStatus
	{
		None = 0,
		Close = 1,
		Show = 2
	}

	private eStatus status_;

	public LoginListener()
	{
		reset();
	}

	public void reset()
	{
		status_ = eStatus.None;
	}

	public eStatus getStatus()
	{
		return status_;
	}

	public void OnShow()
	{
		status_ = eStatus.Show;
	}

	public void OnClose()
	{
		status_ = eStatus.Close;
	}

	public void OnHide()
	{
	}
}
