using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using thelab.mvc;
using UnityEngine.Events;

public class GameView : View<Game>
{
    List<GameObject> m_Player = new List<GameObject>();
    public GameObject[] m_PlayerUI;
    public Text[] m_PlayerScoreUI;
    public PropertyCard m_PropertyCard;
    public Sprite[] m_DiceUI;
    public Button m_PlayButton;
    public Image m_PlayButtonImg;
    public Transform m_GameOver;
    public Text m_GameOverText;
    public Image m_Dice1;
    public Image m_Dice2;
    public Transform m_CellParent;
    public GameObject m_PlayerPrefab;
    public GameObject m_HintBox;
    public Text m_HintText;

    WaypointBehaviour[] m_Waypoints;
    float[] m_Adjust = new float[4];
    Utils util;

    UnityEvent GoPassedEvent = new UnityEvent();
    UnityEvent ReachedCellEvent = new UnityEvent();

    void Start()
    {
        Init();
        GoPassedEvent.AddListener(() => app.controller.GoPassed());
        ReachedCellEvent.AddListener(() => app.controller.Go(HSM.State.PlayerReachCell));
    }
    public void Init()
    {
        util = GameObject.FindGameObjectWithTag("GameController").GetComponent<Utils>();
        m_Waypoints = m_CellParent.gameObject.GetComponentsInChildren<WaypointBehaviour>();
        m_PropertyCard.Hide();
        m_GameOver.DOScale(0, 0.1f);
       for (int i = 0; i < 4; i++)
            m_Adjust[i] = -0.2f + i * 0.15f;
        HideHint();
        m_Dice1.transform.DOScale(0, 0.1f);
        m_Dice2.transform.DOScale(0, 0.1f);

    }

    public Vector3 GetPoint(int i)
    {
        return m_Waypoints[i].GetPoint();
    }

    public void SetPlayer(int i, int player_no)
    {
        m_Waypoints[i].Show(GetPlayerColor(player_no));
    }

    public void InitPlayers(int c)
    {
        for (int i = 0; i < m_PlayerUI.Length; i++)
        {
            if (i < c)
            {
                m_PlayerUI[i].transform.DOScale(1, 0.25f);
                GameObject player = GameObject.Instantiate<GameObject>(m_PlayerPrefab);
                m_Player.Add(player);
                m_PlayerUI[i].GetComponent<Image>().color = GetPlayerColor(i);
                player.GetComponent<MeshRenderer>().material.SetColor("_Color", GetPlayerColor(i));
            }
            else
                m_PlayerUI[i].transform.DOScale(0, 0.1f);
        }
    }

    public void UpdateScore(int i, float value)
    {
        m_PlayerScoreUI[i].text = value.ToString();
    }

    public Color GetPlayerColor(int i)
    {
        PlayerType type = (PlayerType)i;
        return util.GetPlayerColor(type);
    }
    public void SetPlayBtn(int i)
    {
        m_PlayButtonImg.color = GetPlayerColor(i);
    }
    public void RollDice(int d1, int d2)
    {
        m_Dice1.transform.DOScale(1, 0.1f);
        m_Dice2.transform.DOScale(1, 0.1f);
        m_Dice1.sprite = m_DiceUI[d1 - 1];
        m_Dice2.sprite = m_DiceUI[d2 - 1];        
    }
    public void GoToJail(int player_no, Player player)
    {
        int cellNo = Utils.JailCellIndex;
        int currentCell = player.GetCurrentCell();
        int steps = cellNo - currentCell;
        steps = Mathf.Abs(steps);
        Vector3[] waypoints = new Vector3[steps];
        for (int c = 0, i = currentCell - 1; i >= cellNo; i--, c++)
        {
            waypoints[c] = GetPoint(i);
            waypoints[c].y += m_Adjust[player_no];
        }
        player.SetCurrentCell(cellNo);
        player.m_PenaltySteps = 3;
        m_Player[player_no].transform.DOPath(waypoints, 1.0f);
        Logger.Instance.Log(player.m_Player, " Has been Jailed. Will Miss 3 Turns ");
    }

    public void MovePlayer(int player_no, Player player )
    {
        int steps = player.m_LastDiceRoll;
        int currentCell = player.m_PreviousCell;

        Vector3[] waypoints = new Vector3[steps];

        for (int c = 0, i = currentCell + 1; i < currentCell + steps + 1; i++, c++)
        {
            int index = i % 40;
            waypoints[c] = GetPoint(index);
            waypoints[c].y += m_Adjust[player_no];
        }
        int prevCell = currentCell;
        currentCell = (currentCell + steps) % 40;
        if (currentCell < prevCell && currentCell != 0)
        {
            GoPassedEvent.Invoke();
        }
        m_Player[player_no].transform.DOPath(waypoints, 1.0f).OnComplete(() => { ReachedCellEvent.Invoke(); });
    }

    public void ShowBuyDialog(Props property)
    {
        m_PropertyCard.Show(property);
    }
    public void HideBuyDialog()
    {
        m_PropertyCard.Hide();
    }
    public void ShowEndGame(PlayerType winner)
    {
        m_GameOverText.text = "GAME OVER : The Winner is " + winner;
        m_GameOver.transform.DOScale(1, 0.5f);
    }
    public void EnablePlayButton(bool flag)
    {
        m_PlayButton.interactable = flag;
    }
    public void ShowHint(string str)
    {
        m_HintText.text = str;
        m_HintBox.transform.DOScale(1, 0.5f);
        Invoke("HideHint", 3.0f);
    }
    public void HideHint()
    {
        m_HintBox.transform.DOScale(0, 0.1f);
    }
}
