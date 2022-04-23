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

    private bool isOccupied = false;
    [SerializeField] bool isVisible = false;
    int seenBy = 0;
    [SerializeField] GameObject occupant = null;

    [SerializeField] GameObject visuals;
    [SerializeField] Material defaultMaterial;

    [Tooltip("material displayed when the hex is out of vision")]
    [SerializeField] Material hiddenMaterial;

    [SerializeField] Material highlightedMaterial;

    private void Awake()
    {
        if (defaultMaterial == null)
        {
            defaultMaterial = visuals.GetComponentInChildren<MeshRenderer>().material;
        }
        SetVisible(isVisible);
        if(this.GetComponent<HexCoordinates>() != null) { hexCoords = this.GetComponent<HexCoordinates>(); }
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
        hiddenMaterial = mat;
        this.GetComponentInChildren<MeshRenderer>().material = mat;
    }
    //reverts the material to what it started with
    public void RevertMaterial()
    {
        visuals.GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
    }

    public void HighlightHex(bool highlight)
    {
        if (highlight)
        {
            visuals.GetComponentInChildren<MeshRenderer>().material = highlightedMaterial;
        }
        else
        {
            if (isVisible)
            {
                visuals.GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
            }
            else { visuals.GetComponentInChildren<MeshRenderer>().material = hiddenMaterial; }
        }
    }
    
    public bool IsTraversable() { return traversable; }
    public HexCoordinates GetHexCoordinates() { return hexCoords; }
    public float GetMoveDificulty() { return moveDificulty; }
    public Vector3 GetTargetPoint() { return targetPoint.position; }
    public bool IsOccupied() { return isOccupied; }
    public void SetOccupied(bool occupied) { isOccupied = occupied; }
    public GameObject GetOccupant() { return occupant; }
    public void SetOccupent(GameObject newOccupent)
    {
        occupant = newOccupent;
        if (occupant == null)
        {
            isOccupied = false;
        }
        else {
            isOccupied = true;
        }
    }

    public void SetVisible(bool visible)
    {
        if (visible)
        {
            seenBy++;
        }
        else
        {
            seenBy--;
        }
        if (seenBy < 1)
        {
            isVisible = false;
            seenBy = 0;
            visuals.GetComponentInChildren<MeshRenderer>().material = hiddenMaterial;
            if (occupant != null)
            {
                occupant.SendMessage("SetVisible", false, SendMessageOptions.DontRequireReceiver);
            }
        }
        else if (!isVisible)
        {
            isVisible = true;
            visuals.GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
            if(occupant != null)
            {
                occupant.SendMessage("SetVisible", true, SendMessageOptions.DontRequireReceiver);
            }
        }
    }
    public bool IsVisible() { return isVisible; }
}
