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
        if (FindTarget(mousePosition, out result))
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
        if (FindTarget(mousePosition, out result))
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
                    if (aStar.AStarSearch(hexGrid, currentHex, foundHex, out path, .5f, null, out visited))
                    {
                        
                        Dictionary<Hex, Hex>.ValueCollection valueCol = visited.Values;
                        foreach(Hex hex in valueCol)
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
                if (currentSelection != null)
                {
                    foreach (GameObject selected in currentSelection)
                    {
                        selected.GetComponentInParent<UnitController>().SetTarget(result.GetComponentInParent<Hex>());
                    }
                }
            }
            else { Debug.Log("not a hex"); }
        }
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