using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMapMaker : MonoBehaviour
{
    [SerializeField] float noiseOffset = .12345f;
    [SerializeField] float noiseMulti = 5f;
    [SerializeField] GameObject buildingBlock;
    [SerializeField] int visibleArea = 10;
    [SerializeField] Dictionary<Vector3, Transform> map = 
        new Dictionary<Vector3, Transform>();
    [SerializeField] Transform playerTransform;
    //Start is called before the first frame update
    void Start()
    {
        for(int x = -visibleArea; x <= visibleArea; x++)
        {
            for(int z = -visibleArea; z <= visibleArea ; z++)
            {
                float noise = GetPerlinNoise
                (
                    x,
                    z
                );
                map.Add
                (
                    new Vector3
                    (
                        x + (int)playerTransform.position.x,
                        0,
                        z + (int)playerTransform.position.z
                    ),
                    Instantiate
                    (
                        buildingBlock, 
                        new Vector3(x + (int)playerTransform.position.x, 
                        noise, z + (int)playerTransform.position.z), 
                        Quaternion.identity
                    ).transform
                );
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnMap()
    {

    }

    float GetPerlinNoise(float x, float z)
    {
        float noise = Mathf.PerlinNoise
        (
            (x + (int)playerTransform.position.x) * noiseOffset, 
            (z + (int)playerTransform.position.z) * noiseOffset
        );
        return noise * noiseMulti;
    }
}
