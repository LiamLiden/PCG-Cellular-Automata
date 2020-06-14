using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Furniture : MonoBehaviour
{

    public enum Operation
    {
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual
    }

    public enum Neighborhood
    {
        Moores,
        VonNueman,
        CountMoores,
        CountVonNeuman
    }

    public string name;
    [Tooltip("The value that will be placed on the map and used for calculation of neighborhoods.")]
    public int myValue;
    [Tooltip("Neighborhood that will be used for spawning this furniture. Count neighborhoods will count all items with a value included on the value list." + 
        " Example: Count Moores with a value list containing a single '1' will count all walls in the Moores neighborhood.")]
    public Neighborhood myNeighborhood;
    [Tooltip("Only used with Count neighborhoods. Counts any value included in this list.")]
    public List<int> valueList;
    [Tooltip("Operation used in conditional.")]
    public Operation myOperation;
    [Tooltip("Right side of the condition for spawning. Final Form: calculatedValue myOperation targetValue.")]
    public int targetValue;
    [Tooltip("Max amount of this furniture that can spawn on the map.")]
    public int maxAmount;
    [Tooltip("Generation will go over furnishing iterations if required amount is not spawned.")]
    public int requiredAmount;

    public static Hashtable amount = new Hashtable();

    public bool CanSpawn(Cell[,] map, int x, int y)
    {
        if (!amount.ContainsKey(name) || (int)amount[name] < maxAmount)
        {
            int value = 0;
            switch (myNeighborhood)
            {
                case Neighborhood.Moores:
                    value = Cell.MooresNeighborhood(map, x, y);
                    break;
                case Neighborhood.VonNueman:
                    value = Cell.VonNuemanNeighborhood(map, x, y);
                    break;
                case Neighborhood.CountMoores:
                    value = Cell.CountMoores(map, x, y, valueList);
                    break;
                case Neighborhood.CountVonNeuman:
                    value = Cell.CountVonNueman(map, x, y, valueList);
                    break;
            }

            switch (myOperation)
            {
                case Operation.Equal:
                    if (value == targetValue)
                        return true;
                    else
                        return false;
                case Operation.NotEqual:
                    if (value != targetValue)
                        return true;
                    else
                        return false;
                case Operation.LessThan:
                    if (value < targetValue)
                        return true;
                    else
                        return false;
                case Operation.LessThanOrEqual:
                    if (value <= targetValue)
                        return true;
                    else
                        return false;
                case Operation.GreaterThan:
                    if (value > targetValue)
                        return true;
                    else
                        return false;
                case Operation.GreaterThanOrEqual:
                    if (value >= targetValue)
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }
        else
            return false;
    }

    public void Spawn(int x, int y)
    {
        Instantiate(gameObject, new Vector2(x, y), Quaternion.identity);

        // Update HashTable
        if (!amount.ContainsKey(name))
        { 
            amount.Add(name, 1);
        }
        else
        {
            amount[name] = (int)amount[name] + 1;
        }
    }

    public int GetAmount()
    {
        return (int) amount[name];
    }
}
