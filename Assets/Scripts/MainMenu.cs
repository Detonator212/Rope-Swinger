using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Opens MainMenu scene
    public void OpenStartPage()
    {
        SceneManager.LoadScene(0);
    }

    //Opens Singleplayer or Multiplayer scene
    public void OpenSingleplayerOrMultiplayerPage()
    {
        SceneManager.LoadScene(1);
    }

    //Opens Singleplayer Game scene
    public void StartSingleplayerGame()
    {
        SceneManager.LoadScene(2);
    }

    //Opens Lobby scene
    public void EnterMultiplayerLobby()
    {
        SceneManager.LoadScene(4);
    }

    //Opens How To Play scene
    public void OpenHowToPlayPage()
    {
        SceneManager.LoadScene(5);
    }

    //Exits application
    public void QuitGame()
    {
        Application.Quit();
    }
}


