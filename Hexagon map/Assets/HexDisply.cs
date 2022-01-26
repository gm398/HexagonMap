using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexDisply : MonoBehaviour
{
    [SerializeField] Text display;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetText(Vector3 coords)
    {
        display.text = "R: " + coords.x
                + "\nQ: " + coords.y
                + "\nS: " + coords.z;
    }
}
