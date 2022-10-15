using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _lifeTime;
    private float _timer;

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Shoot(Vector3 force)
    {
        _timer = _lifeTime;
        _rb.AddForce(force, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Health health))
        {
            health.ReduceHealth(1f);
            if (collision.transform.TryGetComponent(out Zombie zombie))
            {
                zombie.ApplyKnockback((zombie.transform.position - transform.position).normalized * 15f);
            }
        }
        Destroy(gameObject);
    }

}
