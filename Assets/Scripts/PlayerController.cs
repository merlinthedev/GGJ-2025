using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float movementSpeed = 5f;

    [SerializeField] private float jumpSpeed = 5f;

    [Header("Look Settings")] [SerializeField]
    private float sensitivity = 5f;

    [SerializeField] private float maxPitch = 80f;

    [Header("References")] [SerializeField]
    private Transform cameraTransform;

    [SerializeField] private Rigidbody capsuleRigidbody;

    [Header("Ground Check")] [SerializeField]
    private Transform groundCheck;

    private float groundCheckRadius = 0.4f;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private float pitch = 0f;

    private bool isGrounded = false;
    private bool jumpPressed = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        CheckGround();
        HandleJump();
    }

    private void LateUpdate()
    {
        HandleLook();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
        moveDirection = transform.TransformDirection(moveDirection);
        Vector3 velocity = moveDirection * movementSpeed;

        velocity.y = capsuleRigidbody.linearVelocity.y;

        capsuleRigidbody.linearVelocity = velocity;
    }

    private void HandleLook()
    {
        float yaw = lookInput.x * sensitivity * Time.deltaTime;
        float deltaPitch = lookInput.y * sensitivity * Time.deltaTime;

        pitch = Mathf.Clamp(pitch - deltaPitch, -maxPitch, maxPitch);

        transform.Rotate(Vector3.up * yaw);

        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    private void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            capsuleRigidbody.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
        }

        jumpPressed = false;
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, LayerMask.GetMask("Ground"),
            QueryTriggerInteraction.Ignore);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            jumpPressed = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}