using UnityEngine;

public class CloudMover : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("The base speed of the cloud. (Increased for faster movement)")]
    public float speed = 5f;

    [Tooltip("Random variance added to the speed so clouds don't move uniformly.")]
    public float speedVariation = 1.5f;

    public float destroyXPosition = 45f; // How far right before it despawns

    private float currentSpeed;

    void Start()
    {
        // Calculate this cloud's unique speed once it spawns
        currentSpeed = speed + Random.Range(-speedVariation, speedVariation);
    }

    void Update()
    {
        // Move the cloud to the right using its unique speed
        transform.Translate(Vector3.right * currentSpeed * Time.deltaTime);

        // Clean up the cloud once it is off-screen
        if (transform.position.x > destroyXPosition)
        {
            Destroy(gameObject);
        }
    }
}