using UnityEngine;
using Photon.Pun;

public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーだけ判定
        if (other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();

            // 自分のキャラクターなら次のステージへ
            if (view != null && view.IsMine)
            {
                // シーン内のPlayerSpawnerを探して呼び出す
                FindObjectOfType<PlayerSpawner>().GoToNextStage();
            }
        }
    }
}
