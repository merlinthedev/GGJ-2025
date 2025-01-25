using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace solobranch.ggj2025
{
    public class Krabs : MonoBehaviour
    {
        [Header("Animation")] public Animator animator;

        [Header("Movement")] public float speed = 3f;
        public float enragedSpeed = 6f;

        public NavMeshAgent agent;

        public KRAB_STATE state = KRAB_STATE.IDLE;
        private KRAB_STATE previousState = KRAB_STATE.IDLE;

        private int isWalkingHash;
        private Pickup targetBubble;

        private bool persistentTarget = false;
        public bool isPlayerHidden = false;

        public Vector3 targetPosition;
        public Vector3 currentPosition;


        private void Awake()
        {
            Player.OnBubblePickUp.AddListener(HandlePlayerBubblePickUp);
            Player.OnBushEnter.AddListener(HandlePlayerBushEnter);
            Player.OnBushExit.AddListener(HandlePlayerBushExit);
            KrabsCollider.OnKrabsColliderEntered.AddListener(HandleKrabsColliderEnter);
        }

        private void Start()
        {
            isWalkingHash = Animator.StringToHash("isWalking");

            animator.SetBool(isWalkingHash, false);

            ChangeState(KRAB_STATE.IDLE);
        }

        private void HandlePlayerBubblePickUp(Pickup pickup)
        {
            targetBubble = pickup;
            ChangeState(KRAB_STATE.BUBBLE_CHASING);
        }

        private void Update()
        {
            targetPosition = agent.destination;
            currentPosition = transform.position;

            if (persistentTarget)
            {
                agent.SetDestination(Player.StaticTransform.position);
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                Debug.Log("Destination reached, going to idle...");
                ChangeState(KRAB_STATE.IDLE);
            }
        }

        private void ChangeState(KRAB_STATE newState)
        {
            // Special case: If currently enraged, allow transition only to WALKING if the player is hidden.
            if (state == KRAB_STATE.ENRAGED && !isPlayerHidden && newState != KRAB_STATE.WALKING)
            {
                Debug.Log($"Ignoring transition from ENRAGED to {newState} while player is visible.");
                return;
            }

            // Update previous state and set new state
            previousState = state;
            state = newState;

            // Handle state-specific logic
            switch (state)
            {
                case KRAB_STATE.IDLE:
                    animator.SetBool(isWalkingHash, false);
                    agent.SetDestination(PointHandler.Instance.GetRandomPoint());
                    ChangeState(KRAB_STATE.WALKING);
                    break;

                case KRAB_STATE.WALKING:
                    animator.SetBool(isWalkingHash, true);
                    agent.speed = speed;
                    break;

                case KRAB_STATE.BUBBLE_CHASING:
                    animator.SetBool(isWalkingHash, true);
                    agent.speed = GetBubbleChaseSpeed();
                    agent.SetDestination(new Vector3(targetBubble.transform.position.x,
                        targetBubble.transform.position.y, targetBubble.transform.position.z));
                    break;

                case KRAB_STATE.ENRAGED:
                    animator.SetBool(isWalkingHash, true);
                    agent.speed = enragedSpeed;
                    persistentTarget = true;
                    agent.SetDestination(Player.StaticTransform.position); // Always target player
                    break;

                default:
                    Debug.LogError($"Unhandled state: {newState}");
                    break;
            }
        }

        private void HandlePlayerBushEnter()
        {
            isPlayerHidden = true;
            if (state == KRAB_STATE.ENRAGED)
            {
                persistentTarget = false;
                ChangeState(KRAB_STATE.IDLE);
            }
        }

        private void HandlePlayerBushExit()
        {
            isPlayerHidden = false;
            
            // disable and enable colliders in child components so we reset the trigger methods
            foreach (Collider collider in GetComponentsInChildren<Collider>())
            {
                collider.enabled = false;
                collider.enabled = true;
            }
        }

        private void HandleKrabsColliderEnter()
        {
            if (isPlayerHidden) return;

            ChangeState(KRAB_STATE.ENRAGED);
        }

        private float GetBubbleChaseSpeed()
        {
            float distance = Vector3.Distance(transform.position, targetBubble.transform.position);

            float minDistance = 1f;
            float maxDistance = 10f;

            float clampedDistance = Mathf.Clamp(distance, minDistance, maxDistance);

            float speed = Mathf.Lerp(this.speed, 3 * enragedSpeed,
                (clampedDistance - minDistance) / (maxDistance - minDistance));

            return speed;
        }
    }

    public enum KRAB_STATE
    {
        IDLE,
        WALKING,
        BUBBLE_CHASING,
        ENRAGED
    }
}