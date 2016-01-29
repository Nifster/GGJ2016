using UnityEngine;
using System.Collections.Generic;

public struct Point
{
	public int x;
	public int y;

	public static Point Null()
	{
		return new Point{x=-1,y=-1};
	}

	public bool IsNull()
	{
		return x == -1 && y == -1;
	}
}

internal struct Edge
{
	public int v1;
	public int v2;
}

public class GridGraph
{
	private bool[,] tileGrid;
	public int scale { get; private set; }
	public int sizeX { get; private set; }
	public int sizeY { get; private set; }
	public float realMinX;
	public float realMinY;
	public float width;
	public float height;

	private List<Point> emptyBlockList; 


	public void Initialise(bool[,] tiles, float realMinX, float realMinY, float width, float height)
	{
		scale = 3;

		this.realMinX = realMinX;
		this.realMinY = realMinY;
		this.width = width;
		this.height = height;
		this.sizeX = tiles.GetLength(0);
		this.sizeY = tiles.GetLength(1);
		tileGrid = tiles;
		InitialiseEmptyBlockList();
	}

	private void InitialiseEmptyBlockList()
	{
		emptyBlockList = new List<Point>();
		for (int y = 0; y < sizeY; ++y)
		{
			for (int x = 0; x < sizeX; ++x)
			{
				if (!tileGrid[x, y])
				{
					emptyBlockList.Add(new Point {x = x, y = y});
				}
			}
		}
	}

	public Point RandomEmptyBlock()
	{
		int choice = Random.Range(0, emptyBlockList.Count);
		return emptyBlockList[choice];
	}



	public bool IsBlocked(int gx, int gy)
	{
		if (gx < 0) return true;
		if (gy < 0) return true;
		if (gx >= sizeX) return true;
		if (gy >= sizeY) return true;

		return tileGrid[gx, gy];
	}

	public bool IsBlockedActual(float x, float y, int dx = 0, int dy = 0)
	{
		int gx = (int)((x - realMinX)/width*sizeX) + dx;
		int gy = (int)((y - realMinY)/height*sizeY) + dy;
		return IsBlocked(gx, gy);
	}

	/// <summary>
	/// Input: Unity Coordinates. Output: Grid Coordinates.
	/// </summary>
	public void ToGridCoordinates(float x, float y, out int gx, out int gy)
	{
		x = (x - realMinX) / width * sizeX;
		y = (y - realMinY) / height * sizeY;


		gx = (int)(x + 0.5f);
		gy = (int)(y + 0.5f);

	}

	/// <summary>
	/// Input: Grid Coordinates. Output: Unity Coordinates.
	/// </summary>
	public void ToRealCoordinates(int gx, int gy, out float x, out float y)
	{
		x = width*gx/sizeX + realMinX;
		y = height*gy/sizeY + realMinY;
	}


}