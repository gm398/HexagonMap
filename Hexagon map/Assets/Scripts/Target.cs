using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] HexCoordinates hexCoords;
    private void Awake()
    {
        hexCoords.MoveToGridCords();
    }
    private void Update()
    {
        hexCoords.MoveToGridCords();
    }
}
