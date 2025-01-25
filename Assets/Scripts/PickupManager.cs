using UnityEngine;
using System.Collections.Generic;

namespace solobranch.ggj2025
{
    public class PickupManager : MonoBehaviour
    {
        [Header("Pickup Prefab")] public GameObject pickupPrefab;

        [Header("Spawn Points")] public List<Vector3> spawnPoints = new List<Vector3>();

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var point in spawnPoints)
            {
                Gizmos.DrawSphere(point, 0.5f);
            }
        }

        private void Start()
        {
            foreach (var position in spawnPoints)
            {
                if (pickupPrefab != null)
                {
                    Instantiate(pickupPrefab, position, Quaternion.identity);
                }
            }
        }
    }
}