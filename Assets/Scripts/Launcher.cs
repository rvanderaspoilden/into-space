using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject authentificationPanel;
    [SerializeField] private GameObject loaderPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject currentRoomPanel;
    [SerializeField] private GameObject roomListPanel;
    [SerializeField] private GameObject scrollViewContentRoomList;
    [SerializeField] private LobbyRoom lobbyRoomPrefab;
    [SerializeField] private InputField nickNameInputField;
    [SerializeField] private Toggle rememberToggle;

    private string nickName;
    private bool remember;
    private List<LobbyRoom> rooms;

    public static Launcher instance;

    void Awake() {
        instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;
        this.rooms = new List<LobbyRoom>();
        this.ShowPanel(this.authentificationPanel);
        this.ShowLobbyPanel(this.roomListPanel);
    }

    private void Start() {
        if (nickNameInputField && PlayerPrefs.HasKey("nickname")) {
            this.nickName = PlayerPrefs.GetString("nickname");
            this.nickNameInputField.text = this.nickName;
        }

        if (this.rememberToggle && PlayerPrefs.HasKey("remember")) {
            this.remember = bool.Parse(PlayerPrefs.GetString("remember"));
            this.rememberToggle.isOn = this.remember;
        }
    }

    public void Connect() {
        if (!string.IsNullOrEmpty(this.nickName)) {
            if (this.remember) {
                PlayerPrefs.SetString("nickname", this.nickName);
            }

            PlayerPrefs.SetString("remember", this.remember ? "true" : "false");

            PhotonNetwork.NickName = this.nickName;

            this.ShowPanel(this.loaderPanel);

            // Connect to master server
            PhotonNetwork.ConnectUsingSettings();
        } else {
            Debug.Log("No nickname filled !");
        }
    }

    public void Disconnect() {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.Disconnect();
        }
    }

    public void LeaveRoom() {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.LeaveRoom();
        }
    }

    public void StartGame() {
        PhotonNetwork.LoadLevel("SCENE");
    }

    public void SetNickName(string name) {
        this.nickName = name;
    }

    public void SetRemember(bool value) {
        this.remember = value;
    }

    public void CreateRoom() {
        RoomOptions roomOptions = new RoomOptions() {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        };

        System.Random random = new System.Random();

        string roomName = "Room of " + PhotonNetwork.NickName + "#" + random.Next(0, 999999);
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public void JoinRoom(RoomInfo room) {
        PhotonNetwork.JoinRoom(room.Name);
    }

    private void ShowPanel(GameObject panelToShow) {
        this.authentificationPanel.SetActive(false);
        this.loaderPanel.SetActive(false);
        this.lobbyPanel.SetActive(false);

        panelToShow.SetActive(true);
    }

    private void ShowLobbyPanel(GameObject panelToShow) {
        this.roomListPanel.SetActive(false);
        this.currentRoomPanel.SetActive(false);

        panelToShow.SetActive(true);
    }

    public override void OnConnectedToMaster() {
        Debug.LogFormat("{0} connected to master server", PhotonNetwork.NickName);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        Debug.Log("Joined Lobby");
        this.ShowPanel(this.lobbyPanel);
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("On Disconnected");
        this.ShowPanel(this.authentificationPanel);
    }

    public override void OnCreatedRoom() {
        Debug.Log("Room created");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.LogFormat("Room creation failed for cause {0}", message);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");

        Hashtable data = new Hashtable();
        data["isReady"] = false;

        PhotonNetwork.LocalPlayer.SetCustomProperties(data);

        this.ShowLobbyPanel(this.currentRoomPanel);
    }

    public override void OnLeftRoom() {
        Debug.Log("Left room");
        this.ShowLobbyPanel(this.roomListPanel);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        foreach (RoomInfo room in roomList) {
            Debug.Log("Room to show : " + room.Name + ", isOpen = " + room.IsOpen + ", isVisible = " + room.IsVisible + ", removedFromList = " + room.RemovedFromList);

            if (room.RemovedFromList) {
                LobbyRoom roomToRemove = this.rooms.Find(elem => elem.GetRoom().Name == room.Name);

                if (roomToRemove) {
                    this.rooms.Remove(roomToRemove);
                    Destroy(roomToRemove.gameObject);
                }
            } else {
                LobbyRoom lobbyRoom = this.rooms.Find(elem => elem.GetRoom().Name == room.Name);

                if (!lobbyRoom) {
                    lobbyRoom = Instantiate(this.lobbyRoomPrefab, this.scrollViewContentRoomList.transform);
                }

                lobbyRoom.Setup(room);
                this.rooms.Add(lobbyRoom);
            }
        }
    }
}