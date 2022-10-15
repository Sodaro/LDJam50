using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
public class Zombie : MonoBehaviour
{
    private NavMeshAgent _agent;
    private Health _targetHealth;
    private PlayerMovement _playerMovement;

    [Header("Idle")]
    [SerializeField] private AudioSource _idleSoundSource;
    [SerializeField] private AudioClip[] _idleClips;
    [SerializeField] private float _minTimeBetweenIdleSounds = 0.5f;
    [SerializeField] private float _maxTimeBetweenIdleSounds = 1.5f;
    private float _idleSoundTimer;
    private float _timeBetweenIdleSounds = 0;

    [Header("Combat")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _deathClip;
    [SerializeField] private AudioClip _aggroClip;

    [SerializeField]
    private Health _health;

    [SerializeField]
    private GameObject _damageLight;

    [Header("Shooting")]
    [SerializeField]
    private bool _canShoot = false;
    [SerializeField]
    private float _shootDelay = 3f;
    [SerializeField]
    private ParticleSystem _particleSystem;

    [SerializeField]
    private float _damageLightDuration = 0.25f;

    [SerializeField]
    private float _timeBetweenPositionUpdate = 1f;

    private float _shootTimer = 0f;
    private float _positionUpdateTimer = 0f;
    private float _damageLightTimer = 0f;

    private float _initialSpeed;
    private Coroutine _rotRoutine;

    [SerializeField]
    private float _attackDamage = 20f;

    private bool _hasMoved = false;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _initialSpeed = _agent.speed;
    }

    private void OnEnable()
    {
        _health.onHealthDepleted += Die;
        _health.onDamageTaken += OnDamage;
    }

    private void OnDisable()
    {
        _health.onHealthDepleted -= Die;
        _health.onDamageTaken -= OnDamage;
        if (_playerMovement != null)
        {
            _playerMovement.onCarEntered -= TargetCar;
            _playerMovement.onCarExit -= TargetPlayer;
        }
    }

    void OnDamage(float _, float __)
    {
        _audioSource.Play();

    }

    public void ApplyKnockback(Vector3 force)
    {
        if (_rotRoutine != null)
            StopCoroutine(_rotRoutine);

        _rotRoutine = StartCoroutine(RotationDisable(0.5f));
        _agent.velocity += force;
    }
    
    private IEnumerator RotationDisable(float time)
    {
        _agent.updateRotation = false;
        yield return new WaitForSeconds(time);
        _agent.updateRotation = true;
        _rotRoutine = null;
    }
    void Die()
    {
        AudioSource.PlayClipAtPoint(_deathClip, transform.position);
        Destroy(gameObject);
    }

    private void Update()
    {
        
        _damageLightTimer += Time.deltaTime;
        _positionUpdateTimer -= Time.deltaTime;
        _shootTimer += Time.deltaTime;

        if (_idleSoundSource.isPlaying == false && _targetHealth == null)
        {
            _idleSoundTimer += Time.deltaTime;
            if (_idleSoundTimer > _timeBetweenIdleSounds)
            {
                _idleSoundTimer = 0;
                int index = Random.Range(0, _idleClips.Length);
                _idleSoundSource.clip = _idleClips[index];
                _idleSoundSource.Play();
                _timeBetweenIdleSounds = Random.Range(_minTimeBetweenIdleSounds, _maxTimeBetweenIdleSounds);
            }
        }

        if (_damageLightTimer > _damageLightDuration)
            _damageLight.SetActive(false);

        if (_targetHealth == null)
            return;
        
        if (_positionUpdateTimer <= 0)
        {
            if (_agent.pathPending == false && (_shootTimer >= _shootDelay && _agent.remainingDistance < 0.75f))
            {
                if (_canShoot)
                    _particleSystem.Play();
                _shootTimer = 0f;
                _positionUpdateTimer = 1.5f;
                _agent.ResetPath();
                _targetHealth.ReduceHealth(_attackDamage);
            }
            else
            {
                _agent.SetDestination(_targetHealth.transform.position);
                _positionUpdateTimer = _timeBetweenPositionUpdate;
                _hasMoved = true;
            }
        }

    }
    void TargetCar(Car car)
    {
        _targetHealth = car.GetComponent<Health>();
        _targetHealth = null;
    }
    void TargetPlayer()
    {
        _targetHealth = _playerMovement.GetComponent<Health>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            _playerMovement = player;
            Car car = player.GetCar();
            player.onCarEntered += TargetCar;
            player.onCarExit += TargetPlayer;
            if (car != null)
            {
                //TargetCar(car);
            }
            else
            {
                TargetPlayer();
            }
            _agent.SetDestination(_targetHealth.transform.position);
            _idleSoundSource.clip = _aggroClip;
            _idleSoundSource.Play();
        }
        else if (_targetHealth == null && other.TryGetComponent(out Car car))
        {
            //_targetHealth = car.GetComponent<Health>();
            //_agent.SetDestination(_targetHealth.transform.position);
            //_idleSoundSource.clip = _aggroClip;
            //_idleSoundSource.Play();
        }
    }
}
