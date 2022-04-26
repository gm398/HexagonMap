using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexDisply : MonoBehaviour
{
    [SerializeField] Text display;
   
    public void SetText(Vector3 coords)
    {
        display.text = "R: " + coords.x
                + "\nQ: " + coords.y
                + "\nS: " + coords.z;
    }
}
