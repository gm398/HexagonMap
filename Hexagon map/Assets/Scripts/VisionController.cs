using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionController : MonoBehaviour
{
    [SerializeField] int visionRange = 3;

    [Header("default value if no other vision height is used")]
    [SerializeField] float visionHeightStep = .5f;
    [SerializeField] List<GameObject> visibleComponents;
    bool isVisible = true;
    List<Hex> visibleHexes = new List<Hex>();
    HexGrid hexGrid;

    private void Awake()
    {
        hexGrid = GameObject.FindGameObjectWithTag("Map").GetComponent<HexGrid>();
    }


    public void UpdateVision(Hex currentHex)
    {
        UpdateVision(visionHeightStep, currentHex);
    }
    public void UpdateVision(float height, Hex currentHex)
    {
        List<Hex> newVision = hexGrid.GetHexesInRange(visionRange, currentHex);
        RemoveVision();
        visibleHexes.Clear();
        visibleHexes.TrimExcess();

        foreach (Hex h in newVision)
        {
            if (h.transform.position.y < transform.position.y + height)
            {
                h.SetVisible(true);
                visibleHexes.Add(h);
            }
        }
    }
    public void RemoveVision()
    {
        foreach (Hex h in visibleHexes)
        {
            h.SetVisible(false);
        }
    }
    
    public void SetVisible(bool isVis)
    {
        foreach (GameObject c in visibleComponents)
        {
            c.SetActive(isVis);
            isVisible = isVis;
        }

    }

    public bool IsVisible() { return isVisible; }
}
