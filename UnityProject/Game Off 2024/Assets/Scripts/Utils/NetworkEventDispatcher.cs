using System;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;

public class NetworkEventDispatcher
{
    public static Action<ulong> OnClientConnectedEvent;
    public static Action<ulong> OnClientDisconnectedEvent;
    public static Action OnCLientStartedEvent;
    public static Action<bool> OnCLientStoppedEvent;
    public static Action<ConnectionEventData> OnConnectionEventEvent;
    public static Action OnServerStartedEvent;
    public static Action<bool> OnServerStoppedEvent;
    public static Action<ulong> OnSessionOwnerPromotedEvent;
    public static Action OnTransportFailureEvent;
    public static Action<double> OnReanticipateEvent;
    public static void StartListeningNetworkEvents()
    {
        //NetworkManager
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        NetworkManager.Singleton.OnClientStarted += OnCLientStarted;
        NetworkManager.Singleton.OnClientStopped += OnCLientStopped;
        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;
        NetworkManager.Singleton.OnSessionOwnerPromoted += OnSessionOwnerPromoted;
        NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
        //NetworkManager.Singleton.OnReanticipate += OnReanticipate;

        //RelayService



    }

    public static void StopListeningNetworkEvents()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        NetworkManager.Singleton.OnClientStarted -= OnCLientStarted;
        NetworkManager.Singleton.OnClientStopped -= OnCLientStopped;
        NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        NetworkManager.Singleton.OnSessionOwnerPromoted -= OnSessionOwnerPromoted;
        NetworkManager.Singleton.OnTransportFailure -= OnTransportFailure;
        //NetworkManager.Singleton.OnReanticipate -= OnReanticipate;
    }

    private static void OnReanticipate(double lastRoundTripTime)
    {
        OnReanticipateEvent?.Invoke(lastRoundTripTime);
        Debug.Log($"Last round trip time: {lastRoundTripTime}");
    }

    private static void OnTransportFailure()
    {
        OnTransportFailureEvent?.Invoke();
        Debug.Log("Transport failure");
    }

    private static void OnSessionOwnerPromoted(ulong sessionOwnerPromoted)
    {
        OnSessionOwnerPromotedEvent?.Invoke(sessionOwnerPromoted);
        Debug.Log("Session owner promoted");
    }

    private static void OnServerStopped(bool obj)
    {
        OnServerStoppedEvent?.Invoke(obj);
        Debug.Log("Server stopped");
    }

    private static void OnServerStarted()
    {
        OnServerStartedEvent?.Invoke();
        Debug.Log("Server started");
    }

    private static void OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        OnConnectionEventEvent?.Invoke(data);
        Debug.Log("Connection event");
    }

    private static void OnCLientStopped(bool obj)
    {
        OnCLientStoppedEvent?.Invoke(obj);
        Debug.Log("Client stopped");
    }

    private static void OnCLientStarted()
    {
        OnCLientStartedEvent?.Invoke();
        Debug.Log("Client started");
    }

    private static void OnClientDisconnected(ulong obj)
    {
        OnClientDisconnectedEvent?.Invoke(obj);
        Debug.Log("Client disconnected");
    }

    private static void OnClientConnected(ulong obj)
    {
        OnClientConnectedEvent?.Invoke(obj);
        Debug.Log("Client connected");
    }
}