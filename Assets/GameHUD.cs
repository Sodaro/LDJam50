using UnityEngine;
using UnityEngine.UI;
public class GameHUD : MonoBehaviour
{
    [SerializeField] private Text _ammoText;
    [SerializeField] private Color _tempHealthColor;
    [SerializeField] private Image[] _healthSegments;
    [SerializeField] private Health _playerHealth;
    [SerializeField] private PlayerShooting _playerShoot;
    int numberOfSegments = 10;

    float _previousHealth = 0;
    float _maxHealth = 0;
    private void Awake()
    {
        foreach (Image image in _healthSegments)
        {
            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Horizontal;
            image.fillOrigin = (int)Image.OriginHorizontal.Left;
        }
    }
    private void OnEnable()
    {
        _playerHealth.onDamageTaken += UpdateHealthDisplay;
        _playerHealth.onHealthAdded += OnHealthAdded;
        if (_playerShoot != null)
        _playerShoot.onAmmoChanged += UpdateAmmoCounter;
    }

    private void OnDisable()
    {
        _playerHealth.onDamageTaken -= UpdateHealthDisplay;
        _playerHealth.onHealthAdded -= OnHealthAdded;
        if (_playerShoot != null)
        _playerShoot.onAmmoChanged -= UpdateAmmoCounter;
    }

    private void UpdateAmmoCounter(int amount)
    {
        _ammoText.text = "[1] Pistol: INF.\n[2] Shotgun: " + amount;
    }
    void OnHealthAdded(float current, float max)
    {
        int previousMaxIndex = (int)(_previousHealth / max * 10);
        int maxIndex = (int)(current / max * 10);
        float fillAmount = current % 10 / 10;
        for (int i = 0; i < numberOfSegments; i++)
        {
            if (i < previousMaxIndex)
            {
                //_healthSegments[i].color = Color.red;
                _healthSegments[i].fillAmount = 1;
            }
            else if (i >= previousMaxIndex && i < maxIndex)
            {
                _healthSegments[i].color = _tempHealthColor;
                _healthSegments[i].fillAmount = fillAmount;
            }
            else
            {
                _healthSegments[i].color = _tempHealthColor;
                _healthSegments[i].fillAmount = 0;
            }
        }
    }
    private void UpdateHealthDisplay(float currentAmount, float totalAmount)
    {
        _maxHealth = totalAmount;
        _previousHealth = currentAmount;
        int maxIndex = (int)(currentAmount / totalAmount * 10);
        float fillAmount = currentAmount % 10 / 10;
        for (int i = 0; i < numberOfSegments; i++)
        {
            if (i < maxIndex)
            {
                _healthSegments[i].fillAmount = 1;
            }
            else if (i == maxIndex)
            {
                _healthSegments[i].fillAmount = fillAmount;
            }
            else if (i > maxIndex)
            {
                _healthSegments[i].fillAmount = 0;
            }
        }
    }
}
