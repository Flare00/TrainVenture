using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralGeneration : MonoBehaviour
{
    private Terrain terrain;
    private float[,] heightmap;
    public int resolution;
    public int tileSize;

    [SerializeField]Texture2D grassTexture;
    [SerializeField]Texture2D rockTexture;


    [Range(0,15)]
    public float divRange;
 
    // Start is called before the first frame update
    void Start()
    {
        terrain = gameObject.GetComponent<Terrain>();
 
        initTerrain();
    }

    void initTerrain()
    {
        terrain.terrainData.heightmapResolution = resolution;
        terrain.terrainData.baseMapResolution = resolution;
        GenerateHeightMap();
    }

    void GenerateHeightMap()
    {
        TerrainData TData = terrain.terrainData;
        heightmap = new float[TData.heightmapResolution,TData.heightmapResolution];

        for(int i = 0; i < resolution; i++)
        {
            for(int j = 0; j < resolution; j++)
            {
                heightmap[i, j] = Mathf.PerlinNoise(((float)i / (float)TData .heightmapResolution) * tileSize, ((float)j / (float)TData .heightmapResolution) * tileSize) / divRange;
            }
        }
        TData.SetHeights(0, 0, heightmap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
