using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RBShot : MonoBehaviour
{
    //the force a rigidbody projectile will be shot with
    [SerializeField] float shootForce;
    [SerializeField] float upForce;

    //the projectile that will be shot
    [SerializeField] GameObject projectile;

    //projectiles will automatically de-spawn if temporary
    [SerializeField] bool temporaryProjectiles = false;
    [SerializeField] float lifetime = 5f;

    GunController controller;
    Transform attackPoint;
    // Start is called before the first frame update
    private void Awake()
    {
        if(this.GetComponent<GunController>() != null)
        {
            controller = this.GetComponent<GunController>();
            attackPoint = controller.GetAttackPoint();
        }
    }
    
    void Shoot()
    {
        GameObject proj = Instantiate(projectile, attackPoint.position, attackPoint.rotation);
        proj.transform.SetParent(null);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        rb.AddForce(shootForce * attackPoint.forward, ForceMode.Impulse);
        rb.AddForce(upForce * attackPoint.up, ForceMode.Impulse);
        
        if (temporaryProjectiles)
        {
            Destroy(proj, lifetime);
        }
        
    }
}
