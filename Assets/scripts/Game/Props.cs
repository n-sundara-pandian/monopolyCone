using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerType
{
    Player0,
    Player1,
    Player2,
    Player3,
    Bank
}

public class Props
{
    public enum PropertyType
    {
        Turn,
        Place,
        Community,
        Tax,
        Transport,
        Chance,
        Utility,
        Jail,
        Parking
    };
    public enum PropertyGroup
    {
        Purple,
        LightBlue,
        Violet,
        Orange,
        Red,
        Yellow,
        DarkGreen,
        DarkBlue,
        Utility,
        Transport,
        Go,
        None
    };
    PropertyType type;
    string name;
    int index;
    int cost;    
    int rentamt;
    PropertyGroup group;
    PlayerType owner;
    bool can_buy;
    RentStrategy rent;
    public Props(string str)
    {
        string[] fields = str.Split(',');
        if (fields.Length != 6)
        {
            Debug.Log(" Invalid Data " + str);
        }
        type = (PropertyType)Enum.Parse(typeof(PropertyType), fields[0]);
        name = fields[1];
        index = int.Parse(fields[2]);
        cost = int.Parse(fields[3]);
        rentamt = int.Parse(fields[4]);
        group = (PropertyGroup)Enum.Parse(typeof(PropertyGroup), fields[5]);
        owner = PlayerType.Bank;
        can_buy = group != PropertyGroup.Go && group != PropertyGroup.None;
        rent = new RentStrategy();
        rent.Init(type, rentamt);
    }
    public float GetRent(Dictionary<string, string> param)
    {
        return rent.GetRent(param);
    }
    public string Name() { return name; }
    public int Cost() { return cost; }
    public int Rent() { return rentamt; }
    public bool CanBuy() { return can_buy; }
    public PlayerType Owner() { return owner; }
    public void SetOwner(PlayerType t) {
        owner = t;
    }
    public PropertyType Type() { return type; }
}

