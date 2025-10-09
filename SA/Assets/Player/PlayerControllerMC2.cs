using Photon.Pun;
using UnityEngine;

public class PlayerControllerMC2 : MonoBehaviourPun
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
    public float fpsCamHeightOffset = 0.45f; // FPS時のカメラ高さ調整

    private Rigidbody rb;
    private bool isGrounded;
    private float yaw;
    private float pitch;
    private Transform currentPlatform; //床追従用 
    private Vector3 lastPlatformPos;
    private Quaternion lastPlatformRot;
    private Vector3 lastAngularVelocity;  // 床の回転速度
    private float rotationInertiaTime = 1.0f; // 慣性が続く時間（秒）
    private float rotationInertiaTimer = 0f;


    // public GameObject playerPrefab;

    void Start()
    {
        // PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // --- カメラの自動割り当て ---
        if (cam == null)
            cam = GetComponentInChildren<Camera>()?.transform;

        // --- 初期視点方向合わせ ---
        yaw = body.eulerAngles.y;

        // --- 他人のカメラは無効化 ---
        if (!photonView.IsMine && cam != null)
            cam.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        HandleView();
        Move();
        Jump();
        RotateBody();

        if (Input.GetKeyDown(KeyCode.F5))
            isFPS = !isFPS;
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
            Vector3 eyePos = head.position + Vector3.up * fpsCamHeightOffset;
            cam.position = eyePos;
            cam.rotation = head.rotation;
            //headMesh.enabled = false; // FPSの時に頭を非表示
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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        // HeadとBodyの水平角度差
        float angleDiff = Vector3.SignedAngle(body.forward, head.forward, Vector3.up);

        if (isMoving)
        {
            // --- 動いてる時：マイクラ式で即回転 ---
            Quaternion targetRot = Quaternion.Euler(0, head.eulerAngles.y, 0);
            body.rotation = Quaternion.Lerp(body.rotation, targetRot, Time.deltaTime * 10f);
        }
        else
        {
            // --- 止まってる時：45°を超えたら回転 ---
            if (Mathf.Abs(angleDiff) > 45f)
            {
                Quaternion targetRot = Quaternion.Euler(0, head.eulerAngles.y, 0);
                body.rotation = Quaternion.Lerp(body.rotation, targetRot, Time.deltaTime * 5f);
            }
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // 地面判定
        foreach (ContactPoint contact in collision.contacts)
        {
            // 上向きの面に接触している場合のみ「床」とみなす
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
            {
                isGrounded = true;

                if (collision.gameObject.CompareTag("MovingPlatform") ||
                    collision.gameObject.CompareTag("RotatingPlatform"))
                {
                    // 新しい床に乗ったとき初期化
                    if (currentPlatform != collision.transform)
                    {
                        currentPlatform = collision.transform;
                        lastPlatformPos = currentPlatform.position;
                        lastPlatformRot = currentPlatform.rotation;
                    }
                }
                return;
            }
        }

        // 上向きでない場合は床扱いしない
        isGrounded = false;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform == currentPlatform)
        {
            currentPlatform = null;
        }
        isGrounded = false;
    }

    void LateUpdate()
    {
        UpdatePlatformMovement();
    }

    // 床の移動・回転に追従（親子化しない）
    void UpdatePlatformMovement()
    {
        if (currentPlatform != null)
        {
            // --- 床の移動・回転差分 ---
            Vector3 platformDelta = currentPlatform.position - lastPlatformPos;
            Quaternion rotationDelta = currentPlatform.rotation * Quaternion.Inverse(lastPlatformRot);

            // --- プレイヤーを床と一緒に動かす ---
            Vector3 relativePos = transform.position - currentPlatform.position;
            relativePos = rotationDelta * relativePos;
            transform.position = currentPlatform.position + relativePos + platformDelta;
            transform.rotation = rotationDelta * transform.rotation;

            // --- 床の回転速度を保存 ---
            rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;
            lastAngularVelocity = axis * angle / Time.deltaTime;

            // 記録更新
            lastPlatformPos = currentPlatform.position;
            lastPlatformRot = currentPlatform.rotation;
            rotationInertiaTimer = 0f; // 床にいる間は慣性タイマーをリセット
        }
        else if (rotationInertiaTimer < rotationInertiaTime)
        {
            // --- 床から離れた後の慣性回転処理 ---
            float t = 1f - (rotationInertiaTimer / rotationInertiaTime);
            Quaternion delta = Quaternion.Euler(lastAngularVelocity * Time.deltaTime * t);
            transform.rotation = delta * transform.rotation;

            rotationInertiaTimer += Time.deltaTime;
        }
    }
}
