using UnityEngine;
using System.Collections;

public class RotatingPlatform4 : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationAxis = Vector3.up;   // 回転軸（例: Y軸なら (0,1,0)）
    public float speed = 50f;                    // 回転スピード（deg/sec）
    public int direction = 1;                    // 1 = 正回転, -1 = 逆回転

    [Header("Stop Settings")]
    public float stopAngleStep = 180f;            // 何度ごとに止まるか
    public float stopTime = 0.5f;                 // 停止時間（秒）

    private float internalAngle = 0f;
    private bool isWaiting = false;
    private float nextStopAngle = 0f;

    void Start()
    {
        // 回転軸に対応する現在角度を取得
        internalAngle = GetAxisAngle();

        // 次の停止角度を計算
        nextStopAngle =
            Mathf.Round(internalAngle / stopAngleStep) * stopAngleStep
            + stopAngleStep * direction;
    }

    void Update()
    {
        if (isWaiting) return;

        internalAngle = Mathf.MoveTowards(
            internalAngle,
            nextStopAngle,
            speed * Time.deltaTime
        );

        ApplyRotation(internalAngle);

        if (Mathf.Abs(internalAngle - nextStopAngle) < 0.1f)
        {
            StartCoroutine(StopAndContinue());
        }
    }

    IEnumerator StopAndContinue()
    {
        isWaiting = true;
        yield return new WaitForSeconds(stopTime);

        nextStopAngle += stopAngleStep * direction;
        isWaiting = false;
    }

    // ====== 回転軸の角度を取得 ======
    float GetAxisAngle()
    {
        Vector3 euler = transform.eulerAngles;

        if (rotationAxis == Vector3.right) return euler.x;
        if (rotationAxis == Vector3.up)    return euler.y;
        if (rotationAxis == Vector3.forward) return euler.z;

        return 0f;
    }

    // ====== 回転を適用 ======
    void ApplyRotation(float angle)
    {
        Vector3 euler = transform.eulerAngles;

        if (rotationAxis == Vector3.right) euler.x = angle;
        if (rotationAxis == Vector3.up)    euler.y = angle;
        if (rotationAxis == Vector3.forward) euler.z = angle;

        transform.rotation = Quaternion.Euler(euler);
    }
}
