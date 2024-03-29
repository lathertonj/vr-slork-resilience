﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part1MoveRoom : MonoBehaviour
{
    public ChuckSubInstance theChuck;
    private int numSeedlingsLaunched = 0;
    private Vector3 currentLocation, goalLocation;
    private float locationSlew = 1;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.AddComponent<ChuckEventListener>().ListenForEvent( theChuck, "part1SeedlingNotePlayed", RespondToSeedlingLaunch );
        gameObject.AddComponent<ChuckEventListener>().ListenForEvent( theChuck, "startPart2", RespondToStartPart2 );
        currentLocation = goalLocation = transform.position;
    }

    void RespondToSeedlingLaunch()
    {
        numSeedlingsLaunched++;
        if( numSeedlingsLaunched >= 5 )
        {
            goalLocation += 0.2f * Vector3.up;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentLocation += locationSlew * Time.deltaTime * ( goalLocation - currentLocation );
        transform.position = currentLocation;
    }

    void RespondToStartPart2()
    {
        // turn off part 1 movement mechanism at the start of part 2
        Destroy( this );
    }
}
