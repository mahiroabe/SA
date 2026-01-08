using UnityEngine;
using System.Collections;

public class HidePlate : MonoBehaviour
{
    public enum Mode
    {
        Invisible,  // 見えないが当たり判定あり
        Disappear   // 見えない + 当たり判定なし
    }

    [Header("Mode")]
    public Mode mode = Mode.Invisible;

    [Header("Initial State")]
    public bool startVisible = true;

    [Header("Timing")]
    public float visibleTime = 2f;
    public float hiddenTime = 2f;

    private Renderer[] renderers;
    private Collider[] colliders;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();

        // 初期状態を反映
        SetVisible(startVisible);

        StartCoroutine(VisibilityLoop());
    }

    IEnumerator VisibilityLoop()
    {
        // 最初の状態に応じて待ち時間を切り替え
        yield return new WaitForSeconds(startVisible ? visibleTime : hiddenTime);

        while (true)
        {
            SetVisible(!IsVisible());
            yield return new WaitForSeconds(IsVisible() ? visibleTime : hiddenTime);
        }
    }

    void SetVisible(bool visible)
    {
        foreach (var r in renderers)
        {
            r.enabled = visible;
        }

        if (mode == Mode.Disappear)
        {
            foreach (var c in colliders)
            {
                c.enabled = visible;
            }
        }
    }

    bool IsVisible()
    {
        if (renderers.Length == 0) return false;
        return renderers[0].enabled;
    }
}