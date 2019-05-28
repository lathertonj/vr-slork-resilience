using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCLeafController3 : MonoBehaviour
{
    public Transform myHeadLeaf;
    public Vector2 effectRange;
    public Vector3 effectDamping;
    public float xCycle, zCycle,
        xPhase, zPhase;
    private float xEffectSize, yEffectSize, zEffectSize;
    public float upDelay;

    public float upTime;
    public float downTime;
    private float lastUpTime, lastDownTime;
    private bool goingUp = false;
    private float currentAddedHeadAmount, goalAddedHeadAmount, addedHeadAmountSlew;
    private Vector3 downAmount;
    public Transform myBase;

    [HideInInspector] public float myHeight;

    // Use this for initialization
    void Start()
    {
        currentAddedHeadAmount = goalAddedHeadAmount = 0;
        addedHeadAmountSlew = 0.6f;


        // don't remember what this is for
        // lastUpTime = Random.Range( -upTime + 1, -1 ); 
        lastUpTime = 0;
        lastDownTime = -upDelay;

        xEffectSize = effectDamping.x * Random.Range( effectRange.x, effectRange.y );
        yEffectSize = effectDamping.y * Random.Range( effectRange.x, effectRange.y ) / 2;
        zEffectSize = effectDamping.z * Random.Range( effectRange.x, effectRange.y );

        downAmount = 1.5f * myBase.localScale.x * Vector3.up;
        myBase.position -= downAmount;
    }

    public void StartGrowing()
    {
        StartCoroutine( HoldCurrentAddedHeadAmount( -5f, 0.15f ) );
        StartCoroutine( MoveToPosition( myBase, downAmount, 1f ) );
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

    private IEnumerator HoldCurrentAddedHeadAmount( float newAmount, float timeToHold )
    {
        float t = 0f;
        while( t < 1 )
        {
            t += Time.deltaTime / timeToHold;
            currentAddedHeadAmount = newAmount;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentAddedHeadAmount += addedHeadAmountSlew * Time.deltaTime * ( goalAddedHeadAmount - currentAddedHeadAmount );

        float height = 0;
        if( goingUp )
        {
            float elapsedTime = Time.time - lastUpTime;
            height = elapsedTime / upTime;
            if( elapsedTime > upTime )
            {
                goingUp = false;
                lastDownTime = Time.time;
            }
        }
        else
        {
            float elapsedTime = Time.time - lastDownTime;
            height = 1 - elapsedTime / downTime;
            if( elapsedTime > downTime )
            {
                goingUp = true;
                lastUpTime = Time.time;
            }
        }
        // map from linear [0, 1] to curvy [-1, 1]
        height = -Mathf.Cos( Mathf.PI * Mathf.Clamp01( height ) );
        // add in down bits
        height += currentAddedHeadAmount;

        transform.localPosition = new Vector3(
            xEffectSize * Mathf.Cos( 2 * Mathf.PI * ( Time.time + xPhase ) / xCycle ),
            yEffectSize * height,    
            zEffectSize * Mathf.Cos( 2 * Mathf.PI * ( Time.time + zPhase ) / zCycle )
        );


        // map to angle
        float headAngle = height.MapClamp( -1, 1, 55, 10 );
        Vector3 localEulerAngles = myHeadLeaf.localEulerAngles;
        localEulerAngles.z = headAngle;
        myHeadLeaf.localEulerAngles = localEulerAngles;

        // store for chuck script to access
        myHeight = height;
    }
}
