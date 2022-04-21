using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSpread : MonoBehaviour
{
    //aiming spread
    [SerializeField] float maxSpread = 2f;
    [SerializeField] float minSpread = 0f;
    [SerializeField] float spreadSpeed = 10f;
    public float currentSpread = 0f;
    LayerMask layers;
    [SerializeField] float lerpSpeed = 10f;
    private Quaternion bulletSpread;

    GunController controller;
    Transform hitscanPoint, attackPoint;
   

    private void Awake()
    {
        if (this.gameObject.GetComponent<GunController>() != null)
        {
            controller = this.gameObject.GetComponent<GunController>();
            hitscanPoint = controller.GetHitscanPoint();
            attackPoint = controller.GetAttackPoint();
            layers = controller.GetLayers();
        }
        currentSpread = minSpread;
    }

    // Update is called once per frame
    void Update()
    {
        hitscanPoint = controller.GetHitscanPoint();
        
        CalculateSpread(spreadSpeed);
        
         
        UpdateLookDirection();

      
    }

    void Shoot()
    {
        
    }
    //Lerps the gun to face the direction of shooting or the rest position
    void UpdateLookDirection()
    {
        
        //
        Quaternion q = hitscanPoint.transform.rotation;
        Vector3 targetVec = hitscanPoint.position + hitscanPoint.forward * 100;
        if (Physics.Raycast(hitscanPoint.position, hitscanPoint.forward, out RaycastHit hit, 100, layers, QueryTriggerInteraction.Ignore))
        {
            targetVec = hit.point;
            Debug.Log("raucast hits: " + hit);
        }
        q.SetLookRotation(targetVec - transform.position, hitscanPoint.up);
        
    //
        if (!controller.GetCanShoot())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, bulletSpread * q, lerpSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, q, lerpSpeed * Time.deltaTime);
        }

    }
    

    //updates the spread Quaternion
    void CalculateSpread(float multiplier)
    {
        if (controller.GetCanShoot()) { multiplier *= -2; }
        currentSpread += multiplier * Time.deltaTime;
        if (currentSpread > maxSpread) { currentSpread = maxSpread; }
        if (currentSpread < minSpread) { currentSpread = minSpread; }
        bulletSpread = Quaternion.Euler(Random.Range(-currentSpread, currentSpread), Random.Range(-currentSpread, currentSpread), Random.Range(-currentSpread, currentSpread));
    }

    public Quaternion GetSpread()
    {
        return bulletSpread;
    }

    
}
