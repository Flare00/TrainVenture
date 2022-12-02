using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class ProceduralGeneration : MonoBehaviour
{
    private Terrain terrain;
    private float[,] heightmap;
    public int resolution;
    public int tileSize;
	public SplineContainer spline;

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
		ApplyHeightmapToSpline();
    }
	
	void ApplyHeightmapToSpline(){
		Spline spline = SaveLoad.GetInstance().LoadSpline("spline.data");
		for(int i = 0; i < spline.Count; i++){
			BezierKnot b = spline[i];
			
			Debug.Log(b.Position[0] + " ; " + b.Position[1]  + " ; " + b.Position[2]);
			b.Position[0] = b.Position[0] * (float)(resolution / 2);
			b.Position[2] = b.Position[2] * (float)(resolution / 2);
			Debug.Log(b.Position[0] + " ; " + b.Position[1]  + " ; " + b.Position[2]);
			b.Position[1] = heightmap[(int)b.Position[0],(int) b.Position[2]] * 1024.0f ;
			spline[i] = b;
		}
		
		this.spline.Spline = spline;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
