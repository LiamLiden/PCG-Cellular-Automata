using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Furniture : MonoBehaviour
{

    public enum Operation
    {
        Equal,
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
    public int myValue;
    public Neighborhood myNeighborhood;
    [Tooltip("Only used with Count neighborhoods. Counts any value included in this list.")]
    public List<int> valueList;
    public Operation myOperation;
    public int targetValue;
    public int maxAmount;
    public int requiredAmount;

    public static Hashtable amount = new Hashtable();

    public bool CanSpawn (Cell[,] map, int x, int y)
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

    public void Spawn (int x, int y)
    {
        // Check if it is valid to spawn the furniture
        if (!amount.ContainsKey(name))
        {
            Instantiate(gameObject, new Vector2(x, y), Quaternion.identity);
            amount.Add(name, 1);
        }
        else if ((int)amount[name] < maxAmount)
        {
            // Spawn and increment amount
            Instantiate(gameObject, new Vector2(x, y), Quaternion.identity);
            amount[name] = (int)amount[name] + 1;
        }
    }
}
/*

[CustomEditor(typeof(Furniture))]
public class MyScriptEditor : Editor
{
    void OnInspectorGUI()
    {
        var myFurniture = target as Furniture;

        myFurniture.myNeighborhood = GUILayout.Toggle(myFurniture.flag, "Flag");

        if (myFurniture.flag)
            myFurniture.i = EditorGUILayout.IntSlider("I field:", myFurniture.i, 1, 100);

    }
}
*/
