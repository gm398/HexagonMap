using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camMove : MonoBehaviour
{
    GameObject
        player;
    [SerializeField]
    GameObject[]
        targets;
    int currentTarget = 0;
    [SerializeField]
    Transform
        camXRotation;
    [SerializeField]
    float 
        sensitivity = 500,
        scrollPower = 100,
        elevationUp = 60,
        elevationDown = -20;

    

    float
        maxGunAngle,
        minGunAngle,
        rot;

    List<Transform>
        turrets;

    List<TurretController>
        turretControllers;

    // Start is called before the first frame update
    void Awake()
    {
        AttachToTarget(targets[currentTarget]);
        Cursor.lockState = CursorLockMode.Locked;

        //minGunAngle = gun.localEulerAngles.x - elevationDown;
        //minGunAngle = (minGunAngle + 360) % 360; 
        //maxGunAngle = gun.localEulerAngles.x + elevationUp;
        //maxGunAngle = (maxGunAngle + 360) % 360; 

        minGunAngle = elevationUp;
        maxGunAngle = elevationUp + elevationDown;

        elevationDown *= -1;
        elevationUp *= -1;
        rot = transform.localEulerAngles.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = player.transform.position;

        float yRot = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float xRot = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        transform.Rotate(0, yRot, 0);
        foreach(Transform t in turrets)
        {
            //t.rotation = transform.rotation;
        }
        
        rot -= xRot;
        rot = Mathf.Clamp(rot, elevationUp, elevationDown);
        camXRotation.localEulerAngles = new Vector3(rot, 0, 0);

        float gunRot = Input.mouseScrollDelta.y * scrollPower * Time.deltaTime;
        foreach (TurretController tc in turretControllers)
        {
            tc.ChangeElevation(gunRot);
            tc.FaceDirection(transform.eulerAngles.y);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            foreach (TurretController tc in turretControllers)
            {
                tc.Shoot();
            }
            
            
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            currentTarget++;
            if(currentTarget >= targets.Length)
            { currentTarget = 0; }
            AttachToTarget(targets[currentTarget]);
            
        }
        //transform.Rotate(xRot, 0, 0);

        
        //float tX = gun.localEulerAngles.x;
        //float turretX = transform.localEulerAngles.x;
        //tX = (tX + 360) % 360;
        //turretX = (turretX + 360) % 360;
        //int direc = 1;
        //if(tX < turretX)
        //{
        //    direc = -1;
        //}
        //float rot = tX + 5 * direc + minGunAngle;

        //rot = (rot + 360) % 360;

        //if(rot <= maxGunAngle && rot >= 0)
        //{
        //    gun.Rotate(5 * direc, 0, 0);
        //}


        
    }

    public void AttachToTarget(GameObject target)
    {
        player = target;
        turretControllers = new List<TurretController>();
        turrets = new List<Transform>();
        target.GetComponentInChildren<TankDrive>().SetCamHolder(transform);
        foreach (TurretController tc in target.GetComponentsInChildren<TurretController>())
        {
            turretControllers.Add(tc);
            turrets.Add(tc.gameObject.transform);
        }
    }
}
