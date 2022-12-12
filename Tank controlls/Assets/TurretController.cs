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

    float
        currentElevation,
        currentRotation;
    // Start is called before the first frame update
    void Start()
    {
        currentRotation = transform.localEulerAngles.y;
        maxElevation *= -1;
        minElevation *= -1;
        maxRotRight *= -1;
        if(guns.Length > 0)
        {
            currentElevation = guns[0].transform.localEulerAngles.x;
        }
        ChangeElevation(0);
    }

   
    public void FaceDirection(float dir)
    {
        dir -= transform.eulerAngles.y;
        float deg = Mathf.MoveTowardsAngle(transform.eulerAngles.y, dir, rotationSpeed * Time.deltaTime);
        Debug.Log("deg: "+deg);
        //float deg = Mathf.Sign(dir - transform.localEulerAngles.y) * rotationSpeed * Time.deltaTime;
        RotateTurret(deg);
    }
    void RotateTurret(float degree)
    {
        currentRotation += degree;
        currentRotation = Mathf.Clamp(currentRotation, maxRotRight, maxRotLeft);
        transform.localEulerAngles = new Vector3(0, currentRotation, 0);
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
        foreach (GameObject g in guns)
        {
            g.GetComponent<Gun>().Shoot();
        }
    }
}
