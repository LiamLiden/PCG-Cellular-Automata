using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : IComparer<Cell>
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

    public int Compare(Cell x, Cell y)
    {
        if (x.distance > y.distance)
            return 1;
        else if (x.distance < y.distance)
            return -1;
        else
            return 0;
    }

    public void SetDistance(Cell targetCell)
    {
        distance = Mathf.Sqrt(Mathf.Pow(targetCell.x - x, 2) + Mathf.Pow(targetCell.y - y, 2));
    }
}
