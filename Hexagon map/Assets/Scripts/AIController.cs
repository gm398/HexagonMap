using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{
    [SerializeField] GameObject mainBase;
    [SerializeField] int populationCap = 20;
    [SerializeField] int populationToAttackWith = 15;
    [SerializeField] int populationToRetreatWith = 5;
    [SerializeField] float commandsPerSecond = 1;
    float timer = 0;
    [SerializeField]
    int totalUnits = 0;//number of units
    [SerializeField]
    int ratioTotal = 0;

    [SerializeField] List<PriorityUnit> units;
    List<GameObject> activeUnits = new List<GameObject>();

    List<Hex> areaAroundBase = new List<Hex>();
    bool areaDefined = false;

    MainBaseController baseController;
    bool shouldAttack;


    HexGrid hexGrid;

    private void Awake()
    {
        mainBase = GameObject.FindGameObjectWithTag("EnemyBase");
        baseController = mainBase.GetComponentInChildren<MainBaseController>();
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
        
        foreach (PriorityUnit u in units) {
            ratioTotal += (int)u.idealRatio;
            totalUnits += (int)u.currentNumber;
        }
        if(ratioTotal < 1) { ratioTotal = 1; }
        CheckUnitRatioPriorities();
        Invoke("DefineAreaAroundBase", 1f);
    }
    // Update is called once per frame
    void Update()
    {
        SpawnUnit();
        if (timer >= 1 / commandsPerSecond)
        {
            DecideTactic();
            AssignTarget();
            timer = 0;
        }
        timer += Time.deltaTime;
    }

    void CheckUnitRatioPriorities()
    {
        foreach(PriorityUnit u in units)
        {
            if (totalUnits > 0)
            {
                u.currentPrio = (u.idealRatio / ratioTotal) - (u.currentNumber / totalUnits);
            }
            else { u.currentPrio = u.idealRatio / ratioTotal; }
            if(u.maxNum <= u.currentNumber && u.maxNum > 0) { u.currentPrio = 10; }//low priority if the maximum number has been reached
        }
        units.Sort((u1, u2) => u2.currentPrio.CompareTo(u1.currentPrio));
    }


    void SpawnUnit()
    {
        CheckUnitRatioPriorities();
        
        if (totalUnits < populationCap)
        {
            GameObject newUnit = baseController.SpawnUnit(units[0].unit);
            if (newUnit != null)
            {
                units[0].currentNumber++;
                units[0].activeUnits.Add(newUnit);
                activeUnits.Add(newUnit);
            }
        }
        totalUnits = 0;
        foreach (PriorityUnit u in units)
        {
            totalUnits += (int)u.currentNumber;
        }
    }

    void DecideTactic()
    {
        if(totalUnits >= populationToAttackWith && !shouldAttack) { shouldAttack = true; }
        else if(totalUnits <= populationToRetreatWith && shouldAttack){ shouldAttack = false; }
    }


    void AssignTarget()
    {
        //CheckActiveUnits();
        DefineAreaAroundBase();
        GameObject target;

        if (shouldAttack)
        {
            target = GameObject.FindGameObjectWithTag("MainBase");
            
        }
        else
        {
            target = areaAroundBase[Random.Range(0, areaAroundBase.Count - 1)].gameObject;
        }
        foreach (GameObject u in activeUnits)
        {
            UnitController controller = u.GetComponentInChildren<UnitController>();
            if (controller != null)
            {
                controller.SetTargetEnemy(target);
            }
        }
        
    }

    void DefineAreaAroundBase()
    {
        if (areaDefined){ return; }
        MainBaseController baseController = mainBase.GetComponentInChildren<MainBaseController>();
        Hex center = baseController.GetCenterHex();
        if(center == null) { return; }
        int innerRadius = baseController.GetSpawnRange();
        List<Hex> innerZone = hexGrid.GetHexesInRange(innerRadius + 1, center);
        List<Hex> outerZone = hexGrid.GetHexesInRange(innerRadius + 4, center);
        areaAroundBase.Clear();
        foreach(Hex h in outerZone)
        {
            if (!innerZone.Contains(h)) { areaAroundBase.Add(h); }
        }
        areaAroundBase.TrimExcess();
        areaDefined = true;
    }

    void CheckActiveUnits()
    {
        activeUnits.TrimExcess();
        List<GameObject> inactiveUnits = new List<GameObject>();
        foreach(GameObject u in activeUnits) {
            if (u == null)
            {
                inactiveUnits.Add(u);
            }
            else if (!u.activeInHierarchy)
            {
                inactiveUnits.Add(u);
            }
        } 
        foreach(GameObject u in inactiveUnits)
        {
            if (activeUnits.Contains(u))
            {
                activeUnits.Remove(u);
            }
        }
        activeUnits.TrimExcess();
    }


    public void RemoveUnit(GameObject unit)
    {
        if (activeUnits.Contains(unit))
        {
            activeUnits.Remove(unit);
            activeUnits.TrimExcess();
        }
        foreach(PriorityUnit u in units)
        {
            if (u.activeUnits.Contains(unit))
            {
                u.activeUnits.Remove(unit);
                u.currentNumber--;
            }
        }
    }

    [Serializable]
    private class PriorityUnit
    {
        public GameObject unit;
        [Header("ideal number of this unit compared to the other units")]
        public float idealRatio;
        [Header("negative for no limit")]
        public float maxNum = -1;
        public List<GameObject> activeUnits = new List<GameObject>();
        [HideInInspector]
        public float currentNumber;
        //[HideInInspector]
        public float currentPrio;
    }
    
}
