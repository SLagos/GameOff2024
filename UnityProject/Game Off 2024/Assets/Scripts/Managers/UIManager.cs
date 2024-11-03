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
    }

    public void HideMainMenu()
    {
        _mainMenuScreen.SetActive(false);
        _lobbyScreen.SetActive(false);
        _lobbiesScreen.SetActive(false);
    }

    public void GoToLobbyView()
    {
        _lobbyScreen.SetActive(true);
        _lobbiesScreen.SetActive(false);
    }

    public void GotoLobbiesView()
    {
        _lobbyScreen.SetActive(false);
        _lobbiesScreen.SetActive(true);
    }
}