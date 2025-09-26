using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            Vector3 spawnPos = new Vector3(Random.Range(-2, 2), 1f, Random.Range(-2, 2));
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        }
    }
}
