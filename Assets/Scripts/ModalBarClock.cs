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
    public string[] mySawNotesPart2a1;
    public string[] mySawNotesPart2a2;
    public string[] myAhhNotesPart2b1;
    public string[] myAhhNotesPart2b2;
    public string[] mySawNotesPart2b1;
    public string[] mySawNotesPart2b2;
    public string[] myAhhNotesPart3a1;
    public string[] myAhhNotesPart3a2;
    public string[] mySawNotesPart3a1;
    public string[] mySawNotesPart3a2;
    public string[] myArpeggioPart3a;
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
                case 3:
                    StartChuckPart3();
                    break;
                default:
                    break;
            }
            nextMovementToInit++;
        }

        if( Input.GetKeyDown( "1" ) )
        {
            myChuck.SetFloat( "part2DistortionAmount", 0.0 );
        }
        if( Input.GetKeyDown( "2" ) )
        {
            myChuck.SetFloat( "part2DistortionAmount", 0.25 );
        }
        if( Input.GetKeyDown( "3" ) )
        {
            myChuck.SetFloat( "part2DistortionAmount", 0.5 );
        }
        if( Input.GetKeyDown( "4" ) )
        {
            myChuck.SetFloat( "part2DistortionAmount", 0.75 );
        }
        if( Input.GetKeyDown( "5" ) )
        {
            myChuck.SetFloat( "part2DistortionAmount", 1.0 );
        }
        if( Input.GetKeyDown( "6" ) )
        {
            myChuck.BroadcastEvent( "part2bChords" );
        }


    }

    void StartChuckPart1()
    {
        // part 1: advertise the next note   
        myChuck.RunCode( myOSC.GenerateChucKCode( "vrSays", "vrHear" ) + string.Format( @"
            [{0}] @=> int myArpeggio[];
            [[{1}], [{2}]] @=> int myAhhNotes[][];
            [[{5}], [{6}]] @=> int mySawNotes[][];
            int myCurrentNote;

            global float currentWindExcitation;
            0.97 => float windDecay;

            // advance to part 1
            for( int i; i < vrSays.size(); i++ )
            {{
                vrSays[ i ].startMsg( ""/advanceToPart1"", ""i"" );
                vrSays[ i ].addInt( 1 );
            }}

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

            fun void ListenForWindExcitation()
            {{
                vrHear.event( ""/part1/excitation"", ""f"" ) @=> OscEvent windExcitation;
                while( true )
                {{
                    windExcitation => now;
                    while( windExcitation.nextMsg() != 0 )
                    {{
                        windExcitation.getFloat() +=> currentWindExcitation;
                        windDecay *=> currentWindExcitation;
                    }}
                }}
            }}
            spork ~ ListenForWindExcitation();

            fun void ReplaceAhhNotesForPart2()
            {{
                global Event startPart2;
                startPart2 => now;
                [[{3}], [{4}]] @=> myAhhNotes;
            }}
            spork ~ ReplaceAhhNotesForPart2();

            fun void ReplaceNotesForPart2b()
            {{
                global Event part2bChords;
                part2bChords => now;
                [[{11}], [{12}]] @=> myAhhNotes;
                [[{13}], [{14}]] @=> mySawNotes;
            }}
            spork ~ ReplaceNotesForPart2b();

            global Event startPart3;
            fun void ReplaceNotesForPart3()
            {{
                startPart3 => now;
                [[{7}], [{8}]] @=> myAhhNotes;
                [[{9}], [{10}]] @=> mySawNotes;
            }}
            spork ~ ReplaceNotesForPart3();

            while( true )
            {{
                for( int i; i < vrSays.size(); i++ )
                {{
                    // send out the next note someone should play
                    vrSays[i].startMsg( ""/part1/nextSeedlingNote"", ""f"" );
                    vrSays[i].addFloat( myArpeggio[myCurrentNote] );

                    // send out the current chord notes
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

                    vrSays[i].startMsg( ""/sawNotes"", ""f,f,f,f,f,f,f,f,f,f"" );
                    vrSays[i].addFloat( mySawNotes[0][0] );
                    vrSays[i].addFloat( mySawNotes[0][1] );
                    vrSays[i].addFloat( mySawNotes[0][2] );
                    vrSays[i].addFloat( mySawNotes[0][3] );
                    vrSays[i].addFloat( mySawNotes[0][4] );
                    vrSays[i].addFloat( mySawNotes[1][0] );
                    vrSays[i].addFloat( mySawNotes[1][1] );
                    vrSays[i].addFloat( mySawNotes[1][2] );
                    vrSays[i].addFloat( mySawNotes[1][3] );
                    vrSays[i].addFloat( mySawNotes[1][4] );

                }}
                10::ms => now;
            }}
        ", 
            string.Join( ", ", myModalNotesPart1 ), 
            string.Join( ", ", myAhhNotesPart1a1 ),
            string.Join( ", ", myAhhNotesPart1a2 ),
            string.Join( ", ", myAhhNotesPart2a1 ),
            string.Join( ", ", myAhhNotesPart2a2 ),
            string.Join( ", ", mySawNotesPart2a1 ),
            string.Join( ", ", mySawNotesPart2a2 ),
            string.Join( ", ", myAhhNotesPart3a1 ),
            string.Join( ", ", myAhhNotesPart3a2 ),
            string.Join( ", ", mySawNotesPart3a1 ),
            string.Join( ", ", mySawNotesPart3a2 ),
            string.Join( ", ", myAhhNotesPart2b1 ),
            string.Join( ", ", myAhhNotesPart2b2 ),
            string.Join( ", ", mySawNotesPart2b1 ),
            string.Join( ", ", mySawNotesPart2b2 )
        ) );
    }

    void StartChuckPart2()
    {
        // inform above code to change chords
        myChuck.BroadcastEvent( "startPart2" );
        
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

            global float part2DistortionAmount;
            fun void SendDistortionAmount()
            {{
                while( true )
                {{
                    for( int i; i < vrSays.size(); i++ )
                    {{
                        vrSays[ i ].startMsg( ""/part2Distortion"", ""f"" );
                        vrSays[ i ].addFloat( part2DistortionAmount );
                    }}
                    50::ms => now;
                    <<< part2DistortionAmount >>>;
                }}
            }}
            spork ~ SendDistortionAmount();

            // TODO: timing based on VR
            0.21 => global float noteLengthSeconds;
            true => int hardPick;

            fun void SendTempoEvents()
            {{
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
            }}
            spork ~ SendTempoEvents() @=> Shred part2TempoEvents;

            global Event startPart3;
            startPart3 => now;
            part2TempoEvents.exit();

        ", string.Join( ", ", myArpeggioPart2a ) ) );
        

        
    }

    void StartChuckPart3()
    {
        // inform above code to change chords
        myChuck.BroadcastEvent( "startPart3" );
        
        // regular modal clock, to be used in part 3
        myChuck.RunCode( myOSC.GenerateChucKCode( "vrSays", "vrHear" ) + string.Format( @"
            [{0}] @=> int myArpeggio[];
            0 => int currentReceiver;
            int myCurrentNote;

            // advance to part 3
            for( int i; i < vrSays.size(); i++ )
            {{
                vrSays[ i ].startMsg( ""/advanceToPart3"", ""i"" );
                vrSays[ i ].addInt( 1 );
            }}

            global float currentWindExcitationPart3;
            0.97 => float windDecay;
            fun void ListenForWindExcitation()
            {{
                vrHear.event( ""/part3/excitation"", ""f"" ) @=> OscEvent windExcitation;
                while( true )
                {{
                    windExcitation => now;
                    while( windExcitation.nextMsg() != 0 )
                    {{
                        windExcitation.getFloat() +=> currentWindExcitationPart3;
                        windDecay *=> currentWindExcitationPart3;
                    }}
                }}
            }}
            spork ~ ListenForWindExcitation();

            fun void ListenForPlayedNotes()
            {{
                vrHear.event( ""/part3/playedSadSeedlingNote"", ""i"" ) @=> OscEvent someonePlayedANote;

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

            // TODO: timing based on rain
            0.41 => global float noteLengthSeconds;
            true => int hardPick;

            while( true )
            {{
                for( int i; i < vrSays.size(); i++ )
                {{
                    // send out the next note someone should play
                    vrSays[i].startMsg( ""/part3/nextSeedlingNote"", ""f"" );
                    vrSays[i].addFloat( myArpeggio[myCurrentNote] );

                }}
                10::ms => now;
            }}
            
            // TODO
            // for rain, maybe later
            while( true )
            {{
                for( int i; i < myArpeggio.size(); i++ )
                {{
                    // play note
                    vrSays[ currentReceiver ].startMsg( ""/part3/playModal"", ""f,f,f"" );
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
        ", string.Join( ", ", myArpeggioPart3a ) ) );
        // TODO replace arpeggio
    }
}
