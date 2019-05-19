using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Part2CommunicateWavingHand : MonoBehaviour
{

    public SteamVR_Action_Boolean holdWindGestureAction;
    private SteamVR_Input_Sources handType;
    private SteamVR_Behaviour_Pose pose;
    public ChuckSubInstance theChuck;

    void Start()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        handType = pose.inputSource;
    }

    void Update()
    {
        if( IsFirstHeldDown() )
        {
            theChuck.BroadcastEvent( "wavingHandOn" );
        }

        if( IsHeldDown() )
        {
            // normalize: map clamp to 0 to 1
            theChuck.SetFloat( "wavingHandIntensity", pose.GetVelocity().magnitude.MapClamp( 0, 1.45f, 0, 1 ) );
        }

        if( IsUnheldDown() )
        {
            theChuck.BroadcastEvent( "wavingHandOff" );
            theChuck.SetFloat( "wavingHandIntensity", 0 );
        }
    }

    bool IsFirstHeldDown()
    {
        return holdWindGestureAction.GetStateDown( handType );
    }

    bool IsHeldDown()
    {
        return holdWindGestureAction.GetState( handType );
    }

    bool IsUnheldDown()
    {
        return holdWindGestureAction.GetStateUp( handType );
    }
}
