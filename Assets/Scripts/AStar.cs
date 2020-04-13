using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{

    // Finds shortest path from start to target
    public static List<Cell> Search(Cell[,] map, Cell start, Cell target)
    {
        SortedSet<Cell> sortedCells = new SortedSet<Cell>();
        HashSet<Cell> visited = new HashSet<Cell>();

        Cell prevCell = null;
        Cell currentCell = start;

        while (currentCell != target)
        {
            currentCell.prev = prevCell;
            // Add adjacent cells to PriorityQueue
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int tempX = currentCell.x + i;
                    int tempY = currentCell.y + j;
                    if (tempX >= 0 && tempX <= map.GetUpperBound(0) && tempY >= 0 && tempY <= map.GetUpperBound(1) && !visited.Contains(map[tempX, tempY]))
                    {
                        map[tempX, tempY].SetDistance(target);
                        sortedCells.Add(map[tempX, tempY]);
                        visited.Add(map[tempX, tempY]);
                    }   
                }
            }

            currentCell = sortedCells.Min;
        }

        // Now that we've reached the target, iterate backwards to find optimal path
        List<Cell> path = new List<Cell>();

        while (currentCell != null)
        {
            path.Add(currentCell);
            currentCell = currentCell.prev;
        }
        return path;
    }

}
