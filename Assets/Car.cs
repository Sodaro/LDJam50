using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Car : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _rollFriction = 0.8f;
    [SerializeField] private float _gripFriction = 4.0f;
    [SerializeField] private float _turnSpeed = 5.0f;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Health _health;

    private CharacterController _controller;
    private Vector3 _velocity = Vector3.zero;

    ////HEALTH
    //private float _currentHealth = 100f;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit groundHit, 1f, _groundMask) == false)
        {
            _velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else
            _velocity.y = 0;

        if (_velocity.sqrMagnitude > 100f)
        {
            RaycastHit[] hits = Physics.BoxCastAll(transform.position, transform.localScale/2, transform.forward, transform.rotation, 3f, _layerMask, QueryTriggerInteraction.Ignore);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.TryGetComponent(out Health hitHealth))
                {
                    hitHealth.ReduceHealth(20);
                    //_health.ReduceHealth(20);
                }
            }
        }
    }

    //public void StartEngine()
    //{

    //}
    public void StopEngine()
    {
        _audioSource.Stop();
    }
    public void HandleMovement(Vector3 movementDir, float deltaTime)
    {
        if (_health.CurrentHealth > 0)
        {
            if (_audioSource.isPlaying == false)
                _audioSource.Play();

            if (movementDir.x != 0)
                transform.Rotate(Vector3.up, movementDir.x * _turnSpeed * deltaTime);


            _velocity += transform.forward * movementDir.z * _acceleration * deltaTime;
            if (movementDir.z    == 0)
                _audioSource.Stop();
        }
        else
        {
            _audioSource.Stop();
        }


        Vector3 rollVelocity = Vector3.ProjectOnPlane(_velocity, transform.forward);
        Vector3 gripVelocity = _velocity - rollVelocity;

        rollVelocity -= _rollFriction * rollVelocity * deltaTime;
        gripVelocity -= _gripFriction * gripVelocity * deltaTime;

        _velocity = rollVelocity + gripVelocity;


        _controller.Move(_velocity * deltaTime);

    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawCube(transform.position + transform.forward, transform.localScale);
    //}
}
