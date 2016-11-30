using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;

public class Player {
    public bool m_Active = true;
    int m_CurrentCell = 0;
    public int m_PreviousCell = 0;
    public int m_PenaltySteps = 0;
    public int m_LastDiceRoll = 2;
    private float m_Balance = 1500;
    public PlayerType m_Player;
    public bool m_AI = true;
    // Use this for initialization

    public Player(int player_type, int ai)
    {
        m_Player = (PlayerType)player_type;
        m_AI = (ai == 1)? true : false;
    }
    public bool CanPay(float amt)
    {
        if (amt > m_Balance)
            return false;
        return true;
    }
    public void SetCurrentCell(int cell_no)
    {
        m_PreviousCell = m_CurrentCell;
        m_CurrentCell = cell_no;
    }

    public int GetCurrentCell()
    {
        return m_CurrentCell;
    }
    public void CreditMoney(float amt) { m_Balance += amt; }
    public void DebitMoney(float amt) { m_Balance -= amt; }
    public float GetBalance() { return m_Balance; } 
}
