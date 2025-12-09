using UnityEngine;
using Photon.Pun;

public class GoalTrigger : MonoBehaviour
{
    [Tooltip("ステージ番号")]
    public int StageIndex = 1;
    public bool Goal = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();

            if (view != null && view.IsMine)
            {
                var spawner = Object.FindFirstObjectByType<PlayerSpawner>();
                if (spawner != null)
                {
                    spawner.GoToStage(StageIndex);
                }
                else
                {
                    Debug.LogWarning("GoalTrigger: PlayerSpawner が見つかりません。");
                }
                PlayerTimer timer = other.GetComponent<PlayerTimer>();
                if (timer != null && Goal == true)
                {
                    timer.StopTimer();
                    Debug.Log("ゴール！タイマー停止。記録: " + timer.GetTime());
                }
            }
        }
    }
}
