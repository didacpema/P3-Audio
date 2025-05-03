using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    private float currentSpeed;

    public float speed = 12.0f;
    public float sprintSpeed = 18.0f;
    public float gravity = -9.81f;
    public float jumpHeight = 3.0f;
    public bool canJump = true;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    public bool isGrounded;
    public bool isMoving;

    private Vector3 lastPosition = Vector3.zero;

    // Movement threshold
    public float movementThreshold = 0.01f;

    // Footstep sound
    public AudioSource audioSource;
    public AudioClip footstepSound;
    public float footstepWalkInterval = 0.8f;
    public float footstepRunInterval = 0.4f;
    private bool isPlayingFootstep = false;

    public Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        
        // Ensure cameraTransform is set
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset velocity Y
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Calculate move direction relative to the camera's forward and right directions
        Vector3 move = cameraTransform.right * x + cameraTransform.forward * z;

        // Sprint functionality
        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : speed;

        // Move player
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Jump functionality
        if (canJump && Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;

        // Apply velocity
        controller.Move(velocity * Time.deltaTime);

        // Check if moving
        if (Vector3.Distance(lastPosition, transform.position) > movementThreshold && isGrounded)
        {
            isMoving = true;
            if (!isPlayingFootstep)
            {
                StartCoroutine(PlayFootsteps());
            }
        }
        else
        {
            isMoving = false;
        }
        lastPosition = transform.position;
    }

    private IEnumerator PlayFootsteps()
    {
        isPlayingFootstep = true;

        while (isMoving)
        {
            // Play footstep sound with slight pitch and volume variation
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.volume = Random.Range(0.65f, 0.85f);
            audioSource.PlayOneShot(footstepSound);

            // Determine the interval based on current speed
            float footstepInterval = (currentSpeed == sprintSpeed) ? footstepRunInterval : footstepWalkInterval;

            // Wait for the interval before playing the next footstep
            yield return new WaitForSeconds(footstepInterval);
        }

        isPlayingFootstep = false;
    }
}