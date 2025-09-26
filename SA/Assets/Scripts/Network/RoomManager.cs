using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [Header("UI �Q��")]
    public InputField roomNameInput;     // ���[�������͗p�� InputField
    public Transform roomListParent;     // ���[���ꗗ����ׂ�e�I�u�W�F�N�g�iVertical Layout Group �����j
    public GameObject roomEntryPrefab;   // ���[������1���\������v���n�u (RoomEntryUI �X�N���v�g�t��)

    public void OnClickCreate()
    {
        // ���͂���Ȃ烉���_���Ȕԍ���t�^
        string name = roomNameInput.text;
        if (string.IsNullOrEmpty(name))
            name = "Room_" + Random.Range(1000, 9999);

        // ���[���ݒ�F�ő�5�l�E���J�E�Q����
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 5,
            IsVisible = true,
            IsOpen = true
        };

        // Photon�֍쐬���N�G�X�g
        PhotonNetwork.CreateRoom(name, options);
    }

    /// <summary>
    /// [Join Random �{�^��] �������[���Ƀ����_���Q��
    /// </summary>
    public void OnClickJoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /// <summary>
    /// �ꗗUI����ʂɓ�������ۂɌĂ�
    /// RoomEntryUI ���� Join �{�^���ŌĂяo�����
    /// </summary>
    public void JoinSpecificRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// ���r�[���̃��[���ꗗ���X�V����邽�тɌĂ΂��
    /// </summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // ������UI�G���g����S�폜
        foreach (Transform child in roomListParent)
            Destroy(child.gameObject);

        // �󂯎������������UI�ɐ���
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList) continue; // ����ꂽ�����͖���

            GameObject entry = Instantiate(roomEntryPrefab, roomListParent);
            // RoomEntryUI �ɕ�������n���ĕ\�����X�V
            entry.GetComponent<RoomEntryUI>().SetInfo(info);
        }
    }

    /// <summary>
    /// �����������ɓ�������������
    /// �����ŃQ�[���V�[�������[�h����
    /// </summary>
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room: " + PhotonNetwork.CurrentRoom.Name);

        // MasterClient �� LoadLevel ���ĂԂƑS���������V�[����
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Stage01");
    }

    /// <summary>
    /// ���v���C���[�������ɓ����Ă����Ƃ�
    /// </summary>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("�Q��: " + newPlayer.NickName);
        // �����Ńv���C���[���X�gUI���X�V���鏈�������Ă��ǂ�
    }

    /// <summary>
    /// ���v���C���[��������ޏo�����Ƃ�
    /// </summary>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("�ޏo: " + otherPlayer.NickName);
        // �ޏo���UI�X�V�Ȃǂ������ōs��
    }
}
