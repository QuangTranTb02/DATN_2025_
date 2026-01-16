using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZDepth : MonoBehaviour
{
    Transform t;
    [SerializeField] bool stationary = true;
    private void Start()
    {
        t = transform;
    }
    private void LateUpdate()
    {
        Vector3 Pos = t.position;
        Pos.z = Pos.y * 0.0001f;
        t.position = Pos;

        if (stationary)
        {
            Destroy(this);
        }
    }
}
