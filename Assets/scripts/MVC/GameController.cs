using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using thelab.mvc;
using UnityEngine.SceneManagement;

public class GameController : Controller<Game>
{
    HSM m_Hsm;
    int m_CurrentPlayerIndex = 0;
    int m_TotalPlayers = 0;
    Props m_CurrentProperty;
    Player m_CurrentPlayer;

    void Start()
    {
        Invoke("StartLater", 0.5f);
    }

    void StartLater()
    {
        m_Hsm = GameObject.FindGameObjectWithTag("GameController").GetComponent<HSM>();
        InitStateMap();
    }
    void InitStateMap()
    {
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Start, HSM.State.Init), ToInit);
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Init, HSM.State.Play), ToPlay);
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Play, HSM.State.PlayerReachCell), ToPlayerReachCell);
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.PlayerReachCell, HSM.State.Transaction), ToTransaction);
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Transaction, HSM.State.CompleteTransaction), ToCompleteTransaction);
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.CompleteTransaction, HSM.State.Evaluate), ToEvaluate);
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Evaluate, HSM.State.Play), ToPlay);
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Evaluate, HSM.State.EndGame), ToEndGame);
        m_Hsm.AddTransition(new KeyValuePair<HSM.State, HSM.State>(HSM.State.Play, HSM.State.CompleteTransaction), ToCompleteTransaction);
        Go(HSM.State.Init, 0.5f);
    }
    void InitPlayers()
    {
        m_TotalPlayers = PlayerPrefs.GetInt("players");
        if (m_TotalPlayers == 0)
            m_TotalPlayers = 2;
        app.model.InitPlayers(m_TotalPlayers);
        app.view.InitPlayers(m_TotalPlayers);
        SetCurrentPlayer(0);
    }

    void SetCurrentPlayer(int player_no)
    {
        m_CurrentPlayer = app.model.GetPlayer(player_no);
        m_CurrentPlayerIndex = player_no;
        app.view.SetPlayBtn(player_no);
    }
    public void Go(HSM.State state, float delay = 0.01f)
    {
        m_Hsm.Go(state, delay);
    }
    void ToInit()
    {
        InitPlayers();
        app.model.InitProperties();
        app.view.Init();
        Go(HSM.State.Play);
    }
    void ToPlay()
    {
        if (!m_CurrentPlayer.m_AI)
            ShowHint();
        app.view.EnablePlayButton(!m_CurrentPlayer.m_AI);
        int Penalty = m_CurrentPlayer.m_PenaltySteps;
        if (!m_CurrentPlayer.m_Active)
        {
            Go(HSM.State.CompleteTransaction);
            return;
        }
        else if (Penalty > 0)
        {
            m_CurrentPlayer.m_PenaltySteps--;
            string msg = m_CurrentPlayer.m_Player + " Is In Jail. " + Penalty + " More Rounds to Go (" + m_CurrentPlayer.GetBalance() + ")";
            Logger.Instance.Log(m_CurrentPlayer.m_Player, msg);
            Go(HSM.State.CompleteTransaction);
            return;
        }
        if (m_CurrentPlayer.m_AI)
        {
            Invoke("OnPlay", Random.Range(0.5f, 2.0f));
        }
        InvokeRepeating("ShowHint", 5.0f, 15.0f);
    }
    void ToTransaction()
    {
        int index = m_CurrentPlayer.GetCurrentCell(); 
        PlayerType owner = m_CurrentProperty.Owner();
        Props.PropertyType propertyType = m_CurrentProperty.Type();       

        /* If Landed on Jail */
        if (propertyType == Props.PropertyType.Jail && m_CurrentProperty.Name() == "Jail")
        {
            app.view.GoToJail(m_CurrentPlayerIndex, m_CurrentPlayer);
            Go(HSM.State.CompleteTransaction);
            return;
        }
        else if (owner == PlayerType.Bank)
        {
            bool isAvailable = m_CurrentProperty.CanBuy();
            bool canAfford = m_CurrentPlayer.CanPay(m_CurrentProperty.Cost());
            /* Certain Bank Properties like Tax, Community Cards, Jail, Chance etc or not available to buy*/
            if (isAvailable)
            {
                if (canAfford) /* If player can afford to buy a property show buy dialog */
                {
                    app.view.ShowBuyDialog(m_CurrentProperty);
                    if (m_CurrentPlayer.m_AI)
                    {
                        Invoke("OnBuySell", 0.5f);
                    }
                }
                else
                    Go(HSM.State.CompleteTransaction);
            }
            else
            {
                PayRent(index, new Dictionary<string, string>());
            }
        }
        else if (owner == m_CurrentPlayer.m_Player)
        {
            Go(HSM.State.CompleteTransaction);
        }
        else
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            if (propertyType == Props.PropertyType.Transport)
            {
                int count = app.model.GetBuildingsOwnedByPlayer(propertyType, owner);
                param.Add("count", count.ToString());
            }
            else if (propertyType == Props.PropertyType.Utility)
            {
                int count = app.model.GetBuildingsOwnedByPlayer(propertyType, owner);
                param.Add("count", count.ToString());
                param.Add("dicevalue", m_CurrentPlayer.m_LastDiceRoll.ToString());
            }
            else if (propertyType == Props.PropertyType.Turn)
            {
                GoPassed();
                Go(HSM.State.CompleteTransaction);
                return;
            }
            PayRent(index, param);
        }
    }
    void ToCompleteTransaction()
    {
        m_CurrentPlayerIndex++;
        m_CurrentPlayerIndex %= m_TotalPlayers;
        SetCurrentPlayer(m_CurrentPlayerIndex);
        app.view.EnablePlayButton(!m_CurrentPlayer.m_AI);
        for (int i = 0; i < m_TotalPlayers; i++)
            app.view.UpdateScore(i, app.model.GetPlayer(i).GetBalance());
        Go(HSM.State.Evaluate);
    }
    void ToEvaluate()
    {
        if (app.model.GetActivePlayers() < 2)
            Go(HSM.State.EndGame);
        else
            Go(HSM.State.Play);
    }
    void ToEndGame()
    {
        app.view.ShowEndGame(app.model.m_Winner);
    }
    void ToPlayerReachCell() {
        m_CurrentProperty = app.model.GetProperty(m_CurrentPlayer.GetCurrentCell());
        Go(HSM.State.Transaction);
    }
    public void OnPlay()
    {
        if (m_Hsm.GetCurrentState() == HSM.State.Play)
        {
            int d1 = Random.Range(1, 7);
            int d2 = Random.Range(1, 7);
            app.model.RollDice(m_CurrentPlayerIndex, d1 + d2);
            int val = d1 + d2;
            Logger.Instance.Log(m_CurrentPlayer.m_Player, " Has Rolled " + val );
            app.view.RollDice(d1, d2);
            app.view.MovePlayer(m_CurrentPlayerIndex, app.model.GetPlayer(m_CurrentPlayerIndex));
            app.view.EnablePlayButton(false);
            CancelInvoke("ShowHint");
            app.view.HideHint();
        }
    }
    public void GoPassed()
    {
        Logger.Instance.Log(m_CurrentPlayer.m_Player, " Has Collected $200 From Bank for Go Pass (" + m_CurrentPlayer.GetBalance() + ")");
        app.model.GetPlayer(m_CurrentPlayerIndex).CreditMoney(200);
    }

    void PayRent(int index, Dictionary<string, string> param)
    {
        float rent = m_CurrentProperty.GetRent(param);
        if (m_CurrentPlayer.CanPay(rent))
        {
            m_CurrentPlayer.DebitMoney(rent);
            PlayerType type = m_CurrentProperty.Owner();
            if (type != PlayerType.Bank)
            {
                Player owner = app.model.GetPlayer((int)type);
                owner.CreditMoney(rent);
            }
            if (rent < 0)
                Logger.Instance.Log(m_CurrentPlayer.m_Player, " Has Collected $" + Mathf.Abs(rent) + " From " + type.ToString() + "(" + m_CurrentPlayer.GetBalance() + ")");
            else
                Logger.Instance.Log(m_CurrentPlayer.m_Player, " Has Paid $" + Mathf.Abs(rent) + " To " + type.ToString() + "(" + m_CurrentPlayer.GetBalance() + ")");
            Go(HSM.State.CompleteTransaction);
        }
        else
        {
            m_CurrentPlayer.DebitMoney(rent);
            m_CurrentPlayer.m_Active = false;
            Go(HSM.State.CompleteTransaction);
            string msg = "<color=red>" + m_CurrentPlayer.m_Player + "(" + m_CurrentPlayer.GetBalance() + ")" + " Has Run Out Of Money. Out of Game \n </color>";
            Logger.Instance.Log(msg);
        }
    }
    public void OnBuySell()
    {
        int index = m_CurrentPlayer.GetCurrentCell();
        int cost = m_CurrentProperty.Cost();
        if (m_CurrentPlayer.CanPay(cost))
        {
            m_CurrentPlayer.DebitMoney(cost);
            m_CurrentProperty.SetOwner(m_CurrentPlayer.m_Player);
            app.view.SetPlayer(index, m_CurrentPlayerIndex);
            Logger.Instance.Log(m_CurrentPlayer.m_Player, " Has bought " + m_CurrentProperty.Name()+ " for $" + cost + "(" + m_CurrentPlayer.GetBalance() + ")");
        }
        Go(HSM.State.CompleteTransaction);
        app.view.HideBuyDialog();
    }
    public void OnDismissCard()
    {
        Go(HSM.State.CompleteTransaction);
        app.view.HideBuyDialog();
    }
    public void Replay()
    {
        SceneManager.LoadScene("mvc", LoadSceneMode.Single);
    }
    public void Exit()
    {
        System.IO.File.WriteAllText("Log.txt", Logger.Instance.GetText());
        SceneManager.LoadScene("menu", LoadSceneMode.Single);
    }
    public void ShowHint()
    {
        string msg = m_CurrentPlayer.m_Player + "'s Turn.";
        app.view.ShowHint(msg);
    }
}