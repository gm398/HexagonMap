using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
   [SerializeField] Transform cam;

    private void Awake()
    {
        if (cam == null)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward);
    }
}
