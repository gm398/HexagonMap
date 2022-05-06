using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
// tutorial
//https://www.youtube.com/watch?v=WGo07dMJPtk

public class SelectionManager : MonoBehaviour
{

    [SerializeField] Camera mainCamers;
    [SerializeField] LayerMask selectionMask;
    [SerializeField] HexGrid hexGrid;

    [SerializeField] Material selectionMaterial;
    [SerializeField] Material testMat;

    Hex currentHex;
    [SerializeField] List<GameObject> currentSelection;
    bool isMouseDown = false;
    [SerializeField] float mouseHoldActivationTime = .02f;
    float mouseHeldTimer = 0;
    Vector3 mouseStart;
    Vector3 mouseEnd;
    [SerializeField] GameObject selectionCube;
    [SerializeField] GameObject camHolder;


    Hex hoveringHex = null;

    [SerializeField] bool inRTSMode = true;
    [SerializeField] List<GameObject> rtsPieces;
    [SerializeField] List<GameObject> tppPieces;
    UnitController currentDirectControl;
    float switchPerspectiveCooldown = .2f;
    float timeOfNextSwitch = 0f;
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
        if (!inRTSMode)
        {
            ClearSelected();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                SwitchPerspectives();
            }
        }
        else
        {
            DetectMouseClick();
        }
        
    }
    private void LateUpdate()
    {
        HighlightHoveredHex();
    }

    void HighlightHoveredHex()
    {
        GameObject hoveringOver;
        if (hoveringHex != null)
        {
            hoveringHex.HighlightHex(false);
            hoveringHex = null;
        }
        if (!inRTSMode) { return; }
        if (FindTarget(Input.mousePosition, out hoveringOver))
        {
            Hex newHex = hoveringOver.GetComponentInParent<Hex>();
            if (newHex != null)
            {
                hoveringHex = newHex;
                if (hoveringHex.IsVisible())
                {
                    hoveringHex.HighlightHex(true);
                }
            }
        }
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

            //if shift is not held then the current selection is cleared
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                ClearSelected();
            }

            UnitController unitController = result.GetComponentInParent<UnitController>();
            MainBaseController baseController = result.GetComponentInParent<MainBaseController>();
            Hex targetHex = result.GetComponentInParent<Hex>();
            if (targetHex != null)
            {
                
                Hex selectedHex = targetHex;
                //Debug.Log("Hex RQS:" + selectedHex.GetHexCoordinates().GetHexCoordsRQS() + " .");

                currentHex = selectedHex;//currently selected hex
            }
            else if(unitController != null)
            {
                if (unitController.IsPlayerUnit())
                {
                    currentSelection.Add(result);
                    unitController.SetSelected(true);
                }
            }
            else if(baseController != null)
            {
                if (baseController.IsPlayerUnit())
                {
                    currentSelection.Add(result);
                    baseController.SetSelected(true);
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
                            
                            
                            if (controller != null)
                            {
                                controller.SetTargetEnemy(result);
                               
                            }
                        }
                    }
                }
            }
            
        }
    }
    void ClearSelected()
    {
        foreach (GameObject s in currentSelection)
        {
            UnitController uc = s.GetComponentInParent<UnitController>();
            if (uc != null) { uc.SetSelected(false); }
            else
            {
                MainBaseController bs = s.GetComponentInParent<MainBaseController>();
                if (bs != null) { bs.SetSelected(false); }
            }

        }
        currentSelection.Clear();
        currentSelection.TrimExcess();
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
            GameObject tempResult = hit.collider.gameObject;
            if(!IsPointerOverUIObject())
            {
                result = tempResult;
                return true;
            }
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



    //checks to see if the mouse is over a UI element other than the healthbars
    //slightly modified from this code:
    //https://answers.unity.com/questions/967170/detect-if-pointer-is-over-any-ui-element.html
    //Answer by SkylinR · Jul 07, 2020 at 09:33 AM
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        int modifyer = 0;
        foreach(RaycastResult r in results)
        {
            if (r.gameObject.tag.Equals("HealthBar"))
            {
                modifyer++;
            }
        }
        return results.Count - modifyer > 0;
    }



    public void SwitchPerspectives()
    {
        //if(timeOfNextSwitch > Time.time) { return; }
        if (inRTSMode)
        {
            List<UnitController> controllers = new List<UnitController>();
            int count = 0;
            UnitController controller = null;
            currentSelection.TrimExcess();
            foreach (GameObject g in currentSelection)
            {
                UnitController c = g.GetComponentInParent<UnitController>();
                if (c != null)
                {
                    if (!controllers.Contains(c))
                    {
                        controllers.Add(c);
                        controller = c;
                        count++;
                    }
                    
                }
            }
            if (count != 1) { return; }
            currentDirectControl = controller;
        }
        currentDirectControl.TakeDirectControl(inRTSMode);
        foreach(GameObject g in rtsPieces)
        {
            g.SetActive(!inRTSMode);
        }
        foreach(GameObject g in tppPieces)
        {
            g.SetActive(inRTSMode);
        }
        inRTSMode = !inRTSMode;
        timeOfNextSwitch = Time.time + switchPerspectiveCooldown;
    }
}
