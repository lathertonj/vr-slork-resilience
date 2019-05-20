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
    private ConstantDirectionMover room;

    public float maxRoomSpeed;
    public float slowSpeedNormCutoff;
    public float fastSpeedNormCutoff;

    void Start()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        handType = pose.inputSource;
        room = GetComponentInParent<ConstantDirectionMover>();
    }

    void Update()
    {
        if( IsFirstHeldDown() )
        {
            // tell chuck
            theChuck.BroadcastEvent( "wavingHandOn" );
        }

        if( IsHeldDown() )
        {
            // tell chuck; normalize: map clamp to 0 to 1
            theChuck.SetFloat( "wavingHandIntensity", NormVelocityMagnitude() );
        }

        if( IsUnheldDown() )
        {
            // tell chuck
            theChuck.BroadcastEvent( "wavingHandOff" );
            theChuck.SetFloat( "wavingHandIntensity", 0 );

            // move the room
            Vector3 v = GetVelocity().normalized;
            float magnitude = NormVelocityMagnitude();
            room.SetDirection( v, magnitude * maxRoomSpeed );

            // tell chuck more things
            if( magnitude < slowSpeedNormCutoff )
            {
                theChuck.BroadcastEvent( "part2DecreaseTempo" );
            }
            else if( magnitude > fastSpeedNormCutoff )
            {
                theChuck.BroadcastEvent( "part2IncreaseTempo" );
            }
        }
    }

    private Vector3 GetVelocity()
    {
        return pose.GetVelocity();
    }

    private float NormVelocityMagnitude()
    {
        return GetVelocity().magnitude.MapClamp( 0, 1.45f, 0, 1 );
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
