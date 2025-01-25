using System;
using UnityEngine;
using UnityEngine.Events;

namespace solobranch.ggj2025
{
    public class KrabsCollider : MonoBehaviour
    {
        public static UnityEvent OnKrabsColliderEntered = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnKrabsColliderEntered?.Invoke();
            }
        }

        private void OnDestroy()
        {
            OnKrabsColliderEntered.RemoveAllListeners();
        }
    }
}