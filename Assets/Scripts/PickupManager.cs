using System;
using UnityEngine;
using System.Collections.Generic;
using EasyButtons;

namespace solobranch.ggj2025
{
    public class PickupManager : MonoBehaviour
    {
        private static PickupManager instance;
        [Header("Pickup Prefab")] public GameObject pickupPrefab;

        [Header("Spawn Points")] public List<Vector3> spawnPoints = new List<Vector3>();

        private int amountOfPickups => spawnPoints.Count;
        public static int AmountOfPickups => instance.amountOfPickups;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var point in spawnPoints)
            {
                Gizmos.DrawSphere(point, 0.5f);
            }
        }

        private void Awake()
        {
            instance = this;
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
        
        [Button]
        public void SpawnPrimitivesAtSpawnPoints()
        {
            foreach (var point in spawnPoints)
            {
                GameObject primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                primitive.transform.position = point;
            }
        }
    }
}