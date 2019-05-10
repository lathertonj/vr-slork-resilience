using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalBarClock : MonoBehaviour
{
    public OSCHeadBroadcaster myOSC;
    private ChuckSubInstance myChuck;

    public string[] myModalNotesPart1;
    public string[] myAhhNotesPart1a1;
    public string[] myAhhNotesPart1a2;

    public string[] myArpeggioPart2a;
    public string[] myAhhNotesPart2a1;
    public string[] myAhhNotesPart2a2;
    int nextMovementToInit = 1;

    // Start is called before the first frame update
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();        
    }

    // Update is called once per frame
    void Update()
    {
        if( Input.GetKeyDown( "space" ) )
        {
            switch( nextMovementToInit )
            {
                case 1:
                    StartChuckPart1();
                    break;
                case 2:
                    StartChuckPart2();
                    break;
                default:
                    break;
            }
            nextMovementToInit++;
        }
    }

    void StartChuckPart1()
    {
        // part 1: advertise the next note   
        myChuck.RunCode( myOSC.GenerateChucKCode( "vrSays", "vrHear" ) + string.Format( @"
            [{0}] @=> int myArpeggio[];
            [[{1}], [{2}]] @=> int myAhhNotes[][];
            int myCurrentNote;

            fun void ListenForPlayedNotes()
            {{
                vrHear.event( ""/part1/playedSeedlingNote"", ""i"" ) @=> OscEvent someonePlayedANote;

                while( true )
                {{
                    someonePlayedANote => now;
                    while( someonePlayedANote.nextMsg() != 0 )
                    {{
                        // ignore argument
                        someonePlayedANote.getInt();

                        ( myCurrentNote + 1 ) % myArpeggio.size() => myCurrentNote;
                    }}
                }}
            }}
            spork ~ ListenForPlayedNotes();

            fun void ReplaceAhhNotesForPart2()
            {{
                global Event startPart2;
                startPart2 => now;
                [[{3}], [{4}]] @=> myAhhNotes;
            }}
            spork ~ ReplaceAhhNotesForPart2();

            while( true )
            {{
                for( int i; i < vrSays.size(); i++ )
                {{
                    // send out the next note someone should play
                    vrSays[i].startMsg( ""/part1/nextSeedlingNote"", ""f"" );
                    vrSays[i].addFloat( myArpeggio[myCurrentNote] );

                    // send out the current ahh chord notes
                    // TODO: any chord changes should happen HERE and not in 
                    // Part2, etc.
                    vrSays[i].startMsg( ""/ahhNotes"", ""f,f,f,f,f,f,f,f"" );
                    vrSays[i].addFloat( myAhhNotes[0][0] );
                    vrSays[i].addFloat( myAhhNotes[0][1] );
                    vrSays[i].addFloat( myAhhNotes[0][2] );
                    vrSays[i].addFloat( myAhhNotes[0][3] );
                    vrSays[i].addFloat( myAhhNotes[1][0] );
                    vrSays[i].addFloat( myAhhNotes[1][1] );
                    vrSays[i].addFloat( myAhhNotes[1][2] );
                    vrSays[i].addFloat( myAhhNotes[1][3] );

                }}
                10::ms => now;
            }}
        ", 
            string.Join( ", ", myModalNotesPart1 ), 
            string.Join( ", ", myAhhNotesPart1a1 ),
            string.Join( ", ", myAhhNotesPart1a2 ),
            string.Join( ", ", myAhhNotesPart2a1 ),
            string.Join( ", ", myAhhNotesPart2a2 )
        ) );
    }

    void StartChuckPart2()
    {
        // inform above code to change chords
        myChuck.SignalEvent( "startPart2" );
        
        // regular modal clock, to be used in part 2
        myChuck.RunCode( myOSC.GenerateChucKCode( "vrSays", "vrHear" ) + string.Format( @"
            [{0}] @=> int myArpeggio[];
            0 => int currentReceiver;

            // advance to part 2
            for( int i; i < vrSays.size(); i++ )
            {{
                vrSays[ i ].startMsg( ""/advanceToPart2"", ""i"" );
                vrSays[ i ].addInt( 1 );
            }}

            0.21 => global float noteLengthSeconds;
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
        ", string.Join( ", ", myArpeggioPart2a ) ) );
        

        
    }
}
