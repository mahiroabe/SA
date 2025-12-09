using UnityEngine;
using Photon.Pun;

public class StartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 自分のプレイヤーだけが入ったとき通知
        PhotonView view = other.GetComponent<PhotonView>();
        if (view != null && view.IsMine)
        {
            RaceManager.Instance.PlayerEnteredStartZone();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // 自分のプレイヤーだけが出たとき通知
        PhotonView view = other.GetComponent<PhotonView>();
        if (view != null && view.IsMine)
        {
            RaceManager.Instance.PlayerExitedStartZone();
        }
    }
}
