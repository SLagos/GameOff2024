using System;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerElement : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _playerNameText;


    public void SetPlayerData(string playerName )
    {
        _playerNameText.text = playerName;
    }
}