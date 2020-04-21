using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionInformation
{
    public Cell startCell;
    public Cell endCell;
    public Room targetRoom;

    public ConnectionInformation(Cell sCell, Cell fCell, Room tRoom)
    {
        startCell = sCell;
        endCell = fCell;
        targetRoom = tRoom;
    }
}
