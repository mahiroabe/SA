using UnityEngine;

public class InvisibleWall : MonoBehaviour
{
    private Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        SetVisible(false);
    }

    void SetVisible(bool visible)
    {
        foreach (var r in renderers)
        {
            r.enabled = visible;
        }
    }
}
