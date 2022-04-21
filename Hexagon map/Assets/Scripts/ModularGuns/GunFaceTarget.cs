using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunFaceTarget : MonoBehaviour
{
    
    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] float range = 100f;
    LayerMask layers;
    GunController controller;
    Transform hitscanPoint, attackPoint;
    // Start is called before the first frame update
    private void Awake()
    {
        if (this.gameObject.GetComponent<GunController>() != null)
        {
            controller = this.gameObject.GetComponent<GunController>();
            hitscanPoint = controller.GetHitscanPoint();
            attackPoint = controller.GetAttackPoint();
            layers = controller.GetLayers();
        }
    }

    // Update is called once per frame
    void Update()
    {
        hitscanPoint = controller.GetHitscanPoint();
        FaceTarget();
    }

    void FaceTarget()
    {
        //
        if (controller.GetCanShoot())
        {
            Quaternion q = hitscanPoint.transform.rotation;
            Vector3 targetVec = hitscanPoint.position + hitscanPoint.forward * 100;
            if (Physics.Raycast(hitscanPoint.position,
                hitscanPoint.forward, out RaycastHit hit,
                100,
                layers,
                QueryTriggerInteraction.Ignore))
            {
                targetVec = hit.point;
                Debug.Log("raucast hits: " + hit.transform);
            }
            q.SetLookRotation(targetVec - transform.position, hitscanPoint.up);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                                                q,
                                                lerpSpeed * Time.deltaTime);
            //
        }
    }
}
