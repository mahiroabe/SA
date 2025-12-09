using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject pausePanel;

    private bool isOpen = false;
    private PlayerTimer timer;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    // PlayerTimer から呼ばれる
    public void SetTimer(PlayerTimer timerRef)
    {
        if (timerRef == null)
        {
            Debug.LogWarning("PlayerUI.SetTimer: null が渡されました");
            return;
        }

        // その Timer がローカルプレイヤー所有なら登録（マルチ対策）
        if (!timerRef.photonView.IsMine)
        {
            Debug.Log("PlayerUI.SetTimer: 渡された Timer は自分のものではありません (Ignore).");
            return;
        }

        timer = timerRef;
        Debug.Log("PlayerUI: Timer を登録しました");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TryTogglePause();
        }
    }

    private void TryTogglePause()
    {
        int playerCount = PhotonNetwork.CurrentRoom != null
            ? PhotonNetwork.CurrentRoom.PlayerCount
            : 1; // 念のため

        bool isRaceRunning = (timer != null && timer.isRunning);

        // ★レース中でも 1 人ならポーズ OK！
        if (isRaceRunning && playerCount > 1)
        {
            Debug.Log("レース中 & 他プレイヤーがいるためポーズ不可");
            return;
        }

        TogglePauseMenu();
    }

    public void TogglePauseMenu()
    {
        isOpen = !isOpen;
        pausePanel.SetActive(isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }

    public void ExitToLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LocalPlayer.TagObject = null;

        var spawner = FindFirstObjectByType<PlayerSpawner>();
        if (spawner != null)
            Destroy(spawner.gameObject);

        SceneManager.LoadScene("Lobby");
    }

    public void StartStage()
    {
        TogglePauseMenu();
        if (timer.isRunning)
            timer.StopTimer();
        var spawner = Object.FindFirstObjectByType<PlayerSpawner>();
        spawner.GoToStage(0);
    }
}
