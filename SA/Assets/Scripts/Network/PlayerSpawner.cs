using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // プレイヤープレハブ
    [SerializeField] private Transform stage1Spawn;   // ステージ1の初期位置
    [SerializeField] private Transform stage2Spawn;   // ステージ2の初期位置

    private GameObject myPlayer;
    private int currentStage = 1;                     // 現在のステージ番号（初期値は1）       goaltriggerに持っていく

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, stage1Spawn.position, Quaternion.identity);
        }
    }

    // ステージ番号に応じてスポーン位置を返す
    private Vector3 GetSpawnPosition(int stageIndex)
    {
        switch (stageIndex)
        {
            case 1:
                return stage1Spawn.position;
            case 2:
                return stage2Spawn.position;
            default:
                Debug.LogWarning($"未知のステージ番号: {stageIndex}");
                return stage1Spawn.position;
        }
    }

    // そのプレイヤーだけを次のステージへ移動
    public void GoToNextStage()
    {
        if (myPlayer == null) return;

        currentStage++;

        // ステージ数を超えたら最終ステージで止まる
        if (currentStage > 3)
        {
            currentStage = 3;
            Debug.Log("最終ステージに到達しました！");
            return;
        }

        // 自分のプレイヤーの位置だけ変更（他プレイヤーには影響なし）
        myPlayer.transform.position = GetSpawnPosition(currentStage);
        Debug.Log($"ステージ {currentStage} に移動しました");
    }
}