using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// tutorial
//https://www.youtube.com/watch?v=WGo07dMJPtk

public class SelectionManager : MonoBehaviour
{

    [SerializeField] Camera mainCamers;
    [SerializeField] LayerMask selectionMask;
    [SerializeField] HexGrid hexGrid;

    [SerializeField] Material selectionMaterial;
    [SerializeField] Material testMat;

    private Hex currentHex;
    private List<GameObject> currentSelection;

    private void Awake()
    {
        if(mainCamers == null)
        {
            mainCamers = Camera.main;
        }
        currentSelection = new List<GameObject>();
    }


    public void HandleClick(Vector3 mousePosition)
    {
        GameObject result;
        if (FindTarget(mousePosition, out result) && !Input.GetKey(KeyCode.Mouse3))
        {
            hexGrid.RevertHexs();//resets all the hexes click states
            currentHex = null;
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                currentSelection.Clear();
            }

            if (result.GetComponentInParent<Hex>() != null)
            {
                
                Hex selectedHex = result.GetComponentInParent<Hex>();
                Debug.Log("Hex RQS:" + selectedHex.GetHexCoordinates().GetHexCoordsRQS() + " .");

                currentHex = selectedHex;//currently selected hex
            }
            else if(result.GetComponentInParent<UnitController>() != null)
            {
                if (result.GetComponentInParent<UnitController>().isPlayerUnit())
                {
                    currentSelection.Add(result);
                }
            }
            else { Debug.Log("not a valid selection"); }
        }
    }
    public void HandleRightClick(Vector3 mousePosition)
    {
        GameObject result;
        if (FindTarget(mousePosition, out result) && !Input.GetKey(KeyCode.Mouse3))
        {
            //DisplayPath(result);
            if (currentSelection != null)
                {
                UnitController enemyController = result.GetComponentInParent<UnitController>();
                Hex targetHex = result.GetComponentInParent<Hex>();
                if(targetHex == null) { hexGrid.GetHex(result.GetComponentInParent<HexCoordinates>().GetHexCoordsRQS(), out targetHex); }
                foreach (GameObject selected in currentSelection)
                {
                    if (selected != null)
                    {
                       
                        UnitController controller = selected.GetComponentInParent<UnitController>();
                        if (enemyController != null)
                        {
                            controller.SetTargetEnemy(result);
                        }
                        else if (targetHex != null)
                        {
                            int targetLayer = -1;
                            GameObject enemy = targetHex.GetOccupant();
                            if (enemy != null) { targetLayer = enemy.layer; }
                            if (controller.IsEnemy(targetLayer))
                            {
                                controller.SetTargetEnemy(result);
                            }
                            else
                            {
                                controller.SetTargetHex(targetHex);
                            }
                        }
                        
                        
                    /*
                        Hex targetHex;
                        hexGrid.GetHex(result.GetComponentInParent<HexCoordinates>().GetHexCoordsRQS(), out targetHex);
                        GameObject target = targetHex.GetOccupant();
                        if (target != null)
                        {
                            if (selected.GetComponentInParent<UnitController>().IsEnemy(target.layer))
                            {
                                selected.GetComponentInParent<UnitController>().SetTargetEnemy(target);
                            }
                        }
                        else
                        {
                            selected.GetComponentInParent<UnitController>().SetTargetHex(targetHex);
                        }*/
                    }
                }
            }
            
        }
    }

    //used to display how the pathfinding is workingon a hexgrid
    private void DisplayPath(GameObject result)
    {
        if (result.GetComponentInParent<Hex>() != null)
        {
            Hex foundHex = result.GetComponentInParent<Hex>();
            if (currentHex != null)
            {
                hexGrid.RevertHexs();
                currentHex.SetNewMaterial(selectionMaterial);
                Debug.Log("Distance = " + currentHex.DistanceFromHex(foundHex) + ".");
                //foundHex.SetNewMaterial(selectionMaterial);
                PathFinding aStar = new PathFinding();
                List<Hex> path;
                Dictionary<Hex, Hex> visited;
                if (aStar.AStarSearch(hexGrid, currentHex, foundHex, out path, .5f, null, out visited, -1, 0))
                {

                    Dictionary<Hex, Hex>.ValueCollection valueCol = visited.Values;
                    foreach (Hex hex in valueCol)
                    {
                        hex.SetNewMaterial(testMat);
                    }
                    foreach (Hex hex in path)
                    {
                        currentHex.SetNewMaterial(selectionMaterial);
                        hex.SetNewMaterial(selectionMaterial);
                    }

                }
            }
        }
        else { Debug.Log("not a hex"); }
    }
    private bool FindTarget(Vector3 mousePosition, out GameObject result)
    {
        RaycastHit hit;Ray ray = mainCamers.ScreenPointToRay(mousePosition);
        if(Physics.Raycast(ray, out hit, selectionMask))
        {
            result = hit.collider.gameObject;
            return true;
        }
        result = null;
        return false;
    }
}
