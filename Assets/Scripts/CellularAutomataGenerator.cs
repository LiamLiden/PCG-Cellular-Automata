using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public int initialWallPlacementProbability;
    public int iterations;
    public int wallThreshold;
    public GameObject wall;
    public GameObject ground;

    private Cell[,] map;

    // Start is called before the first frame update
    void Start()
    {
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
                    if (MooresNeighborhood(map, x, y) >= wallThreshold)
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
                if (MooresNeighborhood(map, x, y) >= wallThreshold)
                    finalMap[x, y] = new Cell(x, y, 1);
                else
                {
                    finalMap[x, y] = new Cell(x, y, 0);
                    floors.Enqueue(finalMap[x, y]);
                }
                    
            }
        }
        map = finalMap;

        // Quality Check
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

        foreach (Room curRoom in Room.rooms)
        {
            if (curRoom.connectedRooms.Count == 0)
            {
                CellPair cellsToConnect = curRoom.FindNearestUnconnected();

                List<Cell> path = AStar.Search(map, cellsToConnect.startCell, cellsToConnect.finalCell);
                foreach (Cell cell in path)
                {
                    cell.value = 0;
                }
            }
        }


        // Placement of everything
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
                }
            }
        }
    }

    // Returns 1 if map[x, y] are out of bounds of map, else returns map[x, y]
    private int SafeMapValue(Cell[,] map, int x, int y)
    {
        if (x < 0 || x > map.GetUpperBound(0) || y < 0 || y > map.GetUpperBound(1))
            return 1;
        else
            return map[x, y].value;
    }

    // Returns value of map[x, y] Moores Neighborhood
    private int MooresNeighborhood(Cell[,] map, int x, int y)
    {
        return SafeMapValue(map, x - 1, y) + SafeMapValue(map, x - 1, y + 1) + SafeMapValue(map, x, y + 1) + SafeMapValue(map, x + 1, y + 1)
            + SafeMapValue(map, x + 1, y) + SafeMapValue(map, x + 1, y - 1) + SafeMapValue(map, x, y - 1) + SafeMapValue(map, x - 1, y - 1)
            + SafeMapValue(map, x, y);
    }

    // Recursive function to create a room. Uses BFS to visit all cells in a room and adds any cells adjacent to a wall to edgeCells.
    private void CreateRoom(Cell start, Room room)
    {
        // Base case
        if (start.value == 1 || start.visited == true)
            return;

        start.visited = true;
        if (MooresNeighborhood(map, start.x, start.y) > 0)
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
}
