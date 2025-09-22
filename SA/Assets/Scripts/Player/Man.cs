using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Man : MonoBehaviour
{
    [SerializeField] GameObject player;

    public GameObject Cam;
    private Animator animator; // キャラクターオブジェクトのAnimator
    public RuntimeAnimatorController walking;
    public RuntimeAnimatorController running;
    public RuntimeAnimatorController standing;

    public float moveSpeed = 5.0f; // キャラクターの移動速度

    public bool damaged;

    private void Start()
    {
        animator = GetComponent<Animator>();
        damaged = false;
    }

    void Screen_movement(float mx){
        // X方向に一定量移動していれば横回転
        //0.0000001fは滑らかさ
        if (Mathf.Abs(mx) > 0.0000001f)
        {
            mx = mx * 5;

            // 回転軸はワールド座標のY軸
            player.transform.RotateAround(player.transform.position, Vector3.up, mx);
        }
    }

    void Update()
    {   
        float mx = Input.GetAxis("Mouse X");
        Screen_movement(mx);

        if (Input.GetKey(KeyCode.W))
        {
            // "W"キーが押されたときの処理をここに記述
            print("歩くぞお");
            animator.runtimeAnimatorController = walking;                   

            //プレイヤーの正面に向かって移動する
            transform.position += transform.forward * moveSpeed * Time.deltaTime;

        }
        else if(Input.GetKeyUp(KeyCode.W))
        {
            animator.runtimeAnimatorController = standing;
        }
    }
}
