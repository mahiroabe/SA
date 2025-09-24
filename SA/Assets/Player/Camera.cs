using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    public float distance; // カメラとプレイヤー間の距離
    public float height; // カメラの高さ
    public float smoothSpeed; // カメラの回転速度
	
    void Update()
    {
        // マウスの移動量を取得
        float my = Input.GetAxis("Mouse Y");
        float mx = Input.GetAxis("Mouse X");
        
         // X方向に一定量移動していれば横回転
        //0.0000001fは滑らかさ
        if (Mathf.Abs(mx) > 0.0000001f)
        {
            mx = mx * 5;
            
            // 回転軸はワールド座標のY軸
            transform.RotateAround(player.transform.position, Vector3.up, mx);
            
        }  

        // Y方向に一定量移動していれば縦回転
        if (Mathf.Abs(my) > 0.0000001f)
        {
            //高さの制限
            if ((height - my) < -2 || (height - my) > 4)
            {
                my = 0;
            }
            height -= my / 10;            
        }  
    }

    void LateUpdate()
    {
        // プレイヤーの中心位置を計算
        Vector3 playerCenter = player.transform.position + Vector3.up * height;

        // プレイヤーの後ろに位置するターゲット位置を計算
        Vector3 targetPosition = playerCenter - player.transform.forward * distance;

        
        // カメラの位置を滑らかに更新
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // カメラは常にプレイヤーを向く
        transform.LookAt(player.transform);
    }
}