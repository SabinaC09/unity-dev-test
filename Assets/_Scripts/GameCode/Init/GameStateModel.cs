using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateModel 
{
    public double Money;
    public int WarehouseLevel;
    public int ElevatorLevel;
    public List<int> MineshaftLevels;

    public GameStateModel()
    {
        Money = 500;
        WarehouseLevel = 1;
        ElevatorLevel = 1;
        MineshaftLevels = new List<int>();
    }
}
