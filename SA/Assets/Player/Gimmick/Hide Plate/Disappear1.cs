using UnityEngine;

public class Disappear1 : MonoBehaviour
{
    public Disappear2 platformA;
    public Disappear2 platformB;

    void Start()
    {
        platformA.OnDisappear += () => platformB.AppearAndDisappear();
        platformB.OnDisappear += () => platformA.AppearAndDisappear();

        // 最初はAから
        platformA.AppearAndDisappear();
    }
}
