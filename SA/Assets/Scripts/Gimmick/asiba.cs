using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class asiba : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private float rayDistance;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 最初の位置情報を取得する。
        initialPos = transform.position;
        // 最初の角度を取得する。
        initialRot = transform.rotation;
        rayDistance = 1.0f;
    }
    void Update()
    {
        Vector3 rayPosition = transform.position + new Vector3(0.0f, 0.0f, 0.0f);
        Ray ray = new Ray(rayPosition, Vector3.up);
        Debug.DrawRay(rayPosition, Vector3.up * rayDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.name == "PlayerArmature")
            {
                Invoke("DropDown", 0.1f);
            }
        }
    }
    void DropDown()
    {
        GetComponent<Rigidbody>().isKinematic = false;
        Invoke("BlockReset", 5.0f);
    }
    // リセット（元の位置に戻す）
    private void BlockReset()
    {
        transform.position = initialPos;
        transform.rotation = initialRot;
        rb.isKinematic = true;
    }
}
