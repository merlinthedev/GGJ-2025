using System;
using UnityEngine;
using UnityEngine.UI;

namespace solobranch.ggj2025
{
    public class LivesManager : MonoBehaviour
    {
        public Image[] livesImages;

        private void Awake()
        {
            Player.OnPlayerDamage.AddListener((lives) =>
            {
                
            });
        }
    }
}