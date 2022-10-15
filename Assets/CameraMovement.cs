using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform _target;

    [SerializeField]
    private float _followSpeed = 2f;

    private Vector3 _offset;
    private void Start()
    {
        _offset = _target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, _target.position - _offset, Time.deltaTime * _followSpeed);
    }
}
