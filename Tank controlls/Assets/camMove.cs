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
        TankMovement();
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
    }
    private void Update()
    {
        float gunRot = Input.mouseScrollDelta.y * scrollPower * Time.deltaTime;
        foreach (TurretController tc in turretControllers)
        {
            tc.ChangeElevation(gunRot);
            tc.FaceDirection(transform.forward);
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
            if (currentTarget >= targets.Length)
            { currentTarget = 0; }
            AttachToTarget(targets[currentTarget]);

        }
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
    void TankMovement()
    {
        
        Vector2 direc = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            Vector2 dir = new Vector2(transform.forward.x, transform.forward.z);
            // MoveTank(dir);
            direc += dir;
        }
        if (Input.GetKey(KeyCode.S))
        {
            Vector2 dir = -new Vector2(transform.forward.x, transform.forward.z);
            //MoveTank(-dir);
            direc += dir;
        }
        if (Input.GetKey(KeyCode.D))
        {
            Vector2 dir = new Vector2(transform.right.x, transform.right.z);
            //MoveTank(dir);
            direc += dir;
        }
        if (Input.GetKey(KeyCode.A))
        {
            Vector2 dir = -new Vector2(transform.right.x, transform.right.z);
            // MoveTank(-dir);
            direc += dir;
        }

        
        float relativeDirec = Vector3.SignedAngle(transform.forward, player.transform.right, player.transform.up);
        Debug.Log(relativeDirec);
        int forward = (int)Mathf.Sign(relativeDirec);
        if (Input.GetKey(KeyCode.E))
        {
            Vector2 dir = forward * new Vector2(player.transform.right.x, player.transform.right.z) * 100 + new Vector2(transform.forward.x, transform.forward.z);
            direc += dir.normalized;
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Vector2 dir = forward * -new Vector2(player.transform.right.x, player.transform.right.z) * 100 + new Vector2(transform.forward.x, transform.forward.z);
            direc += dir.normalized;
        }

        if (direc != new Vector2(0, 0))
        {
            player.GetComponentInChildren<TankDrive>().MoveTank(direc);
        }
    }


}
