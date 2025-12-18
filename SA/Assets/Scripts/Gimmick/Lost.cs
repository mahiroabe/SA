using UnityEngine;

public class Lost : MonoBehaviour
{
    private MeshRenderer ms;
    private int count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ms = this.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (count < 5)
        {
            count += 1;
        }
        if (count == 1)
        {
            Miss();
        }
    }
    void Miss()
    {
        ms.enabled = false;
        Invoke("Visible", 3f);
    }
    void Visible()
    {
        ms.enabled = true;
        Invoke("Miss", 0.5f);
    }
}
