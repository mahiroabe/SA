using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerTimer : MonoBehaviourPun
{
    public TMP_Text timerText;

    private float time = 0;
    public bool isRunning = false;

    void Update()
    {
        if (!photonView.IsMine) return; // 自分だけタイマーを動かす

        if (isRunning)
        {
            time += Time.deltaTime;
            timerText.text = time.ToString("F2");
        }
    }

    public void StartTimer()
    {
        if (photonView.IsMine)
        {
            time = 0;
            isRunning = true;
        }
    }

    public void StopTimer()
    {
        if (photonView.IsMine)
        {
            isRunning = false;
        }
    }

    public float GetTime()
    {
        return time;
    }
}
