using UnityEngine;
using Photon.Pun;

public class GoalTrigger : MonoBehaviour
{
    [Tooltip("ステージ番号")]
    public int StageIndex = 1;
    public bool Goal = false;
    public bool Practice = false;

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();

            if (view != null && view.IsMine)
            {
                var spawner = Object.FindFirstObjectByType<PlayerSpawner>();
                PlayerTimer timer = other.GetComponent<PlayerTimer>();
                if (spawner != null && (timer.isRunning || Practice))
                {
                    spawner.GoToStage(StageIndex);
                }
                else if (spawner != null && !timer.isRunning)
                {
                    spawner.GoToStage(0);
                }
                else
                {
                    Debug.LogWarning("GoalTrigger: PlayerSpawner が見つかりません。");
                }
                if (timer != null && Goal == true)
                {
                    timer.StopTimer();
                    timer.isRunning = false;
                    Debug.Log("ゴール！タイマー停止。記録: " + timer.GetTime());
                }
            }
        }
    }
}
