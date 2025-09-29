using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public Transform head;        // プレイヤーの頭
    public float sensitivity = 2f;
    public float distance = 3f;   // 頭からの距離

    private float yaw = 0f;
    private float pitch = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -89f, 89f); // 真下も真上も見られるようにするなら制限は広め

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rot * new Vector3(0, 0, -distance);

        transform.position = head.position + offset;
        transform.LookAt(head.position);
    }
}
