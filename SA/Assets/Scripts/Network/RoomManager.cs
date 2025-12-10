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
    [SerializeField] private TMP_InputField roomNameSearch;  // 部屋名検索入力欄
    [SerializeField] private Transform roomListParent;       // ScrollView Content
    [SerializeField] private GameObject roomEntryPrefab;     // ルーム1件プレハブ
    [SerializeField] private TMP_Text statusText;            // 状態表示
    [SerializeField] private TMP_Text roomNameStatusText;    // ロビーの名前の記入状態警告

    // 内部用
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private bool isLoadingScene = false;

    void Start()
    {
        //if (!PhotonNetwork.InLobby)
        PhotonNetwork.JoinLobby();
        

        // PUN接続開始
        statusText.text = "Connecting to Master...";
        roomNameSearch.onValueChanged.AddListener(_ => RefreshRoomListUI()); // 自動更新用
    }

    #region PUN Callback
    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected. Joining Lobby...";
        PhotonNetwork.JoinLobby();
        Debug.Log("OnConnectedToMaster()");
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Joined Lobby";
        Debug.Log("OnJoinedLobby()");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = $"Disconnected: {cause}";
        Debug.Log("OnDisconnected(DisconnectCause cause)");
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
        Debug.Log("OnRoomListUpdate(List<RoomInfo> roomList)");
    }

    public override void OnJoinedRoom()
    {
        if (isLoadingScene) return;  // 既にロード中ならスルー
        isLoadingScene = true;

        statusText.text = $"Joined Room : {PhotonNetwork.CurrentRoom.Name}";
        // 例：ゲームシーンへ遷移
        PhotonNetwork.LoadLevel("Stage");
        Debug.Log("OnJoinedRoom()");
    }
    #endregion


    #region UI Buttons
    /// <summary>「Create」ボタンから呼ぶ</summary>
    public void CreateRoom()
    {
        string enteredName = roomNameInput.text;

        // 部屋名未入力 → 警告出して終了
        if (string.IsNullOrEmpty(enteredName))
        {
            roomNameStatusText.text = "Please enter the room name!";
            return;
        }

        roomNameStatusText.text = ""; // 警告消す

        string roomName = enteredName;

        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 5,
            IsOpen = true,
            IsVisible = true,
            EmptyRoomTtl = 0, 
            PlayerTtl = 0,
            Plugins = new string[0]
        };

        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);

        statusText.text = $"Creating Room : {roomName}";
        Debug.Log("CreateRoom()");
    }

    /// <summary>RoomEntryUI から呼ばれる参加処理</summary>
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        statusText.text = $"Joining Room : {roomName}";
        Debug.Log("JoinRoom(string roomName)");
    }

    /// <summary>検索ボタンから呼ぶ</summary>
    public void OnSearchButton()
    {
        RefreshRoomListUI();
    }
    #endregion

    /// <summary>ルーム一覧UIを更新</summary>
    private void RefreshRoomListUI()
    {
        string keyword = roomNameSearch.text.ToLower();

        foreach (Transform child in roomListParent)
            Destroy(child.gameObject);

        foreach (var info in cachedRoomList.Values)
        {
            // 検索フィルタ
            if (!string.IsNullOrEmpty(keyword) &&
                !info.Name.ToLower().Contains(keyword))
                continue;

            // 満員なら UI を作らない
            if (info.PlayerCount >= info.MaxPlayers)
                continue;

            GameObject entry = Instantiate(roomEntryPrefab, roomListParent);
            entry.GetComponent<RoomEntryUI>().Setup(info, this);
        }
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = $"CreateRoom failed: {message}";
        Debug.LogError($"OnCreateRoomFailed: {message} (code {returnCode})");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = $"JoinRoom failed: {message}";
        Debug.LogError($"OnJoinRoomFailed: {message} (code {returnCode})");
    }

    public void GoToNextStage()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // この部屋の全員だけを対象にシーンを変更
            photonView.RPC("LoadNextStageRPC", RpcTarget.All);
        }
    }

    public void Quit()
    {
        // ゲーム終了
        Application.Quit();

        // Unity エディタで動作確認するため
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    [PunRPC]
    void LoadNextStageRPC()
    {
        // 各部屋ごとに個別でステージ移行
        PhotonNetwork.LoadLevel("Stage02");
    }
}