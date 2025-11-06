using UnityEngine;
using Photon.Pun;

public class GoalTrigger : MonoBehaviour
{
    public int nextStageNum = 2;                     // 次のステージ番号（初期値は1）

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーだけ判定
        if (other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();

            // 自分のキャラクターなら次のステージへ
            if (view != null && view.IsMine)
            {
                // 新しいAPIを使用（非推奨警告を回避）
                var spawner = Object.FindFirstObjectByType<PlayerSpawner>();
                if (spawner != null)
                {
                    spawner.GoToNextStage(nextStageNum);
                }
            }
        }
    }
}