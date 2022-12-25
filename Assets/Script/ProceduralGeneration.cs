using System.Collections;
using System.Collections.Generic;
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
        var xScale = TData.size.x/resolution;
        var yScale = TData.size.y/resolution;
        var zScale = TData.size.z/resolution;

        var terrainPos = Terrain.activeTerrain.transform.position;
        TData.treeInstances = new TreeInstance[0];
        
        //make these float otherwise your position math below is truncated
        for (float x = 0; x < TData.heightmapResolution; x+=4)
        {
            for (float z = 0; z < TData.heightmapResolution; z+=4)
            {
                if(heightmap[(int)z,(int)x]> 0.25f)
                {
                    //apply trees
                    TreeInstance treeInstance = new TreeInstance(); 
                    treeInstance.position = new Vector3(x / TData.heightmapResolution, 0, z / TData.heightmapResolution);
                    treeInstance.prototypeIndex = Random.Range(0,TData.treePrototypes.Length);
                    treeInstance.widthScale = Random.Range(minSize,maxSize);
                    treeInstance.rotation = Random.Range(0f,360f);
                    treeInstance.lightmapColor = Color.white;
                    treeInstance.heightScale = Random.Range(minSize,maxSize);
                    treeInstance.color = Color.white;

                    terrain.AddTreeInstance(treeInstance);
                    terrain.Flush();
                    
                    nbTrees++;
                }
            }
        }

        Debug.Log(TData.treeInstanceCount);
    }
	
	void ApplyHeightmapToSpline(){
		Spline spline = SaveLoad.GetInstance().LoadSpline("spline.data");
		for(int i = 0; i < spline.Count; i++){
			BezierKnot b = spline[i];
			
			//Debug.Log(b.Position[0] + " ; " + b.Position[1]  + " ; " + b.Position[2]);
			b.Position[0] = b.Position[0] * (float)(resolution / 2);
			b.Position[2] = b.Position[2] * (float)(resolution / 2);
			//Debug.Log(b.Position[0] + " ; " + b.Position[1]  + " ; " + b.Position[2]);
			b.Position[1] = heightmap[(int)b.Position[0],(int) b.Position[2]] * (float)(resolution / 2) ;
			spline[i] = b;
		}
		
		this.spline.Spline = spline;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
