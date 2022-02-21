using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Hex : MonoBehaviour
{

    [SerializeField] HexCoordinates hexCoords;

    //if the hex can be traversed
    [SerializeField] bool traversable = true;

    [SerializeField] float moveDificulty = 1;
    [SerializeField] Transform targetPoint;

    private bool isOcupied = false;
    private GameObject occupant;
    private Material origionalMaterial;
    private Material newMaterial;

    private void Awake()
    {
        origionalMaterial = this.GetComponentInChildren<MeshRenderer>().material;
        if(this.GetComponent<HexCoordinates>() != null) { hexCoords = this.GetComponent<HexCoordinates>(); }
    }

    // Update is called once per frame
    void Update()
    {
        hexCoords.ConvertToHexCords();
    }


    //finds the distance between this hex and another given hex
    public float DistanceFromHex(Hex hex)
    {
        Vector3 b = hex.GetHexCoordinates().GetHexCoordsRQS();
        Vector3 a = hexCoords.GetHexCoordsRQS();

        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
    }
   

    //changes the material of the hex
    public void SetNewMaterial(Material mat)
    {
        newMaterial = mat;
        this.GetComponentInChildren<MeshRenderer>().material = mat;
    }
    //reverts the material to what it started with
    public void RevertMaterial()
    {
        this.GetComponentInChildren<MeshRenderer>().material = origionalMaterial;
    }
    
    public bool IsTraversable() { return traversable; }
    public HexCoordinates GetHexCoordinates() { return hexCoords; }
    public float GetMoveDificulty() { return moveDificulty; }
    public Vector3 GetTargetPoint() { return targetPoint.position; }
    public bool IsOccupied() { return isOcupied; }
    public void SetOccupied(bool occupied) { isOcupied = occupied; }
    public GameObject GetOccupant() { return occupant; }
    public void SetOccupent(GameObject newOccupent) { occupant = newOccupent; }

}
