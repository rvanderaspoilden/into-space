using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    // Start is called before the first frame update
    void Start() {
        PhotonNetwork.Instantiate("Prefabs/" + this.playerPrefab.name, new Vector3(5, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update() {

    }
}
