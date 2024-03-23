using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemy
{
    public class Shooter : AIMove , IDamage
    {
        [SerializeField] private int health = 100;
        [SerializeField] private float shootRange = 2f; // The range the enemy will start shooting
        [SerializeField] private float shootInterval = 1f; // In seconds
        [SerializeField] private GameObject bullet;
        
        private bool _isShooting = false;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, shootRange);
        }

        protected override void Update()
        {
            var collider = Physics2D.OverlapCircle(transform.position, shootRange, target.gameObject.layer);
            if (collider == null)
            {
                if (_isShooting)
                {
                    CancelInvoke(nameof(Shoot));
                    StartMoving();
                    _isShooting = false;
                }

                base.Update();
            }
            else
            {
                if (_isShooting) return;

                InvokeRepeating(nameof(Shoot), 0, shootInterval);
                StopMoving();
                _isShooting = true;
            }
        }

        private void Shoot()
        {
            var direction = (target.position - transform.position).normalized;  
            Instantiate(bullet, transform.position, Quaternion.Euler(direction));
        }

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
