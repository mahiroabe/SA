using UnityEngine;

public class RotatingPlatform2 : MonoBehaviour
{
    public float speed = 50f;
    private bool isWaiting = false;
    private bool goingTo180 = true; // true = 0→180, false = 180→360(=0)

    void Update()
    {
        if (isWaiting) return;

        float targetAngle = goingTo180 ? 180f : 360f; // 次に止まりたい角度

        // 現在のY角度（0〜360補正）
        float currentY = NormalizeAngle(transform.eulerAngles.y);

        // 次の目標角度に向かって回す
        float newY = Mathf.MoveTowardsAngle(currentY, targetAngle, speed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, newY, 0);

        // 目標角度に到達したら停止
        if (Mathf.Abs(NormalizeAngle(newY) - targetAngle) < 0.1f)
        {
            StartCoroutine(StopAtAngle());
        }
    }

    System.Collections.IEnumerator StopAtAngle()
    {
        isWaiting = true;
        yield return new WaitForSeconds(0.5f);

        // 次は180→0 or 0→180 の反転
        goingTo180 = !goingTo180;

        // 360 に達したら 0 に補正してズレ防止
        float y = NormalizeAngle(transform.eulerAngles.y);
        transform.rotation = Quaternion.Euler(0, y, 0);

        isWaiting = false;
    }

    float NormalizeAngle(float angle)
    {
        // 0〜360 の範囲に調整
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }
}
