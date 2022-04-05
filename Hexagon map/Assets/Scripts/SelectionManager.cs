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
    bool isMouseDown = false;
    [SerializeField] float mouseHoldActivationTime = .02f;
    float mouseHeldTimer = 0;
    Vector3 mouseStart;
    Vector3 mouseEnd;
    [SerializeField] GameObject selectionCube;
    [SerializeField] GameObject camHolder;

    private void Awake()
    {
        if(mainCamers == null)
        {
            mainCamers = Camera.main;
        }
        currentSelection = new List<GameObject>();
        selectionCube.SetActive(false);
    }

    private void Update()
    {
        DetectMouseClick();
    }
    void DetectMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(Input.mousePosition);
            mouseHeldTimer = 0f;

            Vector3 f;
            if (MousePointingTo(out f))
            {
                mouseStart = f;
            }
            isMouseDown = false;
        }
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            mouseHeldTimer += Time.deltaTime;
            if(mouseHeldTimer >= mouseHoldActivationTime && !isMouseDown)
            {
                isMouseDown = true;
                selectionCube.SetActive(true);
            }
            if (isMouseDown)
            {
                Vector3 f;
                if (MousePointingTo(out f))
                {
                    mouseEnd = f;
                }
                MoveSelectinBox();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isMouseDown)
            {
                foreach (GameObject g in GetUnitsFromBox())
                {
                    currentSelection.Add(g);
                    UnitController uc = g.GetComponentInParent<UnitController>();
                    if (uc != null)
                    {
                        uc.SetSelected(true);
                    }
                }
            }
            selectionCube.SetActive(false);
            isMouseDown = false;
            mouseHeldTimer = 0f;
            mouseStart = new Vector3(0, 0, -100);
            mouseEnd = new Vector3(0, 0, -100);
        }
    }

    public void HandleClick(Vector3 mousePosition)
    {
        GameObject result;
        if (FindTarget(mousePosition, out result) && !Input.GetKey(KeyCode.Mouse3))
        {
            //hexGrid.RevertHexs();//resets all the hexes click states
            currentHex = null;
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                foreach(GameObject s in currentSelection)
                {
                    UnitController uc = s.GetComponentInParent<UnitController>();
                    if(uc != null) { uc.SetSelected(false); }
                }
                currentSelection.Clear();
            }

            UnitController unitController = result.GetComponentInParent<UnitController>();
            Hex targetHex = result.GetComponentInParent<Hex>();
            if (targetHex != null)
            {
                
                Hex selectedHex = targetHex;
                //Debug.Log("Hex RQS:" + selectedHex.GetHexCoordinates().GetHexCoordsRQS() + " .");

                currentHex = selectedHex;//currently selected hex
            }
            else if(unitController != null)
            {
                if (unitController.isPlayerUnit())
                {
                    currentSelection.Add(result);
                    unitController.SetSelected(true);
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
            if (currentSelection != null && result != null)
                {
                UnitController enemyController = result.GetComponentInParent<UnitController>();
                Hex targetHex = result.GetComponentInParent<Hex>();
                HexCoordinates coords = result.GetComponentInParent<HexCoordinates>();
                if(coords == null && targetHex == null && enemyController == null) { return; }

                if (targetHex == null) { hexGrid.GetHex(coords.GetHexCoordsRQS(), out targetHex); }
                foreach (GameObject selected in currentSelection)
                {
                    if (selected != null)
                    {
                        if (selected.activeInHierarchy)
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
                                    controller.SetTargetEnemy(enemy);
                                }
                                else

                                {
                                    controller.SetTargetHex(targetHex);
                                }
                            }
                        }
                    }
                }
            }
            
        }
    }
    
    private void MoveSelectinBox()
    {
        Vector3 newPosition = (mouseStart + mouseEnd)/2;
        selectionCube.transform.position = newPosition;
       
        Vector3 scale = new Vector3(Mathf.Abs(mouseStart.x - mouseEnd.x), 3, Mathf.Abs(mouseStart.z - mouseEnd.z));
        
        selectionCube.transform.localScale =  scale;//new Vector3(x, 3, z);
        //selectionCube.transform.rotation = camHolder.transform.rotation;
        //selectionCube.transform.rotation = cam.transform.rotation;
        Physics.SyncTransforms();
    }
    private List<GameObject> GetUnitsFromBox()
    {
        //selectionCube
        BoxSelector boxSelector = selectionCube.GetComponent<BoxSelector>();
        List<GameObject> found = boxSelector.CheckForUnits(1<<8, Quaternion.identity);
        
        return found;
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
        RaycastHit hit;
        Ray ray = mainCamers.ScreenPointToRay(mousePosition);
        if(Physics.Raycast(ray, out hit, selectionMask))
        {
            result = hit.collider.gameObject;
            return true;
        }
        result = null;
        return false;
    }
    private bool MousePointingTo(out Vector3 mousePoint)
    {
        RaycastHit hit;
        Ray ray = mainCamers.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100f, (1<<0), QueryTriggerInteraction.Ignore))
        {
            mousePoint = hit.point;
            return true;
        }
        mousePoint = new Vector3();
        return false;
    }
}
