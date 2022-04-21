using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    //inputs
    [SerializeField] string buttonInput = "mouse 0";
    [SerializeField] string reloadIput = "r";

    //true if the buttonInput can be held down for consecutive shots
    [SerializeField] bool allowButtonHold;

    //true if the gun is currently firing a shot 
    bool shooting;

    //true if the gun is able to shoot
    bool canShoot = true;

    //weapon stats
    [SerializeField] float attacksPerSecond = 2f;

    //hitable layers
    [SerializeField] LayerMask layers;

    //amunition managment
    [SerializeField] float maxAmmo = 20f;
    [SerializeField] float currentAmmo;
    [SerializeField] float reloadTime = 1f;
    bool isReloading;

    //important transforms, hitscan origins and the gun muzzle where attacks originate from
    [SerializeField] Transform hitscanPoint, attackPoint;

    
    // Start is called before the first frame update
    void Start()
    {
        currentAmmo = maxAmmo;
    }
    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInputs();
        ChooseAction();
    }

    //gets the players inputs
    void PlayerInputs()
    {
        
        if (allowButtonHold) { shooting = Input.GetKey(buttonInput); }
        else { shooting = Input.GetKeyDown(buttonInput); }
        
        if(Input.GetKey(reloadIput) 
            && currentAmmo < maxAmmo 
            && !isReloading) {
            BroadcastMessage("Reload", SendMessageOptions.DontRequireReceiver);
        }
        
    }

    //acts on the players inputs and current states
    void ChooseAction()
    {
        
        if (isReloading) {
            return;
        }
        if (shooting && canShoot && currentAmmo > 0)
        {
            BroadcastMessage("Shoot", SendMessageOptions.DontRequireReceiver);
        }
        if(shooting && currentAmmo <= 0)
        {
            BroadcastMessage("OutOfAmmo", SendMessageOptions.DontRequireReceiver);
        }
    }

    //the shoot method of this class
    public void Shoot()
    {
        canShoot = false;
        currentAmmo--;
        Invoke("ResetShot", 1 / attacksPerSecond);
    }
  
    //allows the gun to shoot again
    void ResetShot()
    {
        canShoot = true;
    }

    //initiates a reload
    void Reload()
    {
        isReloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    //resets the gun after a reload
    void ReloadFinished()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    //recieves a transform from outside of the script to act as the hitscanPoint
    public void SetHitscanPoint(Transform point)
    {
        hitscanPoint = point;
    }

    // various getter methods
    public float GetAttacksPerSecond() { return attacksPerSecond; }
    public bool GetCanShoot() { return canShoot; }
    public Transform GetHitscanPoint() { return hitscanPoint; }
    public Transform GetAttackPoint() { return attackPoint; }
    public LayerMask GetLayers() { return layers; }
    public float GetReloadTime() { return reloadTime; }
    public float GetCurrentAmmo() { return currentAmmo; }
    public float GetMaxAmmo() { return maxAmmo; }
}
