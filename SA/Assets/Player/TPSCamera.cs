using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public Transform target;      // プレイヤーを指定
    public float distance = 5.0f; // プレイヤーからの距離
    public float height = 2.0f;   // 高さ
    public float sensitivity = 2f;

    private float yaw = 0f;
    private float pitch = 0f;

    void LateUpdate()
    {
        if (target == null) return;

        // マウス入力
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -30f, 60f);

        // 回転計算
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);

        // カメラ位置
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
