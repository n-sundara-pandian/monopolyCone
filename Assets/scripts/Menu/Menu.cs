using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public Toggle[] ToggleList;
    public Toggle[] AIList;

    void Start()
    {
        int current_selection = PlayerPrefs.GetInt("players");
        if (current_selection == 0)
            PlayerPrefs.SetInt("players", 2);
        else
        {
            ToggleList[current_selection - 2].isOn = true;
        }
    }
    public void PlayGame()
    {
        for(int i = 0; i < AIList.Length; i++)
        {
            if (AIList[i].isOn)
                PlayerPrefs.SetInt("AI" + i, 1);
            else
                PlayerPrefs.SetInt("AI" + i, 0);
        }
        SceneManager.LoadScene("mvc", LoadSceneMode.Single);
    }
    public void OnToggleChanged()
    {
        for (int i=0; i< ToggleList.Length; i++)
        {
            if (ToggleList[i].isOn)
            {
                PlayerPrefs.SetInt("players", i + 2);
            }
        }
    }

    public void OnExit()
    {
        Application.Quit();
    }
}
