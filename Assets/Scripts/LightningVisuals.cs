using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LightningVisuals : MonoBehaviour
{
    public Color lightningColor;
    public float holdTime = 0.3f;
    public float releaseTime = 1.2f;
    private Color myClear;
    // Start is called before the first frame update
    void Start()
    {
        myClear = new Color( lightningColor.r, lightningColor.g, lightningColor.b, 0 );
    }

    public void TriggerLightning( float intensity )
    {
        // clamp intensity
        intensity = Mathf.Clamp01( intensity );

        // trigger color immediately
        SteamVR_Fade.Start( new Color( lightningColor.r, lightningColor.g, lightningColor.b, intensity ), 0 );

        // in holdTime seconds, slowly fade away
        Invoke( "ResetColor", holdTime );
    }

    private void ResetColor()
    {
        SteamVR_Fade.Start( myClear, releaseTime );
    }
}
