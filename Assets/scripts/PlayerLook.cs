using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 1000.0f;
    public Transform playerBody;
    public Transform cameraTransform;

    float xRotation = 0.0f;
    float yRotation = 0.0f;

    public float topClamp = -90.0f;
    public float bottomClamp = 90.0f;

    // Head bobbing variables
    public bool enableHeadBobbing = true;
    public float walkBobSpeed = 14f;
    public float walkBobAmount = 0.05f;
    public float sprintBobSpeed = 18f;
    public float sprintBobAmount = 0.1f;

    private float defaultYPos = 0;
    private float timer = 0;

    // Camera sway variables
    public float swayAmount = 0.02f;
    public float maxSwayAmount = 0.06f;
    public float swaySmoothFactor = 4.0f;

    private Vector3 initialCameraPosition;

    // Leaning variables
    public float leanAngle = 15.0f;
    public float leanSpeed = 6.0f;
    public float leanPeekAmount = 0.3f; // Amount to move the camera when leaning
    private float currentLean = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        defaultYPos = cameraTransform.localPosition.y;
        initialCameraPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        HandleMouseLook();
        if (enableHeadBobbing)
        {
            HandleHeadBobbing();
        }
        HandleCameraSway();
        HandleLeaning();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, topClamp, bottomClamp);

        yRotation += mouseX;

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0.0f, currentLean);
        playerBody.localRotation = Quaternion.Euler(0.0f, yRotation, 0.0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    void HandleHeadBobbing()
    {
        float speed = (Input.GetKey(KeyCode.LeftShift) ? sprintBobSpeed : walkBobSpeed);
        float amount = (Input.GetKey(KeyCode.LeftShift) ? sprintBobAmount : walkBobAmount);

        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            timer += Time.deltaTime * speed;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x,
                                                        defaultYPos + Mathf.Sin(timer) * amount,
                                                        cameraTransform.localPosition.z);
        }
        else
        {
            timer = 0;
            cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x,
                                                        Mathf.Lerp(cameraTransform.localPosition.y, defaultYPos, Time.deltaTime * speed),
                                                        cameraTransform.localPosition.z);
        }
    }

    void HandleCameraSway()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Calculate sway
        float swayX = Mathf.Clamp(mouseX * swayAmount, -maxSwayAmount, maxSwayAmount);
        float swayY = Mathf.Clamp(mouseY * swayAmount, -maxSwayAmount, maxSwayAmount);

        Vector3 targetPosition = new Vector3(initialCameraPosition.x + swayX, initialCameraPosition.y + swayY, initialCameraPosition.z);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * swaySmoothFactor);
    }

    void HandleLeaning()
    {
        float targetLean = 0.0f;
        Vector3 peekOffset = Vector3.zero;

        if (Input.GetKey(KeyCode.E))
        {
            targetLean = -leanAngle;
            peekOffset = Vector3.left * leanPeekAmount;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            targetLean = leanAngle;
            peekOffset = Vector3.right * leanPeekAmount;
        }

        currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSpeed);
        Vector3 targetPosition = initialCameraPosition - peekOffset;

        // Apply the lean rotation and position offset
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0.0f, currentLean);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetPosition, Time.deltaTime * leanSpeed);
    }
}