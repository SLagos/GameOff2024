using System;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private GameObject _mainMenuScreen, _lobbiesScreen, _lobbyScreen;



    public void ShowMainMenu()
    {
        _mainMenuScreen.SetActive(true);
        _lobbyScreen.SetActive(false);
        _lobbiesScreen.SetActive(false);
    }

    public void HideMainMenu()
    {
        _mainMenuScreen.SetActive(false);
        _lobbyScreen.SetActive(false);
        _lobbiesScreen.SetActive(false);
    }

    public void GoToLobbyView()
    {
        HideMainMenu();
        _lobbyScreen.SetActive(true);
        _lobbiesScreen.SetActive(false);
    }

    public void GotoLobbiesView()
    {
        HideMainMenu();
        _lobbyScreen.SetActive(false);
        _lobbiesScreen.SetActive(true);
    }
}