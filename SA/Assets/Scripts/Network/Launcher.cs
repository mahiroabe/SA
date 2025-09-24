using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // AppIdで接続
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("接続成功: Masterに入った");
        PhotonNetwork.JoinLobby(); // ロビーに入る
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("ロビー入室完了");
        // ここでUIにルーム一覧を表示できる
    }


    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5; // 最大5人
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("ルームに参加: " + PhotonNetwork.CurrentRoom.Name);
        // プレイヤー生成
        PhotonNetwork.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity);
    }

}
