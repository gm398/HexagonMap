using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBaseController : MonoBehaviour
{
    [SerializeField] bool playerUnit;
    HexCoordinates hexCoords;
    HexGrid hexGrid;
    List<Hex> occupiedHexs = new List<Hex>();
    List<Hex> spawnZone = new List<Hex>();
    Hex centerHex;
    VisionController visionController;

    [SerializeField] List<GameObject> selectedParts;
    [SerializeField] int spawnRange = 2;

    [SerializeField] float resources = 0;
    [SerializeField] float maxResources = 500;
    [SerializeField] float resourcesPerSecond = 5f;
    [SerializeField] Text resourceText;
    [SerializeField] GameObject resourceDisplay;
    GameObject currentUnitToSpawn;

    [SerializeField] List<UnitMenuItem> units;
    [SerializeField] GameObject display;
    [SerializeField] GameObject canvas;

   


    private void Awake()
    {
        if (this.GetComponent<HexCoordinates>() != null) { hexCoords = this.GetComponent<HexCoordinates>(); }
        else { hexCoords = this.gameObject.AddComponent<HexCoordinates>(); }
        hexCoords.MoveToGridCords();
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        visionController = this.GetComponent<VisionController>();
            
        if (!playerUnit) { visionController.SetVisible(false); resourceDisplay.SetActive(false); }
        else { Invoke("UpdateVision", 1f); }//updates vision once everything has been loaded
        float displayOffset = 850;
        Canvas c = canvas.GetComponentInChildren<Canvas>();
        foreach (UnitMenuItem u in units)
        {
            GameObject b = Instantiate(display);
            b.transform.SetParent(c.transform, false);
            b.transform.position = new Vector3(80, displayOffset, 0);
            u.InitializeButton(b);

            displayOffset -= 100;
        }
        SetSelected(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (centerHex == null)
        {
            CheckForCenterHex();
        }

        
        AddResources(resourcesPerSecond * Time.deltaTime);
        
    }


    public GameObject SpawnUnit(GameObject unit)
    {
        if (unit != null)
        {
            currentUnitToSpawn = unit;
            return SpawnUnit();
        }
        else { return null; }
    }
    [ContextMenu("testSpawn")]
    public GameObject SpawnUnit()
    {
        UnitController unitController = currentUnitToSpawn.GetComponentInChildren<UnitController>();
        if(unitController == null) { return null; }
        float cost = unitController.GetResourceCost();
        if (resources > cost)
        {
            AddResources(-cost);

            Hex temp = null;
            foreach (Hex h in spawnZone)
            {
                if (!h.IsOccupied())
                {
                    temp = h;
                }
            }
            if (temp != null)
            {
                return SpawnUnit(temp);
            }
        }
        return null;
    }

    public GameObject SpawnUnit(Hex target)
    {
        return Instantiate(currentUnitToSpawn, target.GetTargetPoint(), Quaternion.identity);
    }
    
    public void AddResources(float resources)
    {
        this.resources += resources;
        if(this.resources > maxResources)
        {
            this.resources = maxResources;
        }
        if(this.resources < 0)
        {
            this.resources = 0;
        }
        resourceText.text = "Resources: " + this.resources.ToString("F0");
    }

    /*
    public void SpawnNormalUnit() {
        currentUnitToSpawn = normalUnit;
        
        SpawnUnit();
    }
    public void SpawnTankUnit() { currentUnitToSpawn = tankUnit; SpawnUnit(); }
    public void SpawnFlyingUnit() { currentUnitToSpawn = flyingUnit; SpawnUnit(); }
    public void SpawnHealerUnit() { currentUnitToSpawn = healerUnit; SpawnUnit(); }
    */
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
    

    public void SetSelected(bool isSelected)
    {
       
        foreach(GameObject g in selectedParts)
        {
            g.SetActive(isSelected);
        }
        
    }
    public void Dead()
    {
        foreach(Hex h in occupiedHexs)
        {
            h.SetOccupent(null);
        }
        if (playerUnit) { visionController.RemoveVision(); }
        GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<SceneManagement>().EndGame();
        this.gameObject.SetActive(false);
    }
    void UpdateVision()
    {
        visionController.UpdateVision(centerHex);
    }

    public bool IsPlayerUnit() { return playerUnit; }
    public Hex GetCenterHex() { return centerHex; }
    public int GetSpawnRange() { return spawnRange; }


    [Serializable]
    private class UnitMenuItem
    {
        public GameObject unit;
        GameObject button;

        public void InitializeButton(GameObject button)
        {
            this.button = button;
            Text t = button.GetComponentInChildren<Text>();
            Button b = button.GetComponentInChildren<Button>();
            t.text = unit.name + " " + unit.GetComponentInChildren<UnitController>().GetResourceCost();
            MainBaseController mbc = GameObject.FindGameObjectWithTag("MainBase").GetComponent<MainBaseController>();
            b.onClick.AddListener(() => mbc.SpawnUnit(unit));
        }

    }
}
