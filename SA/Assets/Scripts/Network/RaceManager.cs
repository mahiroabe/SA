using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RaceManager : MonoBehaviourPunCallbacks
{
    public static RaceManager Instance;

    [Header("UI")]
    public TMP_Text countdownText;
    public TMP_Text timerText;

    [Header("Settings")]
    public Transform startPoint;   // 全員のスタート地点

    private float time;
    private bool isTimerRunning = false;
    private int playersInside = 0;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (isTimerRunning)
        {
            time += Time.deltaTime;
            timerText.text = time.ToString("F2");
        }
    }

    // ────────────────────────────
    // プレイヤーがトリガーに入った時に呼ぶ
    // ────────────────────────────
    public void PlayerEnteredStartZone()
    {
        playersInside++;

        // 全員入ったらMasterがカウントダウン開始
        if (PhotonNetwork.IsMasterClient && playersInside == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            photonView.RPC("StartCountdownRPC", RpcTarget.All);
        }
    }

    // ────────────────────────────
    // カウントダウン（同期）
    // ────────────────────────────
    [PunRPC]
    public void StartCountdownRPC()
    {
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

        // レース開始したのでロビー（部屋）を閉じる
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;     // 参加不可
            PhotonNetwork.CurrentRoom.IsVisible = false;  // ロビー一覧に非表示
        }

        // 全員ワープ
        photonView.RPC("TeleportAllPlayersRPC", RpcTarget.All);

        // タイマー開始
        isTimerRunning = true;
        time = 0;
    }

    // ────────────────────────────
    // 全プレイヤーをワープ（同期）
    // ────────────────────────────
    [PunRPC]
    void TeleportAllPlayersRPC()
    {
        GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
        if (player != null)
        {
            player.transform.position = startPoint.position;
        }
    }
}
