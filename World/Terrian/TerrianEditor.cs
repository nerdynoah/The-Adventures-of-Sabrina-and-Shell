using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TerrainEditor : MonoBehaviour
{
    [SerializeField] private Terrain terrian;


    [SerializeField] private AnimationCurve[] PerlinSlope;
    [SerializeField] private AnimationCurve PerlinFluxSlope;
    [Range(0.0001f, 0.05f)]
    [SerializeField] private float[] PerlinStretch;
    [Range(0.0001f, 0.05f)]
    [SerializeField] private float PerlinFluxStretch;
    [Range(0f, 1f)]
    [SerializeField] private float HeightMultiplier;
    [Header("Island borders")]
    [Range(0f, 1f)]
    [SerializeField] private float PercentDistanceFromEdge;
    [SerializeField] AnimationCurve EdgeBorder;


    //Private vals
    private int HeightRes;
    private float[,] Mesh;
    private float[,] EdgeReductionMap;
    // Start is called before the first frame update
    void OnEnable()
    {
        PullTerrain();
    }
    void Start()
    {
        BuildEdgeReductionMap();
        RedrawTerrainMesh();
        UpdateHeightMap();
        PullTerrain();
    }
    void OnValidate()
    {
        BuildEdgeReductionMap();
        RedrawTerrainMesh();
        UpdateHeightMap();
    }
    public void BuildEdgeReductionMap()
    {
        EdgeReductionMap = new float[HeightRes, HeightRes];

        int half = HeightRes / 2;
        Vector2Int center = new Vector2Int(half, half);
        float highBoarder = PercentDistanceFromEdge * half;
        float range = half - highBoarder;
        for (int x = 0; x < HeightRes; x++)
        {
            for (int y = 0; y < HeightRes; y++)
            {
                float distance = Vector2Int.Distance(center, new Vector2Int(x, y)) - range;
                if (distance < 0) 
                {
                    EdgeReductionMap[x, y] = 1;
                    continue;
                }
                if (distance > range)
                {
                    EdgeReductionMap[x, y] = 0;
                    continue;
                }
                EdgeReductionMap[x, y] = EdgeBorder.Evaluate(1 - distance/range);
            }
        }
    }
    public void PullTerrain()
    {
        HeightRes = terrian.terrainData.heightmapResolution;
        Mesh = terrian.terrainData.GetHeights(
            0,
            0,
            HeightRes,
            HeightRes);
    }

    private void RedrawTerrainMesh()
    {
        Mesh = new float[HeightRes, HeightRes];
        for (int x = 0; x < HeightRes; x++)
        {
            for (int y = 0; y < HeightRes; y++)
            {
                float[] flux = new float[PerlinStretch.Length];
                float[] perlin = new float[PerlinStretch.Length];
                flux[0] = PerlinFluxSlope.Evaluate(Mathf.PerlinNoise(x * PerlinFluxStretch, y * PerlinFluxStretch));
                for (int i = 1; i < PerlinStretch.Length; i++)
                {
                    flux[i] = (1 / PerlinStretch.Length) - flux[i - 1];
                }
                for (int i = 0; i < PerlinStretch.Length; i++)
                {
                    perlin[i] = PerlinSlope[i].Evaluate(Mathf.PerlinNoise(x * PerlinStretch[i], y * PerlinStretch[i]));
                    
                }
                try
                {
                    for (int i = 1; i < perlin.Length; i++)
                    {
                        Mesh[x, y] += (perlin[i] * flux[i] + perlin[i - 1] * perlin[i - 1]) * EdgeReductionMap[x,y];
                    }
                }
                catch
                {
                    Mesh[x, y] = 0;
                }
            }
        }
    }
    public void UpdateHeightMap()
    {
        terrian.terrainData.SetHeights(0, 0, Mesh);
    }
    public int GetHeightResolution()
    {
        return terrian.terrainData.heightmapResolution;
    }
        
}
