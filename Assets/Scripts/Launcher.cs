using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks {
    [SerializeField] private GameObject authentificationPanel;
    [SerializeField] private GameObject loaderPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private InputField nickNameInputField;
    [SerializeField] private Toggle rememberToggle;

    private string nickName;
    private bool remember;

    void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
        this.ShowPanel(this.authentificationPanel);
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
            this.ShowPanel(this.authentificationPanel);
        }
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

        string roomName = "Room of " + PhotonNetwork.NickName;
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    private void ShowPanel(GameObject panelToShow) {
        this.authentificationPanel.SetActive(false);
        this.loaderPanel.SetActive(false);
        this.lobbyPanel.SetActive(false);

        panelToShow.SetActive(true);
    }

    public override void OnConnectedToMaster() {
        Debug.LogFormat("{0} connected to master server", PhotonNetwork.NickName);
        this.ShowPanel(this.lobbyPanel);
    }

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.Log("On Disconnected");
    }

    public override void OnCreatedRoom() {
        Debug.Log("Room created");
    }

    public override void OnCreateRoomFailed(short returnCode, string message) {
        Debug.LogFormat("Room creation failed for cause {0}", message);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
    }
}