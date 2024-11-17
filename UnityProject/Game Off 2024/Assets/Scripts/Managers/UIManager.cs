using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    private GameObject _mainMenuScreen, _lobbiesScreen, _lobbyScreen, _gameScreen;

    [SerializeField]
    private TMP_Text _interactText;

    [SerializeField]
    private float heighOffset = 0.1f;



    public void ShowMainMenu()
    {
        _mainMenuScreen.SetActive(true);
        _lobbyScreen.SetActive(false);
        _lobbiesScreen.SetActive(false);
        _gameScreen.SetActive(false);
    }

    public void HideMainMenu()
    {
        _mainMenuScreen.SetActive(false);
        _lobbyScreen.SetActive(false);
        _lobbiesScreen.SetActive(false);
        _gameScreen.SetActive(false);
    }

    public void GoToLobbyView()
    {
        HideMainMenu();
        _lobbyScreen.SetActive(true);
        _lobbiesScreen.SetActive(false);
        _gameScreen.SetActive(false);
    }

    public void GotoLobbiesView()
    {
        HideMainMenu();
        _lobbyScreen.SetActive(false);
        _lobbiesScreen.SetActive(true);
        _gameScreen.SetActive(false);
    }

    public void GoToGameView()
    {
        HideMainMenu();
        _gameScreen.SetActive(true);
        _interactText.gameObject.SetActive(false);
    }

    public void SetInteractText(string text, Vector3 worldPosition)
    {
        _interactText.gameObject.SetActive(true);
        _interactText.text = text;
        _interactText.transform.position = WorldPositionToScreen(worldPosition);
    }

    private Vector3 WorldPositionToScreen(Vector3 worldPosition)
    {
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        screenPosition.y += heighOffset;
        return screenPosition;
    }

    public void HideInteractText()
    {
        _interactText.gameObject.SetActive(false);
    }

}