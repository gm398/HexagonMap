using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// tutorial
//https://www.youtube.com/watch?v=WGo07dMJPtk

public class PlayerInput : MonoBehaviour
{

    [SerializeField]
    UnityEvent<Vector3> PointerClick;
    [SerializeField]
    UnityEvent<Vector3> RightClick;
    // Update is called once per frame
    void Update()
    {
        //DetectMouseClick();
    }

    void DetectMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            PointerClick?.Invoke(mousePos);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Input.mousePosition;
            RightClick?.Invoke(mousePos);
        }
    }
}
