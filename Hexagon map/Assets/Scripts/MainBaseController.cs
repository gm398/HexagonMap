using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBaseController : MonoBehaviour
{
    [SerializeField] bool playerUnit;
    HexCoordinates hexCoords;
    HexGrid hexGrid;
    List<Hex> occupiedHexs = new List<Hex>();
    List<Hex> spawnZone = new List<Hex>();
    Hex centerHex;
    VisionController visionController;

    [SerializeField] List<GameObject> spawnableUnits;
    [SerializeField] int spawnRange = 2;
    

    private void Awake()
    {
        if (this.GetComponent<HexCoordinates>() != null) { hexCoords = this.GetComponent<HexCoordinates>(); }
        else { hexCoords = this.gameObject.AddComponent<HexCoordinates>(); }
        hexCoords.MoveToGridCords();
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        visionController = this.GetComponent<VisionController>();
            
        if (!playerUnit) { visionController.SetVisible(false); }
        else { Invoke("UpdateVision", 1f); }//updates vision once everything has been loaded
    }

    // Update is called once per frame
    void Update()
    {
        if (centerHex == null)
        {
            CheckForCenterHex();
        }
    }

    [ContextMenu("testSpawn")]
    public void SpawnUnit()
    {
        Hex temp = null;
        foreach(Hex h in spawnZone)
        {
            if (!h.IsOccupied())
            {
                temp = h;
            }
        }
        if(temp != null)
        {
            SpawnUnit(spawnableUnits[0], temp);
        }
    }

    public void SpawnUnit(GameObject unit, Hex target)
    {
        Instantiate(unit, target.GetTargetPoint(), Quaternion.identity);
    }

    void CheckForCenterHex()
    {
        Hex temp;
        if (hexGrid.GetHex(hexCoords.GetHexCoordsRQS(), out temp))
        {
            centerHex = temp;
            centerHex.SetOccupent(this.gameObject);
            occupiedHexs.Add(centerHex);
            List<Hex> surrounding = hexGrid.GetNeighbours(centerHex);
            List<Hex> spawnArea = hexGrid.GetHexesInRange(spawnRange + 1, centerHex);
            foreach (Hex h in surrounding)
            {
                h.SetOccupent(this.gameObject);
                occupiedHexs.Add(h);
            }
            foreach(Hex h in spawnArea)
            {
                if (!surrounding.Contains(h) && h.IsTraversable())
                {
                    spawnZone.Add(h);
                }
            }
        }
    }
    
    public void Dead()
    {
        foreach(Hex h in occupiedHexs)
        {
            h.SetOccupent(null);
        }
        if (playerUnit) { visionController.RemoveVision(); }
        this.gameObject.SetActive(false);
    }
    void UpdateVision()
    {
        visionController.UpdateVision(centerHex);
    }
}
