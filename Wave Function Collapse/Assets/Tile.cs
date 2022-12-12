using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] string name = "Default Name";
    [SerializeField] Tile[] canTouch;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool CanTouch(Tile tile)
    {
        bool can = false;
        foreach (Tile t in canTouch)
        {
            if(t == tile)
            { can = true; }
        }
        return can;
    }
}
