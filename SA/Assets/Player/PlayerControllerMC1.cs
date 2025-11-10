using Photon.Pun;
using UnityEngine;
public class PlayerControllerMC1 : MonoBehaviourPun, IPunObservable
{
    enum PlayerState
    {
        Idle,
        Move,
        Run,
        Jump,
        Attack
    }

    // 【参照設定】
    [Header("References")]
    public Transform head;   // プレイヤーの頭（視点基準）
    public Transform body;   // 体（胴体）
    public Transform cam;    // カメラ（MainCamera）

    // 【移動関連】
    [Header("Movement Settings")]
    public float moveSpeed = 5f;   // 移動速度
    public float jumpForce = 5f;   // ジャンプ力

    // 【ダッシュ用】
    private bool isRunning = false;
    private float lastWPressTime = -1f;
    private float doubleTapThreshold = 0.3f; // 2回押し判定時間

    public float walkSpeed = 4f;
    public float runSpeed = 8f;

    // 【カメラ設定】
    [Header("Camera Settings")]
    public float sensitivity = 2f;     // マウス感度
    public float tpsDistance = 3f;     // TPS時のカメラ距離
    public bool isFPS = false;         // 現在の視点モード（F5で切替）
    public float fpsCamHeightOffset = 0.45f; // FPS時カメラ高さ調整

    // 【内部変数】
    private Rigidbody rb;
    private Animator animator;

    private bool isGrounded;      // 接地判定
    private float yaw;            // 水平方向の回転
    private float pitch;          // 垂直方向の回転

    // 【床追従関連】
    private Transform currentPlatform;
    private Vector3 lastPlatformPos;
    private Quaternion lastPlatformRot;
    private Vector3 lastAngularVelocity;
    private float rotationInertiaTime = 1.0f;
    private float rotationInertiaTimer = 0f;

    // 【ネットワーク同期関連】
    private Vector3 networkPos;
    private Quaternion networkRot;
    private Vector3 velocitySmooth;

    // 【アニメーション関連】
    private PlayerState currentState;
    private PlayerState lastState;

    // --- 初期化 ---
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        animator.applyRootMotion = false;

        // 回転制限（倒れ防止）
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // カーソルロック
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // カメラが未設定なら自動で取得
        if (cam == null)
            cam = GetComponentInChildren<Camera>()?.transform;

        // 初期方向を同期
        yaw = body.eulerAngles.y;

        // 他プレイヤーのカメラは無効化
        if (!photonView.IsMine && cam != null)
            cam.gameObject.SetActive(false);
    }

    // --- フレーム更新 ---
    void Update()
    {
        if (!photonView.IsMine) return;

        HandleView();           // カメラ制御
        HandleMovementState();  // 移動状態処理
        Jump();                 // ジャンプ
        RotateBody();           // 体の向き制御
        UpdateState();

        animator.SetFloat("Speed", rb.velocity.magnitude);

        // --- 視点切り替え（TPS ⇔ FPS） ---
        if (Input.GetKeyDown(KeyCode.F5))
            isFPS = !isFPS;
    }

    // --- アニメーション状態更新 ---
    void UpdateState()
    {
        PlayerState newState;

        // 状態判定
        if (!isGrounded)
        {
            newState = PlayerState.Jump;
        }
        else
        {
            float speed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

            if (speed < 0.1f)
                newState = PlayerState.Idle;
            else if (isRunning)
                newState = PlayerState.Run; // 走り
            else
                newState = PlayerState.Move; // 歩き
        }

        // 状態が変わったときだけアニメーション変更
        if (newState != currentState)
        {
            currentState = newState;

            switch (currentState)
            {
                case PlayerState.Idle:
                    animator.Play("Idle");
                    break;

                case PlayerState.Move:
                    animator.Play("Walk");
                    break;

                case PlayerState.Run:
                    animator.Play("Run");
                    break;

                case PlayerState.Jump:
                    animator.Play("Jump");
                    break;
            }
        }
    }

    // --- カメラ処理 ---
    void HandleView()
    {
        // マウス入力で視点回転
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        // 頭の回転（カメラ基準）
        head.rotation = Quaternion.Euler(pitch, yaw, 0);

        if (isFPS)
        {
            // --- FPSモード ---
            Vector3 eyePos = head.position + Vector3.up * fpsCamHeightOffset;
            cam.position = eyePos;
            cam.rotation = head.rotation;
        }
        else
        {
            // --- TPSモード ---
            Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
            Vector3 desiredPos = head.position - rot * Vector3.forward * tpsDistance;

            // 壁などにカメラがめり込まないようRaycastで補正
            if (Physics.Raycast(head.position, -head.forward, out RaycastHit hit, tpsDistance))
            {
                cam.position = hit.point + head.forward * 0.2f;
            }
            else
            {
                cam.position = desiredPos;
            }

            cam.LookAt(head.position);
        }

        /*/ スムーズトランジション
        Vector3 targetPos = isFPS ? head.position + Vector3.up * fpsCamHeightOffset : head.position - head.forward * tpsDistance;

        cam.position = Vector3.Lerp(cam.position, targetPos, Time.deltaTime * 10f);
         cam.rotation = Quaternion.Lerp(cam.rotation, head.rotation, Time.deltaTime * 10f);
        */
    }

    // --- 歩行処理 ---
    void Walk()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = head.forward;
        Vector3 right = head.right;
        forward.y = 0; 
        right.y = 0;

        Vector3 move = (forward * v + right * h).normalized * walkSpeed;

        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
    }

    // --- 走行処理 ---
    void Run()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = head.forward;
        Vector3 right = head.right;
        forward.y = 0;
        right.y = 0;

        Vector3 move = (forward * v + right * h).normalized * runSpeed;

        rb.velocity = new Vector3(move.x, rb.velocity.y, move.z);
    }

    // --- ジャンプ処理 ---
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("IsJumping");
        }
    }

    // --- 体の回転制御 ---
    void RotateBody()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        float angleDiff = Vector3.SignedAngle(body.forward, head.forward, Vector3.up);

        if (isMoving)
        {
            // 動いている時：即回転
            Quaternion targetRot = Quaternion.Euler(0, head.eulerAngles.y, 0);
            body.rotation = Quaternion.Lerp(body.rotation, targetRot, Time.deltaTime * 10f);
        }
        else if (Mathf.Abs(angleDiff) > 45f)
        {
            // 止まっている時：45°超えたら回転
            Quaternion targetRot = Quaternion.Euler(0, head.eulerAngles.y, 0);
            body.rotation = Quaternion.Lerp(body.rotation, targetRot, Time.deltaTime * 5f);
        }
    }

    // --- 移動状態処理 ---
    void HandleMovementState()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        bool isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;

        // --- Wキーのシングル・ダブルタップ判定 ---
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (Time.time - lastWPressTime < doubleTapThreshold)
            {
                // ★ ダブルタップ：走る
                isRunning = true;
            }

            lastWPressTime = Time.time;
        }

        // --- 移動していない間は状態を維持する ---
        if (!isMoving)
        {
            // 完全に停止したら走り解除
            isRunning = false;
            return;
        }

        // --- 実際の移動 ---
        if (isRunning)
            Run();
        else
            Walk();
    }

    // --- 床との接触判定 ---
    void OnCollisionStay(Collision collision)
    {
        bool groundedThisFrame = false;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
            {
                groundedThisFrame = true;

                // 移動床への追従設定
                if (collision.gameObject.CompareTag("MovingPlatform") ||
                    collision.gameObject.CompareTag("RotatingPlatform"))
                {
                    if (currentPlatform != collision.transform)
                    {
                        currentPlatform = collision.transform;
                        lastPlatformPos = currentPlatform.position;
                        lastPlatformRot = currentPlatform.rotation;
                    }
                }
            }
        }

        isGrounded = groundedThisFrame;
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform == currentPlatform)
            currentPlatform = null;

        isGrounded = false;
    }

    // --- 移動床追従処理 ---
    void LateUpdate()
    {
        if (!photonView.IsMine) return;
        UpdatePlatformMovement();
    }

    void UpdatePlatformMovement()
    {
        if (currentPlatform != null)
        {
            // --- 床の移動・回転差分計算 ---
            Vector3 platformDelta = currentPlatform.position - lastPlatformPos;
            Quaternion rotationDelta = currentPlatform.rotation * Quaternion.Inverse(lastPlatformRot);

            // --- 床の動きに追従 ---
            Vector3 relativePos = transform.position - currentPlatform.position;
            relativePos = rotationDelta * relativePos;
            transform.position = currentPlatform.position + relativePos + platformDelta;
            transform.rotation = rotationDelta * transform.rotation;

            // --- 床の回転慣性を保存 ---
            rotationDelta.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f) angle -= 360f;
            lastAngularVelocity = axis * angle * Mathf.Deg2Rad / Time.deltaTime;

            // --- 記録更新 ---
            lastPlatformPos = currentPlatform.position;
            lastPlatformRot = currentPlatform.rotation;
            rotationInertiaTimer = 0f;
        }
        else if (rotationInertiaTimer < rotationInertiaTime)
        {
            // --- 床から離れた後の慣性回転 ---
            float t = 1f - (rotationInertiaTimer / rotationInertiaTime);
            Quaternion delta = Quaternion.Euler(lastAngularVelocity * Time.deltaTime * t);
            transform.rotation = delta * transform.rotation;
            rotationInertiaTimer += Time.deltaTime;
        }
    }

    // --- ネットワーク同期処理 ---
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 自分の状態を送信
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));
        }
        else
        {
            // 他プレイヤーの状態を受信
            networkPos = (Vector3)stream.ReceiveNext();
            networkRot = (Quaternion)stream.ReceiveNext();
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
        }
    }

    // --- 位置補間（他プレイヤーのみ） ---
    void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // スムーズな補間で同期
            transform.position = Vector3.SmoothDamp(transform.position, networkPos, ref velocitySmooth, 0.1f);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRot, Time.fixedDeltaTime * 10f);
        }
    }
}
