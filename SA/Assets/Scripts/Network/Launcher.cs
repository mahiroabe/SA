using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    private bool isConnecting = false;
    TMP_Text text;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    void Update()
    {
        // 左クリックしたら接続開始
        if (Input.GetMouseButtonDown(0) && !isConnecting)
        {
            isConnecting = true;
            Debug.Log("Photonへ接続開始...");
            text.text = "Connecting to Photon...";
            PhotonNetwork.ConnectUsingSettings(); // Photonへ接続
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Master接続成功");
        text.text = "Connected to Master Server";
        PhotonNetwork.JoinLobby(); // ロビー参加
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobbyに参加しました");
        text.text = "Joined Lobby";
        SceneManager.LoadScene("Lobby"); // Lobbyシーンへ遷移
    }
}
