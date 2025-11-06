using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;     // プレイヤープレハブ
    [SerializeField] private Transform[] stageSpawns;     // 各ステージのスポーン位置を配列で設定

    private GameObject myPlayer;

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady && stageSpawns.Length > 0)
        {
            // 最初はステージ1（配列の0番目）にスポーン
            myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, stageSpawns[0].position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("PlayerSpawner: スポーン位置が設定されていません。");
        }
    }

    // 指定されたステージ番号に移動
    public void GoToStage(int stageIndex)
    {
        if (myPlayer == null)
        {
            Debug.LogWarning("PlayerSpawner: プレイヤーが存在しません。");
            return;
        }

        if (stageIndex < 0 || stageIndex >= stageSpawns.Length)
        {
            Debug.LogWarning($"PlayerSpawner: ステージ {stageIndex} は存在しません。");
            return;
        }

        // 位置変更（少し上に浮かせて床に埋まらないように）
        myPlayer.transform.position = stageSpawns[stageIndex].position;
        Debug.Log($"ステージ {stageIndex + 1} に移動しました");
    }
}
