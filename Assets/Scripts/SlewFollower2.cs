using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlewFollower2 : MonoBehaviour
{

    public Transform objectToFollow;
    public float positionSlew = 0.1f, rotationSlew = 0.1f;
    private Vector3 currentPos, goalPos;
    private Quaternion currentRotation, goalRotation;


    // Use this for initialization
    void Start()
    {
        // set position
        transform.position = currentPos = goalPos = objectToFollow.position;
    }

    // Update is called once per frame
    void Update()
    {
        // slew position and orientation toward object we're following
        goalPos = objectToFollow.position;
        goalRotation = objectToFollow.rotation;

        // compute
        currentPos += positionSlew * ( goalPos - currentPos );
        currentRotation = Quaternion.Slerp( currentRotation, goalRotation, rotationSlew );

        // set
        transform.position = currentPos;
        transform.rotation = currentRotation;
    }

}

