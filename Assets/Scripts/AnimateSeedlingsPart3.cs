using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSeedlingsPart3 : MonoBehaviour
{
    public ChuckSubInstance theChuck;
    public OSCSendReceiver osc;
    private Rigidbody[] mySeedlings;
    private int currentJumpSeedling = 0, currentBurySeedling = 0;
    private ParticleSystem myParticleEmitter;
    private bool[] haveSeedlingsBeenBuried;
    public float upwardForce = 1.8f;
    public Vector3 seedlingBurialDistance;
    public float seedlingBurialTime;
    public Color seedlingBurialColor;
    public float seedlingBurialParticleStartSize;
    public float seedlingBurialParticleLifetime;
    // Start is called before the first frame update
    void Start()
    {
        mySeedlings = GetComponentsInChildren<Rigidbody>();
        haveSeedlingsBeenBuried = new bool[mySeedlings.Length];
        for( int i = 0; i < haveSeedlingsBeenBuried.Length; i++ ) { haveSeedlingsBeenBuried[i] = false; }

        myParticleEmitter = GetComponent<ParticleSystem>();

        // for random sequential seedling launching
        gameObject.AddComponent<ChuckEventListener>().ListenForEvent( theChuck, "part3SeedlingNotePlayed", LaunchASeedling );
        gameObject.AddComponent<ChuckEventListener>().ListenForEvent( theChuck, "part3SwellPlayed", SwellASeedling );
    }

    void AdvanceBurialIndexToSeedlingNotYetBuried()
    {
        int previousBurySeedling = currentBurySeedling;
        for( int i = 1; i <= mySeedlings.Length; i++ )
        {
            int potentialNextSeedling = ( previousBurySeedling + i ) % mySeedlings.Length;
            if( !haveSeedlingsBeenBuried[potentialNextSeedling] )
            {
                currentBurySeedling = potentialNextSeedling;
                return;
            }
        }

        // if none exist, find a buried one instead
        AdvanceBurialIndexToBuriedSeedling();
    }

    void AdvanceBurialIndexToBuriedSeedling()
    {
        int previousBurySeedling = currentBurySeedling;
        for( int i = 1; i <= mySeedlings.Length; i++ )
        {
            int potentialNextSeedling = ( previousBurySeedling + i ) % mySeedlings.Length;
            if( haveSeedlingsBeenBuried[potentialNextSeedling] )
            {
                currentBurySeedling = potentialNextSeedling;
                return;
            }
        }

        // if none exist, find an unburied one instead
        AdvanceBurialIndexToSeedlingNotYetBuried();
    }

    void AdvanceJumpIndexToUnburiedSeedling()
    {
        int previousSeedling = currentJumpSeedling;
        for( int i = 1; i <= mySeedlings.Length; i++ )
        {
            int potentialNextSeedling = ( previousSeedling + i ) % mySeedlings.Length;
            if( !haveSeedlingsBeenBuried[potentialNextSeedling] )
            {
                currentJumpSeedling = potentialNextSeedling;
                return;
            }
        }

        // if none exist, should choose -1 instead; will stop animating jumps
        currentJumpSeedling = -1;
    }

    void SwellASeedling()
    {
        // TODO: dial frequency up or down depending on rehearsal
        if( Random.Range( 0f, 1f ) < 0.45f )
        {
            // X% of time, bury a new seedling
            AdvanceBurialIndexToSeedlingNotYetBuried();
        }
        else 
        {
            // Rest of time, swell an old seedling
            AdvanceBurialIndexToBuriedSeedling();
        }

        BuryASeedling( currentBurySeedling );
    }



    void LaunchASeedling()
    {
        if( currentJumpSeedling < 0 )
        {
            return;
        }

        Rigidbody seedling = mySeedlings[currentJumpSeedling];

        Vector3 seedlingVelocity = upwardForce * Vector3.up + 0.02f * RandomVector3();
        seedling.AddForce( seedlingVelocity, ForceMode.VelocityChange );

        Vector3 randomAngularVelocity = RandomVector3();
        seedling.AddTorque( randomAngularVelocity, ForceMode.VelocityChange );


        // animate particle
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = myParticleEmitter.transform.InverseTransformPoint( seedling.position );
        emitParams.velocity = seedlingVelocity; // take only the y component?
        myParticleEmitter.Emit( emitParams, count: 1 );

        AdvanceJumpIndexToUnburiedSeedling();
    }

    void BuryASeedling( int whichSeedling )
    {
        // first check if seedling has been buried already and if so, ONLY play the particle
        Rigidbody seedling = mySeedlings[whichSeedling];
        if( !haveSeedlingsBeenBuried[whichSeedling] )
        {
            // burial action: rigidbody is kinematic; move down X in Y seconds
            seedling.isKinematic = true;
            seedling.useGravity = false;
            StartCoroutine( MoveToPosition( seedling.transform, seedlingBurialDistance, seedlingBurialTime ) );

            // remember
            haveSeedlingsBeenBuried[whichSeedling] = true;
        }

        // always: emit particle
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = myParticleEmitter.transform.InverseTransformPoint( seedling.position );
        emitParams.velocity = 0.05f * Vector3.up;
        emitParams.startSize = seedlingBurialParticleStartSize;
        emitParams.startColor = seedlingBurialColor;
        emitParams.startLifetime = seedlingBurialParticleLifetime;
        myParticleEmitter.Emit( emitParams, count: 1 );
    }

    public IEnumerator MoveToPosition( Transform seedling, Vector3 relativePosition, float timeToMove )
    {
        Vector3 startPos = seedling.position;
        float t = 0f;
        while( t < 1 )
        {
            t += Time.deltaTime / timeToMove;
            seedling.position = Vector3.Lerp( startPos, startPos + relativePosition, t );
            yield return null;
        }
        seedling.position = startPos + relativePosition;
    }

    Vector3 RandomVector3()
    {
        return new Vector3(
            Random.Range( -1f, 1f ),
            Random.Range( -1f, 1f ),
            Random.Range( -1f, 1f )
        );
    }
}
