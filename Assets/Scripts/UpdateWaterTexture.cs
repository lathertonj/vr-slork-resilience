using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWaterTexture : MonoBehaviour
{
    
    
    public Material part1, part2a, part2b, part3;
    private MeshRenderer me;

    private static List< UpdateWaterTexture > mes = null;

    public static void Part1()
    {
        foreach( UpdateWaterTexture t in mes )
        {
            t.SetMaterial( t.part1 );
        }
    }

    public static void Part2a()
    {
        foreach( UpdateWaterTexture t in mes )
        {
            t.SetMaterial( t.part2a );
        }
    }

    public static void Part2b()
    {
        foreach( UpdateWaterTexture t in mes )
        {
            t.SetMaterial( t.part2b );
        }
    }

    public static void Part3()
    {
        foreach( UpdateWaterTexture t in mes )
        {
            t.SetMaterial( t.part3 );
        }
    }
    void Start()
    {
        me = GetComponent<MeshRenderer>();
        if( mes == null ) { mes = new List< UpdateWaterTexture>(); }
        mes.Add( this );
        SetMaterial( part1 );
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

    void SetMaterial( Material m )
    {
        me.material = m;
    }
}
