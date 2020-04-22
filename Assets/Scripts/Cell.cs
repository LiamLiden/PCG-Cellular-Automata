using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : IComparable<Cell>
{
    public bool visited;
    public int value;
    public int x;
    public int y;
    public float distance;
    public Cell prev;

    public Cell(int _x, int _y, int val)
    {
        value = val;
        x = _x;
        y = _y;
        visited = false;
    }

    /// <summary>
    /// Orders cells by distance.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Cell other)
    {
        if (distance > other.distance)
            return 1;
        else if (distance < other.distance)
            return -1;
        else
            return 0;
    }

    /// <summary>
    /// Find straight line distance to target cell from this cell.
    /// </summary>
    /// <param name="targetCell"></param>
    public void SetDistance(Cell targetCell)
    {
        distance = Mathf.Sqrt(Mathf.Pow(targetCell.x - x, 2) + Mathf.Pow(targetCell.y - y, 2));
    }

    /// <summary>
    /// Returns 1 if map[x, y] are out of bounds of map, else returns map[x, y]
    /// </summary>
    /// <param name="map"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int SafeMapValue(Cell[,] map, int x, int y)
    {
        if (x < 0 || x > map.GetUpperBound(0) || y < 0 || y > map.GetUpperBound(1))
            return 1;
        else
            return map[x, y].value;
    }

    /// <summary>
    /// Returns value of map[x, y] Moores Neighborhood
    /// </summary>
    /// <param name="map"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int MooresNeighborhood(Cell[,] map, int x, int y)
    {
        return SafeMapValue(map, x - 1, y) + SafeMapValue(map, x - 1, y + 1) + SafeMapValue(map, x, y + 1) + SafeMapValue(map, x + 1, y + 1)
            + SafeMapValue(map, x + 1, y) + SafeMapValue(map, x + 1, y - 1) + SafeMapValue(map, x, y - 1) + SafeMapValue(map, x - 1, y - 1)
            + SafeMapValue(map, x, y);
    }

    /// <summary>
    /// Returns value of map[x, y] Von Nueman Neighborhood
    /// </summary>
    /// <param name="map"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static int VonNuemanNeighborhood(Cell[,] map, int x, int y)
    {
        return SafeMapValue(map, x - 1, y) + SafeMapValue(map, x, y + 1) + SafeMapValue(map, x + 1, y) + SafeMapValue(map, x, y - 1) + SafeMapValue(map, x, y);
    }
}
