using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

/// <summary>
/// PUN2 の接続〜ロビー〜部屋一覧〜参加を管理
/// </summary>
public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField roomNameInput;   // 部屋名入力欄
    [SerializeField] private Transform roomListParent;       // ScrollView Content
    [SerializeField] private GameObject roomEntryPrefab;     // ルーム1件プレハブ
    [SerializeField] private TMP_Text statusText;            // 状態表示（任意）

    // 内部用
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    void Start()
    {
        // PUN接続開始
        statusText.text = "Connecting to Master...";
        PhotonNetwork.AutomaticallySyncScene = true; // 同じシーンを全員に同期
        PhotonNetwork.ConnectUsingSettings();        // AppIDやバージョンは PhotonServerSettings に設定済み
    }

    #region PUN Callback
    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected. Joining Lobby...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Joined Lobby";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = $"Disconnected: {cause}";
    }

    // ルーム一覧更新
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList || !info.IsOpen || !info.IsVisible)
                cachedRoomList.Remove(info.Name);
            else
                cachedRoomList[info.Name] = info;
        }
        RefreshRoomListUI();
    }

    public override void OnJoinedRoom()
    {
        statusText.text = $"Joined Room : {PhotonNetwork.CurrentRoom.Name}";
        // 例：ゲームシーンへ遷移
        PhotonNetwork.LoadLevel("GameScene");
    }
    #endregion

    #region UI Buttons
    /// <summary>「Create」ボタンから呼ぶ</summary>
    public void CreateRoom()
    {
        string roomName = string.IsNullOrEmpty(roomNameInput.text)
            ? $"Room_{Random.Range(1000,9999)}"
            : roomNameInput.text;

        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomName, options);
        statusText.text = $"Creating Room : {roomName}";
    }

    /// <summary>RoomEntryUI から呼ばれる参加処理</summary>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        statusText.text = $"Joining Room : {roomName}";
    }
    #endregion

    /// <summary>ルーム一覧UIを更新</summary>
    private void RefreshRoomListUI()
    {
        foreach (Transform child in roomListParent)
            Destroy(child.gameObject);

        foreach (var info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(roomEntryPrefab, roomListParent);
            entry.GetComponent<RoomEntryUI>().Setup(info.Name, this);
        }
    }
}
