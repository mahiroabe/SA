using UnityEngine;
using System.Collections;
using System;

public class Disappear2 : MonoBehaviour
{
    public float visibleTime = 2f;
    public float blinkDuration = 1f;
    public float blinkInterval = 0.15f;

    public event Action OnDisappear; // ← 消えた通知

    private Renderer[] renderers;
    private Collider[] colliders;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void AppearAndDisappear()
    {
        StopAllCoroutines();
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        SetVisible(true);
        yield return new WaitForSeconds(visibleTime);

        yield return StartCoroutine(Blink());

        SetVisible(false);
        OnDisappear?.Invoke(); // ← 親に通知
    }

    IEnumerator Blink()
    {
        float t = 0f;
        bool v = true;

        while (t < blinkDuration)
        {
            v = !v;
            SetRenderer(v);
            yield return new WaitForSeconds(blinkInterval);
            t += blinkInterval;
        }

        SetRenderer(true);
    }

    void SetVisible(bool v)
    {
        SetRenderer(v);
        foreach (var c in colliders) c.enabled = v;
    }

    void SetRenderer(bool v)
    {
        foreach (var r in renderers) r.enabled = v;
    }
}
