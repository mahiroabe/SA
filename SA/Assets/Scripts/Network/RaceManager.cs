using UnityEngine;
using Photon.Pun;
using TMPro;

public class RaceManager : MonoBehaviourPunCallbacks
{
    public static RaceManager Instance;

    [Header("UI")]
    public TMP_Text countdownText;

    [Header("Settings")]
    public Transform startPoint;   // 全員のスタート地点

    private int playersInside = 0;
    public bool isCountdownStarted = false;


    void Awake()
    {
        Instance = this;
    }

    // ────────────────────────────
    // プレイヤーがスタートゾーンに
    // ────────────────────────────
    // 入った
    public void PlayerEnteredStartZone()
    {
        playersInside++;

        CheckStartCondition();
    }

    // 出た
    public void PlayerExitedStartZone()
    {
        if (playersInside > 0)
            playersInside--;

        CheckStartCondition();
    }

    private void CheckStartCondition()
    {
        if (isCountdownStarted) return;

        if (PhotonNetwork.IsMasterClient &&
            playersInside == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            isCountdownStarted = true;
            photonView.RPC("StartCountdownRPC", RpcTarget.All);
        }
    }

    // ────────────────────────────
    // カウントダウン開始（全体同期）
    // ────────────────────────────
    [PunRPC]
    public void StartCountdownRPC()
    {
        isCountdownStarted = true;
        playersInside = 0;  // ← ここでリセット

        StartCoroutine(CountdownCoroutine());
    }


    private System.Collections.IEnumerator CountdownCoroutine()
    {
        countdownText.gameObject.SetActive(true);

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);

        countdownText.gameObject.SetActive(false);

        // ────────────────────────────
        // レース開始 → ロビー締め切り
        // ────────────────────────────
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;    // 新規参加禁止
            PhotonNetwork.CurrentRoom.IsVisible = false; // ロビー非表示
        }

        // ────────────────────────────
        // 全プレイヤーをスタート地点へワープ
        // ────────────────────────────
        photonView.RPC("TeleportAllPlayersRPC", RpcTarget.All);

        // ────────────────────────────
        // 全プレイヤーのタイマー開始
        // ────────────────────────────
        photonView.RPC("StartAllPlayerTimersRPC", RpcTarget.All);
        isCountdownStarted = false;
    }

    // ────────────────────────────
    // 全プレイヤーをワープ（全体同期）
    // ────────────────────────────
    [PunRPC]
    void TeleportAllPlayersRPC()
    {
        GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;

        if (player != null)
        {
            player.transform.position = startPoint.position;

            // 落下中の速度リセット
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    // ────────────────────────────
    // 各プレイヤーの PlayerTimer を開始
    // ────────────────────────────
    [PunRPC]
    void StartAllPlayerTimersRPC()
    {
        var timers = FindObjectsByType<PlayerTimer>(FindObjectsSortMode.None);

        foreach (var t in timers)
        {
            if (t.photonView.IsMine)
            {
                t.StartTimer();
            }
        }
    }

    [PunRPC]
    private void RPC_PlayerEntered()
    {
        playersInside++;
        CheckStartCondition();
    }

    [PunRPC]
    private void RPC_PlayerExited()
    {
        playersInside = Mathf.Max(0, playersInside - 1);
        CheckStartCondition();
    }

}
