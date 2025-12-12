using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviourPun
{
    public AudioSource bgmSource;
    public AudioSource seSource;

    public AudioClip clearSE;

    [PunRPC]
    void PlayClearSE()
    {
        seSource.PlayOneShot(clearSE);
    }

    public void CallClearSE()
    {
        photonView.RPC("PlayClearSE", RpcTarget.All);
    }
}
