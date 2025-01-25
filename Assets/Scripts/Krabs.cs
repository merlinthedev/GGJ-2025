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


        private void Awake()
        {
            Player.OnBubblePickUp.AddListener(HandlePlayerBubblePickUp);
        }

        private void Start()
        {
            state = KRAB_STATE.IDLE;
            isWalkingHash = Animator.StringToHash("isWalking");

            animator.SetBool(isWalkingHash, state != KRAB_STATE.IDLE);

            agent.SetDestination(PointHandler.Instance.GetRandomPoint());
            ChangeState(KRAB_STATE.WALKING);
        }

        private void HandlePlayerBubblePickUp(Pickup pickup)
        {
            targetBubble = pickup;
            ChangeState(KRAB_STATE.BUBBLE_CHASING);
        }

        private void Update()
        {
            if (persistentTarget)
            {
                agent.SetDestination(Player.StaticTransform.position);
            }

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                ChangeState(KRAB_STATE.IDLE);
                StartCoroutineInRandomInterval();
            }
        }

        private void StartCoroutineInRandomInterval()
        {
            // wait between 0.1 and 1.2 seconds before starting the coroutine
            float randomWaitTime = UnityEngine.Random.Range(0.1f, 1.2f);
            StartCoroutine(StartWait(randomWaitTime));
        }

        private IEnumerator StartWait(float time)
        {
            yield return new WaitForSeconds(time);
            agent.SetDestination(PointHandler.Instance.GetRandomPoint());
            ChangeState(KRAB_STATE.WALKING);
        }

        private void ChangeState(KRAB_STATE newState)
        {
            previousState = state;
            state = newState;
            if (previousState == KRAB_STATE.ENRAGED && state != KRAB_STATE.ENRAGED)
            {
                persistentTarget = false;
            }

            switch (state)
            {
                case KRAB_STATE.IDLE:
                    animator.SetBool(isWalkingHash, false);
                    break;
                case KRAB_STATE.WALKING:
                    animator.SetBool(isWalkingHash, true);
                    agent.speed = speed;
                    break;
                case KRAB_STATE.BUBBLE_CHASING:
                    animator.SetBool(isWalkingHash, true);
                    agent.speed = GetBubbleChaseSpeed();
                    agent.SetDestination(targetBubble.transform.position);
                    break;
                case KRAB_STATE.ENRAGED:
                    animator.SetBool(isWalkingHash, true);
                    agent.speed = enragedSpeed;
                    persistentTarget = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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