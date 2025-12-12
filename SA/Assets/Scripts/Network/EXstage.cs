using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EXstage : GoalTrigger
{
    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount != 1)
        {
            this.gameObject.SetActive(false);
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView view = other.GetComponent<PhotonView>();

            if (view != null && view.IsMine)
            {
                var spawner = Object.FindFirstObjectByType<PlayerSpawner>();
                PlayerTimer timer = other.GetComponent<PlayerTimer>();
                if (spawner != null)
                {
                    spawner.GoToStage(StageIndex);
                }
                else
                {
                    Debug.LogWarning("GoalTrigger: PlayerSpawner が見つかりません。");
                }
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;    // 新規参加禁止
                    PhotonNetwork.CurrentRoom.IsVisible = false; // ロビー非表示
                }
            }
        }
    }
}