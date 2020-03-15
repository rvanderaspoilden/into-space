using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyRoom : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playersCountText;

    private Button button;
    private RoomInfo room;

    private void Awake() {
        this.button = GetComponent<Button>();
    }

    private void OnEnable() {
        this.button.onClick.AddListener(() => Launcher.instance.JoinRoom(this.room));
    }

    private void OnDisable() {
        this.button.onClick.RemoveAllListeners();
    }

    public void Setup(RoomInfo room) {
        this.room = room;
        this.Refresh();
    }

    public void Refresh() {
        this.roomNameText.text = this.room.Name;
        this.playersCountText.text = this.room.PlayerCount + "/" + this.room.MaxPlayers;
    }

    public RoomInfo GetRoom() {
        return this.room;
    }
}