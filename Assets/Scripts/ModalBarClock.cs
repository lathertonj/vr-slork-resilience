using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalBarClock : MonoBehaviour
{
    public OSCHeadBroadcaster myOSC;
    private ChuckSubInstance myChuck;
    public string[] myArpeggio;
    bool haveInitChuck = false;

    // Start is called before the first frame update
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();        
    }

    // Update is called once per frame
    void Update()
    {
        if( !haveInitChuck  && Input.GetKeyDown( "space" ) )
        {
            StartChuck();
            haveInitChuck = true;
        }
    }

    void StartChuck()
    {
        myChuck.RunCode( myOSC.GenerateChucKCode( "vrSays", "vrHear" ) + string.Format( @"
            [{0}] @=> int myArpeggio[];
            0 => int currentReceiver;

            0.125 => global float noteLengthSeconds;
            true => int hardPick;
            while( true )
            {{
                for( int i; i < myArpeggio.size(); i++ )
                {{
                    // play note
                    vrSays[ currentReceiver ].startMsg( ""/playModal"", ""f,f,f"" );
                    vrSays[ currentReceiver ].addFloat( myArpeggio[i] );
                    vrSays[ currentReceiver ].addFloat( Math.random2f( 0.2, 0.8 ) );
                    vrSays[ currentReceiver ].addFloat( Math.random2f( 0.3, 0.4 ) + 0.17 * hardPick );

                    // switch for next time
                    !hardPick => hardPick;

                    // find next receiver
                    currentReceiver++;
                    currentReceiver % vrSays.size() => currentReceiver;

                    // wait
                    noteLengthSeconds::second => now;
                }}
            }}
        ", string.Join( ", ", myArpeggio ) ) );
    }
}
