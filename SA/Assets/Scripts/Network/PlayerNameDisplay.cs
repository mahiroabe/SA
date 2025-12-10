using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerNameDisplay : MonoBehaviourPun
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Transform target; // 頭の位置

    private Camera cam;

    void Start()
    {
        // 他人と自分の名前をセット
        nameText.text = photonView.Owner.NickName;

        cam = Camera.main;

        // 自分自身の頭上ネームだけ非表示にしたい場合
        if (photonView.IsMine)
            nameText.gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        // 常にカメラの方向を見る
        nameText.transform.rotation = Quaternion.LookRotation(
            nameText.transform.position - cam.transform.position
        );

        // 頭の位置に追従
        if (target != null)
        {
            nameText.transform.position = target.position + new Vector3(0, 1.0f, 0);
        }
    }
}
