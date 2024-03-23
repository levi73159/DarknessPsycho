using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class Chaser : AIMove , IDamage
    {
        [SerializeField] private int health;

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
                Die();
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
