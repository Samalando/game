using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    //so you can specify where the object moves to
    public float yLevel = 0f;
    public float xLevel = 0f;
    public float zLevel = 0f;
    //glide speed
    public int glideSpeed = 5;
    //object to spawn when duplicating
    public GameObject prefabToSpawn;
    public Vector3 bounceDirection;
    private Vector3 startPosition;
    private bool hasReachedTarget = false;
    private Vector3 movementLocation;
    // Start is called before the first frame update
    void Start()
    {
        movementLocation = new Vector3(xLevel, yLevel, zLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!hasReachedTarget)
        {
            // Move toward the target position
            transform.position = Vector3.MoveTowards(transform.position, movementLocation, glideSpeed * Time.deltaTime);
            
            // Check if we've reached the target
            if (Vector3.Distance(transform.position, movementLocation) < 0.1f)
            {
                hasReachedTarget = true;
                transform.position = movementLocation; // Snap to exact position
            }
        }
        else
        {
            // Start bouncing movement
            transform.position += bounceDirection.normalized * (glideSpeed * Time.deltaTime);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        // Only bounce if we've reached the target position
        if (hasReachedTarget && collision.gameObject.CompareTag("wall"))
        {
            bounceDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            // Duplicate the object
            Instantiate(prefabToSpawn, transform.position, transform.rotation);
           
            // Bounce off the wall
            Vector3 wallNormal = collision.contacts[0].normal;
            bounceDirection = Vector3.Reflect(bounceDirection, wallNormal);
        }
    }
}

