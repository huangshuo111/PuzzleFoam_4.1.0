namespace tk2dRuntime
{
	public class SpriteCollectionSize
	{
		public float orthoSize;

		public float targetHeight;

		private SpriteCollectionSize(float orthoSize, float targetHeight)
		{
			this.orthoSize = orthoSize;
			this.targetHeight = targetHeight;
		}

		public static SpriteCollectionSize Explicit(float orthoSize, float targetHeight)
		{
			return new SpriteCollectionSize(orthoSize, targetHeight);
		}

		public static SpriteCollectionSize ForTk2dCamera()
		{
			return new SpriteCollectionSize(0.5f, 1f);
		}
	}
}
