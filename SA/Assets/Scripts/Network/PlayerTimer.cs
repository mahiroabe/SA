using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerTimer : MonoBehaviourPun
{
    public TMPro.TMP_Text timerText;
    public bool isRunning = false;
    private float time = 0f;

    private void Start()
    {
        // デバッグログでStartが呼ばれたことを確認
        Debug.Log($"PlayerTimer.Start() called for {photonView.ViewID}");

        // まず一回即時登録を試す
        TryRegisterWithUI();

        // UI が遅れて存在する可能性があるのでリトライコルーチンを回す
        StartCoroutine(RegisterWithUI_Retry());
    }

    private void TryRegisterWithUI()
    {
        var ui = Object.FindFirstObjectByType<PlayerUI>();
        if (ui == null) return;

        // ui に登録を依頼（PlayerUI 内で IsMine チェックをしている）
        ui.SetTimer(this);
    }

    private IEnumerator RegisterWithUI_Retry()
    {
        float timeout = 5f; // 最大5秒間リトライ
        float elapsed = 0f;
        float interval = 0.2f;

        while (elapsed < timeout)
        {
            var ui = Object.FindFirstObjectByType<PlayerUI>();
            if (ui != null)
            {
                ui.SetTimer(this);
                yield break; // 成功したら終わり
            }

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        Debug.LogWarning("PlayerTimer: PlayerUI が見つからず SetTimer に登録できませんでした（タイムアウト）");
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (isRunning)
        {
            time += Time.deltaTime;
            if (timerText != null)
                timerText.text = time.ToString("F2");
        }
    }

    public void StartTimer()
    {
        if (!photonView.IsMine) return;
        time = 0;
        isRunning = true;
    }

    public void StopTimer()
    {
        if (!photonView.IsMine) return;
        isRunning = false;
    }

    public float GetTime() => time;
}
