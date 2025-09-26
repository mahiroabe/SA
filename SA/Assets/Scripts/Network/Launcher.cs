using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Launcher : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); // Photon�֐ڑ�
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Master�ڑ�����");
        PhotonNetwork.JoinLobby();           // ���r�[�Q��
    }

    public override void OnJoinedLobby()
    {
        SceneManager.LoadScene("Lobby");     // Lobby�V�[���֑J��
    }
}
