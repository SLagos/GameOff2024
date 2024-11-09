

using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

public class RelayManager : MonoSingleton<RelayManager>
{
    // [SerializeField] private Button hostBtn, joinBtn;
    // [SerializeField] private TMP_InputField joinInput;
    // [SerializeField] private TMP_Text codeText;

    public Action OnRelayDataSet;
    public Action<string> OnRelayCodeCreated;

    public async void JoinRelay(string joinCode)
    {
        try
        {
            var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = joinAllocation.ToRelayServerData("dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            Debug.Log($"Relay data set using code {joinCode}");
            OnRelayDataSet?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

    }

    public async Task CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            //codeText.text = $"Code: {joinCode}";
            RelayServerData relayServerData = allocation.ToRelayServerData("dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            Debug.Log($"Relay code created: {joinCode}");
            OnRelayCodeCreated?.Invoke(joinCode);
            Debug.Log($"Relay data set");
            OnRelayDataSet?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}