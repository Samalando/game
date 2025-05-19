using System;
using UnityEngine;

public class MoveRotateSpeedUp : MonoBehaviour
{
    // **Header for rotation-related settings**
    [Header("Rotation Settings")]
    // `rotationAxis` defines the direction of rotation. Default is "up" (Y-axis).
    public Vector3 rotationAxis = Vector3.up;

    public float rotationSpeed = 50;

    // **Header for movement-related settings**
    [Header("Movement Settings")]
    // `movementAxis` defines the direction of movement. Default is "up" (Y-axis).
    public Vector3 movementAxis = Vector3.up;

    public float movementSpeed = 10;

    // This keeps track of whether the object is moving forward or backward.
    private bool movingReverse = true; 

    // Saves the starting position of the object.
    private Vector3 startPos;
    public float moveDistance = 5f;
    public static Action FinishedSpinning;
    public float maxSpeed = 100f;
    private bool slowDown = false;
    private bool rotationEnabled = false;
    
    // This method runs when the script starts.
    private void OnEnable()
    {
        AutoDialog.doneTalking += StartRotating;

    }
    private void OnDisable()
    {
        AutoDialog.doneTalking -= StartRotating;
      
    }
    private void Start()
    {
        // Store the initial position of the object.
        startPos = transform.position; 
    }

    // This method is called every frame.
    void Update()
    {
        if(!rotationEnabled) return;
        // Handles the rotation of the object.
        HandleRotation();
        // Checks if the object has reached its movement limit.
        CheckDistance();
        // Handles the movement of the object.
        HandleMovement();
        ManageSpeed();
        print(rotationSpeed);
        
        

    }

    // Rotates the object continuously.
    void HandleRotation()
    {
        // Rotate around the `rotationAxis` at a speed of `rotationSpeed`.
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime); // TODO: Create a variable for the value 50
    }

    // Moves the object in a forward or backward direction.
    void HandleMovement()
    {
        if (movingReverse)
        {
            // Move backward along the `movementAxis` at `movementSpeed`.
            transform.Translate(movementAxis * -movementSpeed * Time.deltaTime); // TODO: Create a variable for the value -2
        }
        else
        {
            // Move forward along the `movementAxis` at `movementSpeed`.
            transform.Translate(movementAxis * movementSpeed * Time.deltaTime); // TODO: Create a variable for the value 2
        }
    }

    // Checks how far the object has moved from its starting position.
    void CheckDistance()
    {
        // Calculate the distance between the current position and the starting position.
        float currentDistance = Vector3.Distance(startPos, transform.position); 

        // If the object moves too far, reverse the direction of movement.
        if (currentDistance >= moveDistance) // TODO: Create a variable for the value 5
        {
            // Toggle the `movingReverse` value (true to false or false to true).
            movingReverse = !movingReverse; 

            // Update the starting position to the current position.
            startPos = transform.position; 
        }
    }

    void ManageSpeed()
    {
        
        if (rotationSpeed > maxSpeed)
        {
            slowDown = true;
        }

        if (slowDown)
        {
            rotationSpeed = rotationSpeed - (rotationSpeed/2) * Time.deltaTime;
        }
        else
        {
            rotationSpeed = rotationSpeed + (rotationSpeed/8) * Time.deltaTime;
        }

        if (rotationSpeed < 5f)
        {
            rotationSpeed = 0;
            
            
        }

        if ( rotationSpeed == 0)
        {
            FinishedSpinning?.Invoke();
        }
        
    }

    void StartRotating()
    {
        rotationEnabled = true;
    }
}
