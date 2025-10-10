using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
     [SerializeField] private GameObject playerPrefab; // プレイヤープレハブ
    [SerializeField] private Transform stage1Spawn;   // ステージ1の初期位置
    [SerializeField] private Transform stage2Spawn;   // ステージ2の初期位置

    private GameObject myPlayer;

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, stage1Spawn.position, Quaternion.identity);
        }
    }

    // 他のスクリプトから呼び出すためのワープ関数
    public void WarpToStage2()
    {
        if (myPlayer != null)
        {
            myPlayer.transform.position = stage2Spawn.position;
        }
    }
}
