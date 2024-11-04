using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoSingleton<LobbyManager>
{
    [Header("Lobbies")]
    [SerializeField] private LobbyElement lobbyElementPrefab;
    [SerializeField] private Transform lobbyContainer;

    [Header("Lobby")]

    [SerializeField] private PlayerElement playerElementPrefab;
    [SerializeField] private Transform playerContainer;

    [SerializeField] private Button createLobbyBtn, refreshButton, backButton, startGame;

    private Lobby hostLobby;
    private Lobby joinedLobby;
    private float hearthbeatInterval = 5f;
    private Coroutine heartbeatCoroutine;

    public bool IsInLobby { get; private set; }

    protected override void Start()
    {
        AuthenticationService.Instance.SignedIn += () =>
             {
                 createLobbyBtn.interactable = true;
                 refreshButton.interactable = true;
             };
        createLobbyBtn.interactable = false;
        refreshButton.interactable = false;

        createLobbyBtn.onClick.AddListener(CreateLobby);
        refreshButton.onClick.AddListener(ListLobbies);
        backButton.onClick.AddListener(BackToLobbies);
        startGame.onClick.AddListener(StartGame);
    }

    private async void StartGame()
    {
        try
        {
            string code = await RelayManager.Instance.CreateRelay();
            if (code != null)
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {"RelayCode", new DataObject(DataObject.VisibilityOptions.Member, code, DataObject.IndexOptions.S2)},
                        {"StartGame", new DataObject(visibility: DataObject.VisibilityOptions.Member,value: "1",index: DataObject.IndexOptions.S1)}
                    }
                };
                await LobbyService.Instance.UpdateLobbyAsync(hostLobby.Id, options);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void BackToLobbies()
    {
        try
        {
            if (hostLobby != null)
            {
                StopCoroutine(heartbeatCoroutine);
                heartbeatCoroutine = null;
                await LobbyService.Instance.DeleteLobbyAsync(hostLobby.Id);
                hostLobby = null;
                UIManager.Instance.GotoLobbiesView();
            }
            if (joinedLobby != null)
            {
                string playerId = AuthenticationService.Instance.PlayerId;
                await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, playerId);
                joinedLobby = null;
                UIManager.Instance.GotoLobbiesView();
            }
            IsInLobby = false;
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void CreateLobby()
    {
        try
        {
            string lobbyName = "new lobby";
            int maxPlayers = 4;
            CreateLobbyOptions options = new CreateLobbyOptions();
            options.IsPrivate = false;
            options.Data = new Dictionary<string, DataObject>()
                {
                        {
                        "StartGame", new DataObject(
                            visibility: DataObject.VisibilityOptions.Member,
                            value: "0",
                            index: DataObject.IndexOptions.S1)
                        },
                };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            hostLobby = lobby;
            joinedLobby = lobby;
            heartbeatCoroutine = StartCoroutine(HeartbeatLobby());
            //Go to lobby view
            GoToLobby(lobby);

        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void GoToLobby(Lobby lobby)
    {
        try
        {
            UIManager.Instance.GoToLobbyView();
            var OnLobbyEvent = new LobbyEventCallbacks();
            OnLobbyEvent.PlayerJoined += OnPlayerJoined;
            OnLobbyEvent.PlayerLeft += OnPlayerLeft;
            OnLobbyEvent.DataChanged += OnDataChanged;
            OnLobbyEvent.LobbyChanged += OnLobbyChanged;
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, OnLobbyEvent);

            UpdatePlayersView();
            startGame.gameObject.SetActive(lobby.HostId == AuthenticationService.Instance.PlayerId);
            IsInLobby = true;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    private void OnLobbyChanged(ILobbyChanges changes)
    {
        changes.ApplyToLobby(joinedLobby);
        UpdatePlayersView();
    }

    private void OnDataChanged(Dictionary<string, ChangedOrRemovedLobbyValue<DataObject>> dictionary)
    {
        string relayCode = dictionary["RelayCode"].Value.Value;
        if (relayCode != null)
        {
            RelayManager.Instance.JoinRelay(relayCode);
        }
    }

    private void OnPlayerLeft(List<int> list)
    {
        UpdatePlayersView();
    }

    private void OnPlayerJoined(List<LobbyPlayerJoined> list)
    {
        UpdatePlayersView();
    }

    private void UpdatePlayersView()
    {
        foreach (Transform child in playerContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (var player in joinedLobby.Players)
        {
            string playerId = player.Profile != null ? player.Profile.Name : player.Id;
            Instantiate(playerElementPrefab, playerContainer).SetPlayerData(playerId);
        }
    }

    private IEnumerator HeartbeatLobby()
    {
        while (hostLobby != null)
        {
            yield return new WaitForSeconds(hearthbeatInterval);
            yield return LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }

    private async void ListLobbies()
    {
        try
        {
            refreshButton.enabled = false;
            //Destroy all childrens of lobbyContainer
            foreach (Transform child in lobbyContainer)
            {
                Destroy(child.gameObject);
            }
            var response = await LobbyService.Instance.QueryLobbiesAsync();

            foreach (var lobby in response.Results)
            {
                Instantiate(lobbyElementPrefab, lobbyContainer).SetLobbyData(lobby);
            }
            refreshButton.enabled = true;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }

    private void OnDestroy()
    {
        if (heartbeatCoroutine != null)
        {
            StopCoroutine(heartbeatCoroutine);
            heartbeatCoroutine = null;
        }
    }

    public async void JoinLobby(string lobbyId)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            GoToLobby(joinedLobby);
        }

        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }

    }
}