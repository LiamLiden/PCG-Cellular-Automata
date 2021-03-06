﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CellularAutomataGenerator : MonoBehaviour
{
    public static CellularAutomataGenerator instance;

    public int width;
    public int height;
    [Tooltip("Probability of each cell being a wall during initial creation. Default 50")]
    public int initialWallPlacementProbability;
    [Tooltip("Iterations of cellular automata. Default 6")]
    public int iterations;
    [Tooltip("How many walls must surround a cell for it to become a wall. Default 5")]
    public int wallThreshold;
    [Tooltip("How many extra adjacent cells are changed to floors during connection. Default 1")]
    public int connectionSize;
    public GameObject wall;
    public GameObject ground;
    public int furnishingIterations;
    [Tooltip("Used if any furniture has a required amount value. Default 50")]
    public int maxFurnishingIterations;
    public List<GameObject> furniture;

    private Cell[,] map;

    // Start is called before the first frame update
    void Start()
    {
        // Singleton
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // Initialize size of map
        map = new Cell[width, height];
        Queue<Cell> floors = new Queue<Cell>();

        // Randomly disperse walls onto map
        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                // 50-50 chance of cell becoming a wall
                if (Random.Range(0, 100) < initialWallPlacementProbability)
                    map[x, y] = new Cell(x, y, 1);
                else
                    map[x, y] = new Cell(x, y, 0);
            }
        }

        // Run algorithm for input iterations
        for (int i = 0; i < iterations - 1; i++)
        {
            Cell[,] newMap = new Cell[width, height];
            for (int x = 0; x <= map.GetUpperBound(0); x++)
            {
                for (int y = 0; y <= map.GetUpperBound(1); y++)
                {
                    if (Cell.MooresNeighborhood(map, x, y) >= wallThreshold)
                        newMap[x, y] = new Cell(x, y, 1);
                    else
                        newMap[x, y] = new Cell(x, y, 0);
                }
            }
            map = newMap;
        }

        // Final iteration we add all floors to queue
        Cell[,] finalMap = new Cell[width, height];
        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                if (Cell.MooresNeighborhood(map, x, y) >= wallThreshold)
                    finalMap[x, y] = new Cell(x, y, 1);
                else
                {
                    finalMap[x, y] = new Cell(x, y, 0);
                    floors.Enqueue(finalMap[x, y]);
                }
                    
            }
        }
        map = finalMap;
        

        // Room Creation
        while (floors.Count > 0)
        {
            Cell current = floors.Dequeue();
            if (current.visited == false)
            {
                Room newRoom = new Room();
                CreateRoom(current, newRoom);
                Room.rooms.Add(newRoom);
            }
        }

        Room largestRoom = Room.rooms[0];
        // Find largest room
        foreach (Room curRoom in Room.rooms)
        {
            if (curRoom.edgeCells.Count > largestRoom.edgeCells.Count)
                largestRoom = curRoom;
        }

        // If a room is not connected to the largest room, connect nearest rooms until it is
        // This assures connectivity to all rooms
        foreach (Room curRoom in Room.rooms)
        {
            while (!curRoom.connectedRooms.Contains(largestRoom))
            {
                ConnectionInformation cellsToConnect = curRoom.FindNearestUnconnected();

                List<Cell> path = AStar.Search(map, cellsToConnect.startCell, cellsToConnect.endCell);
                foreach (Cell cell in path)
                {
                    // Use connectionSize to fill in connection cells
                    for (int i = -connectionSize; i <= connectionSize; i++)
                    {
                        for (int j = -connectionSize; j <= connectionSize; j++)
                        {
                            int tempX = cell.x + i;
                            int tempY = cell.y + j;
                            if (tempX >= 0 && tempX <= map.GetUpperBound(0) && tempY >= 0 && tempY <= map.GetUpperBound(1))
                                map[tempX, tempY].value = 0;
                        }
                    }
                }
                curRoom.connectedRooms.UnionWith(cellsToConnect.targetRoom.connectedRooms);
            }
        }
        // Create list that will be used for furnishing
        List<Cell> randomOrder = new List<Cell>();

        // Placement of walls and floors
        for (int x = 0; x <= map.GetUpperBound(0); x++)
        {
            for (int y = 0; y <= map.GetUpperBound(1); y++)
            {
                if (map[x, y].value == 1)
                {
                    Instantiate(wall, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (map[x, y].value == 0)
                {
                    Instantiate(ground, new Vector3(x, y, 0), Quaternion.identity);
                    // Add floors to list used for furnishing
                    randomOrder.Add(map[x, y]);
                }
            }
        }

        // Furnishing
        for (int i = 0; i < furnishingIterations; i++)
        {
            Shuffle(randomOrder);

            Cell[,] newMap = (Cell[,])map.Clone();
            foreach (Cell curCell in randomOrder)
            {
                
                if (map[curCell.x, curCell.y].value == 0)
                {
                    foreach (GameObject obj in furniture)
                    {
                        Furniture curFurniture = obj.GetComponent<Furniture>();
                        if (curFurniture.CanSpawn(map, curCell.x, curCell.y))
                        {
                            curFurniture.Spawn(curCell.x, curCell.y);
                            newMap[curCell.x, curCell.y].value = curFurniture.myValue;
                            break;
                        }
                    }
                }
            }
            map = newMap;
        }

        // Assure required amounts are met
        foreach (GameObject obj in furniture)
        {
            Furniture curFurniture = obj.GetComponent<Furniture>();
            int count = 0;
            while (curFurniture.GetAmount() < curFurniture.requiredAmount)
            {
                if (count > maxFurnishingIterations)
                {
                    Debug.LogError("Could not create level with required furniture");
                    Debug.Break();
                    break;
                }

                Shuffle(randomOrder);

                foreach (Cell curCell in randomOrder)
                {
                    if (map[curCell.x, curCell.y].value == 0 && curFurniture.CanSpawn(map, curCell.x, curCell.y))
                    {
                        curFurniture.Spawn(curCell.x, curCell.y);
                        map[curCell.x, curCell.y].value = curFurniture.myValue;
                        break;
                    }
                }
                count++;
            }
        }
    }

    /// <summary>
    /// Recursive function to create a room. Uses BFS to visit all cells in a room and adds any cells adjacent to a wall to edgeCells.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="room"></param>
    private void CreateRoom(Cell start, Room room)
    {
        // Base case
        if (start.value == 1 || start.visited == true)
            return;

        start.visited = true;
        if (Cell.MooresNeighborhood(map, start.x, start.y) > 0)
        {
            room.edgeCells.Add(start);
        }

        // Call CreateRoom on all adjacent cells
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int tempX = start.x + i;
                int tempY = start.y + j;
                if (tempX >= 0 && tempX <= map.GetUpperBound(0) && tempY >= 0 && tempY <= map.GetUpperBound(1))
                    CreateRoom(map[tempX, tempY], room);
            }
        }
    }


    /// <summary>
    /// Randomly shuffles the elements of an array of any type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    private void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = 0; i < (n - 2); i++)
        {
            int r = Random.Range(i, n);
            T t = list[r];
            list[r] = list[i];
            list[i] = t;
        }
    }
}
