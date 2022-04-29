using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackShape : MonoBehaviour
{
    [Header("Default forward is RQS (0, 1, -1)")]
    [SerializeField] Vector3 forward = new Vector3(0, 1, -1);
    [Header("RQS coordanates relative to the attacks origin with'Forward' ")]
    [SerializeField] List<Vector3> effectedLocations;

    [Header("range of the outer most point effected")]
    [SerializeField] int maxRange = 1;

    

    public List<Vector3> GetShapeFacingDirection(Vector3 RQSDirection)
    {
        List<Vector3> foundHexs = new List<Vector3>();

        int numberOfRotations = NumberOfRotations(RQSDirection);
        foreach(Vector3 coord in effectedLocations)
        {
            foundHexs.Add(RotatePosition(coord, numberOfRotations));
        }
        
        return foundHexs;
    }

    
    Vector3 RotatePosition(Vector3 position, int numOfTimes)
    {
        Vector3 newPosition = position;
        if(numOfTimes < 0 || numOfTimes > 5) { return newPosition; }
        for(int i = 0; i < numOfTimes; i++)
        {
            float temp = newPosition.x;

            newPosition.x = -newPosition.z;
            newPosition.z = -newPosition.y;
            newPosition.y = -temp;
        }

        return newPosition;
    }
    int NumberOfRotations(Vector3 direction)
    {
        int rotations = 0;
        Vector3 rotation = forward;
        for (int i = 0; i < 6; i++)
        {
            if (rotation.Equals(direction)) { rotations = i; }
            float temp = rotation.x;

            rotation.x = -rotation.z;
            rotation.z = -rotation.y;
            rotation.y = -temp;
            
        }


        return rotations;
    }
    public List<Vector3> GetRotatedShape(int numOfRotations)
    {
        List<Vector3> tempShape = new List<Vector3>();
        foreach(Vector3 coord in effectedLocations)
        {
            tempShape.Add(RotatePosition(coord, numOfRotations));
        }
        
        return tempShape;
    }

    public Vector3 GetDirectionVector(int rotations)
    {
        Vector3 rotation = forward;
        for (int i = 0; i < rotations; i++)
        {
            float temp = rotation.x;

            rotation.x = -rotation.z;
            rotation.z = -rotation.y;
            rotation.y = -temp;
        }
        return rotation;
    }
    public List<Vector3> GetShape() { return effectedLocations; }
    public Vector3 GetDefaultDirection() { return forward; }
    public int GetMaxRange() { return maxRange; }

}
