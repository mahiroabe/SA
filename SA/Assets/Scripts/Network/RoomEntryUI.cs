using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// •”‰®–¼‚ð•\Ž¦‚µ Join ƒ{ƒ^ƒ“‚ð‰Ÿ‚·‚Æ RoomManager.JoinRoom ‚ðŒÄ‚Ô
/// </summary>
public class RoomEntryUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    private string roomName;
    private RoomManager manager;

    public void Setup(string name, RoomManager mgr)
    {
        roomName = name;
        manager = mgr;
        roomNameText.text = name;
    }

    public void OnClickJoin()
    {
        manager.JoinRoom(roomName);
    }
}
