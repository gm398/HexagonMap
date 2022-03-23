using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSelector : MonoBehaviour
{

    [SerializeField] List<GameObject> selected = new List<GameObject>();

    public void ClearSelected()
    {
        selected.Clear();
    }
    private void OnTriggerStay(Collider other)
    {
        UnitController controller = other.GetComponentInParent<UnitController>();
        if(controller != null)
        {
            if (controller.isPlayerUnit() && !selected.Contains(other.gameObject))
            {
                selected.Add(other.gameObject);
            }

        }
    }
    public List<GameObject> GetSelected()
    {
        return selected;
    }
}
