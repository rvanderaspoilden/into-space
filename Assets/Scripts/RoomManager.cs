using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RoomManager : MonoBehaviourPunCallbacks {
    [SerializeField] private PlayerLobby playerLobbyPrefab;
    [SerializeField] private TextMeshProUGUI roomTitleText;
    [SerializeField] private GameObject scrollViewContent;
    [SerializeField] private Button startButton;
    [SerializeField] private Button readyButton;

    private List<PlayerLobby> players;

    private void Awake() {
        this.players = new List<PlayerLobby>();
    }

    private void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
        this.roomTitleText.text = PhotonNetwork.CurrentRoom.Name;
        this.SetupPlayerList();
    }

    private void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SetReady() {
        Hashtable data = PhotonNetwork.LocalPlayer.CustomProperties;

        if (data.ContainsKey("isReady")) {
            data["isReady"] = !(bool) data["isReady"];
        } else {
            data["isReady"] = true;
        }

        PhotonNetwork.LocalPlayer.SetCustomProperties(data);
    }

    public void SetupPlayerList() {
        foreach (PlayerLobby player in this.players) {
            Destroy(player.gameObject);
        }

        this.players.Clear();

        foreach (KeyValuePair<int, Player> entry in PhotonNetwork.CurrentRoom.Players.OrderByDescending(elem => elem.Value.IsMasterClient)) {
            PlayerLobby playerLobby = Instantiate(this.playerLobbyPrefab, this.scrollViewContent.transform);
            playerLobby.Setup(entry.Value);
            players.Add(playerLobby);
        }

        this.ManageButtons();
    }

    public void RefreshPlayerList() {
        foreach (PlayerLobby player in this.players) {
            player.Refresh();
        }

        this.ManageButtons();
    }

    public void ManageButtons() {
        this.startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        this.readyButton.gameObject.SetActive(!PhotonNetwork.IsMasterClient);

        bool allReady = true;

        // Check if all players are ready are not
        foreach (KeyValuePair<int, Player> entry in PhotonNetwork.CurrentRoom.Players) {
            if (!entry.Value.IsMasterClient && (!entry.Value.CustomProperties.ContainsKey("isReady") || !(bool) entry.Value.CustomProperties["isReady"])) {
                allReady = false;
                break;
            }
        }

        this.startButton.interactable = allReady;
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps) {
        Debug.Log("Player properties changed for : " + targetPlayer.NickName);
        this.RefreshPlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        Debug.Log("Other player joined room");
        PlayerLobby playerLobby = Instantiate(this.playerLobbyPrefab, this.scrollViewContent.transform);
        playerLobby.Setup(newPlayer);
        players.Add(playerLobby);

        this.RefreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        Debug.Log("Other player left room");
        this.SetupPlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient) {
        Debug.Log(newMasterClient.NickName + " is now the master client");
    }
}