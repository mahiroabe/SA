using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("UI 参照")]
    public InputField roomNameInput;     // ルーム名入力用の InputField
    public Transform roomListParent;     // ルーム一覧を並べる親オブジェクト（Vertical Layout Group 推奨）
    public GameObject roomEntryPrefab;   // ルーム情報を1件表示するプレハブ (RoomEntryUI スクリプト付き)

    public void OnClickCreate()
    {
        // 入力が空ならランダムな番号を付与
        string name = roomNameInput.text;
        if (string.IsNullOrEmpty(name))
            name = "Room_" + Random.Range(1000, 9999);

        // ルーム設定：最大5人・公開・参加可
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 5,
            IsVisible = true,
            IsOpen = true
        };

        // Photonへ作成リクエスト
        PhotonNetwork.CreateRoom(name, options);
    }

    /// <summary>
    /// [Join Random ボタン] 既存ルームにランダム参加
    /// </summary>
    public void OnClickJoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// 一覧UIから個別に入室する際に呼ぶ
    /// RoomEntryUI から Join ボタンで呼び出される
    /// </summary>
    public void JoinSpecificRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// ロビー内のルーム一覧が更新されるたびに呼ばれる
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 既存のUIエントリを全削除
        foreach (Transform child in roomListParent)
            Destroy(child.gameObject);

        // 受け取った部屋情報をUIに生成
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList) continue; // 閉じられた部屋は無視

            GameObject entry = Instantiate(roomEntryPrefab, roomListParent);
            // RoomEntryUI に部屋情報を渡して表示を更新
            entry.GetComponent<RoomEntryUI>().SetInfo(info);
        }
    }

    /// <summary>
    /// 自分が部屋に入室完了した時
    /// ここでゲームシーンをロードする
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        // MasterClient が LoadLevel を呼ぶと全員が同じシーンへ
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Stage01");
    }

    /// <summary>
    /// 他プレイヤーが部屋に入ってきたとき
    /// </summary>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("参加: " + newPlayer.NickName);
        // ここでプレイヤーリストUIを更新する処理を入れても良い
    }

    /// <summary>
    /// 他プレイヤーが部屋を退出したとき
    /// </summary>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("退出: " + otherPlayer.NickName);
        // 退出後のUI更新などをここで行う
    }
}
