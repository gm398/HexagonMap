using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] List<Hex> hexs;
    
    [SerializeField] Dictionary<Vector3, Hex> hexDictionary = new Dictionary<Vector3, Hex>();
    [SerializeField] GameObject[] mapPieces;
    
    [SerializeField] Vector3 offset;


    [SerializeField] Material mat1, mat2;

    PathFinding pathFinder = new PathFinding();
    // Start is called before the first frame update
    private void Awake()
    {
        GetHexesFromGameobject();
        AddHexsToDictionary();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public bool FindPath(Hex start, Hex goal, out List<Hex> rout, float heightStep)
    {
        if(start == null || goal == null || start == goal) { rout = null; return false; }
        Dictionary<Hex, Hex> visited;
        return pathFinder.AStarSearch(this, start, goal, out rout, heightStep, out visited);
        
    }

    public List<Hex> GetNeighbours(Hex startHex)
    {
        List<Hex> neighbours = new List<Hex>();
        Vector3 hexCoords = startHex.GetHexCoordinates().GetHexCoordsRQS();
        if(!hexDictionary.ContainsKey(hexCoords)) { Debug.Log("Hex not in the HexGrid"); return neighbours; }
        Vector3 offset = new Vector3(-1, 1, 0);
        
        if (GetHex(hexCoords + offset, out Hex hex))
        {
            neighbours.Add(hex);
        }
        offset = new Vector3(0, 1, -1);
        if (GetHex(hexCoords + offset, out hex))
        {
            neighbours.Add(hex);
        }
        offset = new Vector3(1, 0, -1);
        if (GetHex(hexCoords + offset, out hex))
        {
            neighbours.Add(hex);
        }
        offset = new Vector3(1, -1, 0);
        if (GetHex(hexCoords + offset, out hex))
        {
            neighbours.Add(hex);
        }
        offset = new Vector3(0, -1, 1);
        if (GetHex(hexCoords + offset, out hex))
        {
            neighbours.Add(hex);
        }
        offset = new Vector3(-1, 0, 1);
        if (GetHex(hexCoords + offset, out hex))
        {
            neighbours.Add(hex);
        }
        return neighbours;
    }

    public bool GetHex(Vector3 RQS, out Hex hex)
    {
        if (hexDictionary.ContainsKey(RQS))
        {
            hex = hexDictionary[RQS];
            return true;
        }
        hex = null;
        return false;
    }

    public void RevertHexs()
    {
        foreach(Hex hex in hexs)
        {
            hex.RevertMaterial();
        }
    }
    
    //adds each hex to the dictionary and moves them into the correct positions
    void AddHexsToDictionary()
    {
        foreach (Hex hex in hexs)
        {
            if (hex != null)
            {
                hex.GetHexCoordinates().MoveToGridCords();
                
                if (!hexDictionary.ContainsKey(hex.GetHexCoordinates().GetHexCoordsRQS()))
                {
                    hexDictionary.Add(hex.GetHexCoordinates().GetHexCoordsRQS(), hex);
                }
                else
                {
                    //logs any hexs that are in the same spot, not counting height
                    Vector3 hexLog = hex.GetHexCoordinates().GetHexCoordsRQS();
                    Debug.Log("hexDictonary already contains hex RQS: "
                        + hexLog.x + " "
                        + hexLog.y + " "
                        + hexLog.z + ". "
                        + "RealWorld Coords XYZ: "
                        + hex.transform.position.x + " "
                        + hex.transform.position.y + " "
                        + hex.transform.position.z + ". ");
                    hex.gameObject.SetActive(false);
                }
            }
        }
    }


    //takes hexs out of a parent, usually a map prefab and adds them to the hex list
    void GetHexesFromGameobject()
    {
        foreach (GameObject piece in mapPieces)
        {
            if (piece != null)
            {
                Transform[] parts = piece.GetComponentsInChildren<Transform>();
                foreach (Transform part in parts)
                {
                    if (part.GetComponent<Hex>() != null)
                    {
                        hexs.Add(part.GetComponent<Hex>());
                        part.SetParent(this.transform);
                    }
                }
            }
        }
    }

    public Dictionary<Vector3, Hex> GetHexDictionary()
    {
        return hexDictionary;
    }

}
   
