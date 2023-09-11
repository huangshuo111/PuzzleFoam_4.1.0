using UnityEngine;

public class Bud : MonoBehaviour
{
	private Ivy.eType ivyType;

	private int row;

	private int column;

	public Ivy.eType getType()
	{
		return ivyType;
	}

	public void setType(Ivy.eType type)
	{
		ivyType = type;
	}

	public int getRow()
	{
		return row;
	}

	public void setRow(int rw)
	{
		row = rw;
	}

	public int getColumn()
	{
		return column;
	}

	public void setColumn(int cl)
	{
		column = cl;
	}
}
