using System;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public static List<Room> rooms = new List<Room>();

    public List<Cell> edgeCells;
    public HashSet<Room> connectedRooms;

    public Room()
    {
        edgeCells = new List<Cell>();
        connectedRooms = new HashSet<Room>();
        connectedRooms.Add(this);
    }

    /// <summary>
    /// Finds the nearest unconnected room and returns information necessary to connect it.
    /// </summary>
    /// <returns></returns>
    public ConnectionInformation FindNearestUnconnected()
    {
        float minDistance = float.MaxValue;
        Cell minCell = null;
        Cell startCell = null;
        Room targetRoom = null;

        foreach (Room otherRoom in rooms)
        {
            if (connectedRooms.Contains(otherRoom))
                continue;

            foreach (Cell curCell in edgeCells)
            {
                foreach (Cell otherCell in otherRoom.edgeCells)
                {
                    float distance = Mathf.Sqrt(Mathf.Pow(otherCell.x - curCell.x, 2) + Mathf.Pow(otherCell.y - curCell.y, 2));
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        minCell = otherCell;
                        startCell = curCell;
                        targetRoom = otherRoom;
                    }
                }
            }
        }

        return new ConnectionInformation(startCell, minCell, targetRoom);
    }
}
