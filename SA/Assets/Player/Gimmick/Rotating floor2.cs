using UnityEngine;

public class RotatingPlatform3 : MonoBehaviour
{
    public float speed = 50f;
    private float currentAngle = 0f; // 自分で管理する角度
    private bool isWaiting = false;
    private bool goingTo180 = true;

    void Start()
    {
        // 現在の Z 角度を初期値にする
        currentAngle = transform.eulerAngles.z;
    }

    void Update()
    {
        if (isWaiting) return;

        float targetAngle = goingTo180 ? 180f : 0f;

        // 🎯 内部管理角度で Z軸回転
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, speed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

        // 停止条件
        if (Mathf.Abs(currentAngle - targetAngle) < 0.1f)
        {
            StartCoroutine(StopAndSwitch());
        }
    }

    System.Collections.IEnumerator StopAndSwitch()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1f);

        // 次の停止角度へ
        goingTo180 = !goingTo180;
        isWaiting = false;
    }
}
