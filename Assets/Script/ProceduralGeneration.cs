using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Splines;

public class ProceduralGeneration : MonoBehaviour
{
   [SerializeField] Terrain terrain;
    private TerrainData TData;
    private float[,] heightmap;
    private int nbTrees;
    public int resolution;
    public int tileSize;
	public SplineContainer spline;

    [SerializeField]Texture2D grassTexture;
    [SerializeField]float minSize,maxSize;
    [SerializeField]int maxTrees;

    [Range(0,15)]
    public float divRange;
 
    // Start is called before the first frame update
    void Start()
    {
        terrain.enabled = true;
        terrain.drawInstanced = true;
        terrain.drawTreesAndFoliage = true;
        terrain.treeDistance = 2000;
        TData = terrain.terrainData;
        TData.RefreshPrototypes();
        initTerrain();
    }

    void initTerrain()
    {
        terrain.terrainData.heightmapResolution = resolution;
        terrain.terrainData.baseMapResolution = resolution;
        GenerateHeightMap();
        GenerateTrees();
    }

    void GenerateHeightMap()
    {
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

    void GenerateTrees()
    {
        var terrainPos = Terrain.activeTerrain.transform.position;
        TData.treeInstances = new TreeInstance[0];
        var maxHeight = TData.bounds.max.y;
        
        //make these float otherwise your position math below is truncated
        for (float x = 0; x < TData.heightmapResolution; x+=4)
        {
            for (float z = 0; z < TData.heightmapResolution; z+=4)
            {
                float height = TData.GetHeight((int)x,(int)z);
                if(height > maxHeight/2 && height< maxHeight/1.5)
                {
                    //apply trees
                    TreeInstance treeInstance = new TreeInstance(); 
                    treeInstance.position = new Vector3(x / TData.heightmapResolution, 0, z / TData.heightmapResolution);
                    treeInstance.prototypeIndex = UnityEngine.Random.Range(0,TData.treePrototypes.Length);
                    treeInstance.widthScale = UnityEngine.Random.Range(minSize,maxSize);
                    treeInstance.rotation = UnityEngine.Random.Range(0f,360f);
                    treeInstance.lightmapColor = Color.white;
                    treeInstance.heightScale = UnityEngine.Random.Range(minSize,maxSize);
                    treeInstance.color = Color.white;

                    terrain.AddTreeInstance(treeInstance);
                    terrain.Flush();
                    
                    nbTrees++;
                }
            }
        }

        Debug.Log("Number of trees: "+TData.treeInstanceCount);
    }

	Spline normalizeSplinePositions(Spline spline)
    {
        var maxX = 0.0f;
        var maxY = 0.0f;
        var maxZ = 0.0f;
        var minX = Single.MaxValue;
        var minY = Single.MaxValue;
        var minZ = Single.MaxValue;
            Debug.Log("Before normalization");
		for(int i = 0; i < spline.Count; i++){
			BezierKnot b = spline[i];
            if(b.Position[0] > maxX)
                maxX=b.Position[0];
            if(b.Position[1] > maxY)
                maxY=b.Position[1];
            if(b.Position[2] > maxZ)
                maxZ=b.Position[2];

            if(b.Position[0] < minX)
                minX=b.Position[0];
            if(b.Position[1] < minY)
                minY=b.Position[1];
            if(b.Position[2] < minZ)
                minZ=b.Position[2];

            Debug.Log(b.Position[0]+"; "+b.Position[1]+"; "+b.Position[2]);
        }

            Debug.Log("After normalization");
		for(int i = 0; i < spline.Count; i++){
			BezierKnot b = spline[i];
            b.Position[0] = (b.Position[0] - minX) / (maxX - minX);
            b.Position[2] = (b.Position[2] - minZ) / (maxZ - minZ);

            Debug.Log(b.Position[0]+"; "+b.Position[1]+"; "+b.Position[2]);
        }
        
        return spline;
    }

	void ApplyHeightmapToSpline(){
		Spline spline = SaveLoad.GetInstance().LoadSpline("spline.data");

        spline = normalizeSplinePositions(spline);
		for(int i = 0; i < spline.Count; i++){
			BezierKnot b = spline[i];
			
			b.Position[0] = b.Position[0] * (TData.bounds.max.x / 2);
			b.Position[2] = b.Position[2] * (TData.bounds.max.z / 2);
            float height = TData.GetHeight((int)b.Position[0],(int) b.Position[2]);
			b.Position[1] = height;
			spline[i] = b;
		}
		
		this.spline.Spline = spline;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
