using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nameInputField; // 名前入力欄
    [SerializeField] private TMP_Text statusText;           // 状態表示テキスト
    [SerializeField] private GameObject connectButton;      // 接続ボタン

    private bool isConnecting = false;

    private void Start()
    {
        // 初期は白文字に戻す
        statusText.color = Color.white;

        //保存された名前を自動入力
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            nameInputField.text = PlayerPrefs.GetString("PlayerName");
        }

        statusText.text = "Enter your name and press Start";
    }

    // UI Button の OnClick に登録する
    public void StartConnection()
    {
        // 名前未記入なら赤い警告
        if (string.IsNullOrWhiteSpace(nameInputField.text))
        {
            statusText.color = Color.red;
            statusText.text = "Please enter your name!";
            return;
        }

        if (isConnecting) return;
        isConnecting = true;

        // ボタン非表示
        if (connectButton != null)
            connectButton.SetActive(false);

        // 通常ステータスは白色に戻す
        statusText.color = Color.white;
        statusText.text = "Connecting to Photon...";

        // 名前をPhotonにセット
        PhotonNetwork.NickName = nameInputField.text;

        // 名前保存
        PlayerPrefs.SetString("PlayerName", nameInputField.text);
        PlayerPrefs.Save();

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        statusText.color = Color.white;
        statusText.text = "Connected to Master Server";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusText.color = Color.white;
        statusText.text = "Joined Lobby";
        Invoke(nameof(LoadLobbyScene), 0.2f);
    }

    private void LoadLobbyScene()
    {
        SceneManager.LoadScene("Lobby");
    }


    public void Quit()
    {
        // ゲーム終了
        Application.Quit();
    }
}
