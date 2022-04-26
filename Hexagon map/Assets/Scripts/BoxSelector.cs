using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSelector : MonoBehaviour
{
    [SerializeField] LayerMask unitLayer;

    public List<GameObject> CheckForUnits(LayerMask layers, Quaternion rotation)
    {
        List<GameObject> found = new List<GameObject>();
        Vector3 scale = this.transform.localScale;
        scale.y = 10; 
        Collider[] units = Physics.OverlapBox(this.transform.position, scale / 2, rotation, layers);
        {
            foreach(Collider c in units)
            {
                UnitController controller = c.GetComponentInParent<UnitController>();
                if (controller != null)
                {
                    if (controller.IsPlayerUnit())
                    {
                        Debug.Log("unit added");
                        found.Add(controller.gameObject);
                    }
                }
            }
        }
        return found;
    }
}
