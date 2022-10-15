using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField]
    private PlayerMovement _playerMovement;

    [SerializeField]
    private Car _car;

    [SerializeField]
    private bool _isPlayerInCar = false;

    private void OnEnable()
    {
        _playerMovement.onCarEntered += OnPlayerEnterCar;
        _playerMovement.onCarExit += OnPlayerExitCar;
    }
    private void OnDisable()
    {
        _playerMovement.onCarEntered -= OnPlayerEnterCar;
        _playerMovement.onCarExit -= OnPlayerExitCar;
    }

    void OnPlayerEnterCar(Car car)
    {
        _car = car;
        _isPlayerInCar = true;
    }

    void OnPlayerExitCar()
    {
        _isPlayerInCar = false;
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(horizontal, 0, vertical).normalized;
        if (_isPlayerInCar == false)
            _playerMovement.HandleMovement(dir, Time.deltaTime);
        else if (_car != null)
            _car.HandleMovement(dir, Time.deltaTime);
    }
    

}
