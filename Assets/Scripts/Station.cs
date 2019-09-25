using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class Station : MonoBehaviour
{
//    public Dictionary<string, bool> StationItems = new Dictionary<string, bool>();
    
    private readonly List<string> StationItems = new List<string>();

    public List<string> GetStationItems()
    {
        return StationItems;
    }
    
    public string GetStationItem(int i)
    {
        return StationItems[i];
    }
    
    public Station(List<string> items)
    {
        StationItems = items;
    }

    public bool CheckForMatch(string check)
    {
        return StationItems.Contains(check);
    }

    public string GetItem(string currentNFC)
    {
        Random rand = new Random();
        string item = "";
        while (true)
        {
            int x = rand.Next(0, StationItems.Count);
            if (StationItems.ElementAt(x).Equals(currentNFC)) continue;
            item = StationItems.ElementAt(x);
            break;
        }
        
        return item;
    }
}
