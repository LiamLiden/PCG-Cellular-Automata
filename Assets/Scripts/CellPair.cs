using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
