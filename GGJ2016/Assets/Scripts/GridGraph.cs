﻿using UnityEngine;
using System.Collections.Generic;
using System.Configuration;

public struct Point
{
	public int x;
	public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Point Null()
	{
		return new Point{x=-1,y=-1};
	}

	public bool IsNull()
	{
		return x == -1 && y == -1;
	}

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        var o = (Point)obj;
        return x == o.x && y == o.y;
    }

}

internal struct Edge
{
    public readonly int x1;
    public readonly int y1;
    public readonly int x2;
    public readonly int y2;

    public Edge(int x1, int y1, int x2, int y2)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        var o = (Edge)obj;
        return x1 == o.x1 && y1 == o.y1 && x2 == o.x2 && y2 == o.y2;
    }

    public override int GetHashCode()
    {
        int hash = 13;
        hash = (hash * 7) + x1.GetHashCode();
        hash = (hash * 7) + y1.GetHashCode();
        hash = (hash * 7) + x2.GetHashCode();
        hash = (hash * 7) + y2.GetHashCode();
        return hash;
    }
}

public class GridGraph
{
	private bool[,] tileGrid;
    private HashSet<Edge> blockedEdges;
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


    public bool IsBlockedEdge(int x1, int y1, int x2, int y2)
    {
        if (IsBlocked(x1, y1)) return true;
        if (IsBlocked(x2, y2)) return true;

        return blockedEdges.Contains(new Edge(x1, y1, x2, y2));
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

    public Queue<Point> PathFind (int sx, int sy, int ex, int ey, bool ignoreObstructions = true)
    {
        var parent = PathFindGetParentPointers(sx, sy, ex, ey, ignoreObstructions);
        if (parent == null) return null;

        var stack = new Stack<Point>();
        var curr = new Point(sx, sy);
        while (curr.x != sx || curr.y != sy)
        {
            stack.Push(curr);
            curr = parent[curr.x, curr.y];
        }
        stack.Push(curr);

        var path = new Queue<Point>();
        while (stack.Count > 0)
        {
            path.Enqueue(stack.Pop());
        }
        return path;
    }

    private Point[,] PathFindGetParentPointers(int sx, int sy, int ex, int ey, bool ignoreObstructions)
    {
        var parent = new Point[sizeX, sizeY];
        var visited = new bool[sizeX, sizeY];

        var q = new Queue<Point>();
        q.Enqueue(new Point(sx, sy));

        while (q.Count > 0)
        {
            var curr = q.Dequeue();
            for (int dx = -1; dx <= 1; ++dx)
            {
                for (int dy = -1; dy <= 1; ++dy)
                {
                    if ((dx == 0) == (dy == 0)) continue;
                    int x = curr.x + dx;
                    int y = curr.y + dy;
                    if (IsBlocked(x, y)) continue;
                    if (visited[x, y]) continue;
                    if (!ignoreObstructions && IsBlockedEdge(curr.x, curr.y, x, y)) continue;

                    parent[x, y] = curr;
                    if (x == ex && y == ey) return parent;
                    visited[x, y] = true;
                    q.Enqueue(new Point(x, y));
                }
            }
        }
        return null;
    }
}