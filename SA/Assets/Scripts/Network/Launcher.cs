using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Photonへ接続
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Master接続成功");
        PhotonNetwork.JoinLobby();           // ロビー参加
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");     // Lobbyシーンへ遷移
    }
}
