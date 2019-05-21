using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSeedlingsPart3 : MonoBehaviour
{
    public ChuckSubInstance theChuck;
    public OSCSendReceiver osc;
    private Rigidbody[] mySeedlings;
    private int currentSeedling = 0;
    private ParticleSystem myParticleEmitter;
    private Chuck.IntCallback myJumpIDGetter, mySwellIDGetter;
    private bool shouldLaunchASeedling = false, shouldBuryASeedling = false;
    private int jumpSeedlingID = 0, burySeedlingID = 0;
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
        myJumpIDGetter = new Chuck.IntCallback( StoreJumpID );
        mySwellIDGetter = new Chuck.IntCallback( StoreSwellID );

        // for random sequential seedling launching
        // gameObject.AddComponent<ChuckEventListener>().ListenForEvent( theChuck, "part3SeedlingNotePlayed", LaunchASeedling );
        // for ID-based seedling launching
        gameObject.AddComponent<ChuckEventListener>().ListenForEvent( theChuck, "part3SeedlingNotePlayed", RespondToSeedlingJump );
        gameObject.AddComponent<ChuckEventListener>().ListenForEvent( theChuck, "part3SwellPlayed", RespondToSwell );
    }

    void AnimateSeedlingJump()
    {
        currentSeedling = ( jumpSeedlingID + ( Random.Range( 0, 3 ) * osc.NumListeners() ) ) % mySeedlings.Length;
        LaunchASeedling();
    }

    void AnimateSwell()
    {
        int whichSeedling = ( jumpSeedlingID + ( Random.Range( 0, 3 ) * osc.NumListeners() ) ) % mySeedlings.Length;
        BuryASeedling( whichSeedling );
    }

    void StoreJumpID( long performerID )
    {
        jumpSeedlingID = (int)performerID;
        shouldLaunchASeedling = true;
    }

    void StoreSwellID( long performerID )
    {
        burySeedlingID = (int)performerID;
        shouldBuryASeedling = true;
    }

    void RespondToSeedlingJump()
    {
        theChuck.GetInt( "part3JumpID", myJumpIDGetter );
    }

    void RespondToSwell()
    {
        theChuck.GetInt( "part3SwellID", mySwellIDGetter );
    }

    void LaunchASeedling()
    {
        // TODO: don't do anything if this seedling has been buried already
        Rigidbody seedling = mySeedlings[currentSeedling];

        Vector3 seedlingVelocity = upwardForce * Vector3.up + 0.02f * RandomVector3();
        seedling.AddForce( seedlingVelocity, ForceMode.VelocityChange );

        Vector3 randomAngularVelocity = RandomVector3();
        seedling.AddTorque( randomAngularVelocity, ForceMode.VelocityChange );


        // animate particle
        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
        emitParams.position = myParticleEmitter.transform.InverseTransformPoint( seedling.position );
        emitParams.velocity = seedlingVelocity; // take only the y component?
        myParticleEmitter.Emit( emitParams, count: 1 );

        currentSeedling++; currentSeedling %= mySeedlings.Length;
    }

    void BuryASeedling( int whichSeedling )
    {
        // first check if seedling has been buried already and if so, ONLY play the particle
		Rigidbody seedling = mySeedlings[ whichSeedling ];
		if( !haveSeedlingsBeenBuried[ whichSeedling] )
		{
			// burial action: rigidbody is kinematic; move down X in Y seconds
			seedling.isKinematic = true;
			seedling.useGravity = false;
			StartCoroutine( MoveToPosition( seedling.transform, seedlingBurialDistance, seedlingBurialTime ) );

			// remember
			haveSeedlingsBeenBuried[ whichSeedling ] = true;
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

    void Update()
    {
        if( shouldLaunchASeedling )
        {
            shouldLaunchASeedling = false;
            AnimateSeedlingJump();
        }

        if( shouldBuryASeedling )
        {
            shouldBuryASeedling = false;
            AnimateSwell();
        }
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
