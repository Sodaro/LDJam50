using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject _meshObject;
    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Health _health;
    private CapsuleCollider _collider;

    [SerializeField]
    private float _moveSpeed = 5.0f;
    private CharacterController _controller;

    public delegate void OnCarEntered(Car car);
    public event OnCarEntered onCarEntered;

    public delegate void OnCarExit();
    public event OnCarExit onCarExit;

    private bool _isInCar = false;
    private Car _car;

    private Vector3 _velocity;

    private float _previousHealth = 0;
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _collider = GetComponent<CapsuleCollider>();
    }

    void PlayHurtSound(float current, float __)
    {
        if (_previousHealth == 0)
            _previousHealth = current;
        else
        {
            if (_previousHealth - current > 10)
            {
                _audioSource.Play();
            }
        }
        _previousHealth = current;
    }
    private void OnEnable()
    {
        _health.onDamageTaken += PlayHurtSound;
    }

    private void OnDisable()
    {
        _health.onDamageTaken -= PlayHurtSound;
    }
    //private void FixedUpdate()
    //{
    //    if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, 1f, 1) == false)
    //    {
    //        _velocity.y += Physics.gravity.y * Time.deltaTime;
    //    }
    //}

    public Car GetCar()
    {
        return _car;
    }

    void Update()
    {
        if (_car != null)
        {
            transform.position = _car.transform.position;
            //transform.rotation = _car.transform.rotation;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_isInCar == false)
            {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 1.5f, 1, QueryTriggerInteraction.Collide);
                foreach (Collider collider in colliders)
                {
                    if (collider.TryGetComponent(out Car car))
                    {
                        PossessCar(car);
                        break;
                    }
                }
            }
            else
            {
                UnpossessCar();
            }
        }
    }
    private void PossessCar(Car car)
    {
        _meshObject.SetActive(false);
        _collider.enabled = false;
        _controller.enabled = false;
        _car = car;
        _isInCar = true;
        onCarEntered.Invoke(car);
    }
    private void UnpossessCar()
    {
        _meshObject.SetActive(true);
        _collider.enabled = true;
        _controller.enabled = true;
        _car.StopEngine();
        _car = null;
        _isInCar = false;
        onCarExit.Invoke();
    }


    public void HandleMovement(Vector3 movementDir, float deltaTime)
    {
        Vector3 moveVelocity = movementDir * _moveSpeed;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit groundHit, 1f, 1) == false)
        {
            _velocity.y += Physics.gravity.y * Time.deltaTime;
        }
        else
            _velocity.y = 0;

        _velocity = new Vector3(moveVelocity.x, _velocity.y, moveVelocity.z);
        if (_velocity.x != 0 || _velocity.z != 0)
        {
            _animator.SetBool("isWalking", true);
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }
        _controller.Move(_velocity * deltaTime);

        //if (movementDir != Vector3.zero)
            
    }
}
