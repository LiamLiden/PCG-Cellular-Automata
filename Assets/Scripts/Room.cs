using System;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public class CellPair
    {
        public Cell startCell;
        public Cell finalCell;

        public CellPair(Cell sCell, Cell fCell)
        {
            startCell = sCell;
            finalCell = fCell;
        }
    }

    public static List<Room> rooms = new List<Room>();

    public List<Cell> edgeCells;
    public List<Room> connectedRooms;

    public Room()
    {
        edgeCells = new List<Cell>();
        connectedRooms = new List<Room>();
    }

    public CellPair FindNearestUnconnected()
    {
        float minDistance = float.MaxValue;
        Cell minCell = null;
        Cell startCell = null;

        foreach (Room tarRoom in rooms)
        {
            if (connectedRooms.Contains(tarRoom))
                continue;

            foreach (Cell curCell in edgeCells)
            {
                foreach (Cell tarCell in tarRoom.edgeCells)
                {
                    float distance = Mathf.Sqrt(Mathf.Pow(tarCell.x - curCell.x, 2) + Mathf.Pow(tarCell.y - curCell.y, 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minCell = tarCell;
                        startCell = curCell;
                    }
                }
            }
        }

        return new CellPair(startCell, minCell);
    }
}
