using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class kieru : MonoBehaviour
{
    private BoxCollider bc;
    private MeshRenderer ms;
    private int count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bc = this.GetComponent<BoxCollider>();
        ms = this.GetComponent<MeshRenderer>();
    }

    void Update()
    {
        if (count < 5)
        {
            count += 1;
        }
        if (count == 1)
        {
            Lost();
        }
    }
    void Lost()
    {
        bc.enabled = false;
        ms.enabled = false;
        Invoke("Appear", 1.5f);
    }
    void Appear()
    {
        bc.enabled = true;
        ms.enabled = true;
        Invoke("Lost", 4f);
    }
}
