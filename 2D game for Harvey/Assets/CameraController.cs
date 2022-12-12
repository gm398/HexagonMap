using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float lerpSpeed = 10f;

    private void Awake()
    {
        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 currentPos = this.transform.position;
        Vector3 newPos = target.position;
        newPos.z = currentPos.z;
        this.transform.position = Vector3.Lerp(currentPos, newPos, lerpSpeed * Time.deltaTime);
       
    }
}
