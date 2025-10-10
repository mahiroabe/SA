using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerStageManager : MonoBehaviourPun
{
    // 現在のステージ番号を記録
    public int currentStage = 1;

    // ステージを進める（自分専用）
    public void GoToNextStage()
    {
        currentStage++;
        string nextSceneName = $"Stage{currentStage:00}"; // 例: Stage02, Stage03...

        // シーンが存在するかチェック
        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.Log($"[{photonView.Owner.NickName}] が {nextSceneName} に移動します");
            SceneManager.LoadScene(nextSceneName); // 自分だけロード
        }
        else
        {
            Debug.Log($"[{photonView.Owner.NickName}] に次のステージは存在しません");
        }
    }
}
