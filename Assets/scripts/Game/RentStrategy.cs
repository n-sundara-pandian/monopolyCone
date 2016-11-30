using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RentStrategy {
    Rent rent;
    public void Init(Props.PropertyType type, float amt)
    {
        switch (type)
        {
            case Props.PropertyType.Turn:
                {
                    rent = new HouseRent(amt);
                    break;
                }
            case Props.PropertyType.Tax:
                {
                    rent = new TaxRent(amt);
                    break;
                }
            case Props.PropertyType.Place:
                {
                    rent = new HouseRent(amt);
                    break;
                }
            case Props.PropertyType.Community:
                {
                    rent = new CommunityRent();
                    break;
                }
            case Props.PropertyType.Transport:
                {
                    rent = new TransportRent();
                    break;
                }
            case Props.PropertyType.Chance:
                {
                    rent = new CommunityRent(); // Chance also uses Community logic
                    break;
                }
            case Props.PropertyType.Utility:
                {
                    rent = new UtilityRent();
                    break;
                }
            case Props.PropertyType.Jail:
                {
                    rent = new NoRent();
                    break;
                }
            case Props.PropertyType.Parking:
                {
                    rent = new NoRent();
                    break;
                }
            default:
                {
                    Debug.Log("Unsupported Rent type" + type.ToString());
                    rent = new NoRent();
                    break;
                }
        }
    }

    public float GetRent(Dictionary<string, string> param)
    {
        return rent.GetRent(param);
    }
}
public interface IRent
{
    float GetRent(Dictionary<string, string> param);
}

public abstract class Rent :  IRent
{
    public abstract float GetRent(Dictionary<string, string> param);
}

public class HouseRent : Rent
{
    float Rent;
    public HouseRent(float r)
    {
        Rent = r;
    }
    public override float GetRent(Dictionary<string, string> param)
    {
        return Rent;
    }
}

public class CommunityRent : Rent
{
    public override float GetRent(Dictionary<string, string> param)
    {
        return Random.Range(-20, 20) * 10;
    }
}

public class TaxRent : Rent
{
    float TaxAmount;
    public TaxRent(float amt)
    {
        TaxAmount = amt;
    }
    public override float GetRent(Dictionary<string, string> param)
    {
        return TaxAmount;
    }
}

public class TransportRent : Rent
{
    public override float GetRent(Dictionary<string, string> param)
    {
        float Rent = 25;
        if(param.ContainsKey("count"))
        {
            int count = int.Parse(param["count"]);
            Rent = Rent * Mathf.Pow(2, count);
        }
        return Rent;
    }
}
public class UtilityRent : Rent
{
    public override float GetRent(Dictionary<string, string> param)
    {
        int count = 1;
        int dice_value = 2;
        if (param.ContainsKey("count"))
            count = int.Parse(param["count"]);
        if (param.ContainsKey("dicevalue"))
            dice_value = int.Parse(param["dicevalue"]);

        if (count == 1)
            return dice_value * 4;
        else if (count == 2)
            return dice_value * 10;

        return dice_value;

    }
}

public class NoRent : Rent
{
    public override float GetRent(Dictionary<string, string> param)
    {
        return 0.0f;
    }
}