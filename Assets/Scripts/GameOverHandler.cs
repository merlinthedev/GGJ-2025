using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace solobranch.ggj2025
{
    public class GameOverHandler : MonoBehaviour
    {
        [Header("Game Over UI")] public Image backgroundImage;
        public TMP_Text deathText;

        private bool shouldFade = false;

        private void Start()
        {
            Player.OnPlayerDamage.AddListener(i1 =>
            {
                if (i1 == 0)
                {
                    shouldFade = true;
                }
            });

            backgroundImage.color =
                new Color(backgroundImage.color.r, backgroundImage.color.g, backgroundImage.color.b, 0);
        }

        private void Update()
        {
            if (shouldFade)
            {
                // slowly up the alpha value of the background image
                backgroundImage.color = new Color(backgroundImage.color.r, backgroundImage.color.g,
                    backgroundImage.color.b, backgroundImage.color.a + Time.deltaTime * 0.5f);

                // if reached max alpha value, stop fading
                if (backgroundImage.color.a >= 1)
                {
                    deathText.gameObject.SetActive(true);
                    shouldFade = false;
                }
            }
        }
    }
}