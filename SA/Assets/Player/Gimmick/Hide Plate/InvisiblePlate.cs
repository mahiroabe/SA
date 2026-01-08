using UnityEngine;

public class InvisiblePlate : MonoBehaviour
{
    [Header("Player Settings")]
    public string playerTag = "Player";

    private Renderer[] renderers;
    private int playerCount = 0;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
        SetVisible(false); // 最初は隠す
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            playerCount++;
            SetVisible(true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
            playerCount--;
            if (playerCount <= 0)
            {
                SetVisible(false);
            }
        }
    }

    void SetVisible(bool visible)
    {
        foreach (var r in renderers)
        {
            r.enabled = visible;
        }
    }
}
