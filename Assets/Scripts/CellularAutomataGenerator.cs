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

    private int[,] map;

    class Cell
    {
        public bool visited;
        public int value;
        public int x;
        public int y;

        public Cell(int _x, int _y, int val)
        {
            value = val;
            x = _x;
            y = _y;
            visited = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize size of map
        map = new int[width, height];

        // Randomly disperse walls onto map
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                // 50-50 chance of cell becoming a wall
                if (Random.Range(0, 100) < initialWallPlacementProbability)
                    map[x, y] = 1;
            }
        }

        // Run algorithm for input iterations
        for (int i = 0; i < iterations; i++)
        {
            int[,] newMap = new int[width, height];
            for (int x = 0; x < map.GetUpperBound(0); x++)
            {
                for (int y = 0; y < map.GetUpperBound(1); y++)
                {
                    if (MooresNeighborhood(map, x, y) >= wallThreshold)
                        newMap[x, y] = 1;
                }
            }
            map = newMap;
        }

        // Placement of everything
        for (int x = 0; x < map.GetUpperBound(0); x++)
        {
            for (int y = 0; y < map.GetUpperBound(1); y++)
            {
                if (map[x, y] == 1)
                {
                    Instantiate(wall, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (map[x, y] == 0)
                {
                    Instantiate(ground, new Vector3(x, y, 0), Quaternion.identity);
                }
            }
        }
    }

    // Returns 1 if map[x, y] are out of bounds of map, else returns map[x, y]
    private int SafeMapValue(int[,] map, int x, int y)
    {
        if (x < 0 || x >= map.GetUpperBound(0) || y < 0 || y >= map.GetUpperBound(1))
            return 1;
        else
            return map[x, y];
    }

    private int MooresNeighborhood(int[,] map, int x, int y)
    {
        return SafeMapValue(map, x - 1, y) + SafeMapValue(map, x - 1, y + 1) + SafeMapValue(map, x, y + 1) + SafeMapValue(map, x + 1, y + 1)
            + SafeMapValue(map, x + 1, y) + SafeMapValue(map, x + 1, y - 1) + SafeMapValue(map, x, y - 1) + SafeMapValue(map, x - 1, y - 1)
            + SafeMapValue(map, x, y);
    }
}
