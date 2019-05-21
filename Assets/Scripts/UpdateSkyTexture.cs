using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UpdateSkyTexture : MonoBehaviour
{
    
    public Material part1, part2a, part2b, part3;
    public float skyFlashTime = 2f;

    private static UpdateSkyTexture me = null;

    public static void Part1()
    {
        me.SetMaterial( me.part1 );
    }

    public static void Part2a()
    {
        me.SetMaterial( me.part2a );
    }

    public static void Part2b()
    {
        // don't flash because lightning will be flashing
        me.SetMaterial( me.part2b, false );
    }

    public static void Part3()
    {
        // don't flash because lightning will be flashing
        me.SetMaterial( me.part3, false );
    }

    void Start()
    {
        me = this;
        SetMaterial( part1, false );
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

    void SetMaterial( Material m, bool flashColor = true )
    {
        // set sky color
        RenderSettings.skybox = m;

        if( flashColor )
        {
            // do a flash of other color
            SteamVR_Fade.Start( m.GetColor( "_Tint" ), 0 );

            // back to clear
            SteamVR_Fade.Start( Color.clear, skyFlashTime );
        }
    }
}
