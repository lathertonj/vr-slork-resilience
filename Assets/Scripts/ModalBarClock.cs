using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalBarClock : MonoBehaviour
{
    public OSCHeadBroadcaster myOSC;
    private ChuckSubInstance myChuck;
    private ChuckEventListener myClockListener;
    public float[] myArpeggio;
    private int currentArpeggioNote;
    bool hardPick = true;

    // Start is called before the first frame update
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
        currentArpeggioNote = 0;
        myChuck.RunCode( @"
            global Event modalBarClock;
            0.125 => global float noteLengthSeconds;
            while( true )
            {
                modalBarClock.broadcast();
                noteLengthSeconds::second => now;
            }
        " );
        myClockListener = gameObject.AddComponent<ChuckEventListener>();
        myClockListener.ListenForEvent( myChuck, "modalBarClock", SendNextArpeggioNote );
    }

    void SendNextArpeggioNote()
    {
        // pick note, strike position, and strike strength
        float modalNote = myArpeggio[ currentArpeggioNote ];
        currentArpeggioNote = ( currentArpeggioNote + 1 ) % myArpeggio.Length;

        float strikePosition = Random.Range( 0.2f, 0.8f );

        float strike = Random.Range( 0.3f, 0.4f ) + ( hardPick ? 0.17f : 0 );

        // TODO: send to only one listener
        myOSC.SendMessage( "playModal", modalNote, strike, strikePosition );
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
