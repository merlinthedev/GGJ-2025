using UnityEngine;

namespace solobranch.ggj2025
{
    public class Pickup : MonoBehaviour
    {
        public void PickUp(Player player)
        {
            player.AddToInventory(this);
            Destroy(gameObject);
        }

        private void Update()
        {
            // with sin wave, the object will move up and down
            transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * 0.003f, transform.position.z);
        }
    }
}