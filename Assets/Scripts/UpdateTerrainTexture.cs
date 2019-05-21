using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTerrainTexture : MonoBehaviour
{
    Terrain theTerrain;
    public Texture2D[] part1Colors, part2aColors, part2bColors, part3Colors;
    private static List< UpdateTerrainTexture > mes = null;

    public static void Part1()
    {
        foreach( UpdateTerrainTexture t in mes )
        {
            t.SetColors( t.part1Colors );
        }
    }

    public static void Part2a()
    {
        foreach( UpdateTerrainTexture t in mes )
        {
            t.SetColors( t.part2aColors );
        }
    }

    public static void Part2b()
    {
        foreach( UpdateTerrainTexture t in mes )
        {
            t.SetColors( t.part2bColors );
        }
    }

    public static void Part3()
    {
        foreach( UpdateTerrainTexture t in mes )
        {
            t.SetColors( t.part3Colors );
        }
    }

    //TODO: tree prototypes
    // modify prototypes prefabs
    // then call terrainData.RefreshPrototypes()
    // Start is called before the first frame update
    void Start()
    {
        theTerrain = GetComponent<Terrain>();
        if( mes == null ) { mes = new List<UpdateTerrainTexture>(); }
        mes.Add( this );
        SetColors( part1Colors );
    }

    // Update is called once per frame
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
        TerrainLayer[] layers = theTerrain.terrainData.terrainLayers;
        for( int i = 0; i < layers.Length; i++ )
        {
            layers[i].diffuseTexture = colors[ i % colors.Length ];
        }
    }

    void OnApplicationQuit()
    {
        SetColors( part1Colors );
    }
}
