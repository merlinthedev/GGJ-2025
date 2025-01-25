using System;
using System.Collections.Generic;
using UnityEngine;

namespace solobranch.ggj2025
{
    public class PointHandler : MonoBehaviour
    {
        public static PointHandler Instance { get; private set; }
        private List<Transform> points = new();


        private void Awake()
        {
            Instance = this;

            for (int i = 0; i < transform.childCount; i++)
            {
                points.Add(transform.GetChild(i));
            }
        }

        public Vector3 GetRandomPoint()
        {
            int randomIndex = UnityEngine.Random.Range(0, points.Count);
            Debug.Log("returning point with index " + randomIndex);
            return points[randomIndex].position;
        }
    }
}