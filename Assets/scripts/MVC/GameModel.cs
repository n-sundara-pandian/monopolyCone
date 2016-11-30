using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameModel : Model<Game>
{
    Props[] PropertyList = new Props[40];
    List<Player> PlayerList = new List<Player>();

    public TextAsset DB;
    public PlayerType m_Winner = PlayerType.Bank;
    public void InitProperties()
    {
        string[] records = DB.text.Split('\n');
        int i = 0;
        foreach (string record in records)
        {
            PropertyList[i] = new global::Props(record);
            i++;
        }
    }
    public void InitPlayers(int no_of_players)
    {
        for (int i = 0; i < no_of_players; i++)
        {            
            PlayerList.Add(new Player(i, PlayerPrefs.GetInt("AI" + i)));
        }
    }
    public void RollDice(int i, int val)
    {
        PlayerList[i].m_LastDiceRoll = val;
        PlayerList[i].SetCurrentCell((PlayerList[i].GetCurrentCell() + val) % 40);
    }
    public Player GetPlayer(int i)
    {
        return PlayerList[i];
    }
    public Props GetProperty(int i)
    {
        return PropertyList[i];
    }
    public int GetActivePlayers()
    {
        int count= 0;
        for(int i = 0; i < PlayerList.Count; i++)
        {
            if (PlayerList[i].m_Active)
            {
                m_Winner = PlayerList[i].m_Player;
                count++;
            }
        }
        return count;
    }
    public int GetBuildingsOwnedByPlayer(Props.PropertyType pType, PlayerType cur_player )
    {
        int count = 0;
        for(int i = 0; i < PropertyList.Length; i++)
        {
            if (PropertyList[i].Owner() == cur_player && PropertyList[i].Type() == pType)
                count++;
        }
        return count;
    }
}