using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMove : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform cam; // TPSカメラのTransformを割り当てる

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 倒れないように回転制御
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.drag = 2f;           // 摩擦力を追加し滑りにくくする
        rb.angularDrag = 5f;    // 回転の摩擦力を追加し安定させる
    }

    void Update()
    {
        // 自分以外操作しないように
        //if (!PhotonView.IsMaine) return;
        Move();
        Jump();
    }

    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // カメラ基準の移動方向
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 moveDir = (camForward.normalized * v + camRight.normalized * h).normalized;

        // 坂道補正：床の法線に沿って移動方向を投影
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.1f))
        {
            moveDir = Vector3.ProjectOnPlane(moveDir, hit.normal);
        }

        // 移動速度を反映
        Vector3 velocity = rb.velocity;
        velocity.x = moveDir.x * moveSpeed;
        velocity.z = moveDir.z * moveSpeed;
        rb.velocity = velocity;

        // 向き変更（動いているときだけ）
        if (moveDir.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    // 接地判定
    void OnCollisionStay(Collision collision)
    {
        isGrounded = true;

        // 動く床に乗っていたら親にする
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(collision.transform);
        }
    }

    // 接地判定解除
    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;

        // 動く床から降りたら親を解除
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            transform.SetParent(null);
        }
    }
}
