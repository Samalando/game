using UnityEngine;

public class MoveRotateTimer : MonoBehaviour
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

    public float movementSpeed = 50;

    // This keeps track of whether the object is moving forward or backward.
    private bool movingReverse = true;
    private bool Var = true;
    // Saves the starting position of the object.
    private Vector3 startPos;
    //public float moveDistance = 5f;

    public float TimerVar = 5;
    // This method runs when the script starts.
    private void Start()
    {
        // Store the initial position of the object.
        startPos = transform.position; 
    }

    // This method is called every frame.
    void Update()
    {
        // Handles the rotation of the object.
        HandleRotation();
        // Handles the movement of the object.
        HandleMovement();

        Timer();
    }

    // Rotates the object continuously.
    void HandleRotation()
    {
        if (Var)
        {
            // Rotate around the `rotationAxis` at a speed of `rotationSpeed`.
            transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime); // TODO: Create a variable for the value 50
        }
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
    void Timer()
    {
        TimerVar = TimerVar - Time.deltaTime;
        if(TimerVar <= 0)
        {
            Var = false;
        }
        
    }
}
