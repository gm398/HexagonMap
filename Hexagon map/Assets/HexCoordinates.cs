using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoordinates : MonoBehaviour
{
    
    [SerializeField] Vector3 hexCoordinatesRQS;
    [SerializeField] Vector3 worldCoords;

   
    //converts the transforms coords into the hexagon coords
    public void ConvertToHexCords()
    {
        float x = transform.position.x;
        float z = transform.position.z;

        hexCoordinatesRQS.x = Mathf.RoundToInt(-z / .866f);
        //s = Mathf.RoundToInt((-r - x / .5f) / 2);
        hexCoordinatesRQS.y = Mathf.RoundToInt((-hexCoordinatesRQS.x + x / .5f) / 2);
        hexCoordinatesRQS.z = -hexCoordinatesRQS.y - hexCoordinatesRQS.x;
        if (this.GetComponent<HexDisply>() != null) {
            this.GetComponent<HexDisply>().SetText(hexCoordinatesRQS);
        }
        
    }
    public Vector3 ConvertToGridCords()
    {
        worldCoords.z = hexCoordinatesRQS.x * -.866f;
        worldCoords.x = (2 * hexCoordinatesRQS.y + hexCoordinatesRQS.x) * .5f;
        worldCoords.y = this.transform.position.y;
        return worldCoords;
    }
    public void MoveToGridCords()
    {
        ConvertToHexCords();
        ConvertToGridCords();
        transform.position = worldCoords;
    }



    public Vector3 GetHexCoordsRQS() { return hexCoordinatesRQS; }
    public float GetHeight() { return worldCoords.y; }
    
}
