using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace solobranch.ggj2025
{
    public class Player : MonoBehaviour
    {
        [Header("Movement Settings")] public float movementSpeed = 5f;
        private float sprintSpeed = 8.5f;
        public float jumpSpeed = 5f;

        [Header("Look Settings")] public float sensitivity = 5f;
        public float maxPitch = 80f;

        [Header("References")] public Transform cameraTransform;
        public Rigidbody capsuleRigidbody;

        [Header("Ground Check")] public Transform groundCheck;

        [Header("Stamina Settings")] public Image staminaBar;
        public float maxStamina;
        public float staminaRegainRate;
        public float staminaDepletionRate;

        [Header("Interact Settings")] public float interactDistance = 10f;
        public TMP_Text scoreText;

        private float currentStamina;
        private bool canSprint => currentStamina > 0;
        private float groundCheckRadius = 0.4f;
        private Vector2 moveInput = Vector2.zero;
        private Vector2 lookInput = Vector2.zero;
        private float pitch;
        private bool isGrounded;
        private bool jumpPressed;
        private bool isSprinting;

        private List<Pickup> inventory = new();

        public static UnityEvent<int> OnPlayerDamage = new();
        public static UnityEvent<Pickup> OnBubblePickUp = new();
        public static UnityEvent OnBushEnter = new(); // player is now hidden
        public static UnityEvent OnBushExit = new(); // player is no longer hidden
        public static Transform StaticTransform { get; private set; }

        private List<Bush> currentlyInBushes = new();

        public FootstepSFX footstepSFX;

        public FootstepSFX footstepSFX;

        private void Start()
        {
            UpdateScoreUI();
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnDestroy()
        {
            OnPlayerDamage.RemoveAllListeners();
            OnBubblePickUp.RemoveAllListeners();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void Update()
        {
            CheckGround();
            HandleJump();

            HandleStamina();
        }

        private void LateUpdate()
        {
            HandleLook();
            StaticTransform = transform;
        }

        private void HandleStamina()
        {
            if (isSprinting && moveInput.magnitude > 0.1f && canSprint)
            {
                currentStamina -= staminaDepletionRate * Time.deltaTime;

                if (currentStamina <= 0)
                {
                    currentStamina = 0;
                    isSprinting = false;
                }
            }
            else
            {
                currentStamina += staminaRegainRate * Time.deltaTime;
            }

            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

            UpdateStaminaUI();
        }

        private void UpdateStaminaUI()
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }

        private void HandleMovement()
        {
            Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y);
            moveDirection = transform.TransformDirection(moveDirection);
            Vector3 velocity = moveDirection * (isSprinting ? sprintSpeed : movementSpeed);

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

        public void AddToInventory(Pickup pickup)
        {
            inventory.Add(pickup);

            OnBubblePickUp?.Invoke(pickup);
            // update UI
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            int score = inventory.Count;
            scoreText.SetText($"{score}/{PickupManager.AmountOfPickups}");
        }

        private void CheckGround()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, LayerMask.GetMask("Ground"),
                QueryTriggerInteraction.Ignore);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
            
            if (moveInput.magnitude > 0.1f)
            {
                footstepSFX.PlayFootstepSound();
            }
            else
            {
                footstepSFX.PauseFootstepSound();
            }
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

        public void OnSprint(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if (canSprint)
                {
                    isSprinting = true;
                }
            }
            else if (ctx.canceled)
            {
                isSprinting = false;
            }
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            Physics.Raycast(ray, out RaycastHit hit, interactDistance, LayerMask.GetMask("Interactable"));

            if (hit.collider != null)
            {
                if (hit.collider.TryGetComponent(out Pickup pickup))
                {
                    pickup.PickUp(this);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Bush bush = other.gameObject.GetComponent<Bush>();

            if (bush != null)
            {
                AddToBushList(bush);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Bush bush = other.gameObject.GetComponent<Bush>();

            if (bush != null)
            {
                RemoveFromBushList(bush);
            }
        }

        private void AddToBushList(Bush bush)
        {
            if (!currentlyInBushes.Contains(bush))
            {
                currentlyInBushes.Add(bush);
                
                // do something
                if(currentlyInBushes.Count == 1)
                {
                    // entered a bush
                    OnBushEnter?.Invoke();
                }
            }
        }

        private void RemoveFromBushList(Bush bush)
        {
            if (currentlyInBushes.Contains(bush))
            {
                currentlyInBushes.Remove(bush);
                
                if(currentlyInBushes.Count == 0)
                {
                    // no longer in any bushes
                    // do something
                    OnBushExit?.Invoke();
                }
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
}