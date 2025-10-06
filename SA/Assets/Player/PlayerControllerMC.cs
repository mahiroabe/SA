using UnityEngine;

public class PlayerControllerMC : MonoBehaviour
{
    [Header("References")]
    public Transform head;   // 頭（視点基準）
    public Transform body;   // 体（胴体）
    public Transform cam;    // Main Camera

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    [Header("Camera")]
    public float sensitivity = 2f;
    public float tpsDistance = 3f;
    public bool isFPS = false; // F5で切り替え

    private Rigidbody rb;
    private bool isGrounded;
    private float yaw;
    private float pitch;

    [SerializeField] private SkinnedMeshRenderer headMesh; // 頭モデル

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleView();
        Move();
        Jump();
        RotateBody();

        if (Input.GetKeyDown(KeyCode.F5))
        {
            isFPS = !isFPS;
        }
    }

    void HandleView()
    {
        // マウス入力で視点回転
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        // Headの回転に適用（プレイヤーの顔の向き）
        head.rotation = Quaternion.Euler(pitch, yaw, 0);

        // カメラ位置切り替え
        if (isFPS)
        {
            // FPS：頭の位置にカメラを置く
            cam.position = head.position;
            cam.rotation = head.rotation;
            //sheadMesh.enabled = false; // FPSの時に頭を非表示
        }
        else
        {
            // TPS：headの後方に配置
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
            Vector3 offset = rot * new Vector3(0, 0, -tpsDistance);
            cam.position = head.position + offset;
            cam.LookAt(head.position);
            //headMesh.enabled = true; // TPSの時は表示
        }
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 視線（headのyaw方向）基準で移動
        Vector3 forward = head.forward;
        Vector3 right = head.right;
        forward.y = 0;
        right.y = 0;

        Vector3 dir = (forward.normalized * v + right.normalized * h).normalized;
        Vector3 vel = rb.velocity;
        vel.x = dir.x * moveSpeed;
        vel.z = dir.z * moveSpeed;
        rb.velocity = vel;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void RotateBody()
    {
        // HeadとBodyの水平角度差
        float angleDiff = Vector3.SignedAngle(body.forward, head.forward, Vector3.up);

        if (Mathf.Abs(angleDiff) > 45f)
        {
            Quaternion targetRot = Quaternion.Euler(0, head.eulerAngles.y, 0);
            body.rotation = Quaternion.Lerp(body.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    void OnCollisionStay(Collision collision) => isGrounded = true;
    void OnCollisionExit(Collision collision) => isGrounded = false;
}
