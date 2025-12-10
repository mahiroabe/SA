using UnityEngine;
using Photon.Pun;

public class StartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView view = other.GetComponent<PhotonView>();

        // 自分のプレイヤーだけ通知
        if (view != null && view.IsMine)
        {
            RaceManager.Instance.photonView.RPC(
                "RPC_PlayerEntered",
                RpcTarget.MasterClient
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView view = other.GetComponent<PhotonView>();

        if (view != null && view.IsMine)
        {
            RaceManager.Instance.photonView.RPC(
                "RPC_PlayerExited",
                RpcTarget.MasterClient
            );
        }
    }
}
