using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanShot : MonoBehaviour
{
    //gun stats
    [SerializeField] float damage = 5f;
    [SerializeField] float bulletForce = 100f;
    LayerMask layers;
    [SerializeField] float range = 100f;

    //hitscan origin
    Transform hitscanPoint;
    [SerializeField] bool useMuzzle = true;
    //gun muzzle
    Transform attackPoint;
    //GunController script
    GunController controller;
    GunSpread spreadController;
    Quaternion bulletSpread;

    [SerializeField] GameObject hitMarker;
    [SerializeField] GameObject bulletTrail;

    private void Awake()
    {
       
        if(this.gameObject.GetComponent<GunController>() != null)
        {
            controller = this.gameObject.GetComponent<GunController>();
            hitscanPoint = controller.GetHitscanPoint();
            attackPoint = controller.GetAttackPoint();
            layers = controller.GetLayers();
        }
        if (this.gameObject.GetComponent<GunSpread>() != null)
        {
            spreadController = this.gameObject.GetComponent<GunSpread>();
        }
        bulletSpread = Quaternion.identity;
        
    }
    

    void Shoot()
    {
        Vector3 hitOrigin;
        if (useMuzzle) { hitOrigin = attackPoint.position; }
        else { hitOrigin = hitscanPoint.position; }
        
        if(spreadController != null) { bulletSpread = spreadController.GetSpread(); }
        
        if (Physics.Raycast(hitOrigin, bulletSpread * attackPoint.forward, out RaycastHit hit, range, layers, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("hit: " + hit.transform.name);
            SpawnHitMarker(hit);
            if (hit.transform.GetComponent<Health>() != null)
            {
                Debug.Log("damaged: " + hit.transform.name);
                hit.transform.GetComponent<Health>().TakeDamage(damage);
            }
            if (hit.transform.GetComponent<Rigidbody>() != null)
            {
                hit.transform.GetComponent<Rigidbody>().AddForceAtPosition(attackPoint.forward * bulletForce, hit.point, ForceMode.Impulse);
            }
            if(bulletTrail != null)
            {
                SpawnBulletTrail(hit.point);
            }
        }
        else {
            if(bulletTrail != null)
            {
                SpawnBulletTrail(attackPoint.position + attackPoint.forward * range);
            }
        }
    }

    void SpawnHitMarker(RaycastHit hit)
    {
        GameObject bulletHitPoint = Instantiate(hitMarker, hit.point, Quaternion.identity);
        bulletHitPoint.transform.parent = hit.transform;
        Destroy(bulletHitPoint, 5f);
    }
    private void SpawnBulletTrail(Vector3 hitPoint)
    {
        GameObject bulletTrailEffect = Instantiate(bulletTrail.gameObject, attackPoint.position, Quaternion.identity);

        LineRenderer lineR = bulletTrailEffect.GetComponent<LineRenderer>();
        lineR.SetPosition(0, attackPoint.position);
        lineR.SetPosition(1, hitPoint);
        Destroy(bulletTrailEffect, 1f);


    }







}
