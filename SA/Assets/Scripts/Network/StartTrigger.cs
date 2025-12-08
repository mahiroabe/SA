using UnityEngine;
using Photon.Pun;

public class StartTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();

            // 自分のプレイヤーだけ通知
            if (view != null && view.IsMine)
            {
                RaceManager.Instance.PlayerEnteredStartZone();
            }
        }
    }
}
