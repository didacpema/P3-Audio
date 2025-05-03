using UnityEngine;

public class WeaponCollision : MonoBehaviour
{
    public LayerMask obstacleLayer; // Layer for obstacles
    public float rotationAngleX = 90f; // Angle to rotate the weapon around the x-axis when encountering an obstacle
    public float rotationAngleZ = 90f; // Angle to rotate the weapon around the z-axis when encountering an obstacle
    public float rotationSpeed = 5f; // Speed of rotation
    public float moveAmountForward = 0.1f; // Amount to move the weapon forward when encountering an obstacle
    public float moveAmountDown = 0.05f; // Amount to move the weapon downward when encountering an obstacle
    public float raycastDistance = 10f; // Maximum distance for the Raycast

    private Quaternion originalRotation; // Original rotation of the weapon
    private Quaternion targetRotation; // Target rotation of the weapon
    private Vector3 originalPosition; // Original position of the weapon
    private Vector3 targetPosition; // Target position of the weapon
    private Camera playerCamera;

    private void Start()
    {
        // Save the original rotation and position of the weapon
        originalRotation = transform.localRotation;
        originalPosition = transform.localPosition;
        targetRotation = originalRotation;
        targetPosition = originalPosition;

        // Find the main camera
        playerCamera = Camera.main;
    }

    private void Update()
    {
        // Check if there is an obstacle in front of the weapon
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, raycastDistance, obstacleLayer))
        {
            // Calculate the target rotation when an obstacle is detected
            targetRotation = Quaternion.Euler(0, rotationAngleX, rotationAngleZ);
            // Calculate the target position when an obstacle is detected
            targetPosition = originalPosition - transform.forward * moveAmountForward - transform.up * moveAmountDown;
        }
        else
        {
            // Reset the target rotation and position when no obstacle is detected
            targetRotation = originalRotation;
            targetPosition = originalPosition;
        }

        // Rotate the weapon towards the target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
        // Move the weapon towards the target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, rotationSpeed * Time.deltaTime);
    }
}
