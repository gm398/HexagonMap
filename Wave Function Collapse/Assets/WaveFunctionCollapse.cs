using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFunctionCollapse : MonoBehaviour
{
    [SerializeField] Tile[] tiles;
    [SerializeField] int[,] entropy;
    [SerializeField] int height = 10;
    [SerializeField] int width = 10;
    [SerializeField] float gap = 1f;

    // Start is called before the first frame update
    void Start()
    {
        entropy = new int[width, height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                entropy[x, y] = tiles.Length;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void CreateMap()
    {

    }
}
