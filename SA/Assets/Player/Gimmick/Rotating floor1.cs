using UnityEngine;

public class RotatingPlatform2 : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 50f, 0);

    private float accumulatedRotation = 0f; // 回転量の蓄積
    private bool isWaiting = false;

    void Update()
    {
        if (isWaiting) return;

        // フレームごとの回転量（角度）
        float deltaRotation = rotationSpeed.y * Time.deltaTime;

        // 実際に回転させる
        transform.Rotate(0, deltaRotation, 0);

        // 回転した角度を蓄積
        accumulatedRotation += Mathf.Abs(deltaRotation);

        // 180度（半回転）超えたらストップして待機
        if (accumulatedRotation >= 180f)
        {
            StartCoroutine(WaitAndReset());
        }
    }

    System.Collections.IEnumerator WaitAndReset()
    {
        isWaiting = true;
        accumulatedRotation = 0f; // カウントリセット
        yield return new WaitForSeconds(1f); // 1秒停止
        isWaiting = false;
    }
}
