using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;     // プレイヤープレハブ
    [SerializeField] private Transform[] stageSpawns;     // 各ステージのスポーン位置

    private GameObject myPlayer;

    void Start()
    {
        // すでに生成されていたら生成しない
        if (PhotonNetwork.LocalPlayer.TagObject != null)
        {
            myPlayer = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            Debug.Log("PlayerSpawner: すでにプレイヤーが存在するため再生成しません。");
            return;
        }

        // スポーン位置がない場合
        if (!PhotonNetwork.IsConnectedAndReady || stageSpawns.Length == 0)
        {
            Debug.LogError("PlayerSpawner: スポーン位置が設定されていません。");
            return;
        }

        // 生成
        myPlayer = PhotonNetwork.Instantiate(
            playerPrefab.name,
            stageSpawns[0].position,
            Quaternion.identity
        );

        // TagObject に登録
        PhotonNetwork.LocalPlayer.TagObject = myPlayer;

        Debug.Log("PlayerSpawner: プレイヤー生成完了");
    }

    // ────────────────────────────────
    // 指定されたステージ番号にワープ
    // ────────────────────────────────
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

        // ワープ処理
        myPlayer.transform.position = stageSpawns[stageIndex].position;

        // 落下中の速度を止める（重要）
        Rigidbody rb = myPlayer.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"ステージ {stageIndex + 1} にワープしました");
    }
}
