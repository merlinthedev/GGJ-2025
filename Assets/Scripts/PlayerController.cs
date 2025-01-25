using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")] [SerializeField]
    private float movementSpeed = 5f;

    [Header("Look Settings")] [SerializeField]
    private float sensitivity = 5f;

    [SerializeField] private float maxPitch = 80f;

    [Header("References")] [SerializeField]
    private Transform cameraTransform;

    [SerializeField] private Rigidbody capsuleRigidbody;

    private Vector2 moveInput = Vector2.zero;
    private Vector2 lookInput = Vector2.zero;
    private float pitch = 0f;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        HandleMovement();
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

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
}