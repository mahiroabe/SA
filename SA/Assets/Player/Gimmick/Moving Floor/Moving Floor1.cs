using UnityEngine;

public class MovingPlatform2 : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 2f;

    private bool goingToB = true;
    private bool isWaiting = false;

    void Update()
    {
        // 待機中なら移動しない
        if (isWaiting) return;

        Vector3 target = goingToB ? pointB : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            // 方向を反転
            goingToB = !goingToB;

            // 1秒待機コルーチン開始
            StartCoroutine(WaitBeforeMove());
        }
    }

    System.Collections.IEnumerator WaitBeforeMove()
    {
        isWaiting = true;
        yield return new WaitForSeconds(1f);  // ← 1秒待つ
        isWaiting = false;
    }
}
