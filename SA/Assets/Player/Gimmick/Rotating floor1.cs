using UnityEngine;

public class RotatingPlatform2 : MonoBehaviour
{
    public float speed = 50f;
    private float internalAngle = 0f; // 自前で管理する角度
    private bool isWaiting = false;
    private float nextStopAngle = 180f; // 次に止まりたい角度（180刻み）

    void Start()
    {
        // 初期角度を読み取り internalAngle にセット
        internalAngle = transform.eulerAngles.y;
        
        // 一番近い180刻みの角度をセット
        nextStopAngle = Mathf.Round(internalAngle / 180f) * 180f + 180f;
    }

    void Update()
    {
        if (isWaiting) return;

        // 内部角度を増やす（常に一方向）
        internalAngle = Mathf.MoveTowards(internalAngle, nextStopAngle, speed * Time.deltaTime);

        // 実際の回転に反映（360でループ）
        float displayAngle = internalAngle % 360f;
        transform.rotation = Quaternion.Euler(0, displayAngle, 0);

        // 180°ごとに停止
        if (Mathf.Abs(internalAngle - nextStopAngle) < 0.1f)
        {
            StartCoroutine(StopAndContinue());
        }
    }

    System.Collections.IEnumerator StopAndContinue()
    {
        isWaiting = true;
        yield return new WaitForSeconds(0.5f);

        // 次の停止ポイントを +180° するだけ！
        nextStopAngle += 180f;

        isWaiting = false;
    }
}
