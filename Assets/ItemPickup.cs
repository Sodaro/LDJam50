using UnityEngine;

[SelectionBase]
public class ItemPickup : MonoBehaviour
{
    [SerializeField] private GameObject[] _meshObjects;
    [SerializeField] private AudioClip[] _audioClips;

    private AudioClip _activeClip;
    public enum ItemType { Health, Pistol, Shotgun};
    public ItemType Type = ItemType.Shotgun;

    [SerializeField] private bool _shouldRotate = true;
    [SerializeField] private float _rotationSpeed = 180f;
    public int Quantity = 5;

    public void Initialize(int quantity, ItemType type)
    {
        Type = type;
        Quantity = quantity;
        if (type == ItemType.Health)
        {
            _meshObjects[0].SetActive(true);
            _activeClip = _audioClips[0];
        }
        else
        {
            _meshObjects[1].SetActive(true);
            _activeClip = _audioClips[1];
        }
    }

    private void Start()
    {
        if (_shouldRotate == false)
            enabled = false;
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerShooting player))
        {
            player.AddItem(Type, Quantity);
            //AudioSource.PlayClipAtPoint(_activeClip, transform.position);
            Destroy(gameObject);
        }
    }

}
