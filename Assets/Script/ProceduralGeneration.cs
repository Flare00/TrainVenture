using System;
using System.Linq;
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

    [Range(0,15)]
    public float divRange;
 
    // Start is called before the first frame update
    void Start()
    {
        terrain.enabled = true;
        terrain.drawInstanced = true;
        terrain.drawTreesAndFoliage = true;
        terrain.treeDistance = 3000;
        TData = terrain.terrainData;
        TData.RefreshPrototypes();
        initTerrain();
    }

    void initTerrain()
    {
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        terrain.terrainData.heightmapResolution = resolution;
        terrain.terrainData.baseMapResolution = resolution;

        GenerateHeightMap();
        GenerateTextures();
        GenerateDetails();
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

        void GenerateTextures()
    {
        float[, ,] splatmapData = new float[TData.alphamapWidth, TData.alphamapHeight, TData.alphamapLayers];

        // For each point on the alphamap...
        for (int y = 0; y < TData.alphamapHeight; y++)
        {
            for (int x = 0; x < TData.alphamapWidth; x++)
            {     
                /// Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y/(float)TData.alphamapHeight;
                float x_01 = (float)x/(float)TData.alphamapWidth;
                 
                float height = TData.GetHeight(Mathf.RoundToInt(y_01 * TData.heightmapResolution),Mathf.RoundToInt(x_01 * TData.heightmapResolution) );
                 
                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = TData.GetInterpolatedNormal(y_01,x_01);
                float steepness = TData.GetSteepness(y_01,x_01);
                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[TData.alphamapLayers];
        
                // Rules
                // Texture[0] is stronger at lower altitudes
                splatWeights[0] = Mathf.Clamp01((TData.heightmapResolution - height));

                // Texture[1] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                splatWeights[1] = 1.0f - Mathf.Clamp01(steepness*steepness/(TData.heightmapResolution/5.0f));

                // Texture[2] has constant influence
                splatWeights[2] = 0.6f;
                 
                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();
                 
                // Loop through each terrain texture
                for(int i = 0; i<TData.alphamapLayers; i++){
                     
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;
                     
                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }
        TData.SetAlphamaps(0, 0, splatmapData);
    }

    void GenerateDetails()
    {
        var maxHeight = TData.bounds.max.y;

        //Grass details
        var map = TData.GetDetailLayer(0, 0, TData.detailWidth, TData.detailHeight, 0);
        for (var y = 0; y < TData.detailHeight; y++)
        {
            for (var x = 0; x < TData.detailWidth; x++)
            {                
                float y_01 = (float)y/(float)TData.detailWidth;
                float x_01 = (float)x/(float)TData.detailWidth;
                float height = TData.GetHeight(Mathf.RoundToInt(y_01 * TData.heightmapResolution),Mathf.RoundToInt(x_01 * TData.heightmapResolution) );
                if( height< maxHeight/1.8f)
                    map[x, y] = 1;
                else
                    map[x, y] = 0;
            }
        }

        // Assign the modified map back.
        TData.SetDetailLayer(0, 0, 0, map);
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
        float maxX = 0.0f;
        float maxY = 0.0f;
        float maxZ = 0.0f;
        float minX = Single.MaxValue;
        float minY = Single.MaxValue;
        float minZ = Single.MaxValue;
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
        }

		for(int i = 0; i < spline.Count; i++){
			BezierKnot b = spline[i];
            b.Position[0] = (b.Position[0] - minX) / (maxX - minX);
            b.Position[2] = (b.Position[2] - minZ) / (maxZ - minZ);
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
