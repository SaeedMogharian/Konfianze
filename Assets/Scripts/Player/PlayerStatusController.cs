using System;
using GamePlace;
using UnityEngine;

namespace Player
{
    public class PlayerStatusController : MonoBehaviour
    {
        [SerializeField] private int food;
        [SerializeField] private int health;

        private void Awake()
        {
            food = 3;
            health = 100;
        }

        public void Consequence(Place place)
        {
            // consume food
            food -= 1;
            // Collect food and health
            food += place.Food;
            health += place.Healing;
        }
    }
}
