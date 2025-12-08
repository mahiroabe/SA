using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class RoomEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;     // ロビーの名前
    [SerializeField] private TMP_Text playerCountText;  // プレイヤーの人数
    [SerializeField] private Button joinButton;         // 参加ボタン

    private string roomName;
    private RoomManager manager;

    public void Setup(RoomInfo info, RoomManager mgr)
    {
        roomName = info.Name;
        manager = mgr;

        roomNameText.text = info.Name;
        playerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";

        // 満員なら Join ボタンを無効化
        if (info.PlayerCount >= info.MaxPlayers)
        {
            joinButton.interactable = false;
        }
        else
        {
            joinButton.interactable = true;
        }
    }

    public void OnClickJoin()
    {
        manager.JoinRoom(roomName);
    }
}
