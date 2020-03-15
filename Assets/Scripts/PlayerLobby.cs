using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class PlayerLobby : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI statusText;

    private Player player;
    
    public void Setup(Player player) {
        this.player = player;
        this.Refresh();
    }

    public void Refresh() {
        this.nicknameText.text = this.player.NickName;
        this.statusText.gameObject.SetActive(this.player.CustomProperties.ContainsKey("isReady") && (bool)this.player.CustomProperties["isReady"] || this.player.IsMasterClient);
    }

    public Player GetPlayer() {
        return this.player;
    }
}