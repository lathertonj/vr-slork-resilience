using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Part2CommunicateWavingHand : MonoBehaviour
{

    public SteamVR_Action_Boolean holdWindGestureAction;
    public SteamVR_Action_Boolean overrideGestureAction;
    public SteamVR_Input_Sources overrideHandType;
    
    private SteamVR_Input_Sources handType;
    private SteamVR_Behaviour_Pose pose;
    public ChuckSubInstance theChuck;
    private ConstantDirectionMover room;

    public float maxRoomSpeed;
    public float slowSpeedNormCutoff;
    public float fastSpeedNormCutoff;
    public static Vector3 mostRecentGestureDirection;
    public Transform overrideGestureLocation;
    private bool headingTowardOverrideLocation = false;

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
            Vector3 v = Vector3.zero;
            float magnitude = NormVelocityMagnitude();

            if( !ShouldOverrideGesture() )
            {
                v = room.transform.TransformDirection( GetVelocity().normalized );
                headingTowardOverrideLocation = false;
            }
            else
            {
                // override: go toward location, at top speed
                v = ( overrideGestureLocation.position - room.transform.position ).normalized;
                magnitude = 1;
                headingTowardOverrideLocation = true;
            }
            // get gesture direction in world space
            room.SetDirection( v, magnitude * maxRoomSpeed );

            // save it for others. yay hackiness
            mostRecentGestureDirection = v * magnitude;

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

        if( headingTowardOverrideLocation && CloseToOverrideLocation() )
        {
            room.GetComponent<Part23MoveRoom>().TransitionFromPart2To3();
        }
    }

    private bool CloseToOverrideLocation()
    {
        return ( room.transform.position - overrideGestureLocation.position ).magnitude < 0.1f;
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

    bool ShouldOverrideGesture()
    {
        return overrideGestureAction.GetState( overrideHandType );
    }
}
