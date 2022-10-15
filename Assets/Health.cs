using UnityEngine;
using UnityEngine.SceneManagement;
public class Health : MonoBehaviour
{
    [SerializeField]
    private float _maxHealth = 100;

    [SerializeField]
    private float _currentHealth = 100;

    public delegate void OnHealthChanged(float currentAmount, float maxAmount);
    public event OnHealthChanged onDamageTaken;

    public event OnHealthChanged onHealthAdded;

    public delegate void OnHealthDepleted();
    public event OnHealthDepleted onHealthDepleted;

    [SerializeField]
    private float _timeBetweenHealthDecay = 1f;

    [SerializeField]
    private bool _decayingHealth = false;

    [SerializeField]
    private bool _isPlayer = false;

    [SerializeField]
    private float _decayAmount = 1f;

    private float _decayTimer = 0f;

    public float CurrentHealth => _currentHealth;

    private void Update()
    {
        if (_decayingHealth == false)
            return;

        _decayTimer += Time.deltaTime;
        if (_decayTimer >= _timeBetweenHealthDecay)
        {
            _decayTimer = 0f;
            ReduceHealth(_decayAmount);
        }
        
    }
    public void ToggleDecay(bool value)
    {
        _decayingHealth = value;
    }
    public void AddHealth(float amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);
        if (onHealthAdded != null)
            onHealthAdded.Invoke(_currentHealth, _maxHealth);
    }
    public void ReduceHealth(float amount)
    {
        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            if (onHealthDepleted != null)
                onHealthDepleted.Invoke();

            if (_isPlayer)
            {
                SceneManager.LoadScene(1);
            }
        }
        else
        {
            if (onDamageTaken != null)
                onDamageTaken.Invoke(_currentHealth, _maxHealth);
        }
            
    }
}