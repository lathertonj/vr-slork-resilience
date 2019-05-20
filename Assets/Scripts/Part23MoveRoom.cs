using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part23MoveRoom : MonoBehaviour
{
    public Transform seedlings;
    public Transform pointOnIslandToMoveTo;
    public SLOrkVR2019OscCommunications theChucker;
    public float userFallTime = 2f;
    private bool haveStarted = false;
    private Rigidbody[] seedlingsRBs = null;
    
    void Start()
    {
        // Initialize gravity to be normal
        Physics.gravity = -9.81f * Vector3.up;
    }

    public void TransitionFromPart2To3()
    {
        if( haveStarted ) { return; }
        haveStarted = true;

        // turn off constant direction movement
        GetComponent<ConstantDirectionMover>().enabled = false;

        // unparent seedings to let them fall independently of me
        seedlings.parent = null;

        seedlingsRBs = seedlings.GetComponentsInChildren<Rigidbody>();

        // add gravity to seedlings
        foreach( Rigidbody seedling in seedlingsRBs )
        {
            seedling.useGravity = true;
            // TODO: mess with other seedling settings?
        }

        // decrease gravity LATER for seedlings to look different?
        Invoke( "DecreaseGravity", 3.5f );

        // start raining LATER
        Invoke( "StartRaining", 4f );

        // tell the chuck controller to move on
        theChucker.PlayFinalChuckLightning();

        // move me toward the ground
        StartCoroutine( MoveToPosition( pointOnIslandToMoveTo.position, userFallTime ) );
    }

    public IEnumerator MoveToPosition( Vector3 position, float timeToMove )
    {
        Vector3 startPos = transform.position;
        float t = 0f;
        while( t < 1 )
        {
            t += Time.deltaTime / timeToMove;
            transform.position = Vector3.Lerp( startPos, position, t );
            yield return null;
        }
        transform.position = position;
    }

    private void DecreaseGravity()
    {
        Physics.gravity = -3f * Vector3.up;
    }

    public ParticleSystem rain;

    private void StartRaining()
    {
        rain.Play();
    }
}
