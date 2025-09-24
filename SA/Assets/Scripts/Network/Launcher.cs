using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // AppId�Őڑ�
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("�ڑ�����: Master�ɓ�����");
        PhotonNetwork.JoinLobby(); // ���r�[�ɓ���
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("���r�[��������");
        // ������UI�Ƀ��[���ꗗ��\���ł���
    }


    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 5; // �ő�5�l
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("���[���ɎQ��: " + PhotonNetwork.CurrentRoom.Name);
        // �v���C���[����
        PhotonNetwork.Instantiate("PlayerPrefab", Vector3.zero, Quaternion.identity);
    }

}
