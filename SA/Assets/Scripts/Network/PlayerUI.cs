using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject pausePanel;

    private bool isOpen = false;

    void Start()
    {
        pausePanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
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
        PhotonNetwork.LeaveRoom();  // ÉãÅ[ÉÄëﬁèo
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
        var spawner = Object.FindFirstObjectByType<PlayerSpawner>();
        spawner.GoToStage(0);
    }
}
