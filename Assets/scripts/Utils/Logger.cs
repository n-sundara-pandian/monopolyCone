using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Logger : MonoBehaviour {
    Text LogText;
    Utils util;
    Scrollbar scrollbar;
    private static Logger instance;
    public static Logger Instance
    {
        get { return instance ?? (instance = new GameObject("Logger").AddComponent<Logger>()); }
    }
    public void Log(string text)
    {
        if (LogText == null)
            LogText = GameObject.FindGameObjectWithTag("Logger").GetComponent<Text>();
        LogText.text += text;
        LogText.text += "\n";
    }
    public void Log(PlayerType player, string text)
    {
        if (LogText == null)
            LogText = GameObject.FindGameObjectWithTag("Logger").GetComponent<Text>();
        if (util == null)
            util = GameObject.FindGameObjectWithTag("GameController").GetComponent<Utils>();
        if (scrollbar == null)
            scrollbar = GameObject.FindGameObjectWithTag("scrollbar").GetComponent<Scrollbar>();
        LogText.text += "<color=";
        LogText.text += util.GetPlayerUIColor(player);
        LogText.text += ">";
        LogText.text += player.ToString();
        LogText.text += "</color>";
        LogText.text += text;
        LogText.text += "\n";
        scrollbar.value = 0;
    }
    public string GetText()
    {
        return LogText.text;
    }
}
