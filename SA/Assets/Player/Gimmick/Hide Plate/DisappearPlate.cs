using UnityEngine;
using System.Collections;

public class DisappearPlate : MonoBehaviour
{
    [Header("Initial State")]
    public bool startVisible = true;     // 最初に表示するかどうか

    [Header("Timing")]
    public float visibleTime = 2f;       // 表示時間
    public float blinkDuration = 0.8f;     // 点滅する時間
    public float blinkInterval = 0.2f;  // 点滅間隔
    public float hiddenTime = 2.8f;        // 消えている時間

    private Renderer[] renderers;
    private Collider[] colliders;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        // 初期状態を反映
        SetVisible(startVisible);

        StartCoroutine(DisappearLoop());
    }

    IEnumerator DisappearLoop()
    {
        // 初期状態に応じて最初の待ち時間を変える
        yield return new WaitForSeconds(startVisible ? visibleTime : hiddenTime);

        while (true)
        {
            // 表示
            SetVisible(true);
            yield return new WaitForSeconds(visibleTime);

            // 点滅
            yield return StartCoroutine(Blink());

            // 完全に消える
            SetVisible(false);
            yield return new WaitForSeconds(hiddenTime);
        }
    }

    IEnumerator Blink()
    {
        float timer = 0f;
        bool visible = true;

        while (timer < blinkDuration)
        {
            visible = !visible;
            SetRenderer(visible);

            timer += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        // 点滅終了時は表示状態に戻す
        SetRenderer(true);
    }

    void SetVisible(bool visible)
    {
        SetRenderer(visible);

        foreach (var c in colliders)
        {
            c.enabled = visible;
        }
    }

    void SetRenderer(bool visible)
    {
        foreach (var r in renderers)
        {
            r.enabled = visible;
        }
    }
}