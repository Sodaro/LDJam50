using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{

    public delegate void OnAmmoChanged(int newAmount);
    public event OnAmmoChanged onAmmoChanged;

    [SerializeField] private AudioSource _pickupSource;
    [SerializeField] private AudioClip[] _pickupClips;

    [SerializeField] private Animator _animator;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _cameraMaxDist = 2f;
    [SerializeField] private Transform _cameraFollowTransform;

    enum Weapon { Pistol, Shotgun, Count};

    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private GameObject[] _weaponObjects;
    [SerializeField] private AudioClip[] _weaponClips;
    

    [SerializeField]
    private GameObject _muzzleFlash;

    [SerializeField]
    private Bullet _bulletPrefab;

    [SerializeField]
    private float _shootForce = 50f;

    [SerializeField]
    private float _delayBetweenPistolShots = 0.5f;

    [SerializeField]
    private float _delayBetweenShotgunShots = 1.3f;

    private float _shootDelay = 0f;

    private float _muzzleFlashDuration = 0.1f;
    private float _muzzleFlashTimer = 0f;

    [SerializeField]
    int _shotgunAmmo = 5;

    private Weapon _currentWeapon;

    private Health _health;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        _health = GetComponent<Health>();
    }
    public void AddItem(ItemPickup.ItemType type, int amount)
    {
        if (type == ItemPickup.ItemType.Shotgun)
        {
            _shotgunAmmo += amount;
            _pickupSource.clip = _pickupClips[1];
            onAmmoChanged.Invoke(_shotgunAmmo);
        }
            
        else if (type == ItemPickup.ItemType.Health)
        {
            _health.AddHealth(amount);
            _pickupSource.clip = _pickupClips[0];
        }
        _pickupSource.Play();
    }

    void ToggleWeapon(bool value, int index)
    {
        if (index >= 0 && index < _weaponObjects.Length && _weaponObjects[index] != null)
        {
            _weaponObjects[index].SetActive(value);
        }
    }

    public void SwitchWeapon(int value)
    {
        if (value >= (int)Weapon.Count)
        {
            value = 0;
        }
        else if (value < 0)
        {
            value = (int)Weapon.Count - 1;
        }

        ToggleWeapon(false, (int)_currentWeapon);
        _currentWeapon = (Weapon)value;
        ToggleWeapon(true, (int)_currentWeapon);

        if (_currentWeapon == Weapon.Shotgun)
            _animator.SetBool("isUsingPistol", false);
        else
            _animator.SetBool("isUsingPistol", true);

        if (value >= 0 && value < _weaponClips.Length)
            _audioSource.clip = _weaponClips[value];
    }
    bool PistolShoot()
    {
        Bullet instance = Instantiate(_bulletPrefab, transform.position + transform.forward, Quaternion.identity, null);
        instance.Shoot(_shootForce * transform.forward);
        return true;
    }
    bool ShotgunShoot()
    {
        if (_shotgunAmmo <= 0)
            return false;

        for (int i = 0; i < 12; i++)
        {
            float angle = 10f;
            float anglePerBullet = angle / 4;
            Vector3 localOffset = new Vector3(-0.5f + ((float)i / 12) * 1, 0, 1.5f);
            Vector3 pos = transform.TransformPoint(localOffset);
            Bullet instance = Instantiate(_bulletPrefab, pos, transform.rotation, null);
            instance.transform.Rotate(Vector3.up, (-angle/2) + anglePerBullet + anglePerBullet*i);
            instance.Shoot(_shootForce * instance.transform.forward);
        }

        _shotgunAmmo--;
        onAmmoChanged.Invoke(_shotgunAmmo);
        return true;
    }

    public void HandleShoot()
    {
        bool success = false;
        switch (_currentWeapon)
        {
            case Weapon.Pistol:
                success = PistolShoot();
                break;
            case Weapon.Shotgun:
                success = ShotgunShoot();
                break;
            default:
                break;
        }
        if (success)
        {
            _audioSource.Play();
            _muzzleFlashTimer = 0;
            _shootDelay = _currentWeapon == Weapon.Shotgun ? _delayBetweenShotgunShots : _delayBetweenPistolShots;
            _muzzleFlash.SetActive(true);
        }
        else
        {
           
            _audioSource.clip = _weaponClips[2];
            if (_audioSource.isPlaying == false)
            {
                _shootDelay = 0.5f;
                _audioSource.Play();
            }
                
            //EMPTY GUN SOUND
        }
    }

    void Update()
    {
        _shootDelay -= Time.deltaTime;
        _muzzleFlashTimer += Time.deltaTime;
        if (_muzzleFlashTimer >= _muzzleFlashDuration)
            _muzzleFlash.SetActive(false);


        float scroll = Input.mouseScrollDelta.y;
        if (scroll != 0)
        {
            int index = scroll < 0 ? -1 : 1;
            SwitchWeapon((int)_currentWeapon + index);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchWeapon(2);

        RaycastHit mouseHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out mouseHit, Mathf.Infinity, _groundMask, QueryTriggerInteraction.Ignore))
        {
            
            Vector3 lookDirection = (mouseHit.point - transform.position).normalized;
            Vector3 followPos = new Vector3(mouseHit.point.x, _cameraFollowTransform.position.y, mouseHit.point.z);
            if (Vector3.Distance(transform.position, followPos) > _cameraMaxDist)
            {
                //Debug.Log("bean");
                _cameraFollowTransform.position = transform.position + lookDirection * _cameraMaxDist;
            }
            else
            {
                _cameraFollowTransform.position = followPos;
            }
            lookDirection.y = 0;            
            transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        }

        bool canShoot = _shootDelay <= 0;
        if ((Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) && canShoot)
        {
            HandleShoot();
        }
    }
}
