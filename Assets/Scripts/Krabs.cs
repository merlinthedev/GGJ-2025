using System;
using UnityEngine;

namespace solobranch.ggj2025
{
    public class Krabs : MonoBehaviour
    {
        [Header("Animation")] public Animator animator;

        [Header("Movement")] public float speed = 3f;
        public float speedIncreasePerBubble = 0.5f;

        private KRAB_STATE state = KRAB_STATE.IDLE;
        
        private int isWalkingHash;


        private void Awake()
        {
            Player.OnBubblePickUp.AddListener(HandlePlayerBubblePickUp);
        }

        private void Start()
        {
            isWalkingHash = Animator.StringToHash("isWalking");
            
            animator.SetBool(isWalkingHash, state == KRAB_STATE.IDLE);
        }

        private void HandlePlayerBubblePickUp(int bubbles)
        {
            speed += speedIncreasePerBubble;
        }

        private void ChangeState(KRAB_STATE newState)
        {
            state = newState;
            switch (state)
            {
                case KRAB_STATE.IDLE:
                    animator.SetBool(isWalkingHash, false);
                    break;
                case KRAB_STATE.WALKING:
                    animator.SetBool(isWalkingHash, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public enum KRAB_STATE
    {
        IDLE,
        WALKING
    }
}