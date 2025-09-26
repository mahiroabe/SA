using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���[���ꗗ��1�G���g�����Ǘ�����UI�X�N���v�g
/// </summary>
public class RoomEntryUI : MonoBehaviour
{
    public Text roomNameText; // ���[�����\��
    public Text countText;    // �l���\��

    private string roomName;  // Join���Ɏg�������ۑ���

    /// <summary>
    /// RoomManager ���畔�������󂯎����UI���X�V
    /// </summary>
    public void SetInfo(RoomInfo info)
    {
        roomName = info.Name;
        roomNameText.text = info.Name;
        countText.text = $"{info.PlayerCount}/{info.MaxPlayers}";
    }

    /// <summary>
    /// Join �{�^��������
    /// RoomManager �� JoinSpecificRoom ���Ă�
    /// </summary>
    public void OnClickJoin()
    {
        PhotonNetwork.JoinRoom(roomName);
    }
}
