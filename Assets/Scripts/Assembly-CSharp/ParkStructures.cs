using UnityEngine;

public class ParkStructures
{
	public struct Size
	{
		public int width;

		public int height;

		public Size(int w, int h)
		{
			width = w;
			height = h;
		}

		public Size(float w, float h)
		{
			width = Mathf.RoundToInt(w + 0.0001f);
			height = Mathf.RoundToInt(h + 0.0001f);
		}

		public Vector2 toVec2()
		{
			return new Vector2(width, height);
		}

		public static Size operator *(Size size, int value)
		{
			return new Size(size.width * value, size.height * value);
		}

		public static Size operator *(Size size, float value)
		{
			return new Size((float)size.width * value, (float)size.height * value);
		}

		public static Size operator /(Size size, int value)
		{
			return new Size(size.width / value, size.height / value);
		}

		public static Size operator /(Size size, float value)
		{
			return new Size((float)size.width * value, (float)size.height * value);
		}
	}

	public struct IntegerXY
	{
		public int x;

		public int y;

		public IntegerXY(int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2 toVec2()
		{
			return new Vector2(x, y);
		}

		public override string ToString()
		{
			return string.Format("[X,Y]=[{0},{1}]", x, y);
		}

		public static bool operator ==(IntegerXY a, IntegerXY b)
		{
			return a.x == b.x && a.y == b.y;
		}

		public static bool operator !=(IntegerXY a, IntegerXY b)
		{
			return a.x != b.x || a.y != b.y;
		}

		public static IntegerXY operator -(IntegerXY a, IntegerXY b)
		{
			return new IntegerXY(a.x - b.x, a.y - b.y);
		}
	}
}
