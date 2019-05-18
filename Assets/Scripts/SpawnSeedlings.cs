using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSeedlings : MonoBehaviour
{
    public Transform seedlingPrefab;
    public int numSeedlings;
    public Vector3 boundingBox;

    void Awake()
    {
        for( int i = 0; i < numSeedlings; i++ )
        {
            Instantiate( 
                seedlingPrefab, 
                transform.position + new Vector3(
                    Random.Range( -boundingBox.x, boundingBox.x ),
                    Random.Range( -boundingBox.y, boundingBox.y ),
                    Random.Range( -boundingBox.z, boundingBox.z )
                ), 
                Quaternion.identity,
                transform 
            );
        }
    }

}
