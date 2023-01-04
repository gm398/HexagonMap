using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{

    [SerializeField]
    GameObject[] 
        guns;

    [SerializeField]
    float
        maxElevation = 90,
        minElevation = -20,
        elevationSpeed = 10,
        maxRotLeft = 90,
        maxRotRight = 90,
        rotationSpeed = 10;
    [SerializeField]
    bool
        clampRotation = true;

    public float
        currentElevation,
        originalElevation,
        currentRotation,
        originalRotation;
    bool
        isAvailable  = false;
    // Start is called before the first frame update
    void Start()
    {
        originalRotation = transform.localEulerAngles.y;
        currentRotation = originalRotation;
        maxElevation *= -1;
        minElevation *= -1;
        maxRotLeft *= -1;
        //maxRotLeft += originalRotation;
        //maxRotRight += originalRotation;
        
        if(guns.Length > 0)
        {
            originalElevation = guns[0].transform.localEulerAngles.x;
            if(originalElevation > 180)
            {
                originalElevation -= 360;
            }
        }
        currentElevation = originalElevation;
        ChangeElevation(0);
    }
   

    public void FaceDirection(Vector3 dir)
    {
        float direction = 0;
        if (Input.GetKey(KeyCode.R))
        { direction = -1; }
        if (Input.GetKey(KeyCode.T))
        { direction = 1; }
        float angleBetween = Vector3.SignedAngle(transform.forward, dir, transform.up);
        //Debug.Log(dir);
        direction = Mathf.Sign(angleBetween);
        if(Mathf.Abs(angleBetween) < 1)
        {
            direction = angleBetween;
            isAvailable = true;
        }else{
            isAvailable = false;
        }
        
        if (clampRotation)
        {
            if ((currentRotation + angleBetween < maxRotLeft + originalRotation || currentRotation + angleBetween > maxRotRight + originalRotation))//given rotation is outside possible turret rotation
            {
                if (currentRotation > originalRotation)
                { direction = -1; }
                else
                { direction = 1; }
                ChangeElevation(Mathf.Sign(originalElevation - currentElevation));//reset elevation
            }
        }
        RotateTurret(direction);
    }
    void RotateTurret(float degree)
    {
        currentRotation += degree * rotationSpeed * Time.deltaTime;
        if (!clampRotation)
        {
            currentRotation = Mathf.Clamp(currentRotation, maxRotLeft + originalRotation, maxRotRight + originalRotation);
        }
        //Debug.Log(currentRotation);
        transform.localEulerAngles = new Vector3(0, currentRotation, 0);
    }
    bool Clamp360(float degree, float min, float max)
    {
        bool clamped = false;
        

        return clamped;
    }
   
    public void ChangeElevation(float degree)
    {
        currentElevation += degree * elevationSpeed * Time.deltaTime;
        currentElevation = Mathf.Clamp(currentElevation, maxElevation, minElevation);
        foreach (GameObject g in guns)
        {
            g.transform.localEulerAngles = new Vector3(currentElevation, 0, 0);
        }
    }
    public void Shoot()
    {
        if (!isAvailable)
        { return; }
        foreach (GameObject g in guns)
        {
            g.BroadcastMessage("Shoot", null, SendMessageOptions.DontRequireReceiver);
        }
    }
}
