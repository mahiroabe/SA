using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerNameManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;

    void Start()
    {
        // 前回の名前があれば初期値に入れる
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            nameInput.text = PlayerPrefs.GetString("PlayerName");
            PhotonNetwork.NickName = nameInput.text;
        }
    }

    public void OnClickSetName()
    {
        if (string.IsNullOrEmpty(nameInput.text))
            return;

        string newName = nameInput.text;
        PhotonNetwork.NickName = newName;

        // ローカル保存
        PlayerPrefs.SetString("PlayerName", newName);
        PlayerPrefs.Save();

        Debug.Log("PlayerName Set : " + PhotonNetwork.NickName);
    }
}
