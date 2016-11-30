using UnityEngine;
using System.Collections;
[SerializeField]
public class Utils : MonoBehaviour {
    Color[] PlayerColors = new Color[4];
    string[] PlayerUIColors = new string[4];
    public static int JailCellIndex = 10;
    void Start()
    {
        PlayerUIColors[0] = "red";
        PlayerUIColors[1] = "blue";
        PlayerUIColors[2] = "green";
        PlayerUIColors[3] = "magenta";

        PlayerColors[0] = Color.red;
        PlayerColors[1] = Color.blue;
        PlayerColors[2] = Color.green;
        PlayerColors[3] = Color.magenta;
    }
    public Color GetPlayerColor(PlayerType index)
    {
        return PlayerColors[(int)index];
    }
    public string GetPlayerUIColor(PlayerType index)
    {
        return PlayerUIColors[(int)index];
    }
}
