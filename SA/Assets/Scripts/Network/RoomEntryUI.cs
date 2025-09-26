using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ルーム一覧の1エントリを管理するUIスクリプト
/// </summary>
public class RoomEntryUI : MonoBehaviour
{
    public Text roomNameText; // ルーム名表示
    public Text countText;    // 人数表示

    private string roomName;  // Join時に使う内部保存名

    /// <summary>
    /// RoomManager から部屋情報を受け取ってUIを更新
    /// </summary>
    public void SetInfo(RoomInfo info)
    {
        roomName = info.Name;
        roomNameText.text = info.Name;
        countText.text = $"{info.PlayerCount}/{info.MaxPlayers}";
    }

    /// <summary>
    /// Join ボタン押下時
    /// RoomManager の JoinSpecificRoom を呼ぶ
    /// </summary>
    public void OnClickJoin()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
