using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class SLOrkVR2019OscCommunications : MonoBehaviour
{
    public SteamVR_Input_Sources handType;

    public SteamVR_Action_Boolean advanceToNextPartAction;
    public SteamVR_Action_Boolean playLightningAction;


    private OSCSendReceiver myOSC;
    private ChuckSubInstance myChuck;
    private LightningVisuals myLightningVisuals;

    public string[] myModalNotesPart1;
    public string[] myAhhNotesPart1a1;
    public string[] myAhhNotesPart1a2;
    public string[] mySawNotesPart1a1;
    public string[] mySawNotesPart1a2;

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
    public string[] myArpeggioPart3b;
    public string[] lightningFiles;
    int nextMovementToInit = 1;
    int nextLightningToPlay = 0;
    bool overIsland = false;

    public NPCLeafController3 theLeafToRaiseInPart4;
    public ParticleSystem theRain;
    public float biggerSwellsDelay = 0f, swellsNotFadeDelay = 15f, sunriseDelay = 12f, seedlingAppearDelay = 20f;
    public float rainFadeoutTime = 15f, sunriseTime = 8f;
    public Color sunriseSky, sunriseHorizon;
    public Material malleableSkyMaterial;

    void StartChuckPart4()
    {
        Invoke( "MakeSeedlingAppear", seedlingAppearDelay );
        StartCoroutine( SlowTheRainDown( rainFadeoutTime ) );
        StartCoroutine( MakeSunriseHappen( sunriseDelay, sunriseTime ) );
    }

    void MakeSeedlingAppear()
    {
        theLeafToRaiseInPart4.StartGrowing();
    }

    IEnumerator SlowTheRainDown( float rainFadeoutTime )
    {
        // ASSUME IT STARTS AT 3
        var emission = theRain.emission;
        yield return new WaitForSecondsRealtime( rainFadeoutTime / 6 );
        emission.rateOverTime = 2;

        yield return new WaitForSecondsRealtime( rainFadeoutTime / 3 );
        emission.rateOverTime = 1;

        yield return new WaitForSecondsRealtime( rainFadeoutTime / 3 );
        emission.rateOverTime = 0;
        
    }

    IEnumerator MakeSunriseHappen( float sunriseDelay, float sunriseTime )
    {
        Color startTopColor = RenderSettings.skybox.GetColor( "_SkyColor1" );
        Color startHorizonColor = RenderSettings.skybox.GetColor( "_SkyColor2" );
        RenderSettings.skybox = malleableSkyMaterial;
        RenderSettings.skybox.SetColor( "_SkyColor1", startTopColor );
        RenderSettings.skybox.SetColor( "_SkyColor2", startHorizonColor );
        float startTopH, startTopS, startTopV, endTopH, endTopS, endTopV;
        float startHorizonH, startHorizonS, startHorizonV, endHorizonH, endHorizonS, endHorizonV;
        Color.RGBToHSV( startTopColor, out startTopH, out startTopS, out startTopV );
        Color.RGBToHSV( sunriseSky, out endTopH, out endTopS, out endTopV );
        Color.RGBToHSV( startHorizonColor, out startHorizonH, out startHorizonS, out startHorizonV );
        Color.RGBToHSV( sunriseHorizon, out endHorizonH, out endHorizonS, out endHorizonV );

        // make the hue go the other way for the horizon
        endHorizonH += 1;

        yield return new WaitForSecondsRealtime( sunriseDelay );

        float t = 0f;
        while( t < 1 )
        {
            t += Time.deltaTime / sunriseTime;
            Color currentTopColor = Color.HSVToRGB(
                t.MapClamp( 0, 1, startTopH, endTopH ),
                t.MapClamp( 0, 1, startTopS, endTopS ),
                t.MapClamp( 0, 1, startTopV, endTopV )
            );
            Color currentHorizonColor = Color.HSVToRGB(
                t.MapClamp( 0, 1, startHorizonH, endHorizonH ) % 1.0f,
                t.MapClamp( 0, 1, startHorizonS, endHorizonS ),
                t.MapClamp( 0, 1, startHorizonV, endHorizonV )
            );
            RenderSettings.skybox.SetColor( "_SkyColor1", currentTopColor );
            RenderSettings.skybox.SetColor( "_SkyColor2", currentHorizonColor );
            yield return null;  
        }
        RenderSettings.skybox.SetColor( "_SkyColor1", sunriseSky );
        RenderSettings.skybox.SetColor( "_SkyColor2", sunriseHorizon );    
    }

    // Start is called before the first frame update
    void Start()
    {
        myChuck = GetComponent<ChuckSubInstance>();
        myOSC = GetComponent<OSCSendReceiver>();
        myLightningVisuals = GetComponent<LightningVisuals>();

        StartChuckOverall();

        // disable visuals
        SteamVR_Fade.Start( Color.black, 0 );
    }

    bool ShouldAdvanceToNextPart()
    {
        // application button or space bar
        return advanceToNextPartAction.GetStateDown( handType ) || Input.GetKeyDown( "space" );
    }

    bool ShouldPlayLightning()
    {
        return playLightningAction.GetStateDown( handType );
    }

    void SetColorsPart1()
    {
        UpdateTerrainTexture.Part1();
        UpdateWaterTexture.Part1();
        UpdateSkyTexture.Part1();
    }

    void SetColorsPart2a()
    {
        UpdateTerrainTexture.Part2a();
        UpdateWaterTexture.Part2a();
        UpdateSkyTexture.Part2a();
    }

    void SetColorsPart2b()
    {
        UpdateTerrainTexture.Part2b();
        UpdateWaterTexture.Part2b();
        UpdateSkyTexture.Part2b();
    }

    void SetColorsPart3()
    {
        UpdateTerrainTexture.Part3();
        UpdateWaterTexture.Part3();
        UpdateSkyTexture.Part3();
    }

    // Update is called once per frame
    void Update()
    {
        if( ShouldAdvanceToNextPart() )
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
                    //Debug.Log( "NOTE: advancing to part 3 via button is disabled!" );
                    break;
                case 4:
                    StartChuckPart4();
                    break;
                default:
                    break;
            }
            nextMovementToInit++;
        }

        // if( Input.GetKeyDown( "1" ) )
        // {
        //     myChuck.SetFloat( "part2DistortionAmount", 0.0 );
        // }
        // if( Input.GetKeyDown( "2" ) )
        // {
        //     myChuck.SetFloat( "part2DistortionAmount", 0.25 );
        // }
        // if( Input.GetKeyDown( "3" ) )
        // {
        //     myChuck.SetFloat( "part2DistortionAmount", 0.5 );
        // }
        // if( Input.GetKeyDown( "4" ) )
        // {
        //     myChuck.SetFloat( "part2DistortionAmount", 0.75 );
        // }
        // if( Input.GetKeyDown( "5" ) )
        // {
        //     myChuck.SetFloat( "part2DistortionAmount", 1.0 );
        // }
        // if( Input.GetKeyDown( "6" ) )
        // {
        //     myChuck.BroadcastEvent( "part2bChords" );
        // }
        // if( Input.GetKeyDown( "'" ) )
        // {
        //     myChuck.BroadcastEvent( "part3Raindrop" );
        // }

        if( ShouldPlayLightning() )
        {
            PlayChuckLightning( false );
        }

    }

    public void PlayRaindrop()
    {
        myChuck.BroadcastEvent( "part3Raindrop" );
    }

    bool haveAdvancedToPart2b = false;

    private void PlayChuckLightning( bool isFinalLightning )
    {
        // something we might have to do at the end of this function
        bool shouldAdvanceToPart3 = false;

        // pick which lightning file to play
        if( isFinalLightning )
        {
            // make sure (TODO: -1 or -2?)
            nextLightningToPlay = lightningFiles.Length - 2;

            // TODO: play it out of more than one station?

            // transition to movement 3!
            shouldAdvanceToPart3 = true;
        }
        else if( nextLightningToPlay >= lightningFiles.Length - 2 )
        {
            // we used too many lightning files.
            // TODO: pick a random one instead?
            nextLightningToPlay = lightningFiles.Length - 4;
        }

        // increase distortion
        myChuck.SetFloat( "part2DistortionAmount", ( (float)nextLightningToPlay ).MapClamp( 0, lightningFiles.Length - 3, 0.2f, 1 ) );

        // switch to the next chords and visuals on the third lightning
        if( nextLightningToPlay >= 2  && !haveAdvancedToPart2b )
        {
            haveAdvancedToPart2b = true;

            // chords
            myChuck.BroadcastEvent( "part2bChords" );

            // visuals
            SetColorsPart2b();
        }


        // play it
        if( !shouldAdvanceToPart3 )
        {
            // global string lightningFilename;
            // global Event playTheLightning;
            // global int allShouldPlay;
            myChuck.SetInt( "allShouldPlay", 0 );
            myChuck.SetString( "lightningFilename", lightningFiles[nextLightningToPlay] );
            myChuck.BroadcastEvent( "playTheLightning" );
        }
        else
        {
            // final one: play out of all of the hemis
            myChuck.SetInt( "allShouldPlay", 1 );
            myChuck.SetString( "lightningFilename", lightningFiles[nextLightningToPlay] );
            myChuck.BroadcastEvent( "playTheLightning" );
        }

        // do the visuals
        myLightningVisuals.TriggerLightning( ( (float)nextLightningToPlay ).MapClamp( 0, lightningFiles.Length - 2, 0.3f, 1 ) );

        // remember for next time
        nextLightningToPlay++;

        // advance to next part?
        if( shouldAdvanceToPart3 )
        {
            StartChuckPart3();
        }
    }

    public void PlayFinalChuckLightning()
    {
        PlayChuckLightning( true );
    }

    void StartChuckOverall()
    {   
        // part 1: advertise the next note   
        myChuck.RunCode( myOSC.GenerateChucKCode( "vrSays", "vrHear" ) + string.Format( @"
            // establish some globals early on
            global float wavingHandIntensity;
            global Event wavingHandOn;
            global Event wavingHandOff;
            global Event startPart1, startPart2, startPart3;
            startPart1 => now;

            [{0}] @=> int myArpeggio[];
            [[{1}], [{2}]] @=> int myAhhNotes[][];
            [[{20}], [{21}]] @=> int mySawNotes[][];
            int myCurrentNote;

            global float currentWindExcitation;
            global Event part1SeedlingNotePlayed;
            0.97 => float windDecay;

            1 => global int currentPart;

            global string lightningFilename;
            global Event playTheLightning;
            global int allShouldPlay;
            fun void ListenForLightnings()
            {{
                while( true )
                {{
                    playTheLightning => now;
                    if( allShouldPlay )
                    {{
                        for( int i; i < vrSays.size(); i++ )
                        {{
                            spork ~ PlayTheLightning( i );
                        }}
                        1::second => now;
                    }}
                    else
                    {{
                        spork ~ PlayTheLightning( Math.random2( 0, vrSays.size() - 1 ) );
                        1::second => now;
                    }}
                }}
            }}
            spork ~ ListenForLightnings();

            fun void PlayTheLightning( int which )
            {{
                vrSays[which].startMsg( ""/playLightning"", ""s"" );
                vrSays[which].addString( lightningFilename );
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
                        part1SeedlingNotePlayed.broadcast();
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
                startPart2 => now;
                [[{3}], [{4}]] @=> myAhhNotes;
                [[{5}], [{6}]] @=> mySawNotes;
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

            fun void ReplaceNotesForPart3()
            {{
                startPart3 => now;
                [[{7}], [{8}]] @=> myAhhNotes;
                [[{9}], [{10}]] @=> mySawNotes;
            }}
            spork ~ ReplaceNotesForPart3();

            200::ms => dur oscInformInterval;

            fun void WhileLoop1()
            {{
                while( true )
                {{
                    for( int i; i < vrSays.size(); i++ )
                    {{
                        // make sure we're all on same part 
                        vrSays[ i ].startMsg( ""/currentPart"", ""i"" );
                        vrSays[ i ].addInt( currentPart );    

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
                    oscInformInterval => now;
                }}
            }}
            spork ~ WhileLoop1();

            startPart2 => now;

            // advance to part 2
            2 => currentPart;

            fun void RespondToWavingHand()
            {{
                1 => int currentChord;
                while( true )
                {{
                    wavingHandOn => now;
                    // switch chords locally
                    !currentChord => currentChord;
                    // inform performers to switch chords to currentChord
                    for( int i; i < vrSays.size(); i++ )
                    {{
                        vrSays[ i ].startMsg( ""/part2/switchChord"", ""i"" );
                        vrSays[ i ].addInt( currentChord );

                        vrSays[ i ].startMsg( ""/part2/okToMakeNoise"", ""i"" );
                        vrSays[ i ].addInt( 1 );
                    }}

                    // inform performers of specific intensity
                    spork ~ InformOfWavingHandIntensity() @=> Shred intensity;

                    wavingHandOff => now;
                    intensity.exit();

                    for( int i; i < vrSays.size(); i++ )
                    {{
                        vrSays[ i ].startMsg( ""/part2/beSilentSoon"", ""i"" );
                        vrSays[ i ].addInt( 3 );
                    }}
                }}
            }}
            spork ~ RespondToWavingHand();

            fun void InformOfWavingHandIntensity()
            {{
                // offset
                0.5::oscInformInterval => now;

                while( true ) 
                {{
                    for( int i; i < vrSays.size(); i++ )
                    {{
                        vrSays[ i ].startMsg( ""/part2/chordBaselineIntensity"", ""f"" );
                        vrSays[ i ].addFloat( wavingHandIntensity );
                    }}
                    oscInformInterval => now;
                }}
            }}
            
            
            [{15}] @=> int myArpeggio2[];
            0 => int currentReceiver;

            global float part2DistortionAmount;
            float currentDistortionAmount;
            0.1 => float distortionAmountSlew;
            fun void SendDistortionAmount()
            {{
                // offset
                0.75::oscInformInterval => now;
                while( true )
                {{
                    distortionAmountSlew * ( part2DistortionAmount - currentDistortionAmount ) +=> currentDistortionAmount;
                    for( int i; i < vrSays.size(); i++ )
                    {{
                        vrSays[ i ].startMsg( ""/part2Distortion"", ""f"" );
                        vrSays[ i ].addFloat( currentDistortionAmount );
                    }}
                    oscInformInterval => now;
                }}
            }}
            spork ~ SendDistortionAmount();

            // timing based on VR
            0.4::second => dur noteLength;
            0.5::second => dur maxNoteLength;
            0.21::second => dur minNoteLength;

            fun void ListenForDecreaseEvents()
            {{
                global Event part2DecreaseTempo;
                while( true )
                {{
                    part2DecreaseTempo => now;
                    1.08 *=> noteLength;
                    if( noteLength > maxNoteLength )
                    {{
                        maxNoteLength => noteLength;
                    }}
                }}
            }}
            spork ~ ListenForDecreaseEvents();

            fun void ListenForIncreaseEvents()
            {{
                global Event part2IncreaseTempo;
                while( true )
                {{
                    part2IncreaseTempo => now;
                    0.94 *=> noteLength;
                    if( noteLength < minNoteLength )
                    {{
                        minNoteLength => noteLength;
                    }}
                }}
            }}
            spork ~ ListenForIncreaseEvents();

            true => int hardPick;


            global Event part2SeedlingNotePlayed;
            fun void SendTempoEvents()
            {{
                while( true )
                {{
                    for( int i; i < myArpeggio2.size(); i++ )
                    {{
                        // play note
                        vrSays[ currentReceiver ].startMsg( ""/playModal"", ""f,f,f"" );
                        vrSays[ currentReceiver ].addFloat( myArpeggio2[i] );
                        vrSays[ currentReceiver ].addFloat( Math.random2f( 0.2, 0.8 ) );
                        vrSays[ currentReceiver ].addFloat( Math.random2f( 0.3, 0.4 ) + 0.17 * hardPick );

                        // switch for next time
                        !hardPick => hardPick;

                        // find next receiver
                        currentReceiver++;
                        currentReceiver % vrSays.size() => currentReceiver;

                        // say to the animation
                        part2SeedlingNotePlayed.broadcast();

                        // wait
                        noteLength => now;
                    }}
                }}
            }}
            spork ~ SendTempoEvents() @=> Shred part2TempoEvents;

            startPart3 => now;
            part2TempoEvents.exit();

            [{16}] @=> int myArpeggio3[];
            [[{17}], [{18}]] @=> int mySawNotes3[][];
            [{19}] @=> int myRainArpeggio[];

            0 => int currentReceiver3;
            int myCurrentNote3;

            3 => currentPart;

            global float currentWindExcitationPart3;
            0.97 => float windDecay3;
            fun void ListenForWindExcitation()
            {{
                vrHear.event( ""/part3/excitation"", ""f"" ) @=> OscEvent windExcitation;
                while( true )
                {{
                    windExcitation => now;
                    while( windExcitation.nextMsg() != 0 )
                    {{
                        windExcitation.getFloat() +=> currentWindExcitationPart3;
                        windDecay3 *=> currentWindExcitationPart3;
                    }}
                }}
            }}
            spork ~ ListenForWindExcitation();

            global Event part3SeedlingNotePlayed;
            global int part3JumpID;
            fun void ListenForPlayedNotes3()
            {{
                
                 vrHear.event( ""/part3/playedSadSeedlingNote"", ""i"" ) @=> OscEvent someonePlayedANote;

                while( true )
                {{
                    someonePlayedANote => now;
                    while( someonePlayedANote.nextMsg() != 0 )
                    {{
                        // argument is ID
                        someonePlayedANote.getInt() => part3JumpID;

                        ( myCurrentNote3 + 1 ) % myArpeggio3.size() => myCurrentNote3;

                        // send message up as well
                        part3SeedlingNotePlayed.broadcast();
                    }}
                }}
            }}
            spork ~ ListenForPlayedNotes3();

            global Event part3SwellPlayed;
            global int part3SwellID;
            fun void ListenForPlayedSwells()
            {{
                vrHear.event( ""/part3/playedRainSwell"", ""i"" ) @=> OscEvent someonePlayedASwell;

                while( true )
                {{
                    someonePlayedASwell => now;
                    while( someonePlayedASwell.nextMsg() != 0 )
                    {{
                        // argument is ID
                        someonePlayedASwell.getInt() => part3SwellID;

                        // send message up as well
                        part3SwellPlayed.broadcast();
                    }}
                }}
            }}
            spork ~ ListenForPlayedSwells();

            // TODO: timing based on rain
            0.41 => global float noteLengthSeconds;
            true => int hardPick3;

            global Event part3Raindrop;
            fun void DoRaindrops()
            {{
                while( true )
                {{
                    for( int i; i < myRainArpeggio.size(); i++ )
                    {{
                        part3Raindrop => now;

                        // play note
                        vrSays[ currentReceiver3 ].startMsg( ""/playRainModal"", ""f,f,f"" );
                        vrSays[ currentReceiver3 ].addFloat( myRainArpeggio[i] );
                        vrSays[ currentReceiver3 ].addFloat( Math.random2f( 0.2, 0.8 ) );
                        vrSays[ currentReceiver3 ].addFloat( Math.random2f( 0.3, 0.4 ) + 0.17 * hardPick3 );

                        // switch for next time
                        !hardPick3 => hardPick3;

                        // also ask for a swell
                        vrSays[ currentReceiver3 ].startMsg( ""/playRainSwell"", ""i,i,i,i,i"" );
                        0 => int which;
                        // randomly pick between chord 0 or 1 with 0.5 probability
                        if( Math.random2f( 0, 1 ) < 0.5 ) 
                        {{
                            1 => which;
                        }}
                        for( int i; i < mySawNotes3[which].size(); i++ )
                        {{
                            vrSays[ currentReceiver3 ].addInt( mySawNotes3[which][i] );
                        }}

                        // find next receiver
                        currentReceiver3++;
                        currentReceiver3 % vrSays.size() => currentReceiver3;
                    }}
                }}
            }}
            spork ~ DoRaindrops();

            // offset
            0.25::oscInformInterval => now;
            while( true )
            {{
                for( int i; i < vrSays.size(); i++ )
                {{
                    // send out the next note someone should play
                    vrSays[i].startMsg( ""/part3/nextSeedlingNote"", ""f"" );
                    vrSays[i].addFloat( myArpeggio3[myCurrentNote3] );

                }}
                oscInformInterval => now;
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
            string.Join( ", ", mySawNotesPart2b2 ),
            string.Join( ", ", myArpeggioPart2a ),
            string.Join( ", ", myArpeggioPart3a ),
            string.Join( ", ", mySawNotesPart3a1 ),
            string.Join( ", ", mySawNotesPart3a2 ),
            string.Join( ", ", myArpeggioPart3b ),
            string.Join( ", ", mySawNotesPart1a1 ),
            string.Join( ", ", mySawNotesPart1a2 )
        ) );
    }

    void StartChuckPart1()
    {
        // also do visuals
        SetColorsPart1();

        // inform code to start running
        myChuck.BroadcastEvent( "startPart1" );
    }

    void StartChuckPart2()
    {
        // also do visuals
        SetColorsPart2a();

        // inform above code to change chords, etc
        myChuck.BroadcastEvent( "startPart2" );

        // turn on trail rendering
        SlewFollower.trailsRendering = true;
    }

    void StartChuckPart3()
    {
        // also do visuals
        SetColorsPart3();

        // inform above code to change chords
        myChuck.BroadcastEvent( "startPart3" );

        // turn off trail rendering
        SlewFollower.trailsRendering = false;
    }

    
}
