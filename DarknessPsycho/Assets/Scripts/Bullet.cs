using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private LayerMask hitableMask;
    [SerializeField] private float fadeStrength = 1f;
    [SerializeField] private float speed;
    [FormerlySerializedAs("damage")] [SerializeField] private int maxDamage;
    
    private Rigidbody2D _rb;
    private Light2D _light;
    private float _startIntensity;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.velocity = transform.right * speed;

        _light = GetComponent<Light2D>();
        _startIntensity = _light.intensity;
    }

    private void Update()
    {
        _light.intensity -= fadeStrength * Time.deltaTime;
        if (_light.intensity <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        CheckCollision(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision.gameObject);
    }

    private void CheckCollision(GameObject otherObject)
    {
        if (IsObjectInHitableMask(otherObject))
        {
            BulletHit(otherObject);
        }
    }

    private bool IsObjectInHitableMask(GameObject obj)
    {
        return (hitableMask.value & (1 << obj.layer)) != 0;
    }

    private void BulletHit(GameObject hitObject)
    {
        if (hitObject.CompareTag("Enemy"))
        {
            var ratio = _light.intensity / _startIntensity;
            var damage = Mathf.FloorToInt(maxDamage * ratio);
            
            hitObject.GetComponent<IDamage>().TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    
}
