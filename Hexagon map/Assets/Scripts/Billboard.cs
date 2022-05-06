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
        if (!cam.gameObject.activeInHierarchy)
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }
        transform.LookAt(transform.position + cam.forward);
    }
}
