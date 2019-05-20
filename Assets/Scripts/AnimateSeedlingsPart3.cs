using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSeedlingsPart3 : MonoBehaviour
{
    public ChuckSubInstance theChuck;
    private Rigidbody[] mySeedlings;
    private int currentSeedling = 0;
    private ParticleSystem myParticleEmitter;
	public float upwardForce = 1.8f;
    // Start is called before the first frame update
    void Start()
    {
        mySeedlings = GetComponentsInChildren<Rigidbody>();
        myParticleEmitter = GetComponent<ParticleSystem>();
        gameObject.AddComponent<ChuckEventListener>().ListenForEvent( theChuck, "part3SeedlingNotePlayed", LaunchASeedling );
    }

    void LaunchASeedling()
    {
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

    Vector3 RandomVector3()
	{
		return new Vector3(
			Random.Range( -1f, 1f ),
			Random.Range( -1f, 1f ),
			Random.Range( -1f, 1f )
		);
	}
}
