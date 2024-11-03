using System;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyElement : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _lobbyNameText;
    [SerializeField]
    private TMP_Text _playersText;

    [SerializeField]
    private Button _joinButton;

    public string lobbyId;
    public string lobbyName;
    public int maxPlayers;
    public int currentPlayers;

    void Start()
    {
        _joinButton.onClick.AddListener(JoinLobby);
    }

    private void JoinLobby()
    {
        LobbyManager.Instance.JoinLobby(lobbyId);
    }

    public void SetLobbyData(Lobby lobby)
    {
        lobbyId = lobby.Id;
        lobbyName = lobby.Name;
        maxPlayers = lobby.MaxPlayers;
        currentPlayers = lobby.Players.Count;
        _lobbyNameText.text = $"{lobbyName}";
        _playersText.text = $"{currentPlayers}/{maxPlayers}";
    }
}