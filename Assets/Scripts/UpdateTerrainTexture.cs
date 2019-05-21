using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTerrainTexture : MonoBehaviour
{
    Terrain theTerrain;
    public Texture2D[] part1Colors, part2aColors, part2bColors, part3Colors;
    public GameObject[] part1Trees, part2aTrees, part2bTrees, part3Trees;
    private static List< UpdateTerrainTexture > mes = null;

    public static void Part1()
    {
        foreach( UpdateTerrainTexture t in mes )
        {
            t.SetColors( t.part1Colors );
            t.SetTrees( t.part1Trees );
        }
    }

    public static void Part2a()
    {
        foreach( UpdateTerrainTexture t in mes )
        {
            t.SetColors( t.part2aColors );
            t.SetTrees( t.part2aTrees );
        }
    }

    public static void Part2b()
    {
        foreach( UpdateTerrainTexture t in mes )
        {
            t.SetColors( t.part2bColors );
            t.SetTrees( t.part2bTrees );
        }
    }

    public static void Part3()
    {
        foreach( UpdateTerrainTexture t in mes )
        {
            t.SetColors( t.part3Colors );
            t.SetTrees( t.part3Trees );
        }
    }

    void Start()
    {
        theTerrain = GetComponent<Terrain>();
        if( mes == null ) { mes = new List<UpdateTerrainTexture>(); }
        mes.Add( this );
        
        Part1();
    }

    void Update()
    {
        if( Input.GetKeyDown( "c" ) )
        {
            Part2a();
        }
        if( Input.GetKeyDown( "v" ) )
        {
            Part2b();
        }
        if( Input.GetKeyDown( "b" ) )
        {
            Part3();
        }
        if( Input.GetKeyDown( "n" ) )
        {
            Part1();
        }
    }

    void SetColors( Texture2D[] colors )
    {
        // fetch
        TerrainLayer[] layers = theTerrain.terrainData.terrainLayers;

        // replace
        for( int i = 0; i < layers.Length; i++ )
        {
            layers[i].diffuseTexture = colors[ i % colors.Length ];
        }
    }

    void SetTrees( GameObject[] treePrefabs )
    {
        // fetch
        TreePrototype[] trees = theTerrain.terrainData.treePrototypes;

        // update
        for( int i = 0; i < trees.Length; i++ )
        {
            trees[i].prefab = treePrefabs[ i % treePrefabs.Length ];
        }

        // replace
        theTerrain.terrainData.treePrototypes = trees;
    }

    void OnApplicationQuit()
    {
        Part1();
    }
}
