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
        maxElevation,
        minElevation,
        maxRotLeft,
        maxRotRight,
        rotationSpeed;

    public float
        currentElevation,
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
            currentElevation = guns[0].transform.localEulerAngles.x;
        }
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
        Debug.Log("signedAngle = "+angleBetween);
        float curr = (currentRotation - 180) % 180;
        Debug.Log("currentRotation = " + curr);
        direction = Mathf.Sign(angleBetween);
        //if (currentRotation + angleBetween < maxRotLeft + originalRotation)
        //{ direction = 1;}
        //if (currentRotation + angleBetween > maxRotRight + originalRotation)
        //{ direction = -1;}

        RotateTurret(direction);
    }
    void RotateTurret(float degree)
    {
        currentRotation += degree * rotationSpeed * Time.deltaTime;
        currentRotation = Mathf.Clamp(currentRotation, maxRotLeft + originalRotation, maxRotRight + originalRotation);
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
        currentElevation += degree;
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
