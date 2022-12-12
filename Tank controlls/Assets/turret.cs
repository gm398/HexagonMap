using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret : MonoBehaviour
{
    [SerializeField]
    Transform
        gun,
        camHolder;
    [SerializeField]
    float
        elevationSpeed = 20,
        maxGunAngle = 60,
        minGunAngle = 20;
    private
    float
        gunUp,
        gunDown,
        gunRear;

    private void Awake()
    {
        gunUp = (360 - maxGunAngle) + minGunAngle;
        gunUp = (gunUp + 360) % 360;
        gunDown = 360;
        gunRear = (gun.rotation.x + 180) % 360;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //float
        //    gunX = gun.rotation.x,
        //    camX = camHolder.rotation.x;
        //Vector3 
        //    rot = gun.rotation.eulerAngles;
        //rot.x = camHolder.rotation.eulerAngles.x;
        //gun.rotation = Quaternion.Euler(rot);
    }
}
