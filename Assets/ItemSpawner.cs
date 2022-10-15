using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    
    [SerializeField] private Health _health;
    [SerializeField] private ItemPickup _itemPrefab;

    [SerializeField] private ItemPickup.ItemType _type;
    [SerializeField] private int _quantity;
    private void OnEnable()
    {
        _health.onHealthDepleted += SpawnItem;
    }
    private void OnDisable()
    {
        _health.onHealthDepleted -= SpawnItem;
    }

    void SpawnItem()
    {
        if (_itemPrefab != null)
        {
            ItemPickup pickup = Instantiate(_itemPrefab, transform.position, Quaternion.identity, null);
            pickup.Initialize(_quantity, _type);
        }  
        Destroy(gameObject);
    }
}
