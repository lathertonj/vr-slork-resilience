using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffGravityOnFirstCollision : MonoBehaviour
{
    private Rigidbody myRB;
    // Start is called before the first frame update
    void Start()
    {
        myRB = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter( Collision c ) 
    {
        myRB.useGravity = false;
        Destroy( this );
    }
}
